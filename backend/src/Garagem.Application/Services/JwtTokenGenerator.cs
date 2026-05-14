using Garagem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Garagem.Application.Services;

public class JwtTokenGenerator
{
	private readonly IConfiguration _configuration;

	public JwtTokenGenerator(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Generate(User user)
	{
		var key = _configuration["Jwt:Key"]!;

		var securityKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(key));

		var credentials = new SigningCredentials(
			securityKey,
			SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim(ClaimTypes.Name, user.Email)
		};

		var token = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}