using Garagem.Application.UseCases.Auth;
using Garagem.Application.UseCases.Profile;
using Garagem.Domain.Interfaces;
using Garagem.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseInMemoryDatabase("db"));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<LoginUserHandler>();
builder.Services.AddScoped<UploadProfileImageHandler>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger (só em dev normalmente)
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();