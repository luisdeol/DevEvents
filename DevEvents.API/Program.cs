using DevEvents.API.Domain.Repositories;
using DevEvents.API.Endpoints;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Infrastructure.Persistence.Repositories;
using DevEvents.API.Mappers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("AppDb");

builder.Services
    .AddDbContext<AppDbContext>(o => 
        o.UseSqlServer(connectionString)
    );

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

var outputTemplate = "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";

var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "Logs",
    AutoCreateSqlTable = true
};

var columnOptions = new ColumnOptions();
columnOptions.Store.Add(StandardColumn.LogEvent);
columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.LogEvent.DataLength = 2048;
columnOptions.Id.DataType = SqlDbType.BigInt;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .WriteTo.File(
        "logs/logs.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: outputTemplate
    )
    .WriteTo.MSSqlServer(
        connectionString: connectionString,
        sinkOptions: sinkOptions,
        columnOptions: columnOptions
    )
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.RegisterMaps();

builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app
    .AddConferenceEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();