using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Services;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// DbContext correto
builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

// Repositório genérico (se for o caso)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Serviço de e-mail
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("MyPolicy");

app.MapGet("/", () => Results.Ok("API is healthy!"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();