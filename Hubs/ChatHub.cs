using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.SignalR;

namespace AlanCart.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnected()
        {
            System.Diagnostics.Debug.WriteLine(Context.ConnectionId);
            System.Diagnostics.Debug.WriteLine(HttpContext.Current.Session["Id"].ToString());
            return base.OnConnected();
        }

        public void Send(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);

            //Clients.Caller.receive("SERVER", message);
            Clients.All.receive("SERVER", message);
            ///Clients.Caller.reveice(message);
        }
    }
}