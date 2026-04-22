using Garagem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garagem.Domain.Interfaces
{
	public interface IUserRepository
	{
		Task AddAsync(User user);
		Task<User?> GetByEmailAsync(string email);
		Task<User?> GetByIdAsync(Guid id);
		Task UpdateAsync(User user);
	}
}
