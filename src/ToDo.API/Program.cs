using Microsoft.OpenApi.Models;
using Serilog;
using ToDo.API.Endpoints;
using ToDo.API.Extensions;
using ToDo.Domain;
using ToDo.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddDomainServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidators();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo Api", Version = "v1" });
    c.DescribeAllParametersInCamelCase();
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabase();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapToDoEndpoints();

app.Run();

public partial class Program;