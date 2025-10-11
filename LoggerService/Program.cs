using DotNetEnv;
using LoggerService;
using Messaging;

Env.Load("RabbitMQ.env");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<RabbitMQService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();