using Microsoft.AspNetCore.SignalR;

namespace VelastoProductionSystem.Hubs
{
    public class DashboardHub : Hub
    {
        // Hub can be empty if we only use it for broadcasting from controllers
        public async Task NotifyUpdate()
        {
            await Clients.All.SendAsync("ReceiveUpdate");
        }
    }
}
