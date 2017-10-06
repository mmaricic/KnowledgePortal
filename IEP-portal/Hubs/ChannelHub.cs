using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using IEP_portal.Models;

namespace IEP_portal.Hubs
{
    public class ChannelHub : Hub
    {
        public void ActivateChannel(Channel channel)
        {
            Clients.All.addChannel(channel);
        }

        public void DeactivateChannel(Channel channel)
        {
            Clients.All.removeChannel(channel);
        }
    }
}