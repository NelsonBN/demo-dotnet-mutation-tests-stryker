using Demo.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Demo.Application;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddApplicationLayer()
    .AddInfrastructureLayer();

var app = builder.Build();

app.MapProductsEndpoints();

app.Run();

public partial class Program;
