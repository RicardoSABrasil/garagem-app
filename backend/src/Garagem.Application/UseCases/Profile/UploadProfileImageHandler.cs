using Garagem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garagem.Application.UseCases.Profile
{

	public class UploadProfileImageHandler
	{
		private readonly IUserRepository _repo;

		public UploadProfileImageHandler(IUserRepository repo)
		{
			_repo = repo;
		}

		public async Task Handle(Guid userId, string imageUrl)
		{
			var user = await _repo.GetByIdAsync(userId);
			if (user == null)
				throw new Exception("Usuário não encontrado");

			user.UpdateProfileImage(imageUrl);

			await _repo.UpdateAsync(user);
		}
	}
}
