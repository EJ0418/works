

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddAuthentication("CookieAuth")
.AddCookie("CookieAuth", config =>
{
    config.Cookie.Name = "CRUD_API.Cookie";
    config.LoginPath = "/signin";
    config.LogoutPath = "/signout";

    config.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api") ||
                context.Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddScoped<RedisService>();

DotNetEnv.Env.Load();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
.Replace("${DB_SERVER}", Environment.GetEnvironmentVariable("DB_SERVER"))
.Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT"))
.Replace("${DB_DB}", Environment.GetEnvironmentVariable("DB_DB"))
    .Replace("${DB_USER}", "root")//Environment.GetEnvironmentVariable("DB_USER"))
    .Replace("${DB_PASSWORD}", "root_pw");//, Environment.GetEnvironmentVariable("DB_PASSWORD"));

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
Console.WriteLine(connectionString);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(opt =>
    {
        opt.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
    }
    );
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"SQL API");
        c.RoutePrefix = "doc";
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

// app.Urls.Add("http://localhost:4000");

// app.MapControllers();

app.MapGet("/health", () => Results.Ok("後端服務Healthy"));

app.Run();
