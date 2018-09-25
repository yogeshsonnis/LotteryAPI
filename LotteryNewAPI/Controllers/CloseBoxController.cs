using LotteryNewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LotteryNewAPI.Controllers
{
    public class CloseBoxController : ApiController
    {
        LotteryBlankDatabaseEntities context;

        [Route("api/CloseBox/OnClose_Box")]

        public HttpResponseMessage OnClose_Box([FromBody] Close_Box data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var r = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_Id && s.Game_Id == data.Game_Id && s.Store_Id == Shift.StoreId select s).FirstOrDefault();
                int EndNo = Convert.ToInt32((from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_Id && s.Game_Id == data.Game_Id && s.Store_Id == Shift.StoreId select s.End_No).FirstOrDefault());
                int StartNo = Convert.ToInt32((from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_Id && s.Game_Id == data.Game_Id && s.Store_Id == Shift.StoreId select s.Start_No).FirstOrDefault());

                if (Convert.ToInt32(data.Close_At) > EndNo || Convert.ToInt32(data.Close_At) < StartNo)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else if (r != null)
                {
                    var v = context.tblClose_Box.Create();
                    v.Game_Id = data.Game_Id;
                    v.Packet_Id = data.Packet_Id;
                    v.Status = "Close";
                    v.Box_No = data.Box_No;
                    v.Store_Id = data.Store_Id;
                    v.Ticket_Name = data.Ticket_Name;
                    v.Price = data.Price;
                    v.State = data.State;
                    v.Created_On = Shift.Date;
                    v.Start_No = data.Start_No;
                    v.Close_At = (Convert.ToInt32(data.Close_At) - 1).ToString();
                    v.End_No = r.End_No;
                    //if(Convert.ToInt32(data.Start_No) == 0)
                    //{
                    //    v.Count = (Convert.ToInt32(data.Close_At) - Convert.ToInt32(data.Start_No) + 1).ToString();
                    //}
                    //else
                    //{
                        v.Count = (Convert.ToInt32(data.Close_At) - Convert.ToInt32(data.Start_No)).ToString();
                    //}
                    v.Total_Price = Convert.ToInt32(v.Count) * v.Price;
                    v.EmployeeId = data.EmployeeID;
                    v.ShiftID = Shift.ShiftID;
                    context.tblClose_Box.Add(v);
                    if (v.Box_No == 0 || v.Box_No == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict);
                    }
                    r.Status = "Close";
                    r.Stopped_At = data.Close_At;
                    r.Count = (Convert.ToInt32(data.Close_At) - Convert.ToInt32(data.Start_No)+1).ToString();
                    context.SaveChanges();
                    //r.Start_No = data.Close_At;
                    //r.End_No = EndNo.ToString();
                    //r.Created_On = System.DateTime.Today;
                    //r.Count = (Convert.ToInt32(r.End_No) - Convert.ToInt32(r.Start_No)).ToString();
                    //r.Total_Price = int.Parse(r.Count) * r.Price;
                    //r.ShiftID = Shift.ShiftID;
                    //context.SaveChanges();

                    var v1 = (from s in context.Box_Master
                              where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                              select s).FirstOrDefault();
                    v1.Status = "Close";

                    context.SaveChanges();

                    //var v3 = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_Id && s.Store_Id == data.Store_Id && s.Status == "Active" select s).FirstOrDefault();
                    //if (v3 != null)
                    //{
                    //    //context.tblActivated_Tickets.Attach(v3);
                    //    //context.tblActivated_Tickets.Remove(v3);
                    //    v3.Status = "Close";
                    //    v3.Stopped_At = data.Close_At;
                    //    v3.Count = (Convert.ToInt32(data.Close_At) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                    //    v3.Total_Price = Convert.ToInt32(v3.Count) * data.Price;
                    //    v3.ShiftID = Shift.ShiftID;
                    //    context.SaveChanges();
                    //}

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                //else
                //{
                //    var v3 = (from s in context.tblActivated_Tickets where s.Packet_Id == data.Packet_Id && s.Store_Id == data.Store_Id && s.Status == "Active" select s).FirstOrDefault();
                //    if (v3 != null)
                //    {
                //        //context.tblActivated_Tickets.Attach(v3);
                //        //context.tblActivated_Tickets.Remove(v3);
                //        v3.Status = "Close";
                //        v3.Stopped_At = data.Close_At;
                //        v3.Count = (Convert.ToInt32(data.Close_At) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                //        v3.Total_Price = Convert.ToInt32(v3.Count) * data.Price;
                //        v3.ShiftID = Shift.ShiftID;
                //        context.SaveChanges();
                //    }

                //    if (context.tblActivated_Tickets.Any(o => o.Game_Id == data.Game_Id && o.Packet_Id == data.Packet_Id && o.Store_Id == data.Store_Id))
                //    {
                //        var v = context.tblClose_Box.Create();
                //        v.Game_Id = data.Game_Id;
                //        v.Packet_Id = data.Packet_Id;
                //        v.Status = "Close";
                //        v.Box_No = data.Box_No;
                //        v.Store_Id = data.Store_Id;
                //        v.Ticket_Name = data.Ticket_Name;
                //        v.Price = data.Price;
                //        v.State = data.State;
                //        v.Created_On = System.DateTime.Now;
                //        v.Start_No = data.Start_No;
                //        v.Close_At = (Convert.ToInt32(data.Close_At) - 1).ToString();
                //        v.End_No = data.Close_At;
                //        v.Count = (Convert.ToInt32(v.Close_At) - Convert.ToInt32(data.Start_No) + 1).ToString();
                //        v.Total_Price = Convert.ToInt32(v.Count) * v.Price;
                //        v.EmployeeId = data.EmployeeID;
                //        v.ShiftID = Shift.ShiftID;
                //        context.tblClose_Box.Add(v);
                //        if (v.Box_No == 0 || v.Box_No == null)
                //        {
                //            return Request.CreateResponse(HttpStatusCode.Conflict);
                //        }
                //        context.SaveChanges();


                //        context = new LotteryBlankDatabaseEntities();
                //        var v1 = (from s in context.Box_Master
                //                  where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                //                  select s).FirstOrDefault();
                //        v1.Status = "Close";
                //        context.SaveChanges();

                //        return Request.CreateResponse(HttpStatusCode.OK);
                //    }

                //    else
                //    {
                //        return Request.CreateResponse(HttpStatusCode.BadRequest);
                //    }
                //}
            }
        }

        [Route("api/CloseBox/ReopenBox")]
        public HttpResponseMessage ReopenBox([FromBody] Close_Box data)
        {
            context = new LotteryBlankDatabaseEntities();
            var ShiftInfo = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

            var v = (from s in context.tblClose_Box
                     where s.Game_Id == data.Game_Id && s.Packet_Id == data.Packet_Id && s.Store_Id == data.Store_Id
                    
                     select s).ToList().LastOrDefault();

            var v3 = (from s in context.tblActivated_Tickets
                      where s.Box_No == data.Box_No && s.Packet_Id == data.Packet_Id && s.Store_Id == data.Store_Id && s.ShiftID == ShiftInfo.ShiftID
                      select s).FirstOrDefault();

            if (v != null)
            {
                context.tblClose_Box.Attach(v);
                context.tblClose_Box.Remove(v);
                context.SaveChanges();

                if (v3 != null)
                {
                    v3.Status = "Active";
                    //v3.End_No = v.Close_At;
                    v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                    v3.Stopped_At = null;
                    v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
                    v3.Created_On = Convert.ToDateTime(ShiftInfo.Date);
                    v3.Store_Id = data.Store_Id;
                    v3.ShiftID = ShiftInfo.ShiftID;
                    context.SaveChanges();
                }
                else
                {
                    var v1 = context.tblActivated_Tickets.Create();
                    v1.Game_Id = v.Game_Id;
                    v1.Packet_Id = v.Packet_Id;
                    v1.Status = "Active";
                    v1.Box_No = v.Box_No;
                    v1.Store_Id = v.Store_Id;
                    v1.Ticket_Name = v.Ticket_Name;
                    v1.EmployeeId = v.EmployeeId;
                    v1.Price = v.Price;
                    v1.State = v.State;
                    v1.Created_On = Convert.ToDateTime(ShiftInfo.Date);
                    v1.Start_No = v.Start_No;
                    v1.End_No = v.End_No;
                    v1.Count = (Convert.ToInt32(v1.Start_No) + Convert.ToInt32(v1.End_No) + 1).ToString();
                    v1.Total_Price = v1.Price * Convert.ToInt32(v1.Count);
                    v1.Store_Id = v.Store_Id;
                    v1.ShiftID = ShiftInfo.ShiftID;
                    context.tblActivated_Tickets.Add(v1);
                    context.SaveChanges();
                }
                var v2 = (from s in context.Box_Master
                          where s.Box_No == data.Box_No && s.Store_Id == data.Store_Id
                          select s).FirstOrDefault();
                if (v2 != null)
                {
                    v2.Status = "Active";
                    v2.Store_Id = data.Store_Id;
                    context.SaveChanges();
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/CloseBox/NewGetChangetoActive")]
        public HttpResponseMessage NewGetChangetoActive([FromBody] Close_Box data)
        {
            context = new LotteryBlankDatabaseEntities();

            var ShiftInfo = (from s in context.tblShifts where s.StoreId == data.Store_Id  select s).ToList().LastOrDefault();
            if (ShiftInfo != null)
            {
                ShiftInfo.IsClose = true;
                ShiftInfo.EndTime = System.DateTime.Now.ToString("h:mm:ss tt");
                ShiftInfo.CloseDate = System.DateTime.Now;
                context.SaveChanges();
            }

            //var v = (from s in context.tblClose_Box
            //         where s.EmployeeId == data.EmployeeID && s.Store_Id == data.Store_Id && s.ShiftID == ShiftInfo.ShiftID
            //         select s).ToList();

            //if (v != null)
            //{
            //    foreach (var b in v)
            //    {
            //        var v3 = (from s in context.tblActivated_Tickets
            //                  where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id && s.ShiftID == ShiftInfo.ShiftID
            //                  && s.Status == "Close"
            //                  select s).FirstOrDefault();
            //        if (v3 != null)
            //        {
            //            int EndNo = Convert.ToInt32(v3.End_No);
            //            int StoppedAt = Convert.ToInt32(v3.Stopped_At);

            //            if (EndNo == StoppedAt)
            //            {
            //                v3.Status = "Active";
            //                v3.End_No = (Convert.ToInt32(v3.End_No)).ToString();
            //                v3.Created_On = System.DateTime.Today;
            //                v3.Start_No = (Convert.ToInt32(b.Close_At) + 1).ToString();
            //                v3.ShiftID = ShiftInfo.ShiftID + 1;
            //                v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
            //                v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
            //                v3.Store_Id = data.Store_Id;
            //                v3.EmployeeId = data.EmployeeID;
            //                context.SaveChanges();

            //                var v1 = (from s in context.Box_Master
            //                          where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
            //                          select s).FirstOrDefault();
            //                if (v1 != null)
            //                {
            //                    v1.Status = "Active";
            //                    context.SaveChanges();
            //                }
            //            }
            //            else
            //            {
            //                v3.Status = "Active";
            //                v3.End_No = v3.End_No;
            //                v3.Created_On = System.DateTime.Today;
            //                v3.EmployeeId = data.EmployeeID;
            //                v3.Start_No = (Convert.ToInt32(b.Close_At) + 1).ToString();
            //                v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
            //                v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
            //                v3.Store_Id = data.Store_Id;
            //                v3.ShiftID = ShiftInfo.ShiftID + 1;
            //                context.SaveChanges();

            //                var v1 = (from s in context.Box_Master
            //                          where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
            //                          select s).FirstOrDefault();
            //                if (v1 != null)
            //                {
            //                    v1.Status = "Active";
            //                    context.SaveChanges();
            //                }

            //                context.tblClose_Box.Attach(b);
            //                context.tblClose_Box.Remove(b);
            //                context.SaveChanges();
            //            }
            //        }
            //    }
            //}

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/CloseBox/NewGetChangeSoldOutToEmpty")]
        public HttpResponseMessage NewGetChangeSoldOutToEmpty([FromBody] Close_Box data)
        {
            context = new LotteryBlankDatabaseEntities();

            var v = (from s in context.tblSoldouts
                     where s.EmployeeId == data.EmployeeID && s.Store_Id == data.Store_Id
                     select s).ToList();

            if (v != null)
            {
                foreach (var i in v)
                {
                    var v1 = (from s in context.tblActivated_Tickets
                              where s.EmployeeId == data.EmployeeID && s.Store_Id == data.Store_Id && s.Status == "Active"
                              && s.Box_No == i.Box_No
                              select s).FirstOrDefault();
                    if (v1 != null)
                    {
                        var boxMaster = (from s in context.Box_Master
                                         where s.Store_Id == data.Store_Id && s.Box_No == i.Box_No 
                                         select s).FirstOrDefault();
                        if (boxMaster != null)
                        {
                            boxMaster.Status = "Active";
                            context.SaveChanges();
                        }
                    }

                    else
                    {
                        var boxMaster = (from s in context.Box_Master
                                         where s.Store_Id == data.Store_Id && s.Box_No == i.Box_No
                                         select s).FirstOrDefault();
                        if (boxMaster != null)
                        {
                            boxMaster.Status = "Empty";
                            context.SaveChanges();
                        }
                    }
                }

            }


            //    foreach (var b in v)
            //    {
            //        if (b.Status == "SoldOut")
            //        {
            //            var boxMaster = (from s in context.Box_Master
            //                             where s.Store_Id == data.Store_Id && s.Box_No == b.Box_No
            //                             select s).FirstOrDefault();
            //            if (boxMaster != null)
            //            {
            //                boxMaster.Status = "Empty";
            //                context.SaveChanges();
            //            }
            //        }
            //    }
            //}                  

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/CloseBox/ChangeBox")]

        public HttpResponseMessage ChangeBox([FromBody] Activate_Ticket Data)
        {
            context = new LotteryBlankDatabaseEntities();

            var i = (from s in context.Box_Master where s.Store_Id == Data.Store_Id && s.Box_No == Data.Box_No select s).ToList().FirstOrDefault();

            var j = (from s in context.Box_Master where s.Store_Id == Data.Store_Id && s.Box_No == Data.ChangeToBox select s).ToList().FirstOrDefault();

            if(i == null || j==null)
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable);
            }

            if(i.Status == "Empty")
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            if (i.Status == "Active" || i.Status == "Settle")
            {
                if (j.Status == "Empty" || j.Status == "SoldOut")
                {
                    if (i.Status == "Settle")
                    {
                        j.Status = i.Status;
                        context.SaveChanges();

                        i.Status = "Empty";
                        context.SaveChanges();
                        var r = (from s in context.tblShifts where s.StoreId == Data.Store_Id select s).ToList().LastOrDefault();
                        var v = (from s in context.tblActivated_Tickets
                                 where s.Store_Id == Data.Store_Id && s.Box_No == Data.Box_No
                                 && s.ShiftID == r.ShiftID && s.Created_On == r.Date && s.Status == "Active"
                                 select s).ToList().FirstOrDefault();
                        var p = (from s in context.tblSettleTickets
                                 where s.Store_Id == Data.Store_Id && s.Box_No == Data.Box_No
                                   && s.ShiftID == r.ShiftID && s.Created_On == r.Date && s.Status == "Settle" && s.Game_Id == v.Game_Id && s.Packet_Id == v.Packet_Id
                                 select s).ToList().FirstOrDefault();
                        p.Box_No = Data.ChangeToBox;
                        context.SaveChanges();
                        v.Box_No = p.Box_No;
                        context.SaveChanges();


                    }
                    if (i.Status == "Active")
                    {


                        j.Status = i.Status;
                        context.SaveChanges();

                        i.Status = "Empty";
                        context.SaveChanges();


                        var r = (from s in context.tblShifts where s.StoreId == Data.Store_Id select s).ToList().LastOrDefault();
                        var v = (from s in context.tblActivated_Tickets
                                 where s.Store_Id == Data.Store_Id && s.Box_No == Data.Box_No
                                 && s.ShiftID == r.ShiftID && s.Created_On == r.Date && s.Status == "Active"
                                 select s).ToList().FirstOrDefault();
                        v.Box_No = Data.ChangeToBox;
                        context.SaveChanges();
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
                       
            


           
        }
    }
}

   

