using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Interfaces.Hubs;

public interface IGpsClient {
   Task ReceivePosition(VehicleEntity vehicle);
}
