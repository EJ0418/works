

using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using works.Hubs;

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

var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_DB");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var sqlConnStr = $"server={dbServer};port={dbPort};database={dbName};user={dbUser};password={dbPassword};";

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseMySql(sqlConnStr, ServerVersion.AutoDetect(sqlConnStr)));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

var redis_host = Environment.GetEnvironmentVariable("REDIS_HOST");
var redis_port = Environment.GetEnvironmentVariable("REDIS_PORT");
var redis_pw = Environment.GetEnvironmentVariable("REDIS_PASSWORD");
var redisConnStr = $"{redis_host}:{redis_port},password={redis_pw}";
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnStr);

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"WebSocket API");
        c.RoutePrefix = "doc";
    });
}


app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.MapHub<ChatHub>("/chathub", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});

app.MapHealthChecks("/health");

app.Run();
