using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class TodoContextFactory : IDesignTimeDbContextFactory<TodoContext>
{
    /// <summary>
    /// Creates a new instance of the TodoContext for design-time usage.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A new TodoContext instance.</returns>
    public TodoContext CreateDbContext(string[] args)
    {
        var server = Environment.GetEnvironmentVariable("DB_SERVER");
        var db = Environment.GetEnvironmentVariable("DB_DB");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pw = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
        optionsBuilder.UseMySql(
            $"Server={server};Database={db};User={user};Password={pw};",
            new MySqlServerVersion(new Version(10, 5, 8))
        );

        return new TodoContext(optionsBuilder.Options);
    }
}