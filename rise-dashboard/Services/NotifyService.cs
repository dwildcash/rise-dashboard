using Microsoft.AspNetCore.SignalR;
using rise.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rise.Services
{
    public class NotifyService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotifyService(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task SendNotificationAsync(string message)
        {
             await _hub.Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
