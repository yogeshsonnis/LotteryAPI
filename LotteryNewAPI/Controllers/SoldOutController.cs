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
    public class SoldOutController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        // Lottery_ApplicationEntities context1;

        public ObservableCollection<SoldOut_TicketInfo> LotteryHistory { get; set; }

        [Route("api/SoldOut/GetSoldOutBoxCount")]
        public int GetSoldOutBoxCount()
        {
            context = new LotteryBlankDatabaseEntities();

            int soldOutcount = (from t in context.tblSoldouts
                                where t.Status == "SoldOut"
                                select t).Count();
            return soldOutcount;
        }

        [Route("api/SoldOut/SoldOut_Ticket")]
        public HttpResponseMessage SoldOut_Ticket([FromBody] SoldOut_TicketInfo data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                if (context.tblActivated_Tickets.Any(o => o.Game_Id == data.Game_Id && o.Packet_Id == data.Packet_No && o.Store_Id == data.Store_Id))
                {
                    var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

                    var setteleticket = (from s in context.tblSettleTickets where s.Game_Id==data.Game_Id && s.Packet_Id==data.Packet_No && s.Store_Id == data.Store_Id  select s).FirstOrDefault();

                    string EndNo = (from s in context.tblActivated_Tickets  where s.Packet_Id == data.Packet_No && 
                                    s.Game_Id == data.Game_Id && s.Store_Id == data.Store_Id select s.End_No).FirstOrDefault();

                    string Totaltickets = (from s in context.tblActivated_Tickets
                                           where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.Store_Id == data.Store_Id
                                           select s.Count).FirstOrDefault();
                    

                    var StoreInfo = (from s in context.tblStore_Info where s.Store_Id == data.Store_Id select s).ToList().LastOrDefault();

                    int? totaltickets = Convert.ToInt32(Totaltickets);

                    if (Convert.ToInt32(data.End_No) > Convert.ToInt32(EndNo))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    else if (data.End_No == (totaltickets).ToString())
                    {
                        var v = context.tblSoldouts.Create();
                        v.Game_Id = data.Game_Id;
                        v.Packet_Id = data.Packet_No;
                        v.Ticket_Name = data.Ticket_Name;
                        v.Price = data.Price.ToString();
                        v.Box_No = data.Box_No;
                        v.Store_Id = data.Store_Id;
                        v.PackPosition_Open = data.Start_No;
                        v.EmployeeId = data.EmployeeID;
                        v.ShiftID = Shift.ShiftID;
                        v.PackPosition_Close = data.End_No;
                        v.Modified_On = data.Modified_Date;
                        v.Status = "SoldOut";
                        v.State = data.State;
                        v.Partial_Packet = "N";
                        v.Created_On = Shift.Date;
                        v.Total_Tickets = totaltickets;
                        //if (Totaltickets > Convert.ToInt32(data.End_No))
                        // L v.RemainingTickets = (Convert.ToInt32(data.End_No) - totaltickets);
                        v.RemainingTickets = (totaltickets-Convert.ToInt32(data.End_No)-1 );
                        //v.PackPosition_Open =( Convert.ToInt32(v.PackPosition_Close) + 1).ToString();
                        context.tblSoldouts.Add(v);
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v4 = (from s in context.tblActivated_Tickets
                                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                                  select s).FirstOrDefault();
                        //context.tblActivated_Tickets.Attach(v4);
                        //context.tblActivated_Tickets.Remove(v4);
                        v4.Status = "SoldOut";
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v5 = (from s in context.Box_Master
                                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                                  select s).FirstOrDefault();
                        v5.Status = "SoldOut";
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v3 = (from s in context.tblRecievedTickets
                                  where s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id
                                  select s).FirstOrDefault();
                        v3.Status = "SoldOut";
                        v3.Store_Id = data.Store_Id;
                        context.SaveChanges();

                    }
                    else
                    {
                        var v = context.tblSoldouts.Create();
                        v.Game_Id = data.Game_Id;
                        v.Packet_Id = data.Packet_No;
                        v.Ticket_Name = data.Ticket_Name;
                        v.Price = data.Price.ToString();
                        v.Box_No = data.Box_No;
                        v.PackPosition_Open = data.Start_No;
                        v.PackPosition_Close = data.End_No;
                        v.EmployeeId = data.EmployeeID;
                        v.ShiftID = Shift.ShiftID;
                        v.Modified_On = data.Modified_Date;
                        v.Status = "SoldOut";
                        v.Store_Id = data.Store_Id;
                        v.State = data.State;
                        v.Partial_Packet = "N";
                        v.Created_On = Shift.Date;
                        v.Total_Tickets = totaltickets;
                        //if (Totaltickets > Convert.ToInt32(data.End_No))
                        // L v.RemainingTickets = totaltickets - (Convert.ToInt32(data.End_No));
                        v.RemainingTickets = (totaltickets - Convert.ToInt32(data.End_No) - 1);
                        //v.PackPosition_Open =( Convert.ToInt32(v.PackPosition_Close) + 1).ToString();
                        context.tblSoldouts.Add(v);
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v5 = (from s in context.Box_Master
                                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                                  select s).FirstOrDefault();
                        v5.Status = "SoldOut";
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v4 = (from s in context.tblActivated_Tickets
                                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id && s.Game_Id == data.Game_Id
                                  && s.Packet_Id==data.Packet_No && s.ShiftID == Shift.ShiftID
                                  select s).FirstOrDefault();
                        //context.tblActivated_Tickets.Attach(v4);
                        //context.tblActivated_Tickets.Remove(v4);
                        v4.Status = "SoldOut";
                        context.SaveChanges();

                        if(StoreInfo.Auto_Settle == true)
                        {
                            if(setteleticket!=null)
                            {
                                context.tblSettleTickets.Attach(setteleticket);
                                context.tblSettleTickets.Remove(setteleticket);
                                context.SaveChanges();
                            }
                            var w = context.tblSettleTickets.Create();
                            w.Game_Id = data.Game_Id;
                            w.Packet_Id = data.Packet_No;
                            w.Ticket_Name = data.Ticket_Name;
                            w.Price = data.Price.ToString();
                            w.Box_No = data.Box_No;
                            w.Status = "Settle";
                            w.Store_Id = data.Store_Id;
                            w.Created_On = Shift.Date;
                            w.State = data.State;
                            w.PackPosition_Open = data.Start_No;
                            w.PackPosition_Close = data.End_No;
                            w.EmployeeId = data.EmployeeID;
                            w.ShiftID = Shift.ShiftID;
                            //   v.State = data.State;
                            //v.Modified_On = data.Modified_Date;
                            v.Created_On = Shift.Date;
                            context.tblSettleTickets.Add(w);
                            context.SaveChanges();
                        }


                    }

                    context = new LotteryBlankDatabaseEntities();
                    var v1 = context.tblSoldOut_History.Create();
                    v1.Packet_Id = data.Packet_No;
                    v1.SoldOut_Date = Shift.Date;
                    v1.No_of_Tickets_Sold = Convert.ToInt32(data.End_No) + 1;
                    //v1.No_of_Tickets_Sold = Convert.ToInt32(data.Start_No) - int.Parse(data.End_No);
                    v1.EmployeeId = data.EmployeeID;
                    v1.Shift_Id = data.ShiftID;
                    context.tblSoldOut_History.Add(v1);
                    context.SaveChanges();


                    if (StoreInfo.Auto_Settle == false)
                    {
                        var v6 = (from b in context.tblSettleTickets
                                  where b.Box_No == data.Box_No && b.Store_Id == data.Store_Id
                                  select b).FirstOrDefault();
                        if (v6 != null)
                        {
                            context.tblSettleTickets.Attach(v6);
                            context.tblSettleTickets.Remove(v6);
                            context.SaveChanges();
                        }
                    }
                        
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }

            }
        }        

        //[Route("api/SoldOut/GetSoldOutHistory")]
        //public IEnumerable<SoldOut_TicketInfo> GetHistory()
        //{
        //    LotteryHistory = new ObservableCollection<SoldOut_TicketInfo>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var data = (from s in context.tblSoldouts where s.Status == "SoldOut" && s.Created_On == System.DateTime.Today select s).ToList();
        //    foreach (var v in data)
        //    {
        //        LotteryHistory.Add(new SoldOut_TicketInfo
        //        {
        //            Game_Id = v.Game_Id,
        //            Created_Date = Convert.ToDateTime(v.Created_On),
        //            Packet_No = v.Packet_Id,
        //            Ticket_Name = v.Ticket_Name,
        //            Price = Convert.ToInt32(v.Price),
        //            Box_No = Convert.ToInt16(v.Box_No),
        //            Status = v.Status,
        //            No_of_Tickets_Sold = v.Total_Tickets,
        //            EmployeeID = v.EmployeeId,
        //            Modified_Date = Convert.ToDateTime(v.Modified_On),
        //            End_No = v.PackPosition_Close,
        //            Start_No = v.PackPosition_Open,
        //            Partial_Packet = v.Partial_Packet,
        //            Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
        //        });
        //    }
        //    return LotteryHistory;
        //}

        //[Route("api/SoldOut/NewGetSoldOutHistory")]
        //public HttpResponseMessage NewGetSoldOutHistory([FromBody] SoldOut_TicketInfo data)
        //{

        //    LotteryHistory = new ObservableCollection<SoldOut_TicketInfo>();
        //    context = new LotteryBlankDatabaseEntities();
        //    if (data.ShiftID == 0)
        //    {
        //        var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

        //        var result = (from s in context.tblSoldouts
        //                      where s.Status == "SoldOut" && s.Store_Id == data.Store_Id
        //                      select s).ToList();
        //        foreach (var v in result)
        //        {
        //            if (v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
        //            {
        //                LotteryHistory.Add(new SoldOut_TicketInfo
        //                {
        //                    Game_Id = v.Game_Id,
        //                    Created_Date = Convert.ToDateTime(v.Created_On),
        //                    Packet_No = v.Packet_Id,
        //                    Ticket_Name = v.Ticket_Name,
        //                    Price = Convert.ToInt32(v.Price),
        //                    Box_No = Convert.ToInt16(v.Box_No),
        //                    Status = v.Status,
        //                    Store_Id = v.Store_Id,
        //                    ShiftID = v.ShiftID,
        //                    No_of_Tickets_Sold = v.Total_Tickets,
        //                    EmployeeID = v.EmployeeId,
        //                    Modified_Date = Convert.ToDateTime(v.Modified_On),
        //                    End_No = v.PackPosition_Close,
        //                    Start_No = v.PackPosition_Open,
        //                    Partial_Packet = v.Partial_Packet,
        //                    Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
        //                });
        //            }
        //            else if (Shift.EndTime == null && v.Created_On == Shift.Date && v.ShiftID == Shift.ShiftID)
        //            {
        //                LotteryHistory.Add(new SoldOut_TicketInfo
        //                {
        //                    Game_Id = v.Game_Id,
        //                    Created_Date = Convert.ToDateTime(v.Created_On),
        //                    Packet_No = v.Packet_Id,
        //                    Ticket_Name = v.Ticket_Name,
        //                    Price = Convert.ToInt32(v.Price),
        //                    Box_No = Convert.ToInt16(v.Box_No),
        //                    Status = v.Status,
        //                    Store_Id = v.Store_Id,
        //                    ShiftID = v.ShiftID,
        //                    No_of_Tickets_Sold = v.Total_Tickets,
        //                    EmployeeID = v.EmployeeId,
        //                    Modified_Date = Convert.ToDateTime(v.Modified_On),
        //                    End_No = v.PackPosition_Close,
        //                    Start_No = v.PackPosition_Open,
        //                    Partial_Packet = v.Partial_Packet,
        //                    Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
        //                });
        //            }
        //        }


        //    }

        //    else
        //    {
        //        //var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.EmployeeId == data.EmployeeID
        //        //             && s.Date == data.Created_Date && s.ShiftID == data.ShiftID select s).ToList().FirstOrDefault();

        //        var result = (from s in context.tblSoldouts
        //                      where s.Status == "SoldOut" && s.Store_Id == data.Store_Id && s.EmployeeId == data.EmployeeID
        //                      select s).ToList();
        //        foreach (var v in result)
        //        {
        //            if (v.ShiftID == data.ShiftID && v.Created_On == data.Created_Date)
        //            {
        //                LotteryHistory.Add(new SoldOut_TicketInfo
        //                {
        //                    Game_Id = v.Game_Id,
        //                    Created_Date = Convert.ToDateTime(v.Created_On),
        //                    Packet_No = v.Packet_Id,
        //                    Ticket_Name = v.Ticket_Name,
        //                    Price = Convert.ToInt32(v.Price),
        //                    Box_No = Convert.ToInt16(v.Box_No),
        //                    Status = v.Status,
        //                    Store_Id = v.Store_Id,
        //                    ShiftID = v.ShiftID,
        //                    No_of_Tickets_Sold = v.Total_Tickets,
        //                    EmployeeID = v.EmployeeId,
        //                    Modified_Date = Convert.ToDateTime(v.Modified_On),
        //                    End_No = v.PackPosition_Close,
        //                    Start_No = v.PackPosition_Open,
        //                    Partial_Packet = v.Partial_Packet,
        //                    Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
        //                });
        //            }
        //            //else if (Shift.EndTime == null && v.Created_On == Shift.Date && v.ShiftID == Shift.ShiftID)
        //            //{
        //            //    LotteryHistory.Add(new SoldOut_TicketInfo
        //            //    {
        //            //        Game_Id = v.Game_Id,
        //            //        Created_Date = Convert.ToDateTime(v.Created_On),
        //            //        Packet_No = v.Packet_Id,
        //            //        Ticket_Name = v.Ticket_Name,
        //            //        Price = Convert.ToInt32(v.Price),
        //            //        Box_No = Convert.ToInt16(v.Box_No),
        //            //        Status = v.Status,
        //            //        Store_Id = v.Store_Id,
        //            //        ShiftID = v.ShiftID,
        //            //        No_of_Tickets_Sold = v.Total_Tickets,
        //            //        EmployeeID = v.EmployeeId,
        //            //        Modified_Date = Convert.ToDateTime(v.Modified_On),
        //            //        End_No = v.PackPosition_Close,
        //            //        Start_No = v.PackPosition_Open,
        //            //        Partial_Packet = v.Partial_Packet,
        //            //        Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
        //            //    });
        //            //}
        //        }
        //    }
            
        //    return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        //}

        [Route("api/SoldOut/Unsold")]
        public HttpResponseMessage UpdateTicketStatus([FromBody] SoldOut_TicketInfo data)
         {
            context = new LotteryBlankDatabaseEntities();

            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id  select s).ToList().LastOrDefault();

            var v = (from s in context.tblSoldouts
                     where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.PackPosition_Open == data.Start_No
                     && s.Store_Id == data.Store_Id
                     select s).FirstOrDefault();

            var v1 = (from s in context.tblActivated_Tickets
                      where  s.Packet_Id == v.Packet_Id && s.Store_Id == v.Store_Id && s.Status == "Active" && s.EmployeeId == data.EmployeeID && s.ShiftID == Shift.ShiftID
                      select s).FirstOrDefault();

           

            var v2 = (from s in context.tblReturntickets
                      where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id 
                      select s).FirstOrDefault();

            var xyz = (from s in context.tblSoldOut_History
                       where s.Packet_Id == data.Packet_No
                       select s).FirstOrDefault();

            var pqr = (from s in context.tblRecievedTickets
                       where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id
                       select s).FirstOrDefault();

            var abc = (from s in context.tblActivated_Tickets
                       where s.Box_No == v.Box_No && s.Store_Id == v.Store_Id && s.Status == "Active" && s.EmployeeId == data.EmployeeID && s.ShiftID == Shift.ShiftID
                       select s).FirstOrDefault();

            
            if (abc != null || v1 != null)
            {
                if (v1 != null)
                {
                    if (v.Packet_Id == v1.Packet_Id)
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }
                }
                else if (abc == null || abc.Box_No == data.Box_No || v1.Game_Id == data.Game_Id)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            else if (context.tblSoldouts.Any(o => o.Packet_Id == v.Packet_Id))
            {
                var v3 = context.tblActivated_Tickets.Create();
                v3.Game_Id = v.Game_Id;
                v3.Packet_Id = v.Packet_Id;
                v3.Status = "Active";
                v3.State = v.State;
                v3.Box_No = v.Box_No;
                v3.Ticket_Name = v.Ticket_Name;
                v3.Store_Id = v.Store_Id;
                v3.State = v.State;
                v3.EmployeeId = v.EmployeeId;
                v3.ShiftID = v.ShiftID;
                v3.Price = Convert.ToInt32(v.Price);
                v3.Created_On = Convert.ToDateTime(Shift.Date);
                v3.Activation_Date = System.DateTime.Now;
                v3.Start_No = v.PackPosition_Open;
                context.SaveChanges();
                if (v2 != null)
                {
                    if (v2.Game_Id == data.Game_Id && v2.Packet_Id == data.Packet_No)
                    {
                        v3.End_No = v2.PackPosition_Close;
                        v3.Count = (Convert.ToInt32(v2.PackPosition_Close) - Convert.ToInt32(v.PackPosition_Open) + 1).ToString();
                        context.tblReturntickets.Remove(v2);
                        context.SaveChanges();
                        //context.tblSoldouts.Remove(v);
                        //context.SaveChanges();
                        var v6 = (from s in context.tblActivated_Tickets
                                  where s.Game_Id==v.Game_Id && s.Packet_Id == v.Packet_Id && s.Store_Id == v.Store_Id && s.Status == "Return" && s.EmployeeId == data.EmployeeID && s.ShiftID == Shift.ShiftID
                                  select s).FirstOrDefault();
                        if(v6!=null)
                        {
                            context.tblActivated_Tickets.Remove(v6);
                            context.SaveChanges();
                        }
                        
                    }
                }
                else if (pqr != null)
                {
                    v3.End_No = pqr.End_No;
                    v3.Count = (Convert.ToInt32(pqr.End_No) - Convert.ToInt32(v.PackPosition_Open) + 1).ToString();
                }
                else
                {
                    v3.End_No = v.PackPosition_Close;
                    v3.Count = (Convert.ToInt32(v.PackPosition_Close) - Convert.ToInt32(v.PackPosition_Open) + 1).ToString();
                }
                context.tblActivated_Tickets.Add(v3);
                if (v.Box_No == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                context.SaveChanges();

                context = new LotteryBlankDatabaseEntities();
                var v4 = (from s in context.Box_Master
                          where s.Box_No == v.Box_No && s.Store_Id==v.Store_Id
                          select s).FirstOrDefault();
                v4.Status = "Active";
                context.SaveChanges();

                var v5 =(from s in context.tblActivated_Tickets
                          where s.Store_Id == data.Store_Id && s.Status == "SoldOut" && s.EmployeeId == data.EmployeeID && s.ShiftID == Shift.ShiftID
                            && s.Game_Id == v.Game_Id && s.Packet_Id == v.Packet_Id
                         select s).FirstOrDefault();

                if(v5 != null)
                {
                    context.tblActivated_Tickets.Remove(v5);
                    context.SaveChanges();
                }

                if (v != null)
                {
                    context.tblSoldouts.Attach(v);
                    context.tblSoldouts.Remove(v);
                    //context.tblActivated_Tickets.Remove(v5);
                    context.SaveChanges();
                    var settleticket = (from s in context.tblSettleTickets
                                        where s.Packet_Id == v.Packet_Id && s.Store_Id == v.Store_Id && s.ShiftID == Shift.ShiftID
                                        select s).FirstOrDefault();
                    if (settleticket!=null)
                    {
                        context.tblSettleTickets.Remove(settleticket);
                    }
                }
                if (pqr != null)
                {
                    context.tblRecievedTickets.Attach(pqr);
                    context.tblRecievedTickets.Remove(pqr);
                    context.SaveChanges();
                }

                if (xyz != null)
                {
                    context.tblSoldOut_History.Attach(xyz);
                    context.tblSoldOut_History.Remove(xyz);
                    context.SaveChanges();
                }

                var Deactivate = (from s in context.tblDeactivateds
                          where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id && s.Game_Id == data.Game_Id && s.Packet_Id==data.Packet_No
                          select s).FirstOrDefault();
                if(Deactivate!=null)
                {
                    context.tblDeactivateds.Attach(Deactivate);
                    context.tblDeactivateds.Remove(Deactivate);
                    context.SaveChanges();
                }               
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[Route("api/SoldOut/NewGetDailySoldOutHistory")]
        //public HttpResponseMessage NewGetDailySoldOutHistory([FromBody] SoldOut_TicketInfo data)
        //{
        //    LotteryHistory = new ObservableCollection<SoldOut_TicketInfo>();
        //    context = new LotteryBlankDatabaseEntities();
        //    if(data.EmployeeID != 0)
        //    {
        //        var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

        //        var result = (from s in context.tblSoldouts
        //                      where s.Status == "SoldOut" && s.Store_Id == data.Store_Id
        //                      && s.Created_On == Shift.Date
        //                      select s).ToList();

        //        foreach (var v in result)
        //        {
        //            LotteryHistory.Add(new SoldOut_TicketInfo
        //            {
        //                Game_Id = v.Game_Id,
        //                Created_Date = Convert.ToDateTime(v.Created_On),
        //                Packet_No = v.Packet_Id,
        //                Ticket_Name = v.Ticket_Name,
        //                Price = Convert.ToInt32(v.Price),
        //                Box_No = Convert.ToInt16(v.Box_No),
        //                Status = v.Status,
        //                Store_Id = v.Store_Id,
        //                ShiftID = v.ShiftID,
        //                No_of_Tickets_Sold = v.Total_Tickets,
        //                EmployeeID = v.EmployeeId,
        //                Modified_Date = Convert.ToDateTime(v.Modified_On),
        //                End_No = v.PackPosition_Close,
        //                Start_No = v.PackPosition_Open,
        //                Partial_Packet = v.Partial_Packet,
        //                Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
        //            });
        //        }
        //    }
        //    else
        //    {
        //        var result = (from s in context.tblSoldouts
        //                      where s.Status == "SoldOut" && s.Store_Id == data.Store_Id
        //                      && s.Created_On == data.Created_Date
        //                      select s).ToList();

        //        foreach (var v in result)
        //        {
        //            LotteryHistory.Add(new SoldOut_TicketInfo
        //            {
        //                Game_Id = v.Game_Id,
        //                Created_Date = Convert.ToDateTime(v.Created_On),
        //                Packet_No = v.Packet_Id,
        //                Ticket_Name = v.Ticket_Name,
        //                Price = Convert.ToInt32(v.Price),
        //                Box_No = Convert.ToInt16(v.Box_No),
        //                Status = v.Status,
        //                Store_Id = v.Store_Id,
        //                ShiftID = v.ShiftID,
        //                No_of_Tickets_Sold = v.Total_Tickets,
        //                EmployeeID = v.EmployeeId,
        //                Modified_Date = Convert.ToDateTime(v.Modified_On),
        //                End_No = v.PackPosition_Close,
        //                Start_No = v.PackPosition_Open,
        //                Partial_Packet = v.Partial_Packet,
        //                Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
        //            });
        //        }
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        //}
    }
}
