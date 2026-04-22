using System;
using System.Collections.Generic;
using System.Text;

namespace Garagem.Domain.Entities
{
	public class User
	{
		public Guid Id { get; private set; }
		public string Email { get; private set; } = string.Empty;
		public string PasswordHash { get; private set; } = string.Empty;
		public string? ProfileImageUrl { get; private set; }

		public DateTime CreatedAt { get; private set; }

		private User() { }

		public User(string email, string passwordHash)
		{
			Id = Guid.NewGuid();
			Email = email;
			PasswordHash = passwordHash;
			CreatedAt = DateTime.UtcNow;
		}

		public void UpdateProfileImage(string url)
		{
			ProfileImageUrl = url;
		}
	}
}
