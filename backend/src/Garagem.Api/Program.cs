using System.Text;

using Amazon;
using Amazon.S3;
using Garagem.Application.Services;
using Garagem.Application.UseCases.Auth;
using Garagem.Application.UseCases.Profile;
using Garagem.Domain.Interfaces;
using Garagem.Infrastructure.Persistence;
using Garagem.Infrastructure.Storage;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
// OpenAPI types removed to avoid package mismatch in this environment

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

// Registra os handlers de autenticação
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<LoginUserHandler>();

// Registra os handlers de perfil
builder.Services.AddScoped<GetCurrentUserHandler>();
builder.Services.AddScoped<UpdateProfileHandler>();
builder.Services.AddScoped<UploadProfileImageHandler>();

// Registra o serviço JWT
builder.Services.AddScoped<JwtTokenGenerator>();

// Configuração do MinIO/S3
var minioEndpoint = builder.Configuration["Minio:Endpoint"]
	?? throw new Exception("Minio:Endpoint não configurado");
var minioAccessKey = builder.Configuration["Minio:AccessKey"]
	?? throw new Exception("Minio:AccessKey não configurado");
var minioSecretKey = builder.Configuration["Minio:SecretKey"]
	?? throw new Exception("Minio:SecretKey não configurado");

// Registra o cliente S3 do AWS SDK (compatível com MinIO)
builder.Services.AddScoped<IAmazonS3>(sp =>
{
	var config = new AmazonS3Config
	{
		ServiceURL = minioEndpoint,
		ForcePathStyle = true,
		SignatureVersion = "4"
	};

	return new AmazonS3Client(minioAccessKey, minioSecretKey, config);
});

// Registra o serviço de armazenamento
builder.Services.AddScoped<IStorageService, MinioStorageService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy
			.WithOrigins("http://localhost:4200")
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new()
	{
		Title = "Garagem API",
		Version = "v1"
	});
});

var jwtKey = builder.Configuration["Jwt:Key"]
			 ?? throw new Exception("Jwt:Key não configurado");

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],

			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(jwtKey))
		};
	});

builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();