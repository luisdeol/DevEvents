using Azure.Storage.Blobs;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Endpoints;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Infrastructure.Persistence.Repositories;
using DevEvents.API.Infrastructure.Storage;
using DevEvents.API.Mappers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
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

builder.Services.RegisterMaps();

builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();
builder.Services.AddScoped<IAttendeeRepository, AttendeeRepository>();

var storageAccountConnectionString = builder.Configuration.GetConnectionString("StorageAccount");

builder.Services.AddSingleton(o => new BlobServiceClient(storageAccountConnectionString));

builder.Services.AddScoped<IStorageService, BlobStorageService>();

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