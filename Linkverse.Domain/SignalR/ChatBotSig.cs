using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Linkverse.Domain.SignalR
{
    public class ChatBotSig : Hub
    {
        public async Task SendTaskUpdate(string message)
        {
            await Clients.All.SendAsync("RecieveTaskUpdate", message);
        }
    }
}
