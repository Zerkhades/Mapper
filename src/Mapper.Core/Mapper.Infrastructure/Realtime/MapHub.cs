using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Mapper.Infrastructure.Realtime
{
    public class MapHub : Hub
    {
        public Task JoinMap(string mapId)
            => Groups.AddToGroupAsync(Context.ConnectionId, Group(mapId));

        public Task LeaveMap(string mapId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, Group(mapId));

        public static string Group(string mapId) => $"map:{mapId}";
    }
}
