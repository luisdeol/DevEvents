using DevEvents.API.Domain.Repositories;
using DevEvents.API.Endpoints;
using DevEvents.API.Infrastructure.Notifications;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Infrastructure.Persistence.Repositories;
using DevEvents.API.Mappers;
using Hangfire;
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

builder.Services.AddScoped<INotificationService, EmailService>();

builder.Services.AddHangfire((s, config) =>
{
    config.UseSqlServerStorage(connectionString);
});

builder.Services.AddHangfireServer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app
    .AddConferenceEndpoints()
    .AddJobsEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.Run();