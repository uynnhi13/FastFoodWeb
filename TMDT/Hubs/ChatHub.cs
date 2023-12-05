using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;
using TMDT.Hubs;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TMDT.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        public void Send(string user_id, string message,int decentral)
        {
            MongoDBconnection db = new MongoDBconnection();

            DateTime dateTime = DateTime.Now;

            db.AddCollectionMessage(new Bmessage(user_id, message, dateTime, decentral));

            Clients.All.broadcastMessage(user_id, message, dateTime, decentral);
        }
    }
}