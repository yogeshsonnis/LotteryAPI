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
    public class MasterInventoryController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        public ObservableCollection<Master_List_Inventory> MasterHistory { get; set; }

        [Route("api/MasterInventory/SaveNewInventory")]
        public HttpResponseMessage SaveNewInventory([FromBody] Master_List_Inventory data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var x= (from s in context.tblBarcode_Format where  s.State==data.State select s).FirstOrDefault();
                var p = (from s in context.tblLottery_Inventory where (s.GameID == data.Game_Id || s.TicketName==data.Ticket_Name) && s.State==data.State && s.Store_Id == data.Store_Id select s).FirstOrDefault();
                if (p != null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else if(x==null)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                else
                {
                    var v = context.tblLottery_Inventory.Create();
                    v.GameID = data.Game_Id;
                    v.TicketName = data.Ticket_Name;
                    v.Price = data.Rate;
                    v.State = data.State;
                    v.Start_No = data.Start_No;
                    v.End_No = data.End_No;
                    v.State = data.State;
                    v.Store_Id = Convert.ToInt32(data.Store_Id);
                    v.Employee_Id = Convert.ToInt32(data.Employee_Id);
                    int count = Convert.ToInt32(data.End_No) - Convert.ToInt32(data.Start_No) + 1;
                    v.Count = count.ToString();
                    v.Date = System.DateTime.Now;
                    context.tblLottery_Inventory.Add(v);
                    context.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        [Route("api/MasterInventory/GetMasterHistory")]
        public IEnumerable<Master_List_Inventory> GetMasterHistory()
        {
            MasterHistory = new ObservableCollection<Master_List_Inventory>();
            context = new LotteryBlankDatabaseEntities();
            var data =  context.tblLottery_Inventory.ToList();
            foreach (var v in data)
            {
                MasterHistory.Add(new Master_List_Inventory
                {
                    Game_Id = v.GameID,
                    // Packet_No=v.PacketID,
                    Ticket_Name = v.TicketName,
                    Rate = v.Price,
                    Start_No = v.Start_No,
                    End_No = v.End_No,
                    Count = v.Count,
                    State = v.State,
                    Date=v.Date,
                    Store_Id = Convert.ToInt32(v.Store_Id),
                    Employee_Id = Convert.ToInt32(v.Employee_Id),
                });
            }
            return MasterHistory;
        }

    }
}
