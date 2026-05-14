using Garagem.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Garagem.Infrastructure.Persistence;

public class AppDbContextFactory
	: IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

		// Try to load connection string from appsettings.json.
		// When running design-time tools the current directory may be the project folder for this project,
		// so also check the Garagem.Api project where appsettings.json lives.
		var basePath = Directory.GetCurrentDirectory();
		string? appSettingsPath = Path.Combine(basePath, "appsettings.json");
		if (!File.Exists(appSettingsPath))
		{
			appSettingsPath = Path.Combine(basePath, "..", "Garagem.Api", "appsettings.json");
		}
		if (!File.Exists(appSettingsPath))
		{
			appSettingsPath = Path.Combine(basePath, "..", "..", "src", "Garagem.Api", "appsettings.json");
		}
		if (!File.Exists(appSettingsPath))
		{
			throw new FileNotFoundException("Could not find appsettings.json to read the DefaultConnection string.", appSettingsPath);
		}

		var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
			.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: false)
			.Build();

		var connectionString = config.GetConnectionString("DefaultConnection")
				?? config["ConnectionStrings:DefaultConnection"]
				?? throw new Exception("DefaultConnection not found in appsettings.json");

		optionsBuilder.UseSqlServer(connectionString);

		return new AppDbContext(optionsBuilder.Options);
	}
}