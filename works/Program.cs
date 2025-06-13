

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
.Replace("${DB_SERVER}", Environment.GetEnvironmentVariable("DB_SERVER"))
.Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT"))
.Replace("${DB_DB}", Environment.GetEnvironmentVariable("DB_DB"))
    .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER"))
    .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"));

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
builder.Services.AddSignalR()
    .AddStackExchangeRedis($"{redis_host}:{redis_port},password={redis_pw}");

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

app.MapGet("/health", () => Results.Ok("後端服務Healthy"))
.WithTags("Health")
.WithMetadata(new SwaggerOperationAttribute(summary: "健康狀態", description: "檢查後端服務狀態"));
app.Run();
