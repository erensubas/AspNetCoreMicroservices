using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var ass = Assembly.GetExecutingAssembly();
builder.Services.AddAutoMapper(ass);

IConfiguration configuration = builder.Configuration;
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.MigrateDatabase<OrderContext>((context, services) =>
 {
     var logger = services.GetService<ILogger<OrderContextSeed>>();
     OrderContextSeed
         .SeedAsync(context, logger)
         .Wait();
 });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
