using GameServer.Data;
using GameServer.Entities;
using GameServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); //MVC의 Controller가 만들어진다.

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "Casual RPG Game API server";
        document.Info.Version = "0.0.1";
        document.Info.Description = "캐쥬얼 RPG 서버 REST API";
        //원래 이건 비동기 작업을 위한 람다식인데 우리가 지금 한건 비동기 작업이 없기 때문에 
        //그냥 빈 태스크를 리턴하면 된다.
        return Task.CompletedTask;
    });
});

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 11)));
});

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CasualRPG Server");
    });
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Server is running!");
app.MapControllers(); //라우팅에 엔드포인트를 추가해주는 작업을 한다.

app.Run();
