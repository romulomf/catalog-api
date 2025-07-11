using Asp.Versioning;
using CatalogApi.Context;
using CatalogApi.Dtos.Mappings;
using CatalogApi.Filters;
using CatalogApi.Logging;
using CatalogApi.Models;
using CatalogApi.Repositories;
using CatalogApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration { LogLevel = LogLevel.Information }));

builder.Services.AddApiVersioning(avo =>
{
	// declara qual é a versão padrão da API.
	avo.DefaultApiVersion = new ApiVersion(1, 0);
	// determina que quando nenhuma versão for especificada explicitamente, usa a versão padrão.
	avo.AssumeDefaultVersionWhenUnspecified = true;
	// indica que as versões da API devem ser incluídas no cabeçalho (header) da resposta (response).
	avo.ReportApiVersions = true;
	avo.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
}).AddApiExplorer(ae => {
	ae.GroupNameFormat = "'v'VVV";
	ae.SubstituteApiVersionInUrl = true;
});

// configuração da open api
builder.Services.AddOpenApi("v1", openApiOptions => {
	openApiOptions.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
	openApiOptions.AddDocumentTransformer((document, context, cancellationToken) => {
		document.Info.Contact = new OpenApiContact
		{
			Name = "Catalog Api",
			Email = "romuloflores@gmail.com"
		};
		return Task.CompletedTask;
	});
});

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ITokenService, TokenService>();

// registro dos repositórios para o container de injeção de dependência
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnityOfWork, UnitOfWork>();

builder.Services.AddDbContext<CatalogApiDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging(true)
);

builder.Services.AddMemoryCache(options => {
	// determina o tamanho em unidades do cache.
	options.SizeLimit = 5120;
});

builder.Services
	.AddControllers(options => options.Filters.Add<ApiExceptionFilter>())
	.AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
	.AddNewtonsoftJson();

builder.Services.AddAutoMapper(configuration => configuration.AddProfile<DtoMappingProfile>());

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("https://apirequest.io").WithMethods("GET", "POST").AllowAnyHeader().AllowCredentials()));

var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new InvalidConfigurationException("Missing secret key configuration!");

builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
	options.SaveToken = true;
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ClockSkew = TimeSpan.Zero,
		ValidAudience = builder.Configuration["JWT:ValidAudience"],
		ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
	};
});

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
	.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").RequireClaim("id", "romulo"))
	.AddPolicy("UserOnly", policy => policy.RequireRole("User"))
	.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "romulo") || context.User.IsInRole("SuperAdmin")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<CatalogApiDbContext>()
	.AddDefaultTokenProviders();

var app = builder.Build();

app.UseCors();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

// Configure the HTTP request pipeline.
app.MapControllers();
app.MapOpenApi();

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<CatalogApiDbContext>();

SeedDataBase.Populate(context);

app.Run();