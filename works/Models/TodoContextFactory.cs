using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class TodoContextFactory : IDesignTimeDbContextFactory<TodoContext>
{
    public TodoContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connStr = config.GetConnectionString("DefaultConnection")
        .Replace("{DB_USER}", Environment.GetEnvironmentVariable("DB_USER"))
        .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"));

        var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
        optionsBuilder.UseOracle(connStr);

        return new TodoContext(optionsBuilder.Options);
    }
}
