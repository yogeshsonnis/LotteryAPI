using LotteryNewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LotteryNewAPI.Controllers
{
    public class DeleteReceiveController : ApiController
    {
        Lottery_ApplicationEntities context = new Lottery_ApplicationEntities();
        public HttpResponseMessage UpdateTicketStatus([FromBody] Activate_Ticket data)
        {
            //string id = data.Game_Id;
            var v = (from s in context.tblRecievedTickets
                     where s.Game_Id == data.Game_Id
                     select s).FirstOrDefault();
            if (v != null)
            {
                v.IsDelete = "Y";
                context.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK);

        }
    }
}
