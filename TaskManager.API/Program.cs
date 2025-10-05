using System.Text.Json.Serialization;
using TaskManager.API.Models;
using TaskManager.API.Services;
using DotNetEnv;
using Microsoft.Extensions.Options;
using Redis.OM;
using TaskManager.API.Services.Repositories;

Env.Load("Mongo.env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));

builder.Services.AddSingleton<RedisConnectionProvider>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    return new RedisConnectionProvider($"redis://{settings.Host}/");
});

builder.Services.AddSingleton<RedisService>();
builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddSingleton<TasksService>();
builder.Services.AddHostedService<RedisIndexingService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"); });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();