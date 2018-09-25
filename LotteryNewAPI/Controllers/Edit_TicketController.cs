using LotteryNewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LotteryNewAPI.Controllers
{
    public class Edit_TicketController : ApiController
    {
        Lottery_ApplicationEntities context;

        public HttpResponseMessage Edit_TicketDetails([FromBody] Activate_Ticket data)
        {
            using (context = new Lottery_ApplicationEntities())
            {
                //var v = context.Masters.Create();
                //v.Game_Id = data.Game_Id;
                //v.Created_On = System.DateTime.Now;
                //v.Modified_ON = System.DateTime.Now;
                //context.Masters.Add(v);
                //context.SaveChanges();
                var v = context.tblActivated_Tickets.SingleOrDefault(b => b.Game_Id == data.Game_Id);
                if (v != null)
                {
                    
                    v.Game_Id = data.Game_Id;
                    v.Packet_Id = data.Packet_No;
                    v.Status = "Active";
                    v.Box_No = data.Box_No;
                    v.Ticket_Name = data.Ticket_Name;
                    v.Price = data.Price;
                    v.Stopped_At = data.First_Ticket;
                    //context.tblActivated_Tickets.Add(v);
                    context.SaveChanges();

                    context = new Lottery_ApplicationEntities();
                    var v3 = (from s in context.tblRecievedTickets
                              where s.Game_Id == data.Game_Id
                              select s).FirstOrDefault();
                    //v3.Stopped_At = data.First_Ticket;
                    v3.Status = "Active";
                    v3.Game_Id = data.Game_Id;
                    v3.Packet_Id = data.Packet_No;
                    v3.Ticket_Name = data.Ticket_Name;
                    v3.Price = data.Price;
                    v3.Stopped_At = data.First_Ticket;
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            //return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
