using Microsoft.EntityFrameworkCore;
using Npgsql;
using SDMISAppQG.Database;

namespace SDMISAppQG.Infrastructure.AppBuilder; 
public static class ServiceCollectionExtensions {
   public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration) {
      // Configuration de la base de donnée
      services.AddPostgresDatabase(configuration);
      return services;
   }

   private static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration) {
      string? connectionString = configuration.GetConnectionString("DefaultConnection");
      if (configuration is null)
         throw new InvalidOperationException("Connection string is missing from appsettings.json");
      NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
      // Utilisation de NewtonSoft pour les colonnes Json
      dataSourceBuilder.UseJsonNet();
      NpgsqlDataSource dataSource = dataSourceBuilder.Build();
      services.AddDbContext<AppDbContext>(options => {
         options.UseNpgsql(dataSource);
      });
      return services;
   }

   
}
