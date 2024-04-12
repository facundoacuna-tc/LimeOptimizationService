using System.Reflection;
using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using Newtonsoft.Json;
using LOS.Common.Settings;
using LimeOptimizationService.Token;
using LOS.Data.Context;
using LOS.Services.Interfaces;
using LOS.Service;
using LOS.Services;
using LOS.Common.ExtensionMethods;
using LOS.Services.ImporterManager.Implementation;
using LOS.Services.ImporterManager.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using LOS.Data.ADO.Implementation;
using LOS.Data.ADO.Interface;
using LOS.Data.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
        options.SerializerSettings.Formatting = Formatting.Indented;
    });

builder.Services
    .AddAuthentication("tokenAuth")
    .AddScheme<TokenAuthenticationSchemeOptions, TokenAuthenticationService>("tokenAuth", ops => { });

var connection = builder.Configuration.GetConnectionString(appSettings.UseDatabase);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

//Add Dependency Injection
builder.Services.AddScoped<IMigratorService, MigratorService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddTransient<ICSVDataImporter, CSVDataImporterManager>();
builder.Services.AddTransient<ICSVDataReader, CSVDataReader>();
builder.Services.AddTransient<IDatabaseConnection, DatabaseConnection>();


string migrationAssemblyPath = Path.Combine(appSettings.ExecutingAssembly.Location.LeftOfRightmostOf("\\"), appSettings.MigrationAssembly);
Assembly migrationAssembly = Assembly.LoadFrom(migrationAssemblyPath);


builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(connection)
        .ScanIn(migrationAssembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
