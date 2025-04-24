using CatalogApi.Dtos;
using CatalogApi.Models;
using CatalogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuthController(ILogger<AuthController> logger, ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration) : ControllerBase
{
	private readonly ILogger<AuthController> _logger = logger;

	private readonly ITokenService _tokenService = tokenService;

	private readonly UserManager<ApplicationUser> _userManager = userManager;

	private readonly RoleManager<IdentityRole> _roleManager = roleManager;

	private readonly IConfiguration _configuration = configuration;

	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> Login([FromBody] LoginModel model)
	{
		var user = await _userManager.FindByNameAsync(model.Username!);
		if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
		{
			var userRoles = await _userManager.GetRolesAsync(user);
			var authClaims = new List<Claim>
			{
				new(ClaimTypes.Name, user.UserName!),
				new(ClaimTypes.Email, user.Email!),
				new("id", user.UserName!),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
			foreach (var userRole in userRoles)
			{
				authClaims.Add(new(ClaimTypes.Role, userRole));
			}
			var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
			var refreshToken = _tokenService.GenerateRefreshToken();
			_ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpityTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
			await _userManager.UpdateAsync(user);
			return Ok(new
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				RefreshToken = refreshToken,
				Expiration = token.ValidTo
			});
		}
		return Unauthorized();
	}

	[HttpPost]
	[Route("register")]
	public async Task<IActionResult> Register([FromBody] RegisterModel model)
	{
		var userExists = await _userManager.FindByNameAsync(model.Username!);
		if (userExists != null)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
		}
		ApplicationUser user = new()
		{
			Email = model.Email,
			SecurityStamp = Guid.NewGuid().ToString(),
			UserName = model.Username,
		};
		var result = await _userManager.CreateAsync(user, model.Password!);
		if (!result.Succeeded)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed!" });
		}
		return Ok(new Response { Status = "Success", Message = "User created successfully!" });
	}

	[HttpPost]
	[Route("refresh-token")]
	public async Task<IActionResult> RefreshToken(TokenModel model)
	{
		if (model is null)
		{
			return BadRequest("Invalid client request");
		}
		string? accessToken = model.AccessToken ?? throw new ArgumentNullException(nameof(model));
		string? refreshToken = model.RefreshToken ?? throw new ArgumentNullException(nameof(model));
		var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);
		if (principal == null)
		{
			return BadRequest("Invalid access token/refresh token");
		}
		string? username = principal.Identity?.Name;
		var user = await _userManager.FindByNameAsync(username!);
		if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpityTime <= DateTime.Now)
		{
			return BadRequest("Invalid access token/ refresh token");
		}
		var newAccessToken = _tokenService.GenerateAccessToken([.. principal.Claims], _configuration);
		var newRefreshToken = _tokenService.GenerateRefreshToken();
		user.RefreshToken = newRefreshToken;
		await _userManager.UpdateAsync(user);
		return new ObjectResult(new
		{
			accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
			refreshToken = newRefreshToken
		});
	}

	[Authorize(Policy = "ExclusiveOnly")]
	[HttpPost]
	[Route("revoke/{username}")]
	public async Task<IActionResult> Revoke(string username)
	{
		var user = await _userManager.FindByNameAsync(username);
		if (user == null)
		{
			return BadRequest("Invalid username");
		}
		user.RefreshToken = null;
		await _userManager.UpdateAsync(user);
		return NoContent();
	}

	[Authorize(Policy = "SuperAdminOnly")]
	[HttpPost]
	[Route("CreateRole")]
	public async Task<IActionResult> CreateRole([FromQuery] string roleName)
	{
		var roleExist = await _roleManager.RoleExistsAsync(roleName);
		if (!roleExist)
		{
			var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
			if (roleResult.Succeeded)
			{
				_logger.LogInformation(1, "Role added successfully");
				return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"Role {roleName} added successfully" });
			}
			else
			{
				_logger.LogInformation(2, "Error when adding role");
				return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"Problem occurred when tried to add role {roleName}" });
			}
		}
		return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"Role {roleName} already exists" });
	}

	[Authorize(Policy = "SuperAdminOnly")]
	[HttpPost]
	[Route("AddUserToRole")]
	public async Task<IActionResult> AddUserToRole([FromQuery] string email, [FromQuery] string roleName)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user != null)
		{
			var result = await _userManager.AddToRoleAsync(user, roleName);
			if (result.Succeeded)
			{
				var message = $"User {user.Email} was added to the role {roleName}";
				_logger.LogInformation(1, message);
				return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = message });
			}
			else
			{
				var message = $"Unable to add user {user.Email} to the role {roleName}";
				_logger.LogInformation(1, message);
				return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = message });
			}
		}
		return BadRequest(new { Error = $"Unable to find user with email address {email}" });
	}
}