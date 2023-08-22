using Serilog;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using SerilogAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Host.UseSerilog((hostingContext, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
    
});


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

