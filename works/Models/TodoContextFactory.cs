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
        var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Database=tododb;User=ej;Password=ej_pw;",
            new MySqlServerVersion(new Version(10, 5, 8))
        );

        return new TodoContext(optionsBuilder.Options);
    }
}