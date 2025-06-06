﻿using Microsoft.AspNetCore.Identity;

namespace CatalogApi.Models;

public class ApplicationUser : IdentityUser
{
	public string? RefreshToken { get; set; }

	public DateTime RefreshTokenExpityTime { get; set; }
}