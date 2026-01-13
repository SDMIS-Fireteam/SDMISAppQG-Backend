using SDMISAppQG.Models.DTOs;

namespace SDMISAppQG.Interfaces.Hubs;

public interface IGpsClient {
   Task ReceivePosition(VehicleDto vehicle);
}
