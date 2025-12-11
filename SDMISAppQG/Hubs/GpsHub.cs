using Microsoft.AspNetCore.SignalR;
using SDMISAppQG.Interfaces.Hubs;

namespace SDMISAppQG.Hubs;

public class GpsHub : Hub<IGpsClient>;
