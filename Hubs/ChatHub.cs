using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.SignalR;

namespace AlanCart.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string message)
        {
            var user = Context.User?.Identity?.Name ?? "Guest";
            Clients.All.reveive(user, message);
        }
    }
}