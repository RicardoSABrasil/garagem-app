using Garagem.Application.DTOs;
using Garagem.Domain.Entities;
using Garagem.Domain.Interfaces;

namespace Garagem.Application.UseCases.Auth
{
	public class RegisterUserHandler
	{
		private readonly IUserRepository _repo;

		public RegisterUserHandler(IUserRepository repo)
		{
			_repo = repo;
		}

		public async Task Handle(RegisterRequest request)
		{
			var existing = await _repo.GetByEmailAsync(request.Email);
			if (existing != null)
				throw new Exception("Usuário já existe");

			var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

			var user = new User(
								request.FirstName,
								request.LastName,
								request.Email,
								hash);

			await _repo.AddAsync(user);
		}
	}
}
