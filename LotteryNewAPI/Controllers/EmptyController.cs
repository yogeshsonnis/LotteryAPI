using LotteryNewAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LotteryNewAPI.Controllers
{
    public class EmptyController : ApiController
    {
      LotteryBlankDatabaseEntities context;
        public ObservableCollection<Activation_Box> EmptyBoxCollection { get; set; }

        [Route("api/Empty/NewGetEmptyBoxCount")]
        public HttpResponseMessage NewGetEmptyBoxCount([FromBody] Settle_TicketInfo data)
        {
            context = new LotteryBlankDatabaseEntities();
            int empty = (from t in context.Box_Master
                         where t.Store_Id == data.Store_Id && (t.Status == "Empty" || t.Status == "Deactivated")
                         select t).Count();
            //return empty;
            return Request.CreateResponse(HttpStatusCode.OK, empty);
        }

        [Route("api/Empty/NewGetEmptyBoxCollection")]
        public HttpResponseMessage NewGetEmptyBoxCollection([FromBody] Settle_TicketInfo data)
        {
            context = new LotteryBlankDatabaseEntities();
            EmptyBoxCollection = new ObservableCollection<Activation_Box>();
            var v = (from s in context.Box_Master
                     where s.Store_Id == data.Store_Id && (s.Status == "Empty" || s.Status == "SoldOut" || s.Status == "Deactivated")
                     select s).ToList();
            foreach (var i in v)
            {
                EmptyBoxCollection.Add(new Activation_Box
                {
                    Box_No = Convert.ToInt32( i.Box_No)
                });
            }
            // return EmptyBoxCollection;
            return Request.CreateResponse(HttpStatusCode.OK, EmptyBoxCollection);
        }
    }
}
