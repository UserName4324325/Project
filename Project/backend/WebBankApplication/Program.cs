using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WebBankApplication.BackgroundServices;
using WebBankApplication.Data;
using WebBankApplication.Extensions;
using WebBankApplication.Repository;
using WebBankApplication.TokenService;

var builder = WebApplication.CreateBuilder(args);

 
// postgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

//ElasticsearchClient
var elasticSettings = builder.Configuration.GetSection("Elasticsearch");
var url = elasticSettings["Url"] ?? "http://localhost:9200";

var settings = new ElasticsearchClientSettings(new Uri(url));

var elasticClient = new ElasticsearchClient(settings);

builder.Services.AddSingleton(elasticClient);



// DI && Репозитории
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IDepositRepository, DepositRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRemittanceRepository, RemittanceRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();


// фоновые службы
builder.Services.AddHostedService<BankBackgroundService>();


// Контроллеры
builder.Services.AddControllers();


// JWT
builder.Services.AddIdentityServices(builder.Configuration);


// CORS
builder.Services.AddCorsServices(builder.Configuration);


var app = builder.Build();


// Migration
app.ApplyMigrations();


// Первичная синхронизация данных с Elasticsearch
await app.SeedElasticsearchDataAsync();


app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
