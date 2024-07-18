using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using TrendDataBackend.Models;
using TrendDataBackend.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.Bind("AzureStorage", new AzureStorageOptions());

// Register services
builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection("AzureStorage"));
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });
}

var app = builder.Build();

builder.Configuration.GetSection("AzureStorage");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeviceApi");
    });
}
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll"); 
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
