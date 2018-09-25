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
    public class ReturnController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        public ObservableCollection<Return_TicketInfo> LotteryHistory { get; set; }

        [Route("api/Return/GetReturnBoxCount")]
        public int GetReturnBoxCount()
        {
            context = new LotteryBlankDatabaseEntities();

            int returncount = (from t in context.tblReturntickets
                               select t).Count();
            return returncount;
        }

        [Route("api/Return/Return_Ticket")]
        public HttpResponseMessage Return_Ticket([FromBody] Return_TicketInfo data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                string EndNo;
                int ClosePosition;
                string StartNo;
                int flag = 0;
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                if (data.Box_No == null)
                {
                    EndNo = (from s in context.tblRecievedTickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id 
                             && s.Store_Id==data.Store_Id select s.End_No).FirstOrDefault();
                    ClosePosition = Convert.ToInt32(EndNo);

                    StartNo = (from s in context.tblRecievedTickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id
                              && s.Store_Id == data.Store_Id select s.Start_No).FirstOrDefault();

                    if (EndNo != data.End_No)
                    {
                        flag = 1;
                    }
                }
                else
                {
                    EndNo = (from s in context.tblActivated_Tickets
                             where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.Store_Id == data.Store_Id && s.ShiftID == Shift.ShiftID select s.End_No).ToList().LastOrDefault();
                    ClosePosition = Convert.ToInt32(EndNo);

                    StartNo = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id
                               && s.Store_Id == data.Store_Id select s.Start_No).FirstOrDefault();
                }
                //if (context.tblActivated_Tickets.Any(o => o.Game_Id == data.Game_Id && o.Packet_Id == data.Packet_No))
                //{
                var v = context.tblReturntickets.Create();
                if (data.Box_No == null)
                {
                    v.Game_Id = data.Game_Id;
                    v.Packet_Id = data.Packet_No;
                    v.Ticket_Name = data.Ticket_Name;
                    v.Price = data.Price;
                    v.EmplyeeeId = data.EmployeeID;
                    v.State = data.State;
                    v.Store_Id = data.Store_Id;
                    v.Box_No = 0;
                    v.Modified_On = data.Modified_Date;
                    v.Created_On = Shift.Date;
                    v.Return_At = data.End_No;
                    v.ShiftID = Shift.ShiftID;
                    v.Status = "Return";
                }
                else
                {
                   
                    v.Game_Id = data.Game_Id;
                    v.Packet_Id = data.Packet_No;
                    v.Ticket_Name = data.Ticket_Name;
                    v.Price = data.Price;
                    v.EmplyeeeId = data.EmployeeID;
                    v.State = data.State;
                    v.Store_Id = data.Store_Id;
                    v.Box_No = data.Box_No;
                    v.Modified_On = data.Modified_Date;
                    v.Created_On = Shift.Date;
                    v.Return_At = data.End_No;
                    v.ShiftID = Shift.ShiftID;
                    v.Status = "Return";
                }
                if (ClosePosition == Convert.ToInt32(data.End_No))
                {
                    v.PackPosition_Open = data.Start_No;
                    v.PackPosition_Close = data.End_No;
                    v.Count = (Convert.ToInt32(v.PackPosition_Close) - Convert.ToInt32(v.PackPosition_Open)) + 1;
                }
                else if (EndNo == null)
                {
                    v.PackPosition_Open = data.Start_No;
                    v.PackPosition_Close = data.End_No;
                    v.Count = (Convert.ToInt32(v.PackPosition_Close) - Convert.ToInt32(v.PackPosition_Open)) + 1;
                }
                else if (Convert.ToInt32(data.End_No) > Convert.ToInt32(EndNo) || (Convert.ToInt32(data.End_No) < Convert.ToInt32(StartNo)) || flag == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    v.PackPosition_Open = Convert.ToInt32(data.End_No).ToString();
                    v.PackPosition_Close = EndNo;
                    v.Count = (Convert.ToInt32(v.PackPosition_Close) - Convert.ToInt32(v.PackPosition_Open)) + 1;
                }

                context.tblReturntickets.Add(v);
                context.SaveChanges();
                if (data.Box_No != 0 && ClosePosition != Convert.ToInt32(data.End_No))
                {
                    var v4 = context.tblSoldouts.Create();
                    v4.Game_Id = data.Game_Id;
                    v4.Packet_Id = data.Packet_No;
                    v4.Ticket_Name = data.Ticket_Name;
                    v4.State = data.State;
                    v4.Price = data.Price;
                    v4.Store_Id = data.Store_Id;
                    v4.Box_No = data.Box_No;
                    v4.EmployeeId = data.EmployeeID;
                    v4.PackPosition_Open = data.Start_No;
                    v4.PackPosition_Close = (Convert.ToInt32(data.End_No)-1).ToString();
                    v4.Modified_On = data.Modified_Date;
                    v4.Total_Tickets = (Convert.ToInt32(v4.PackPosition_Close) - Convert.ToInt32(v4.PackPosition_Open)+1);
                    v4.Created_On = Shift.Date;
                    //v.Return_At = data.Return_At;
                    v4.Status = "SoldOut";
                    v4.ShiftID = Shift.ShiftID;
                    v4.Partial_Packet = "Y";
                    context.tblSoldouts.Add(v4);
                    context.SaveChanges();

                    context = new LotteryBlankDatabaseEntities();
                    var v5 = context.tblSoldOut_History.Create();
                    v5.Packet_Id = data.Packet_No;
                    v5.SoldOut_Date = Shift.Date;
                    v5.No_of_Tickets_Sold = Convert.ToInt32(data.End_No);
                    v5.EmployeeId = data.EmployeeID;
                    //v1.Shift_Id = data.ShiftID;
                    context.tblSoldOut_History.Add(v5);
                    context.SaveChanges();
                }
                else
                {
                    var v3 = (from s in context.tblRecievedTickets
                              where s.Packet_Id == data.Packet_No
                              select s).FirstOrDefault();
                    if (v3 != null)
                    {
                        //context.tblRecievedTickets.Attach(v3);
                        //context.tblRecievedTickets.Remove(v3);
                        v3.Status = "Return";
                        v3.Store_Id = data.Store_Id;
                        context.SaveChanges();
                    }
                }

                var v2 = (from s in context.Box_Master
                          where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                          select s).FirstOrDefault();
                if (v2 != null)
                {
                    v2.Status = "Empty";
                    context.SaveChanges();
                }

                var v1 = (from s in context.tblActivated_Tickets
                          where s.Packet_Id == data.Packet_No && s.ShiftID==Shift.ShiftID && s.Store_Id == data.Store_Id
                          select s).FirstOrDefault();
                if (v1 != null)
                {
                    //context.tblActivated_Tickets.Attach(v1);
                    //context.tblActivated_Tickets.Remove(v1);
                    v1.Status = "Return";
                    context.SaveChanges();
                }

                var Settle = (from s in context.tblSettleTickets
                              where s.Packet_Id == data.Packet_No && s.Game_Id==data.Game_Id && s.Store_Id==data.Store_Id
                              select s).FirstOrDefault();
                if(Settle!=null)
                {
                    context.tblSettleTickets.Attach(Settle);
                    context.tblSettleTickets.Remove(Settle);
                    context.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
                //}
                //else
                //{
                //    return Request.CreateResponse(HttpStatusCode.Conflict);
                //}               
            }
        }

        //[Route("api/Return/GetReturnHistory")]
        //public IEnumerable<Return_TicketInfo> GetHistory()
        //{
        //    LotteryHistory = new ObservableCollection<Return_TicketInfo>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var data = (from s in context.tblReturntickets where s.Status == "Return" && s.Created_On == System.DateTime.Today select s).ToList();
        //    foreach (var v in data)
        //    {
        //        LotteryHistory.Add(new Return_TicketInfo
        //        {
        //            Game_Id = v.Game_Id,
        //            Created_Date = Convert.ToDateTime(v.Created_On),
        //            Packet_No = v.Packet_Id,
        //            Box_No = v.Box_No,
        //            Ticket_Name = v.Ticket_Name,
        //            Price = v.Price,
        //            Start_No = v.PackPosition_Open,
        //            End_No = v.PackPosition_Close,
        //            Return_At = v.Return_At,
        //            EmployeeID = v.EmplyeeeId,
        //            Count = v.Count,
        //            Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
        //        });
        //    }
        //    return LotteryHistory;
        //}

        [Route("api/Return/ReturnBack")]
        public HttpResponseMessage RetunBack([FromBody] Return_TicketInfo data)
        {
            context = new LotteryBlankDatabaseEntities();

            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

            var v = (from s in context.tblRecievedTickets
                     where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.Store_Id == data.Store_Id
                     select s).FirstOrDefault();
            var v2 = (from s in context.tblReturntickets
                      where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id && s.ShiftID == Shift.ShiftID
                      select s).FirstOrDefault();
           
            if (v != null)
            {
                v.Status = "Receive";
                context.tblReturntickets.Remove(v2);
                context.SaveChanges();
            }
            if (data.Box_No != null)
            {
                var v1 = (from s in context.tblActivated_Tickets
                          where s.Box_No == data.Box_No && s.Store_Id==data.Store_Id && s.Status == "Active" && s.ShiftID == Shift.ShiftID
                          select s).FirstOrDefault();

                var v7=(from s in context.tblActivated_Tickets
                        where s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id && s.Status == "Active"  && s.ShiftID == Shift.ShiftID
                        select s).FirstOrDefault();
                if (v1 != null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else if(v7!=null)
                {
                    if(v7.Packet_Id == data.Packet_No)
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    } 
                }
                else
                {
                    var v9 = (from s in context.tblReturntickets
                              where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id && s.ShiftID == Shift.ShiftID
                              select s).FirstOrDefault();

                    var v5 = (from s in context.tblSoldouts
                              where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id && s.ShiftID == Shift.ShiftID
                              select s).FirstOrDefault();

                    var v6 = (from s in context.tblSoldOut_History
                              where s.Packet_Id == data.Packet_No
                              select s).FirstOrDefault();
                    if(v9!=null)
                    {
                        var v8 = (from s in context.tblActivated_Tickets
                                  where s.Store_Id == data.Store_Id && s.Status == "Return"  && s.ShiftID == Shift.ShiftID
                                  && s.Game_Id == v9.Game_Id && s.Packet_Id == v9.Packet_Id
                                  select s).FirstOrDefault();
                        if (v8 != null)
                        {
                            context.tblActivated_Tickets.Remove(v8);
                        }
                    }
                    if (v9 != null || v5 != null)
                    {
                        var v3 = context.tblActivated_Tickets.Create();
                        v3.Game_Id = v9.Game_Id;
                        v3.Packet_Id = v9.Packet_Id;
                        v3.Status = "Active";
                        v3.Box_No = v9.Box_No;
                        v3.Ticket_Name = v9.Ticket_Name;
                        v3.EmployeeId = data.EmployeeID;
                        v3.Store_Id = v9.Store_Id;
                        v3.Price = int.Parse(v9.Price);
                        v3.Activation_Date = Shift.Date;
                        v3.ShiftID = v9.ShiftID;
                        v3.Created_On = Convert.ToDateTime(Shift.Date);
                        if (v5 == null)
                        {
                            v3.Start_No = v2.PackPosition_Open;
                            v3.Count = (Convert.ToInt32(v9.PackPosition_Close) - Convert.ToInt32(v9.PackPosition_Open) + 1).ToString();
                        }
                        else
                        {
                            v3.Start_No = v5.PackPosition_Open;
                            v3.Count = (Convert.ToInt32(v9.PackPosition_Close) - Convert.ToInt32(v5.PackPosition_Open) + 1).ToString();
                        }

                        v3.End_No = v9.PackPosition_Close;

                        v3.State = v9.State;
                        //v.Stopped_At = int.Parse(data.First_Ticket);
                        //v.EmployeeId = v1.EmployeeID;
                        context.tblActivated_Tickets.Add(v3);
                        if(v9 != null)
                        {
                            context.tblReturntickets.Remove(v9);
                        }
                        if (v5 != null)
                        {
                            context.tblSoldouts.Remove(v5);
                        }
                        if (v6 != null)
                        {
                            context.tblSoldOut_History.Remove(v6);
                        }

                        context.SaveChanges();

                        var v4 = (from s in context.Box_Master
                                  where s.Box_No == data.Box_No && s.Store_Id==data.Store_Id
                                  select s).FirstOrDefault();
                        if (v4 != null)
                        {
                            v4.Status = "Active";
                            context.SaveChanges();
                        }
                    }
                }
            }

            else
            {
                var v10 = (from s in context.tblReturntickets
                          where s.Game_Id == v.Game_Id && s.Packet_Id == v.Packet_Id && s.Store_Id == v.Store_Id && s.ShiftID == Shift.ShiftID
                           select s).FirstOrDefault();
                if (v10 != null)
                {
                    context.tblReturntickets.Attach(v10);
                    context.tblReturntickets.Remove(v10);
                    context.SaveChanges();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[Route("api/Return/NewGetReturnHistory")]
        //public HttpResponseMessage NewGetReturnHistory([FromBody] Return_TicketInfo data)
        //{
        //    LotteryHistory = new ObservableCollection<Return_TicketInfo>();
        //    context = new LotteryBlankDatabaseEntities();
        //    if(data.ShiftID == 0)
        //    {
        //        var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

        //        var result = (from s in context.tblReturntickets where s.Status == "Return" && s.Store_Id == data.Store_Id select s).ToList();
        //        if (Shift != null)
        //        {
        //            foreach (var v in result)
        //            {
        //                if (v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
        //                {
        //                    LotteryHistory.Add(new Return_TicketInfo
        //                    {
        //                        Game_Id = v.Game_Id,
        //                        Created_Date = Convert.ToDateTime(v.Created_On),
        //                        Packet_No = v.Packet_Id,
        //                        Box_No = Convert.ToInt32(v.Box_No),
        //                        Ticket_Name = v.Ticket_Name,
        //                        Price = v.Price,
        //                        Store_Id = v.Store_Id,
        //                        Start_No = v.PackPosition_Open,
        //                        End_No = v.PackPosition_Close,
        //                        Return_At = v.Return_At,
        //                        EmployeeID = v.EmplyeeeId,
        //                        ShiftID = v.ShiftID,
        //                        Count = v.Count,
        //                        Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
        //                    });
        //                }
        //                else if (Shift.EndTime == null && v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
        //                {
        //                    LotteryHistory.Add(new Return_TicketInfo
        //                    {
        //                        Game_Id = v.Game_Id,
        //                        Created_Date = Convert.ToDateTime(v.Created_On),
        //                        Packet_No = v.Packet_Id,
        //                        Box_No = Convert.ToInt32(v.Box_No),
        //                        Ticket_Name = v.Ticket_Name,
        //                        Price = v.Price,
        //                        Store_Id = v.Store_Id,
        //                        Start_No = v.PackPosition_Open,
        //                        End_No = v.PackPosition_Close,
        //                        Return_At = v.Return_At,
        //                        EmployeeID = v.EmplyeeeId,
        //                        ShiftID = v.ShiftID,
        //                        Count = v.Count,
        //                        Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    else
        //    {
               
        //        var result = (from s in context.tblReturntickets where s.Status == "Return" && 
        //                      s.Store_Id == data.Store_Id && s.EmplyeeeId == data.EmployeeID select s).ToList();
                
        //            foreach (var v in result)
        //            {
        //                if (v.ShiftID == data.ShiftID && v.Created_On == data.Created_Date)
        //                {
        //                    LotteryHistory.Add(new Return_TicketInfo
        //                    {
        //                        Game_Id = v.Game_Id,
        //                        Created_Date = Convert.ToDateTime(v.Created_On),
        //                        Packet_No = v.Packet_Id,
        //                        Box_No = Convert.ToInt32(v.Box_No),
        //                        Ticket_Name = v.Ticket_Name,
        //                        Price = v.Price,
        //                        Store_Id = v.Store_Id,
        //                        Start_No = v.PackPosition_Open,
        //                        End_No = v.PackPosition_Close,
        //                        Return_At = v.Return_At,
        //                        EmployeeID = v.EmplyeeeId,
        //                        ShiftID = v.ShiftID,
        //                        Count = v.Count,
        //                        Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
        //                    });
        //                }
        //                //else if (Shift.EndTime == null && v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
        //                //{
        //                //    LotteryHistory.Add(new Return_TicketInfo
        //                //    {
        //                //        Game_Id = v.Game_Id,
        //                //        Created_Date = Convert.ToDateTime(v.Created_On),
        //                //        Packet_No = v.Packet_Id,
        //                //        Box_No = Convert.ToInt32(v.Box_No),
        //                //        Ticket_Name = v.Ticket_Name,
        //                //        Price = v.Price,
        //                //        Store_Id = v.Store_Id,
        //                //        Start_No = v.PackPosition_Open,
        //                //        End_No = v.PackPosition_Close,
        //                //        Return_At = v.Return_At,
        //                //        EmployeeID = v.EmplyeeeId,
        //                //        ShiftID = v.ShiftID,
        //                //        Count = v.Count,
        //                //        Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
        //                //    });
        //                //}
        //            }
                
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        //}


        [Route("api/Return/NewGetDailyReturnHistory")]
        public HttpResponseMessage NewGetDailyReturnHistory([FromBody] Return_TicketInfo data)
        {
            LotteryHistory = new ObservableCollection<Return_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            if(data.EmployeeID != 0)
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

                var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == Shift.Date select s).ToList();

                foreach(var r in x)
                {
                    var result = (from s in context.tblReturntickets
                                  where s.Status == "Return" && s.Created_On == Shift.Date && s.Store_Id == data.Store_Id
                                  && s.ShiftID == r.ShiftID select s).ToList();
                    foreach (var v in result)
                    {
                        LotteryHistory.Add(new Return_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Box_No = Convert.ToInt32(v.Box_No),
                            Ticket_Name = v.Ticket_Name,
                            Price = v.Price,
                            Store_Id = v.Store_Id,
                            Start_No = v.PackPosition_Open,
                            End_No = v.PackPosition_Close,
                            Return_At = v.Return_At,
                            EmployeeID = v.EmplyeeeId,
                            ShiftID = v.ShiftID,
                            Count = v.Count,
                            Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
                        });
                    }
                }

                

            }
            else
            {

                var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == data.Created_Date select s).ToList();
                
                foreach(var j in x)
                {
                    var result = (from s in context.tblReturntickets
                                  where s.Status == "Return" && s.Created_On == data.Created_Date && s.Store_Id == data.Store_Id
                                  && s.ShiftID == j.ShiftID select s).ToList();
                    foreach (var v in result)
                    {
                        LotteryHistory.Add(new Return_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Box_No = Convert.ToInt32(v.Box_No),
                            Ticket_Name = v.Ticket_Name,
                            Price = v.Price,
                            Store_Id = v.Store_Id,
                            Start_No = v.PackPosition_Open,
                            End_No = v.PackPosition_Close,
                            Return_At = v.Return_At,
                            EmployeeID = v.EmplyeeeId,
                            ShiftID = v.ShiftID,
                            Count = v.Count,
                            Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
                        });
                    }

                }


            }

            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        }

    }
}
