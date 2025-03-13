using DevEvents.API.Domain.Repositories;
using DevEvents.API.Endpoints;
using DevEvents.API.Infrastructure.Caching;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Infrastructure.Persistence.Repositories;
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

var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfig>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig.Configuration;
    options.InstanceName = redisConfig.InstanceName;
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.RegisterMaps();

builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();
builder.Services.Decorate<IConferenceRepository, CachedConferenceRepository>();

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