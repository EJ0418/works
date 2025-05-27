using System.Text;
using Log;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("CookieAuth")
.AddCookie("CookieAuth", opt =>
{
    opt.LoginPath = "/login";
    opt.AccessDeniedPath = "/denied";
});

builder.Services.AddAuthentication();

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "作業",
            Version = "v1"
        });
        options.EnableAnnotations();
    }
);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "作業 API V1");
        c.RoutePrefix = "doc";
    });    
}
else
{
    app.UseExceptionHandler("/error");
}

// app.UseHttpsRedirection();

app.Urls.Add("http://localhost:4000");

app.UseStaticFiles();   // 預設會對應 wwwroot 資料夾

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.UseMiddleware<LogMiddleware>();

app.Map("/log", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        await context.Response.WriteAsync("Log in middleware map.\n");
    });
});

// app.MapControllers();

app.UseStatusCodePages(async context =>
{
    context.HttpContext.Response.ContentType = "text/plain";
    await context.HttpContext.Response.WriteAsync(
        $" {context.HttpContext.Response.StatusCode}\n");
});

app.Run();