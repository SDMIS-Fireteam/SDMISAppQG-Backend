
using Microsoft.AspNetCore.SignalR;
using SDMISAppQG.Hubs;
using SDMISAppQG.Interfaces.Hubs;

namespace SDMISAppQG.Infrastructure.Workers; 
public class GpsWorker(IHubContext<GpsHub, IGpsClient> hubContext) : BackgroundService {
   private readonly IHubContext<GpsHub, IGpsClient> _hubContext = hubContext;

   protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      var random = new Random();
      while (!stoppingToken.IsCancellationRequested) {
         double lat = 18.3004292 + (random.NextDouble() * 0.001);
         double lng = -64.827866 + (random.NextDouble() * 0.001);

         await _hubContext.Clients.All.ReceivePosition(lat, lng);
         await Task.Delay(2000, stoppingToken);
      }
   }
}
