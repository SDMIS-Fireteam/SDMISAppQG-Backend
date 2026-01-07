namespace SDMISAppQG.Interfaces.Hubs; 
public interface IGpsClient {
   Task ReceivePosition(Guid vehicleId, double lat, double lng);
}
