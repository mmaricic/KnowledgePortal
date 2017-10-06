using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using IEP_portal.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Security;
using IEP_portal.Controllers;
using System.Collections.Generic;

namespace IEP_portal.Hubs
{
    [HubName("QuestionHub")]
    public class QuestionHub : Hub
    {
      public override Task OnConnected()
        {
            using (var db = new DatabaseModel())
            {
                var user = Context.User.Identity.GetUserId();
                var groups = db.AspNetUsers.Include(c => c.Subscribed).Where(x => x.Id == user).FirstOrDefault();

              
                    // Add to each assigned group.
                    foreach (var item in groups.Subscribed)
                    {
                        Groups.Add(Context.ConnectionId, "Channel "+item.Channelid);
                    }
                }
            
            return base.OnConnected();
        }

        [CustomAuthorize(Roles="Teacher")]
        public void SendQuestions(int id, int [] selected)
        {
           
            Clients.Group("Channel "+id).newQuestions();
        }

       
    }
}