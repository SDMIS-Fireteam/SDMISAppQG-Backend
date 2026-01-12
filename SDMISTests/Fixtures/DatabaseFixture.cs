using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;

namespace SDMISTests.Fixtures;

public class DatabaseFixture : IDisposable {
   public AppDbContext Context { get; private set; }

   public DatabaseFixture() {
      // Load .env file from the solution root
      // Assuming the tests run in bin/Debug/net10.0, we need to go up to find the .env
      var root = Directory.GetCurrentDirectory();
      var envPath = Path.Combine(root, "../../../../.env");

      if (File.Exists(envPath)) {
         DotNetEnv.Env.Load(envPath);
      }

      var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "sdmis_db";
      var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "sdmis";
      var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "sdmis";
      var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
      var host = "localhost"; // Local tests assume localhost

      var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
      optionsBuilder.UseNpgsql(connectionString,
          o => o.UseNetTopologySuite());

      Context = new AppDbContext(optionsBuilder.Options);

      // Ensure database exists (optional, but good for local checks)
      // Context.Database.EnsureCreated(); // Do NOT use this on production/shared DBs blindly, but fine for local dev if carefully used.
      // Since we use existing local DB, we assume it's migrated.
   }

   public void Dispose() {
      Context.Dispose();
   }
}
