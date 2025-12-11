namespace SDMISAppQG.Interfaces.Hubs; 
public interface IGpsClient {
   Task ReceivePosition(double lat, double lng);
}
