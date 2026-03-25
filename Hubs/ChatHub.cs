using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR.Infrastructure;

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
            //System.Diagnostics.Debug.WriteLine(Context.ConnectionId);
            //System.Diagnostics.Debug.WriteLine(Context.User.Identity.Name);

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
        public void StartPrivateConversation(string strtargetuserid)
        {
            int.TryParse(strtargetuserid, out int targetuserid);
            int.TryParse(Context.User.Identity.Name, out int userid);
            System.Diagnostics.Debug.WriteLine("targetuserid:" + strtargetuserid);
            System.Diagnostics.Debug.WriteLine("me:" + Context.User.Identity.Name);

            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                //int conversationid = (from s in db.ConversationMember where s.UserId == targetuserid || s.UserId == userid group s by s.ConversationId into g where g.Count() == 2 && g.Any(x => x.UserId == targetuserid) && g.Any(x => x.UserId == userid) select g.Key).FirstOrDefault();
                int conversationid = (from s in db.Conversation where s.ConversationMember.Count() == 2 && s.ConversationMember.Any(m => m.UserId == userid) && s.ConversationMember.Any(m => m.UserId == targetuserid) select s.Id).FirstOrDefault();
                //找出conversationmember中conversationid=conversation.id且userid等於我設的，然後總數是兩個的資料
                if(conversationid == default(int))  //沒找到conversationid就自己新增
                {
                    System.Diagnostics.Debug.WriteLine("not found");
                    Models.Conversation newconversation = new Models.Conversation();
                    newconversation.CreatedAt = DateTime.Now;
                    db.Conversation.Add(newconversation);
                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("Id:" + newconversation.Id);
                    Models.ConversationMember cm1 = new Models.ConversationMember();
                    cm1.ConversationId = newconversation.Id;
                    cm1.UserId = userid;
                    cm1.IsPrivateMessage = true;
                    Models.ConversationMember cm2 = new Models.ConversationMember();
                    cm2.ConversationId = newconversation.Id;
                    cm2.UserId = targetuserid;
                    cm2.IsPrivateMessage = true;
                    db.ConversationMember.Add(cm1);
                    db.ConversationMember.Add(cm2);
                    db.SaveChanges();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("found id = "+ conversationid);
                    
                }
                Clients.Caller.receiveConversationId(conversationid);
                CheckHistory(conversationid,userid);
            }
        }

        public void CheckHistory(int conversationid,int userid)
        {
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.ChatMessage> listChatMessage = (from s in db.ChatMessage where s.ConversationId == conversationid select s).ToList();
                foreach(var chatMessage in listChatMessage)
                {
                    if(chatMessage.SenderId == userid)
                    {
                        Clients.Caller.updateHistory(chatMessage.Message);
                    }
                    else
                    {
                        Clients.Caller.receive(chatMessage.UserData.Nickname,chatMessage.Message);
                    }
                }
            }
        }
        /*public void Send(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);

            //Clients.Caller.receive("SERVER", message);
            Clients.All.receive("SERVER", message);
            ///Clients.Caller.reveice(message);
        }*/
        public void SendTo(string strconversationid,string message)
        {

            int.TryParse(Context.User.Identity.Name, out int calleruserid);
            int.TryParse(strconversationid, out int conversationid);
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                string callername;
                Models.UserData userdata = (from s in db.UserData where s.Id == calleruserid select s).FirstOrDefault();
                if(userdata != default(Models.UserData)){
                    callername = userdata.Nickname;
                    List<Models.UserData> listud = (from s in db.ConversationMember where s.ConversationId == conversationid && s.UserId != calleruserid select s.UserData).ToList();
                    foreach(Models.UserData receivers in listud){
                        if (_connections.TryGetValue(receivers.Id.ToString(), out var set))
                        {
                            foreach (var connectionid in set)
                            {
                                Clients.Client(connectionid).receive(callername, message);
                                Models.ChatMessage chatmessage = new Models.ChatMessage();
                                chatmessage.ConversationId = conversationid;
                                chatmessage.SenderId = calleruserid;
                                chatmessage.Message = message;
                                chatmessage.CreatedAt = DateTime.Now;
                                db.ChatMessage.Add(chatmessage);
                                db.SaveChanges();
                                //Clients.Caller.receive(callername, message);
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("NotFound");
                            Clients.Caller.receive(callername, "target is offline");
                        }
                    }
                }

            }
        }

    }
}