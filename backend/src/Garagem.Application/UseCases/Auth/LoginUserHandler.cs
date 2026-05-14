using Garagem.Application.DTOs;
using Garagem.Application.Services;
using Garagem.Domain.Interfaces;

namespace Garagem.Application.UseCases.Auth;

public class LoginUserHandler
{
	private readonly IUserRepository _repo;
	private readonly JwtTokenGenerator _jwt;

	public LoginUserHandler(
		IUserRepository repo,
		JwtTokenGenerator jwt)
	{
		_repo = repo;
		_jwt = jwt;
	}

	public async Task<AuthResponse> Handle(LoginRequest request)
	{
		var user = await _repo.GetByEmailAsync(request.Email);

		if (user == null ||
			!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
		{
			throw new Exception("Credenciais inválidas");
		}

		var token = _jwt.Generate(user);

		return new AuthResponse(token);
	}
}