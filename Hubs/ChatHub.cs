using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;

namespace AlanCart.Hubs
{
    public class ChatHub : Hub
    {
        private readonly static ConcurrentDictionary<string, HashSet<string>> _connections
            = new ConcurrentDictionary<string, HashSet<string>>();
        public override Task OnConnected()
        {
            string userid = Context.User.Identity.Name;
            string connid = Context.ConnectionId;
            var set = _connections.GetOrAdd(userid, _ => new HashSet<string>());
            lock (set)
            {
                set.Add(connid);
            }
            System.Diagnostics.Debug.WriteLine(Context.ConnectionId);
            System.Diagnostics.Debug.WriteLine(Context.User.Identity.Name);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userid = Context.User.Identity.Name;
            string connid = Context.ConnectionId;
            if(_connections.TryGetValue(userid, out var set))
            {
                lock (set)
                {
                    set.Remove(connid);
                    if(set.Count == 0)
                    {
                        _connections.TryRemove(userid, out _);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
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