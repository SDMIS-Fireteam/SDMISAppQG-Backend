using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Scalar.AspNetCore;
using SDMISAppQG.Hubs;
using SDMISAppQG.Infrastructure.AppBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
   .AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
   .AddNewtonsoftJson(options =>
   {
       // Configuration pour NetTopologySuite (géométries PostGIS)
       var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
       var geoJsonSerializer = GeoJsonSerializer.Create(geometryFactory);
       foreach (var converter in geoJsonSerializer.Converters)
       {
           options.SerializerSettings.Converters.Add(converter);
       }
   });

// Configuration de l'authentification JWT avec Keycloak
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       var keycloakConfig = builder.Configuration.GetSection("Keycloak");

       options.Authority = keycloakConfig["Authority"];
       options.Audience = keycloakConfig["Audience"];
       options.RequireHttpsMetadata = keycloakConfig.GetValue<bool>("RequireHttpsMetadata");

       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = keycloakConfig.GetValue<bool>("ValidateIssuer"),
           ValidateAudience = keycloakConfig.GetValue<bool>("ValidateAudience"),
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ClockSkew = TimeSpan.Zero
       };

       options.Events = new JwtBearerEvents
       {
           OnAuthenticationFailed = context =>
           {
               Console.WriteLine($"Authentication failed: {context.Exception.Message}");
               return Task.CompletedTask;
           },
           OnTokenValidated = context =>
           {
               Console.WriteLine("Token validated successfully");
               return Task.CompletedTask;
           }
       };
   });

builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuration SignalR avec logging détaillé
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddAppServices(builder.Configuration);

// Configuration CORS (indispensable pour que React puisse se connecter)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClientPermission", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Permet toutes les origines (React + Python)
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials(); // Nécessaire pour SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("ReactClientPermission");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<GpsHub>("/hubs/gps");
app.MapHub<TelemetryHub>("/hubs/telemetry");

await app.RunAsync();
