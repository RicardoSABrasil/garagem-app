using Garagem.Application.DTOs;
using Garagem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garagem.Application.UseCases.Auth
{

	public class LoginUserHandler
	{
		private readonly IUserRepository _repo;

		public LoginUserHandler(IUserRepository repo)
		{
			_repo = repo;
		}

		public async Task<AuthResponse> Handle(LoginRequest request)
		{
			var user = await _repo.GetByEmailAsync(request.Email);

			if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
				throw new Exception("Credenciais inválidas");

			return new AuthResponse("fake-jwt-token");
		}
	}
}
