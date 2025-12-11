using Scalar.AspNetCore;
using SDMISAppQG.Hubs;
using SDMISAppQG.Infrastructure.AppBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
   .AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
   .AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddAppServices(builder.Configuration);

// Configuration CORS (indispensable pour que React puisse se connecter)
builder.Services.AddCors(options => {
   options.AddPolicy("ReactClientPermission", policy => {
      policy.WithOrigins("http://localhost:5173") // L'URL de votre app React
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Important pour SignalR
   });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
   app.MapOpenApi();
   app.MapScalarApiReference();
} else {
   app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.UseCors("ReactClientPermission");

app.MapHub<GpsHub>("/hubs/gps");

await app.RunAsync();
