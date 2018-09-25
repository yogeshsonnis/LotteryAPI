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
    public class DeactivateController : ApiController
    {
        LotteryBlankDatabaseEntities context;

        public ObservableCollection<Activate_Ticket> LotteryHistory { get; set; }

        [Route("api/Deactivate/GetDeactiveBoxCount")]
        public int GetDeactiveBoxCount()
        {
            context = new LotteryBlankDatabaseEntities();

            int deactivecount = (from t in context.tblDeactivateds where t.Status == "Deactivated" select t).Count();

            return deactivecount;
        }

        [Route("api/Deactivate/Deactivate_Ticket")]
        public HttpResponseMessage Deactivate_Ticket([FromBody] Activate_Ticket data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                if (context.tblActivated_Tickets.Any(o => o.Game_Id == data.Game_Id && o.Packet_Id == data.Packet_No && o.ShiftID == Shift.ShiftID))
                {
                   // var Shift1 = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.EmployeeId == data.EmployeeID select s).ToList().LastOrDefault();

                    var g = (from s in context.tblDeactivateds where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID == Shift.ShiftID select s).FirstOrDefault();

                    string StartNo = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID==Shift.ShiftID select s.Start_No).FirstOrDefault();

                    int EndNo = Convert.ToInt32((from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID == Shift.ShiftID select s.End_No).FirstOrDefault());

                    string Totaltickets = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID == Shift.ShiftID select s.Count).FirstOrDefault();

                    

                    int? totaltickets = Convert.ToInt32(Totaltickets);

                    // int? RemainingTickets = Convert.ToInt32(EndNo) - Convert.ToInt32(StartNo);
                    if (Convert.ToInt32(data.Stopped_At) > EndNo || Convert.ToInt32(data.Stopped_At) < Convert.ToInt32(StartNo))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else if (g != null)
                    {
                        if (Convert.ToInt32(data.Stopped_At) < Convert.ToInt32(StartNo))
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                        else
                        {
                            var v = context.tblDeactivateds.Create();
                            v.Game_Id = g.Game_Id;
                            v.Packet_Id = g.Packet_Id;
                            v.Ticket_Name = g.Ticket_Name;
                            v.Status = "Deactivated";
                            v.State = data.State;
                            v.EmployeeId = g.EmployeeId;
                            v.ShiftID = g.ShiftID;
                            v.Box_No = g.Box_No;
                            v.Price = g.Price;
                            v.Store_Id = g.Store_Id;
                            v.Created_On = Shift.Date;
                            v.Stopped_At = data.Stopped_At;
                            v.Start_No = StartNo;
                            v.ShiftID = Shift.ShiftID;
                            context.tblDeactivateds.Add(v);
                            context.SaveChanges();

                            var v2 = (from s in context.Box_Master
                                      where s.Box_No == data.Box_No && s.Store_Id==data.Store_Id
                                      select s).FirstOrDefault();
                            v2.Status = "Deactivated";
                            context.SaveChanges();


                            var v4 = (from b in context.tblSettleTickets where b.Box_No == data.Box_No select b).FirstOrDefault();
                            if (v4 != null)
                            {
                                context.tblSettleTickets.Attach(v4);
                                context.tblSettleTickets.Remove(v4);
                                context.SaveChanges();
                            }

                            // string EndNo = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No select s.End_No).FirstOrDefault();

                            //string StartNo = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No select s.Start_No).FirstOrDefault();

                            var h = (from s in context.tblSoldouts where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID == Shift.ShiftID select s).FirstOrDefault();

                            if (h != null)
                            {
                                var v5 = context.tblSoldouts.Create();
                                v5.Game_Id = h.Game_Id;
                                v5.Box_No = data.Box_No;
                                v5.Packet_Id = h.Packet_Id;
                                v5.EmployeeId = h.EmployeeId;
                                v5.ShiftID = h.ShiftID;
                                v5.Created_On = Shift.Date;
                                v5.Ticket_Name = h.Ticket_Name;
                                v5.Modified_On = Shift.Date; 
                                v5.Store_Id = h.Store_Id;
                                v5.Price = h.Price.ToString();
                                v5.Partial_Packet = "Y";
                                v5.PackPosition_Open = StartNo;
                                v5.PackPosition_Close = data.Stopped_At.ToString();
                                //v5.RemainingTickets = (Convert.ToInt32(data.Stopped_At) - 000) - 1;
                                v5.RemainingTickets = EndNo- int.Parse(data.Stopped_At);
                                v5.Total_Tickets = (Convert.ToInt32(data.Stopped_At) - Convert.ToInt32(StartNo)) + 1;
                                v5.Status = "SoldOut";
                                v5.State = data.State;
                                v5.ShiftID = Shift.ShiftID;
                                context.tblSoldouts.Add(v5);
                                context.SaveChanges();
                            }
                            else
                            {
                                var v5 = context.tblSoldouts.Create();
                                v5.Game_Id = data.Game_Id;
                                v5.Box_No = data.Box_No;
                                v5.Packet_Id = data.Packet_No;
                                v5.Created_On = Shift.Date;
                                v5.Ticket_Name = data.Ticket_Name;
                                v5.Modified_On = Shift.Date;
                                v5.Price = data.Price.ToString();
                                v5.Partial_Packet = "Y";
                                v5.Store_Id = data.Store_Id;
                                v5.EmployeeId = data.EmployeeID;
                                v5.ShiftID = Shift.ShiftID;
                                v5.PackPosition_Open = StartNo;
                                v5.PackPosition_Close = data.Stopped_At.ToString();
                                //v5.RemainingTickets = (Convert.ToInt32(data.Stopped_At)) - 1;
                                v5.RemainingTickets = EndNo - int.Parse(data.Stopped_At);
                                v5.Total_Tickets = (Convert.ToInt32(data.Stopped_At) - Convert.ToInt32(StartNo)) + 1;
                                v5.Status = "SoldOut";
                                v5.State = data.State;
                                v5.ShiftID = Shift.ShiftID;
                                context.tblSoldouts.Add(v5);
                                context.SaveChanges();
                            }


                            var r = (from s in context.tblRecievedTickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.IsDelete == "N" select s).FirstOrDefault();

                            if (r != null)
                            {
                                r.Start_No = (Convert.ToInt32(r.Start_No) + data.Stopped_At).ToString();
                                context.SaveChanges();
                            }
                            else
                            {
                                if (Convert.ToInt32(data.Stopped_At) != Convert.ToInt32(EndNo))
                                {
                                    var v3 = context.tblRecievedTickets.Create();
                                    v3.Game_Id = data.Game_Id;
                                    v3.Packet_Id = data.Packet_No;
                                    v3.Created_On = Shift.Date;
                                    v3.Ticket_Name = data.Ticket_Name;
                                    v3.Modified_On = Shift.Date;
                                    v3.Status = "Receive";
                                    v3.State = data.State;
                                    v3.Store_Id = data.Store_Id;
                                    v3.Price = data.Price;
                                    v3.EmployeeId = data.EmployeeID;
                                    v3.ShiftID = Shift.ShiftID;
                                    v3.Start_No = (Convert.ToInt32(data.Stopped_At) + 1).ToString();
                                    v3.End_No = EndNo.ToString();
                                    v3.Count = Convert.ToString(Convert.ToInt32(EndNo) - Convert.ToInt32(data.Stopped_At));
                                    int? count = Convert.ToInt32(EndNo) - Convert.ToInt32(data.Stopped_At);
                                    v3.IsDelete = "N";
                                    v3.Total_Price = count * data.Price;
                                    context.tblRecievedTickets.Add(v3);
                                    context.SaveChanges();
                                }
                            }


                            var v7 = context.tblSoldOut_History.Create();
                            v7.Packet_Id = data.Packet_No;
                            v7.EmployeeId = data.EmployeeID;
                            int? Totalcount = Convert.ToInt32(data.Stopped_At) - Convert.ToInt32(EndNo);
                            v7.No_of_Tickets_Sold = Convert.ToInt32(data.Stopped_At) + 1;
                            //v7.No_of_Tickets_Sold = Convert.ToInt32(data.Stopped_At) - int.Parse(StartNo);
                            v7.SoldOut_Date = Shift.Date;
                            v7.Shift_Id = Shift.ShiftID;
                            context.tblSoldOut_History.Add(v7);
                            context.SaveChanges();


                            var v1 = (from s in context.tblActivated_Tickets
                                      where s.Box_No == data.Box_No && s.Store_Id==data.Store_Id
                                      select s).FirstOrDefault();
                            context.tblActivated_Tickets.Attach(v1);
                            context.tblActivated_Tickets.Remove(v1);
                            context.SaveChanges();
                        }
                    }

                    else
                    {
                        var v = context.tblDeactivateds.Create();
                        v.Game_Id = data.Game_Id;
                        v.Packet_Id = data.Packet_No;
                        v.Status = "Deactivated";
                        v.Box_No = data.Box_No;
                        v.Ticket_Name = data.Ticket_Name;
                        v.Store_Id = data.Store_Id;
                        v.Start_No = StartNo;
                        v.EmployeeId = data.EmployeeID;
                        v.Price = data.Price;
                        v.ShiftID = Shift.ShiftID;
                        v.Created_On = Shift.Date;
                        v.Stopped_At = data.Stopped_At;
                        v.State = data.State;
                        v.ShiftID = Shift.ShiftID;
                        context.tblDeactivateds.Add(v);
                        context.SaveChanges();

                        var v2 = (from s in context.Box_Master
                                  where s.Box_No == data.Box_No && s.Store_Id==data.Store_Id
                                  select s).FirstOrDefault();
                        v2.Status = "Deactivated";
                        context.SaveChanges();


                        var v4 = (from b in context.tblSettleTickets where b.Box_No == data.Box_No select b).FirstOrDefault();
                        if (v4 != null)
                        {
                            context.tblSettleTickets.Attach(v4);
                            context.tblSettleTickets.Remove(v4);
                            context.SaveChanges();
                        }

                        string EndNo1 = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID==Shift.ShiftID select s.End_No).FirstOrDefault();

                        string StartNo1 = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID == Shift.ShiftID select s.Start_No).FirstOrDefault();

                        var h = (from s in context.tblSoldouts where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.ShiftID == Shift.ShiftID select s).FirstOrDefault();

                        if (h != null)
                        {
                            var v5 = context.tblSoldouts.Create();
                            v5.Game_Id = h.Game_Id;
                            v5.Box_No = h.Box_No;
                            v5.Packet_Id = h.Packet_Id;
                            v5.Created_On = Shift.Date;
                            v5.Ticket_Name = h.Ticket_Name;
                            v5.Store_Id = h.Store_Id;
                            v5.Modified_On = Shift.Date;
                            v5.EmployeeId = h.EmployeeId;
                            v5.ShiftID = h.ShiftID;
                            v5.Price = h.Price.ToString();
                            v5.PackPosition_Open = (Convert.ToInt32(h.PackPosition_Close) + 1).ToString();
                            v5.PackPosition_Close = data.Stopped_At.ToString();
                            //v5.RemainingTickets = (Convert.ToInt32(data.Stopped_At) - 000) - 1;
                            v5.RemainingTickets = EndNo - int.Parse(data.Stopped_At);
                            v5.Total_Tickets = (Convert.ToInt32(data.Stopped_At) - Convert.ToInt32(StartNo1)) + 1;
                            v5.Status = "SoldOut";
                            v5.State = data.State;
                            v5.Partial_Packet = "Y";
                            context.tblSoldouts.Add(v5);
                            context.SaveChanges();
                        }
                        else
                        {
                            var v5 = context.tblSoldouts.Create();
                            v5.Game_Id = data.Game_Id;
                            v5.Box_No = data.Box_No;
                            v5.Packet_Id = data.Packet_No;
                            v5.Created_On = Shift.Date;
                            v5.Ticket_Name = data.Ticket_Name;
                            v5.Modified_On = Shift.Date;
                            v5.Price = data.Price.ToString();
                            v5.PackPosition_Close = data.Stopped_At.ToString();
                            v5.PackPosition_Open = StartNo;
                            v5.EmployeeId = data.EmployeeID;
                            v5.ShiftID = Shift.ShiftID;
                            v5.Store_Id = data.Store_Id;
                           
                            //v5.RemainingTickets = (Convert.ToInt32(data.Stopped_At)) - 1;
                            v5.RemainingTickets = EndNo - int.Parse(data.Stopped_At);
                            v5.Total_Tickets = (Convert.ToInt32(data.Stopped_At) - Convert.ToInt32(StartNo1)) + 1;
                            v5.Status = "SoldOut";
                            v5.State = data.State;
                            v5.Partial_Packet = "Y";
                            context.tblSoldouts.Add(v5);
                            context.SaveChanges();
                        }


                        var r = (from s in context.tblRecievedTickets where s.Packet_Id == data.Packet_No && s.Game_Id == data.Game_Id && s.IsDelete == "N" select s).FirstOrDefault();

                        if (r != null)
                        {
                            r.Start_No = (Convert.ToInt32(r.Start_No) + data.Stopped_At).ToString();
                            context.SaveChanges();
                        }
                        else
                        {
                            if (Convert.ToInt32(data.Stopped_At) != Convert.ToInt32(EndNo))
                            {
                                var v3 = context.tblRecievedTickets.Create();
                                v3.Game_Id = data.Game_Id;
                                v3.Packet_Id = data.Packet_No;
                                v3.Created_On = Shift.Date;
                                v3.Ticket_Name = data.Ticket_Name;
                                v3.Modified_On = Shift.Date;
                                v3.Status = "Receive";
                                v3.Store_Id = data.Store_Id;
                                v3.State = data.State;
                                v3.Price = data.Price;
                                v3.EmployeeId = data.EmployeeID;
                                v3.ShiftID = Shift.ShiftID;
                                v3.Start_No = (Convert.ToInt32(data.Stopped_At) + 1).ToString();
                                v3.End_No = EndNo.ToString();
                                v3.Count = Convert.ToString(Convert.ToInt32(EndNo1) - Convert.ToInt32(data.Stopped_At));
                                int? count = Convert.ToInt32(EndNo) - Convert.ToInt32(data.Stopped_At);
                                v3.IsDelete = "N";
                                v3.Total_Price = count * data.Price;
                                context.tblRecievedTickets.Add(v3);
                                context.SaveChanges();
                            }

                        }


                        var v7 = context.tblSoldOut_History.Create();
                        v7.Packet_Id = data.Packet_No;
                        v7.EmployeeId = data.EmployeeID;
                        int? Totalcount = Convert.ToInt32(data.Stopped_At) - Convert.ToInt32(EndNo1);
                        v7.No_of_Tickets_Sold = Convert.ToInt32(data.Stopped_At) + 1;
                        //v7.No_of_Tickets_Sold = Convert.ToInt32(data.Stopped_At) - int.Parse(StartNo);
                        v7.SoldOut_Date = Shift.Date;
                        v7.Shift_Id = Shift.ShiftID;
                        context.tblSoldOut_History.Add(v7);
                        context.SaveChanges();


                        var v1 = (from s in context.tblActivated_Tickets
                                  where s.Box_No == data.Box_No && s.Store_Id==data.Store_Id
                                  select s).FirstOrDefault();
                        if(v1!=null)
                        {
                            context.tblActivated_Tickets.Attach(v1);
                            context.tblActivated_Tickets.Remove(v1);
                            context.SaveChanges();
                        }

                        data = null;

                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }

            }

        //[Route("api/Deactivate/GetDeactivateHistory")]
        //public IEnumerable<Activate_Ticket> GetHistory()
        //{
        //    LotteryHistory = new ObservableCollection<Activate_Ticket>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var data = (from s in context.tblDeactivateds where s.Status == "Deactivated" select s).ToList();
        //    foreach (var v in data)
        //    {
        //        LotteryHistory.Add(new Activate_Ticket
        //        {
        //            Game_Id = v.Game_Id,
        //            Box_No = v.Box_No,
        //            Created_Date = Convert.ToDateTime(v.Created_On),
        //            Packet_No = v.Packet_Id,
        //            Ticket_Name = v.Ticket_Name,
        //            Start_No = v.Start_No,
        //            Price = Convert.ToInt16(v.Price),
        //            Stopped_At = v.Stopped_At,
        //            State = v.State,
        //            EmployeeID = v.EmployeeId,
        //        });
        //    }
        //    return LotteryHistory;
        //}

        [Route("api/Deactivate/NewGetDeactivateHistory")]
        public HttpResponseMessage NewGetHostory([FromBody] Activate_Ticket data)
        {
            LotteryHistory = new ObservableCollection<Activate_Ticket>();
            context = new LotteryBlankDatabaseEntities();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.EmployeeId == data.EmployeeID select s).ToList().LastOrDefault();
            if(Shift != null)
            {

            }

            var result = (from s in context.tblDeactivateds where s.Status == "Deactivated" && s.Store_Id == data.Store_Id 
                          && s.EmployeeId == data.EmployeeID select s).ToList();
            foreach (var v in result)
            {
                if (v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
                {
                    LotteryHistory.Add(new Activate_Ticket
                    {
                        Game_Id = v.Game_Id,
                        Box_No = v.Box_No,
                        Created_Date = Convert.ToDateTime(v.Created_On),
                        Packet_No = v.Packet_Id,
                        Ticket_Name = v.Ticket_Name,
                        Start_No = v.Start_No,
                        Price = Convert.ToInt16(v.Price),
                        Stopped_At = v.Stopped_At,
                        ShiftID = v.ShiftID,
                        State = v.State,
                        EmployeeID = v.EmployeeId,
                        Store_Id = v.Store_Id
                    });
                }
                else if(Shift.EndTime == null && v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
                {
                    LotteryHistory.Add(new Activate_Ticket
                    {
                        Game_Id = v.Game_Id,
                        Box_No = v.Box_No,
                        Created_Date = Convert.ToDateTime(v.Created_On),
                        Packet_No = v.Packet_Id,
                        Ticket_Name = v.Ticket_Name,
                        Start_No = v.Start_No,
                        Price = Convert.ToInt16(v.Price),
                        Stopped_At = v.Stopped_At,
                        ShiftID = v.ShiftID,
                        State = v.State,
                        EmployeeID = v.EmployeeId,
                        Store_Id = v.Store_Id
                    });
                }
                   
            }
            // return LotteryHistory;
            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        }
    }
}
