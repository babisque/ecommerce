using Catalog.Core.DTO.Product;
using Catalog.Core.Logging;
using Catalog.Core.Repositories;
using Catalog.Core.Validators;
using Catalog.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
    opts.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IValidator<ProductPostReq>, ProductValidator>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();