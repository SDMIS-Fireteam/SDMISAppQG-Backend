using Scalar.AspNetCore;
using SDMISAppQG.Infrastructure.AppBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
   .AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
   .AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
   app.MapOpenApi();
   app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
