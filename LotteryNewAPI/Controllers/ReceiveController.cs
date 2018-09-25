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
    public class ReceiveController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        LotteryBlankDatabaseEntities context1;

        public ObservableCollection<Activate_Ticket> LotteryHistory { get; set; }
        public ObservableCollection<Receive_Inventory> ReceiveAndActiveCollection { get; set; }
        public ObservableCollection<Receive_Inventory> GetAutoPackid { get; set; }
        public object DAL { get; private set; }

        [Route("api/Receive/InsertInventoryRecord")]
        public HttpResponseMessage InsertInventoryRecord([FromBody] Receive_Inventory data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id  select s).ToList().LastOrDefault();
                var r = (from s in context.tblRecievedTickets where s.Packet_Id == data.Packet_No && s.IsDelete =="N" && s.Status=="Receive" && s.Store_Id == data.Store_Id  select s).ToList().FirstOrDefault();
                var a = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.State == data.State && s.Status == "Active" && s.Store_Id == data.Store_Id select s).ToList().FirstOrDefault();
                //var sld = (from s in context.tblSoldouts where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id select s).FirstOrDefault();
                if (r != null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else if(a != null)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }

                else
                {
                    var v = context.TicketMasters.Create();
                    v.Game_Id = data.Game_Id;
                    v.Created_On = System.DateTime.Now;
                    v.Modified_ON = System.DateTime.Now;
                    v.State = data.State;
                    context.TicketMasters.Add(v);
                    context.SaveChanges();

                    context1 = new LotteryBlankDatabaseEntities();
                    var v1 = context1.tblRecievedTickets.Create();
                    v1.Game_Id = data.Game_Id;
                    v1.Packet_Id = data.Packet_No;
                    v1.Ticket_Name = data.Ticket_Name;
                    v1.Price = data.Rate;
                    v1.Store_Id = data.Store_Id;
                    v1.ShiftID = Shift.ShiftID;
                    v1.Count = (Convert.ToInt32(data.End_No) - Convert.ToInt32(data.Start_No) + 1).ToString();
                    int rate = Convert.ToInt32(data.Rate);
                    int count = (Convert.ToInt32(data.End_No) - Convert.ToInt32(data.Start_No) + 1);
                    v1.Total_Price = (rate * count);
                    v1.Start_No = data.Start_No;
                    v1.Status = "Receive";
                    v1.State = data.State;
                    v1.End_No = data.End_No;
                    v1.Created_On = System.DateTime.Now;
                    v1.Modified_On = System.DateTime.Now;
                    v1.EmployeeId = data.EmployeeID;
                    v1.IsDelete = "N";
                    context1.tblRecievedTickets.Add(v1);
                    context1.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }

        }

        //[Route("api/Receive/GetReceiveHistory")]
        //public IEnumerable<Activate_Ticket> GetReceiveHistory()
        //{
        //    LotteryHistory = new ObservableCollection<Activate_Ticket>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var data = (from s in context.tblRecievedTickets where s.IsDelete == "N" && s.Status == "Receive" select s).ToList();
        //    foreach (var v in data)
        //    {
        //        LotteryHistory.Add(new Activate_Ticket
        //        {
        //            Game_Id = v.Game_Id,
        //            Created_Date = Convert.ToDateTime(v.Created_On),
        //            Packet_No = v.Packet_Id,
        //            Ticket_Name = v.Ticket_Name,
        //            Price = Convert.ToInt16(v.Price),
        //            Start_No = Convert.ToString(v.Start_No),
        //            State = v.State,
        //            End_No = Convert.ToString(v.End_No),
        //            Stopped_At = v.Stopped_At,
        //            Count = v.Count,
        //            Total_Price = v.Total_Price,
        //            EmployeeID = v.EmployeeId
        //        });
        //    }
        //    return LotteryHistory;
        //}

        [Route("api/Receive/NewGetActivateTicketTotalPrice")]
        public HttpResponseMessage NewGetActivateTicketTotalPrice([FromBody] Receive_Inventory data)
        {
            context = new LotteryBlankDatabaseEntities();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.EmployeeId == data.EmployeeID select s).ToList().LastOrDefault();

            int? count = 0;
            var c = (from t in context.tblRecievedTickets
                          where t.IsDelete == "N" && t.Status == "Receive" && t.Store_Id == data.Store_Id 
                          select t).ToList();
            // return count;
            foreach(var i in c)
            {
                
                    count = count + i.Total_Price;
                
            }
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        //[Route("api/Receive/GetReceiveBoxCount")]
        //public int GetReceiveBoxCount()
        //{
        //    context = new LotteryBlankDatabaseEntities();

        //    int activecount = (from t in context.tblRecievedTickets
        //                       where t.IsDelete == "N" && t.Status == "Receive" && t.Store_Id == 1
        //                       select t).Count();
        //    return activecount;
        //}

        [Route("api/Receive/NewGetReceiveBoxCount")]
        public HttpResponseMessage NewGetReceiveBoxCount([FromBody] Receive_Inventory data)
        {
            try
            {
                context = new LotteryBlankDatabaseEntities();
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.EmployeeId == data.EmployeeID select s).ToList().LastOrDefault();

                
                int activecount = (from t in context.tblRecievedTickets
                                   where t.IsDelete == "N" && t.Status == "Receive" && t.Store_Id == data.Store_Id
                                   select t).Count();
                //return activecount;
                

                return Request.CreateResponse(HttpStatusCode.OK, activecount);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, 0);
            }
        }

        [Route("api/Receive/DeleteSelectedRecord")]
        public HttpResponseMessage UpdateTicketStatus([FromBody] Activate_Ticket data)
        {
            context = new LotteryBlankDatabaseEntities();

            var v = (from s in context.tblRecievedTickets
                     where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id
                     select s).FirstOrDefault();
            if (v != null)
            {
                v.IsDelete = "Y";
                context.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK);

        }

        [Route("api/Receive/NewGetTotalPackets")]
        public HttpResponseMessage NewGetTotalPackets([FromBody] Receive_Inventory data)
        {
            context = new LotteryBlankDatabaseEntities();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.EmployeeId == data.EmployeeID select s).ToList().LastOrDefault();

            int? count = 0;
            var c = (from t in context.tblRecievedTickets
                          where t.IsDelete == "N" && t.Status == "Receive" && t.Store_Id == data.Store_Id
                          select t).ToList();
            // return count;

            foreach(var i in c)
            {
             
                    count = count + 1;
                   
            }
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [Route("api/Receive/GetReceiveAndActiveCollection")]
        public IEnumerable<Receive_Inventory> GetReceiveAndActiveCollection()
        {
            ReceiveAndActiveCollection = new ObservableCollection<Receive_Inventory>();
            context = new LotteryBlankDatabaseEntities();
            var receivelist = (from s in context.tblRecievedTickets
                               where s.Status == "Receive" || s.Status == "Active"
                               select s).ToList();

            foreach (var i in receivelist)
            {
                ReceiveAndActiveCollection.Add(new Receive_Inventory
                {
                    Game_Id = i.Game_Id,
                    Packet_No = i.Packet_Id,
                    Ticket_Name = i.Ticket_Name,
                    Rate = i.Price,
                    Status = i.Status,
                    State = i.State,
                    Start_No = i.Start_No.ToString(),
                    End_No = i.End_No.ToString(),
                });
            }


            return ReceiveAndActiveCollection;
        }

        [Route("api/Receive/NewGetReceiveHistory")]
        public HttpResponseMessage NewGetReceiveHistory([FromBody] Receive_Inventory data)
        {
            LotteryHistory = new ObservableCollection<Activate_Ticket>();
            context = new LotteryBlankDatabaseEntities();
            var result = (from s in context.tblRecievedTickets
                          where s.IsDelete == "N" && s.Status == "Receive" && s.Store_Id == data.Store_Id 
                          select s).ToList();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.EmployeeId == data.EmployeeID select s).ToList().LastOrDefault();

            foreach (var v in result)
            {
                    LotteryHistory.Add(new Activate_Ticket
                    {
                        Game_Id = v.Game_Id,
                        Created_Date = Convert.ToDateTime(v.Created_On),
                        Packet_No = v.Packet_Id,
                        Ticket_Name = v.Ticket_Name,
                        Price = Convert.ToInt16(v.Price),
                        Start_No = Convert.ToString(v.Start_No),
                        State = v.State,
                        End_No = Convert.ToString(v.End_No),
                        Stopped_At = v.Stopped_At,
                        Count = v.Count,
                        Total_Price = v.Total_Price,
                        EmployeeID = v.EmployeeId,
                        ShiftID = v.ShiftID,
                        Store_Id = v.Store_Id
                    });
            }
            //return LotteryHistory;
            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        }

    }
}
