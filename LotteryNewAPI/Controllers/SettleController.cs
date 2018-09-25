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
    public class SettleController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        public ObservableCollection<Settle_TicketInfo> LotteryHistory { get; set; }

        [Route("api/Settle/GetSettleBoxCount")]
        public int GetSettleBoxCount()
        {
            context = new LotteryBlankDatabaseEntities();
            int settle = (from t in context.tblSettleTickets select t).Count();
            return settle;
        }

        [Route("api/Settle/Settle_Ticket")]
        public HttpResponseMessage Settle_Ticket([FromBody] Settle_TicketInfo data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var ShiftInfo = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var h = (from s in context.tblActivated_Tickets where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.ShiftID==ShiftInfo.ShiftID select s).ToList(). FirstOrDefault();
                if (h != null)
                {
                    if (h.End_No != data.End_No)
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }
                }

                //if (!context.tblActivated_Tickets.Any(s => s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No))
                //{
                //    return Request.CreateResponse(HttpStatusCode.BadRequest);
                //}


                var p = (from s in context.tblSettleTickets where s.Box_No == data.Box_No && s.Packet_Id == data.Packet_No && s.ShiftID == ShiftInfo.ShiftID select s).FirstOrDefault();
                if (p != null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (ShiftInfo != null)
                {
                        var v = context.tblSettleTickets.Create();
                        v.Game_Id = data.Game_Id;
                        v.Packet_Id = data.Packet_No;
                        v.Ticket_Name = data.Ticket_Name;
                        v.Price = data.Price;
                        v.Box_No = data.Box_No;
                        v.Status = "Settle";
                        v.Store_Id = data.Store_Id;
                        v.State = data.State;
                        v.PackPosition_Open = data.Start_No;
                        v.PackPosition_Close = data.End_No;
                        v.EmployeeId = data.EmployeeID;
                        v.ShiftID = ShiftInfo.ShiftID;
                     //   v.State = data.State;
                        //v.Modified_On = data.Modified_Date;
                        v.Created_On = ShiftInfo.Date;
                        context.tblSettleTickets.Add(v);
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v2 = (from s in context.Box_Master
                                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                                  select s).FirstOrDefault();
                        if (v2 != null)
                        {
                            v2.Status = "Settle";
                            context.SaveChanges();
                        }

                        context = new LotteryBlankDatabaseEntities();
                        var v3 = (from s in context.tblRecievedTickets
                                  where s.Packet_Id == data.Packet_No
                                  select s).FirstOrDefault();
                        if (v3 != null)
                        {
                            context.tblRecievedTickets.Attach(v3);
                            context.tblRecievedTickets.Remove(v3);
                            context.SaveChanges();
                        }
                }
                

                return Request.CreateResponse(HttpStatusCode.OK);
                //}
                //else
                //{
                //    return Request.CreateResponse(HttpStatusCode.Conflict);
                //}
            }


        }

        //[Route("api/Settle/GetSettleHistory")]
        //public IEnumerable<Settle_TicketInfo> GetHistory()
        //{
        //    LotteryHistory = new ObservableCollection<Settle_TicketInfo>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var data = (from s in context.tblSettleTickets where s.Status == "Settle" && s.Created_On == System.DateTime.Today select s).ToList();
        //    foreach (var v in data)
        //    {
        //        LotteryHistory.Add(new Settle_TicketInfo
        //        {
        //            Game_Id = v.Game_Id,
        //            Created_Date = Convert.ToDateTime(v.Created_On),
        //            Packet_No = v.Packet_Id,
        //            Ticket_Name = v.Ticket_Name,
        //            Price = v.Price,
        //            Start_No = v.PackPosition_Open,
        //            End_No = v.PackPosition_Close,
        //            EmployeeID = v.EmployeeId,
        //            State = v.State
        //        });
        //    }

        //    return LotteryHistory;
        //}
        [Route("api/Settle/Unsettle")]
        public HttpResponseMessage Unsettle([FromBody] Settle_TicketInfo data)
        {
            context = new LotteryBlankDatabaseEntities();

            var ShiftInfo = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

            var v1 = (from s in context.tblSettleTickets
                      where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.ShiftID == ShiftInfo.ShiftID
                      select s).FirstOrDefault();

            var soldout = (from s in context.tblSoldouts
                      where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.ShiftID == ShiftInfo.ShiftID
                      select s).FirstOrDefault();

            if (v1==null || soldout!=null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
            else if (v1.Box_No == 0)
            {
                var v2 = context.tblRecievedTickets.Create();
                v2.Game_Id = v1.Game_Id;
                v2.Packet_Id = v1.Packet_Id;
                v2.Status = "Receive";
                v2.State = data.State;
                v2.Ticket_Name = v1.Ticket_Name;
                v2.Price = int.Parse(v1.Price);
                v2.Created_On = System.DateTime.Now;
                v2.Modified_On = System.DateTime.Now;
                v2.Start_No = v1.PackPosition_Open;
                v2.End_No = v1.PackPosition_Close;
                v2.Count = (Convert.ToInt32(v1.PackPosition_Close) - Convert.ToInt32(v1.PackPosition_Open) + 1).ToString();
                v2.IsDelete = "N";
                v2.EmployeeId = v1.EmployeeId;
                v2.ShiftID = v1.ShiftID;
                v2.Store_Id = v1.Store_Id;
                v2.Total_Price = (v2.Price * int.Parse(v2.Count));
                context.tblRecievedTickets.Add(v2);
                // context.tblSettleTickets.Remove(v1);
                context.SaveChanges();
            }
            else
            {
                var v2 = (from s in context.Box_Master
                          where s.Box_No == v1.Box_No
                          select s).FirstOrDefault();
                v2.Status = "Active";
                context.SaveChanges();

                var v3 = (from s in context.tblActivated_Tickets
                          where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No
                          select s).FirstOrDefault();
                if (v3 != null)
                {
                    v3.Created_On = System.DateTime.Now;
                    context.SaveChanges();
                }
            }

            if (v1 != null)
            {
                context.tblSettleTickets.Attach(v1);
                context.tblSettleTickets.Remove(v1);
                context.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/Settle/NewGetSettleHistory")]
        public HttpResponseMessage NewGetSettleHistory([FromBody] Settle_TicketInfo data)
        {
            LotteryHistory = new ObservableCollection<Settle_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

            var result = (from s in context.tblSettleTickets where s.Status == "Settle" && 
                          s.Store_Id == data.Store_Id select s).ToList();
            foreach (var v in result)
            {   
                if(v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
                {
                    LotteryHistory.Add(new Settle_TicketInfo
                    {
                        Box_No = Convert.ToInt32(v.Box_No),
                        Game_Id = v.Game_Id,
                        Created_Date = Convert.ToDateTime(v.Created_On),
                        Packet_No = v.Packet_Id,
                        Ticket_Name = v.Ticket_Name,
                        Price = v.Price,
                        Start_No = v.PackPosition_Open,
                        End_No = v.PackPosition_Close,
                        Store_Id = v.Store_Id,
                        EmployeeID = v.EmployeeId,
                        ShiftID = v.ShiftID,
                        State = v.State
                    });
                }  
                else if(Shift.EndTime == null)
                {
                    LotteryHistory.Add(new Settle_TicketInfo
                    {
                        Box_No = Convert.ToInt32(v.Box_No),
                        Game_Id = v.Game_Id,
                        Created_Date = Convert.ToDateTime(v.Created_On),
                        Packet_No = v.Packet_Id,
                        Ticket_Name = v.Ticket_Name,
                        Price = v.Price,
                        Start_No = v.PackPosition_Open,
                        End_No = v.PackPosition_Close,
                        Store_Id = v.Store_Id,
                        EmployeeID = v.EmployeeId,
                        ShiftID = v.ShiftID,
                        State = v.State
                    });
                } 
            }
            // return LotteryHistory;
            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        }

    }
}
