using Garagem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garagem.Infrastructure.Persistence
{
	public class AppDbContext : DbContext
	{
		public DbSet<User> Users => Set<User>();

		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }
	}
}
