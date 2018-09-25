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
    public class ActivateController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        public ObservableCollection<Activate_Ticket> Active_BoxCollection { get; set; }
        public ObservableCollection<Activate_Ticket> LotteryHistory { get; set; }
        public ObservableCollection<Activate_Ticket> ActiveAndCloseBox { get; set; }
        public ObservableCollection<Activate_Ticket> ActiveTicketTotal { get; set; }

        int sid ;
        DateTime sdate ;
        int temp = 0;
        int endno;

        

        [Route("api/Activate/NewGetActiveBoxCollection")]
        public HttpResponseMessage NewGetActiveBoxCollection([FromBody] Activate_Ticket data)
        {
           
            Active_BoxCollection = new ObservableCollection<Activate_Ticket>();
            context = new LotteryBlankDatabaseEntities();

                var shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var v = (from s in context.tblActivated_Tickets
                         where s.Store_Id == data.Store_Id && (s.Status == "Active" || s.Status == "Close")
                         orderby s.Box_No ascending
                         select s).ToList();
                foreach (var i in v)
                {
                    if (shift != null)
                    {
                        if (i.ShiftID == shift.ShiftID && i.Created_On == shift.Date)
                        {
                            Active_BoxCollection.Add(new Activate_Ticket
                            {
                                Box_No = Convert.ToInt32(i.Box_No),
                                Status = i.Status,
                                Price = Convert.ToInt16(i.Price),
                                Game_Id = i.Game_Id,
                                Packet_No = i.Packet_Id,
                                Ticket_Name = i.Ticket_Name,
                                First_Ticket = i.Stopped_At,
                                Start_No = i.Start_No,
                                Stopped_At = i.Stopped_At,
                                End_No = i.End_No,
                                ShiftID = i.ShiftID,
                                EmployeeID = i.EmployeeId,
                                Total_Price = i.Total_Price,
                                Store_Id = i.Store_Id
                            });
                        }
                        else if (shift.EndTime == null)
                        {
                            Active_BoxCollection.Add(new Activate_Ticket
                            {
                                Box_No = Convert.ToInt32(i.Box_No),
                                Status = i.Status,
                                Price = Convert.ToInt16(i.Price),
                                Game_Id = i.Game_Id,
                                Packet_No = i.Packet_Id,
                                Ticket_Name = i.Ticket_Name,
                                First_Ticket = i.Stopped_At,
                                Start_No = i.Start_No,
                                Stopped_At = i.Stopped_At,
                                End_No = i.End_No,
                                ShiftID = i.ShiftID,
                                EmployeeID = i.EmployeeId,
                                Total_Price = i.Total_Price,
                                Store_Id = i.Store_Id
                            });
                        }
                    }
                }
                //  return Active_BoxCollection;
                return Request.CreateResponse(HttpStatusCode.OK, Active_BoxCollection);
            

            
        }

        [Route("api/Activate/ActivateTicket")]
        public HttpResponseMessage UpdateTicketStatus([FromBody] Activate_Ticket data)
         {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var r = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.Store_Id == data.Store_Id && s.Status == "Active" select s).FirstOrDefault();
                if (r != null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    if (context.tblRecievedTickets.Any(o => o.Game_Id == data.Game_Id && o.Packet_Id == data.Packet_No))
                    {
                        var v = context.tblActivated_Tickets.Create();
                        v.Game_Id = data.Game_Id;
                        v.Packet_Id = data.Packet_No;
                        v.Status = "Active";
                        v.ShiftID = Shift.ShiftID;
                        v.Box_No = data.Box_No;
                        v.Ticket_Name = data.Ticket_Name;
                        v.Price = data.Price;
                        v.State = data.State;
                        v.Created_On =Convert.ToDateTime(Shift.Date);
                        v.Activation_Date = Shift.Date;
                        v.Start_No = data.Start_No;
                        v.End_No = data.End_No;
                        v.Count = (Convert.ToInt32(data.End_No) - Convert.ToInt32(data.Start_No) + 1).ToString();
                        v.Stopped_At = data.First_Ticket;
                        v.Total_Price = Convert.ToInt32(v.Count) * v.Price;
                        v.EmployeeId = data.EmployeeID;
                        v.Store_Id = data.Store_Id;
                        context.tblActivated_Tickets.Add(v);
                        if (v.Box_No == 0 || v.Box_No == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v1 = (from s in context.Box_Master
                                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                                  select s).FirstOrDefault();
                        v1.Status = "Active";
                        context.SaveChanges();


                        var v3 = (from s in context.tblRecievedTickets
                                  where s.Packet_Id == data.Packet_No
                                  select s).FirstOrDefault();
                        context.tblRecievedTickets.Attach(v3);
                        context.tblRecievedTickets.Remove(v3);
                        //v3.Status = "Active";
                        context.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                }

            }

        }

        [Route("api/Activate/NewStoreActivateTicket")]
        public HttpResponseMessage NewStoreActivateTicket([FromBody] Activate_Ticket data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var r = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No || s.Box_No == data.Box_No && s.Store_Id == data.Store_Id && s.Status == "Active" select s).FirstOrDefault();
                if (r != null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    if (context.tblLottery_Inventory.Any(o => o.GameID == data.Game_Id))
                    {
                        var v = context.tblActivated_Tickets.Create();
                        v.Game_Id = data.Game_Id;
                        v.Packet_Id = data.Packet_No;
                        v.Status = "Active";
                        v.ShiftID = Shift.ShiftID;
                        v.Box_No = data.Box_No;
                        v.Ticket_Name = data.Ticket_Name;
                        v.Price = data.Price;
                        v.State = data.State;
                        v.Created_On = Convert.ToDateTime(Shift.Date);
                        v.Activation_Date = Shift.Date;
                        v.Start_No = data.Start_No;
                        v.End_No = data.End_No;
                        v.Count = (Convert.ToInt32(data.End_No) - Convert.ToInt32(data.Start_No) + 1).ToString();
                        v.Stopped_At = data.First_Ticket;
                        v.Total_Price = Convert.ToInt32(v.Count) * v.Price;
                        v.EmployeeId = data.EmployeeID;
                        v.Store_Id = data.Store_Id;
                        context.tblActivated_Tickets.Add(v);
                        if (v.Box_No == 0 || v.Box_No == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.Conflict);
                        }
                        context.SaveChanges();

                        context = new LotteryBlankDatabaseEntities();
                        var v1 = (from s in context.Box_Master
                                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                                  select s).FirstOrDefault();
                        v1.Status = "Active";
                        context.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                }

            }

        }

        [Route("api/Activate/NewStoreRemoveBox")]
        public HttpResponseMessage NewStoreRemoveBox([FromBody] Activate_Ticket data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var ActiveBoxno = (from s in context.tblActivated_Tickets where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id && s.Status == "Active" select s).FirstOrDefault();
                if (ActiveBoxno != null)
                {
                    
                    context.tblActivated_Tickets.Attach(ActiveBoxno);
                    context.tblActivated_Tickets.Remove(ActiveBoxno);
                    var v1 = (from s in context.Box_Master
                              where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                              select s).FirstOrDefault();
                    v1.Status = "Empty";
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }

        }

        //[Route("api/Activate/GetActiveBoxCount")]
        //public int GetActiveBoxCount()
        //{
        //    context = new LotteryBlankDatabaseEntities();

        //    int activecount = (from t in context.tblActivated_Tickets where t.Status=="Active"
        //                       select t).Count();
        //    return activecount;
        //}

        //[Route("api/Activate/NewGetActiveBoxCount")]
        //public HttpResponseMessage NewGetActiveBoxCount([FromBody] Activate_Ticket data)
        //{
        //    context = new LotteryBlankDatabaseEntities();
        //    int activecount = 0;
        //    var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

        //    var c = (from t in context.tblActivated_Tickets
        //                       where t.Status == "Active" && t.Store_Id == data.Store_Id
        //                       select t).ToList();
        //    //return activecount;


        //    foreach(var i in c)
        //    {
        //        if(i.ShiftID == Shift.ShiftID)
        //        {
        //            if (i.Store_Id == Shift.StoreId )
        //            {
        //                activecount = activecount + 1;
        //            }
        //        }

        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, activecount);
        //}

        //[Route("api/Activate/GetActivateHistory")]
        //public IEnumerable<Activate_Ticket> GetHistory()
        //{
        //    LotteryHistory = new ObservableCollection<Activate_Ticket>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var data = (from s in context.tblActivated_Tickets where s.Status == "Active" select s).ToList();
        //    foreach (var v in data)
        //    {
        //        LotteryHistory.Add(new Activate_Ticket
        //        {
        //            Game_Id = v.Game_Id,
        //            Box_No = v.Box_No,
        //            Created_Date = Convert.ToDateTime(v.Created_On),
        //            Packet_No = v.Packet_Id,
        //            Ticket_Name = v.Ticket_Name,
        //            Price = Convert.ToInt32(v.Price),
        //            Start_No = v.Start_No,
        //            End_No = v.End_No,
        //            Status = v.Status,
        //            EmployeeID = v.EmployeeId,
        //            Stopped_At = v.Stopped_At,
        //            State = v.State,
        //            Count = v.Count,
        //            Total_Price = Convert.ToInt32(v.Price) * Convert.ToInt32(v.Count)
        //        });
        //    }
        //    return LotteryHistory;
        //}

        [Route("api/Activate/NewGetActivateHistory")]
        public HttpResponseMessage NewGetActivateHistory([FromBody] Activate_Ticket data)
        {
            LotteryHistory = new ObservableCollection<Activate_Ticket>();
            context = new LotteryBlankDatabaseEntities();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

            var result = (from s in context.tblActivated_Tickets
                          where s.Status == "Active" && s.Store_Id == data.Store_Id  
                          select s).ToList();

            var settelementdays = (from s in context.tblStore_Info where s.Store_Id == data.Store_Id
                                   select s).FirstOrDefault();

            foreach (var v in result)
            {
                    LotteryHistory.Add(new Activate_Ticket
                    {
                        Game_Id = v.Game_Id,
                        Box_No = v.Box_No,
                        Created_Date = Convert.ToDateTime(v.Created_On),
                        Activation_Date = Convert.ToDateTime(v.Activation_Date),
                        Packet_No = v.Packet_Id,
                        Ticket_Name = v.Ticket_Name,
                        Price = Convert.ToInt32(v.Price),
                        Start_No = v.Start_No,
                        End_No = v.End_No,
                        Status = v.Status,
                        Stopped_At = v.Stopped_At,
                        EmployeeID = v.EmployeeId,
                        State = v.State,
                        ShiftID = v.ShiftID,
                        Count = v.Count,
                        Total_Price = Convert.ToInt32(v.Price) * Convert.ToInt32(v.Count),
                        SettlementDays = Convert.ToInt32(settelementdays.Settlement_Day)
                    });
                         
            }
            // return LotteryHistory;
            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        }

        [Route("api/Activate/NewGetActivateTicketTotalPrice")]
        public HttpResponseMessage NewGetActivateTicketTotalPrice([FromBody] Activate_Ticket data)
        {
            context = new LotteryBlankDatabaseEntities();
            ActiveTicketTotal = new ObservableCollection<Activate_Ticket>();
                     
            var Activate = (from s in context.tblActivated_Tickets where s.Store_Id == data.Store_Id
                            && s.Status=="Close" select s).ToList();
            foreach (var i in Activate)
            {
                if (Activate != null)
                {
                    ActiveTicketTotal.Add(new Activate_Ticket
                    {

                        Box_No = i.Box_No,
                        Price = i.Price,
                        EmployeeID = i.EmployeeId,
                        Total_Price = (int.Parse(i.End_No) - int.Parse(i.Stopped_At) + 1) * i.Price,
                        ShiftID = i.ShiftID                        
                    });
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ActiveTicketTotal);
        }

        //[Route("api/Activate/NewGetShiftActivateHistory")]
        //public HttpResponseMessage NewGetShiftActivateHistory([FromBody] Activate_Ticket data)
        //{
        //    LotteryHistory = new ObservableCollection<Activate_Ticket>();
        //    context = new LotteryBlankDatabaseEntities();
        //    if (data.ShiftID == 0)
        //    {
        //        var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
        //        sid = Shift.ShiftID;
        //        sdate = Convert.ToDateTime(Shift.Date);
        //    }
        //    else
        //    {
        //        sid = Convert.ToInt32(data.ShiftID);
        //        sdate = data.Created_Date;
        //    }

        //        var closeticket = (from s in context.tblClose_Box
        //                           where s.Store_Id == data.Store_Id && s.ShiftID == sid && 
        //                           s.EmployeeId == data.EmployeeID && s.Created_On == sdate
        //                           select s).ToList();
        //        var soldoutticket = (from s in context.tblSoldouts
        //                             where s.Store_Id == data.Store_Id && s.ShiftID == sid && 
        //                             s.EmployeeId == data.EmployeeID && s.Created_On == sdate
        //                             select s).ToList();
        //        var returnticket = (from s in context.tblReturntickets
        //                            where s.Store_Id == data.Store_Id && s.ShiftID == sid && 
        //                            s.EmplyeeeId == data.EmployeeID && s.Created_On == sdate
        //                            select s).ToList();
                           
        //        foreach (var v in closeticket)
        //        {
        //            LotteryHistory.Add(new Activate_Ticket
        //            {
        //                Game_Id = v.Game_Id,
        //                Box_No = v.Box_No,
        //                Created_Date = Convert.ToDateTime(v.Created_On),
        //                Packet_No = v.Packet_Id,
        //                Ticket_Name = v.Ticket_Name,
        //                Price = Convert.ToInt32(v.Price),
        //                Start_No = v.Start_No,
        //                End_No = v.End_No,
        //                Status = v.Status,
        //                // Stopped_At = v.Close_At,
        //                EmployeeID = v.EmployeeId,
        //                State = v.State,
        //                ShiftID = v.ShiftID,
        //                Count = (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1).ToString(),
        //                Total_Price = Convert.ToInt32(v.Price) * (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1)
        //            });
        //        }

        //        foreach (var i in soldoutticket)
        //            {
        //                if (i.Partial_Packet != "Y")
        //                {
        //                    LotteryHistory.Add(new Activate_Ticket
        //                    {
        //                        Game_Id = i.Game_Id,
        //                        Box_No = i.Box_No,
        //                        Created_Date = Convert.ToDateTime(i.Created_On),
        //                        Packet_No = i.Packet_Id,
        //                        Ticket_Name = i.Ticket_Name,
        //                        Price = Convert.ToInt32(i.Price),
        //                        Start_No = i.PackPosition_Open,
        //                        End_No = i.PackPosition_Close,
        //                        Status = i.Status,
        //                        //Stopped_At = i.Close_At,
        //                        EmployeeID = i.EmployeeId,
        //                        State = i.State,
        //                        ShiftID = i.ShiftID,
        //                        Count = (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1).ToString(),
        //                        Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1)
        //                    });
        //                }
        //            }

        //            foreach (var j in returnticket)
        //            {
        //                var soldoutpartial = (from s in context.tblSoldouts
        //                                      where s.Game_Id == j.Game_Id && s.Packet_Id == j.Packet_Id && s.Store_Id == j.Store_Id && s.ShiftID == sid
        //                                      && s.Created_On == sdate
        //                                      select s).ToList().FirstOrDefault();
        //                if (soldoutpartial != null)
        //                {
        //                    LotteryHistory.Add(new Activate_Ticket
        //                    {
        //                        Game_Id = j.Game_Id,
        //                        Box_No = j.Box_No,
        //                        Created_Date = Convert.ToDateTime(j.Created_On),
        //                        Packet_No = j.Packet_Id,
        //                        Ticket_Name = j.Ticket_Name,
        //                        Price = Convert.ToInt32(j.Price),
        //                        Start_No = soldoutpartial.PackPosition_Open,
        //                        End_No = j.PackPosition_Close,
        //                        Status = j.Status,
        //                        //Stopped_At = i.Close_At,
        //                        EmployeeID = j.EmplyeeeId,
        //                        State = j.State,
        //                        ShiftID = j.ShiftID,
        //                        Count = (int.Parse(j.PackPosition_Close) - int.Parse(soldoutpartial.PackPosition_Open) + 1).ToString(),
        //                        Total_Price = Convert.ToInt32(j.Price) * (int.Parse(j.PackPosition_Close) - int.Parse(soldoutpartial.PackPosition_Open) + 1)
        //                    });
        //                }
        //                else
        //                {
        //                    LotteryHistory.Add(new Activate_Ticket
        //                    {
        //                        Game_Id = j.Game_Id,
        //                        Box_No = j.Box_No,
        //                        Created_Date = Convert.ToDateTime(j.Created_On),
        //                        Packet_No = j.Packet_Id,
        //                        Ticket_Name = j.Ticket_Name,
        //                        Price = Convert.ToInt32(j.Price),
        //                        Start_No = j.PackPosition_Open,
        //                        End_No = j.PackPosition_Close,
        //                        Status = j.Status,
        //                        //Stopped_At = i.Close_At,
        //                        EmployeeID = j.EmplyeeeId,
        //                        State = j.State,
        //                        ShiftID = j.ShiftID,
        //                        Count = (int.Parse(j.PackPosition_Close) - int.Parse(j.PackPosition_Open) + 1).ToString(),
        //                        Total_Price = Convert.ToInt32(j.Price) * (int.Parse(j.PackPosition_Close) - int.Parse(j.PackPosition_Open) + 1)
        //                    });
        //                }
        //            }
                
            

        //    //else
        //    //{
        //    //    var closeticket = (from s in context.tblClose_Box
        //    //                       where s.Store_Id == data.Store_Id && s.ShiftID == data.ShiftID &&
        //    //                       s.EmployeeId == data.EmployeeID && s.Created_On == data.Created_Date
        //    //                       select s).ToList();
        //    //    var soldoutticket = (from s in context.tblSoldouts
        //    //                         where s.Store_Id == data.Store_Id && s.ShiftID == data.ShiftID &&
        //    //                         s.EmployeeId == data.EmployeeID && s.Created_On == data.Created_Date
        //    //                         select s).ToList();
        //    //    var returnticket = (from s in context.tblReturntickets
        //    //                        where s.Store_Id == data.Store_Id && s.ShiftID == data.ShiftID &&
        //    //                        s.EmplyeeeId == data.EmployeeID && s.Created_On == data.Created_Date
        //    //                        select s).ToList();

        //    //    foreach (var v in closeticket)
        //    //    {
        //    //        LotteryHistory.Add(new Activate_Ticket
        //    //        {
        //    //            Game_Id = v.Game_Id,
        //    //            Box_No = v.Box_No,
        //    //            Created_Date = Convert.ToDateTime(v.Created_On),
        //    //            Packet_No = v.Packet_Id,
        //    //            Ticket_Name = v.Ticket_Name,
        //    //            Price = Convert.ToInt32(v.Price),
        //    //            Start_No = v.Start_No,
        //    //            End_No = v.End_No,
        //    //            Status = v.Status,
        //    //            // Stopped_At = v.Close_At,
        //    //            EmployeeID = v.EmployeeId,
        //    //            State = v.State,
        //    //            ShiftID = v.ShiftID,
        //    //            Count = (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1).ToString(),
        //    //            Total_Price = Convert.ToInt32(v.Price) * (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1)
        //    //        });
        //    //    }


        //    //        foreach (var i in soldoutticket)
        //    //        {
        //    //            if (i.Partial_Packet != "Y")
        //    //            {
        //    //                LotteryHistory.Add(new Activate_Ticket
        //    //                {
        //    //                    Game_Id = i.Game_Id,
        //    //                    Box_No = i.Box_No,
        //    //                    Created_Date = Convert.ToDateTime(i.Created_On),
        //    //                    Packet_No = i.Packet_Id,
        //    //                    Ticket_Name = i.Ticket_Name,
        //    //                    Price = Convert.ToInt32(i.Price),
        //    //                    Start_No = i.PackPosition_Open,
        //    //                    End_No = i.PackPosition_Close,
        //    //                    Status = i.Status,
        //    //                    //Stopped_At = i.Close_At,
        //    //                    EmployeeID = i.EmployeeId,
        //    //                    State = i.State,
        //    //                    ShiftID = i.ShiftID,
        //    //                    Count = (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1).ToString(),
        //    //                    Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1)
        //    //                });
        //    //            }
        //    //        }

        //    //        foreach (var j in returnticket)
        //    //        {
        //    //            var soldoutpartial = (from s in context.tblSoldouts
        //    //                                  where s.Game_Id == j.Game_Id && s.Packet_Id == j.Packet_Id && s.Store_Id == j.Store_Id && 
        //    //                                  s.ShiftID == data.ShiftID
        //    //                                  && s.Created_On == data.Created_Date
        //    //                                  select s).ToList().FirstOrDefault();
        //    //            if (soldoutpartial != null)
        //    //            {
        //    //                LotteryHistory.Add(new Activate_Ticket
        //    //                {
        //    //                    Game_Id = j.Game_Id,
        //    //                    Box_No = j.Box_No,
        //    //                    Created_Date = Convert.ToDateTime(j.Created_On),
        //    //                    Packet_No = j.Packet_Id,
        //    //                    Ticket_Name = j.Ticket_Name,
        //    //                    Price = Convert.ToInt32(j.Price),
        //    //                    Start_No = soldoutpartial.PackPosition_Open,
        //    //                    End_No = j.PackPosition_Close,
        //    //                    Status = j.Status,
        //    //                    //Stopped_At = i.Close_At,
        //    //                    EmployeeID = j.EmplyeeeId,
        //    //                    State = j.State,
        //    //                    ShiftID = j.ShiftID,
        //    //                    Count = (int.Parse(j.PackPosition_Close) - int.Parse(soldoutpartial.PackPosition_Open) + 1).ToString(),
        //    //                    Total_Price = Convert.ToInt32(j.Price) * (int.Parse(j.PackPosition_Close) - int.Parse(soldoutpartial.PackPosition_Open) + 1)
        //    //                });
        //    //            }
        //    //            else
        //    //            {
        //    //                LotteryHistory.Add(new Activate_Ticket
        //    //                {
        //    //                    Game_Id = j.Game_Id,
        //    //                    Box_No = j.Box_No,
        //    //                    Created_Date = Convert.ToDateTime(j.Created_On),
        //    //                    Packet_No = j.Packet_Id,
        //    //                    Ticket_Name = j.Ticket_Name,
        //    //                    Price = Convert.ToInt32(j.Price),
        //    //                    Start_No = j.PackPosition_Open,
        //    //                    End_No = j.PackPosition_Close,
        //    //                    Status = j.Status,
        //    //                    //Stopped_At = i.Close_At,
        //    //                    EmployeeID = j.EmplyeeeId,
        //    //                    State = j.State,
        //    //                    ShiftID = j.ShiftID,
        //    //                    Count = (int.Parse(j.PackPosition_Close) - int.Parse(j.PackPosition_Open) + 1).ToString(),
        //    //                    Total_Price = Convert.ToInt32(j.Price) * (int.Parse(j.PackPosition_Close) - int.Parse(j.PackPosition_Open) + 1)
        //    //                });
        //    //            }
        //    //        }
        //    //    }
          
           
        //    return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        //}

        //[Route("api/Activate/NewGetDailyActivateHistory")]
        //public HttpResponseMessage NewGetDailyActivateHistory([FromBody] Activate_Ticket data)
        //{
        //    LotteryHistory = new ObservableCollection<Activate_Ticket>();
        //    context = new LotteryBlankDatabaseEntities();
        //    if (data.EmployeeID != 0)
        //    {
        //        var Shift = (from s in context.tblShifts
        //                     where s.StoreId == data.Store_Id
        //                     select s).ToList().LastOrDefault();

        //        sdate = Convert.ToDateTime(Shift.Date);
        //    }

        //    else
        //    {
        //        sdate = data.Created_Date;
        //        temp = 1;
        //    }

        //        var close = (from s in context.tblClose_Box
        //                     where s.Store_Id == data.Store_Id
        //                     && s.Created_On == sdate
        //                     select s).ToList();

        //        var soldret = (from s in context.tblActivated_Tickets
        //                       where s.Store_Id == data.Store_Id
        //                       && s.Created_On == sdate && (s.Status == "SoldOut" || s.Status == "Return")
        //                       select s).ToList();

        //        foreach (var v in close)
        //        {
        //            var active = (from s in context.tblActivated_Tickets
        //                          where s.Store_Id == v.Store_Id && s.Game_Id == v.Game_Id && s.Packet_Id == v.Packet_Id
        //                          && s.Created_On == sdate
        //                          select s).ToList().FirstOrDefault();
                    
        //            if(temp == 1)
        //            {
        //                endno = Convert.ToInt32(v.End_No);
        //            }
        //            else
        //            {
        //                endno = Convert.ToInt32(active.End_No);
        //            }

        //            LotteryHistory.Add(new Activate_Ticket
        //            {
        //                Game_Id = v.Game_Id,
        //                Box_No = v.Box_No,
        //                Created_Date = Convert.ToDateTime(v.Created_On),
        //                Packet_No = v.Packet_Id,
        //                Ticket_Name = v.Ticket_Name,
        //                Price = Convert.ToInt32(v.Price),
        //                Start_No = v.Start_No,
        //                End_No = endno.ToString(),
        //                Status = v.Status,
        //                Stopped_At = v.Close_At,
        //                EmployeeID = v.EmployeeId,
        //                State = v.State,
        //                ShiftID = v.ShiftID,
        //                Count = (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1).ToString(),
        //                Total_Price = Convert.ToInt32(v.Price) * (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1)
        //            });
        //        }

        //        foreach (var i in soldret)
        //        {
        //            LotteryHistory.Add(new Activate_Ticket
        //            {
        //                Game_Id = i.Game_Id,
        //                Box_No = i.Box_No,
        //                Created_Date = Convert.ToDateTime(i.Created_On),
        //                Packet_No = i.Packet_Id,
        //                Ticket_Name = i.Ticket_Name,
        //                Price = Convert.ToInt32(i.Price),
        //                Start_No = i.Start_No,
        //                End_No = i.End_No,
        //                Status = i.Status,
        //                Stopped_At = i.Stopped_At,
        //                EmployeeID = i.EmployeeId,
        //                State = i.State,
        //                ShiftID = i.ShiftID,
        //                Count = (int.Parse(i.End_No) - int.Parse(i.Start_No) + 1).ToString(),
        //                Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.End_No) - int.Parse(i.Start_No) + 1)
        //            });
        //        }
            
        //    //else
        //    //{               
        //    //    var close = (from s in context.tblClose_Box
        //    //                 where s.Store_Id == data.Store_Id
        //    //                 && s.Created_On == data.Created_Date
        //    //                 select s).ToList();

        //    //    var soldret = (from s in context.tblActivated_Tickets
        //    //                   where s.Store_Id == data.Store_Id
        //    //                   && s.Created_On == data.Created_Date && (s.Status == "SoldOut" || s.Status == "Return")
        //    //                   select s).ToList();

        //    //    foreach (var v in close)
        //    //    {
        //    //        //var active = (from s in context.tblActivated_Tickets
        //    //        //              where s.Store_Id == v.Store_Id && s.Game_Id == v.Game_Id && s.Packet_Id == v.Packet_Id
        //    //        //              && s.Created_On == data.Created_Date
        //    //        //              select s).ToList().FirstOrDefault();
        //    //        //if(active != null)
        //    //        //{
        //    //            LotteryHistory.Add(new Activate_Ticket
        //    //            {
        //    //                Game_Id = v.Game_Id,
        //    //                Box_No = v.Box_No,
        //    //                Created_Date = Convert.ToDateTime(v.Created_On),
        //    //                Packet_No = v.Packet_Id,
        //    //                Ticket_Name = v.Ticket_Name,
        //    //                Price = Convert.ToInt32(v.Price),
        //    //                Start_No = v.Start_No,
        //    //                End_No = v.End_No,
        //    //                Status = v.Status,
        //    //                Stopped_At = v.Close_At,
        //    //                EmployeeID = v.EmployeeId,
        //    //                State = v.State,
        //    //                ShiftID = v.ShiftID,
        //    //                Count = (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1).ToString(),
        //    //                Total_Price = Convert.ToInt32(v.Price) * (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1)
        //    //            });
        //    //        //}
                   
        //    //    }

        //    //    foreach (var i in soldret)
        //    //    {
        //    //        LotteryHistory.Add(new Activate_Ticket
        //    //        {
        //    //            Game_Id = i.Game_Id,
        //    //            Box_No = i.Box_No,
        //    //            Created_Date = Convert.ToDateTime(i.Created_On),
        //    //            Packet_No = i.Packet_Id,
        //    //            Ticket_Name = i.Ticket_Name,
        //    //            Price = Convert.ToInt32(i.Price),
        //    //            Start_No = i.Start_No,
        //    //            End_No = i.End_No,
        //    //            Status = i.Status,
        //    //            Stopped_At = i.Stopped_At,
        //    //            EmployeeID = i.EmployeeId,
        //    //            State = i.State,
        //    //            ShiftID = i.ShiftID,
        //    //            Count = (int.Parse(i.End_No) - int.Parse(i.Start_No) + 1).ToString(),
        //    //            Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.End_No) - int.Parse(i.Start_No) + 1)
        //    //        });
        //    //    }
        //    //}
            
        //    return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        //}
    }
}
