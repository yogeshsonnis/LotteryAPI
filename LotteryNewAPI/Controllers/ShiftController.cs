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
    public class ShiftController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        public ObservableCollection <Shift> ShiftDetails { get; set; }
        int i = 0;

        [Route("api/Shift/SaveShiftInfo")]
        public HttpResponseMessage SaveShiftInfo([FromBody] Shift data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var ShiftInfo = (from s in context.tblShifts where s.StoreId == data.StoreId select s).ToList().LastOrDefault();
                //if(ShiftInfo != null)
                //{
                //    if (ShiftInfo.EmployeeId != data.EmployeeId)
                //    {
                //        if (ShiftInfo.IsClose == false || ShiftInfo.IsClose == null)
                //        {
                //            return Request.CreateResponse(HttpStatusCode.NotFound);
                //        }
                //    }
                //}
                //if(ShiftInfo != null)
                //{
                //    if (ShiftInfo.IsLastShift == true && ShiftInfo.IsClose == null && data.IsReportGenerated == null)
                //    {
                //        ShiftInfo.IsLastShift = false;
                //        context.SaveChanges();
                //    }
                //}     


                if (ShiftInfo != null && data.IsCheck == 0 && data.IsLastShift == true)
                {
                    if (ShiftInfo.IsLastShift == true && ShiftInfo.IsClose == null && data.IsReportGenerated == null)
                    {
                        ShiftInfo.IsLastShift = false;
                        context.SaveChanges();
                    }
                }
                if (ShiftInfo != null && data.IsCheck == 1)
                {

                    ShiftInfo.IsLastShift = false;
                    ShiftInfo.IsReportGenerated = false;
                    context.SaveChanges();

                }


                if (data.IsLastShift == true)
                {

                    ShiftInfo.IsLastShift = true;

                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                if (data.IsLastShift == false)
                {
                    ShiftInfo.IsLastShift = null;

                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (data.IsReportGenerated == true)
                {
                    ShiftInfo.IsReportGenerated = true;
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (ShiftInfo != null)
                {
                    if (ShiftInfo.IsLastShift == true && ShiftInfo.IsClose == true && ShiftInfo.Date == System.DateTime.Today)
                    {
                        ShiftInfo.IsLastShift = null;
                        context.SaveChanges();
                        var v = context.tblShifts.Create();
                        v.StoreId = data.StoreId;
                        v.EmployeeId = data.EmployeeId;
                        v.ShiftID = ShiftInfo.ShiftID + 1;
                        v.StartTime = System.DateTime.Now.ToString("h:mm:ss tt");
                        v.IsLastShift = null;
                        v.Date = ShiftInfo.Date;
                        context.tblShifts.Add(v);
                        context.SaveChanges();
                    }

                    else if (ShiftInfo.IsClose == true)
                    {
                        //DateTime temp = Convert.ToDateTime(ShiftInfo.Date);
                        var v = context.tblShifts.Create();
                        v.StoreId = data.StoreId;
                        v.EmployeeId = data.EmployeeId;
                        //v.ShiftID = 1;
                        v.StartTime = System.DateTime.Now.ToString("h:mm:ss tt");
                        v.IsLastShift = null;
                        if (ShiftInfo.IsLastShift == true)
                        {
                            Double temp = (System.DateTime.Today - Convert.ToDateTime(ShiftInfo.Date)).TotalDays;
                            v.Date = Convert.ToDateTime(ShiftInfo.Date).AddDays(temp);
                            v.ShiftID = 1;
                        }
                        else
                        {
                            v.Date = ShiftInfo.Date;
                            v.ShiftID = ShiftInfo.ShiftID + 1;
                        }

                        context.tblShifts.Add(v);
                        context.SaveChanges();
                    }

                    else if (ShiftInfo.IsClose == null)
                    {

                        ShiftInfo.EmployeeId = data.EmployeeId;
                        //v.ShiftID = 1;
                        //v.StartTime = System.DateTime.Now.ToString("h:mm:ss tt");
                        context.SaveChanges();

                    }


                }
                else
                {
                    var v = context.tblShifts.Create();
                    v.StoreId = data.StoreId;
                    v.EmployeeId = data.EmployeeId;
                    v.ShiftID = 1;
                    v.StartTime = System.DateTime.Now.ToString("h:mm:ss tt");
                    //v.IsLastShift = null;
                    v.Date = System.DateTime.Today;
                    context.tblShifts.Add(v);
                    context.SaveChanges();
                }


                if (ShiftInfo != null && data.IsLastShift == null)
                {
                    //if (ShiftInfo.IsClose != false || ShiftInfo.IsClose != null)
                    //{
                    var ShiftInfo1 = (from s in context.tblShifts where s.StoreId == data.StoreId select s).ToList().LastOrDefault();

                    var v1 = (from s in context.tblClose_Box
                              where s.Store_Id == ShiftInfo.StoreId
                              select s).ToList();      // Just to declare v1


                    if (ShiftInfo.ShiftID != ShiftInfo1.ShiftID)
                    {
                        v1 = (from s in context.tblClose_Box
                              where s.Store_Id == ShiftInfo.StoreId && s.ShiftID == ShiftInfo.ShiftID
                              select s).ToList();

                        if (v1 != null)
                        {
                            foreach (var b in v1)
                            {
                                var v3 = (from s in context.tblActivated_Tickets
                                          where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id && s.ShiftID == ShiftInfo.ShiftID
                                          && s.Status == "Close"
                                          select s).FirstOrDefault();

                                if (v3 != null)
                                {
                                    var v4 = (from s in context.tblClose_Box
                                              where s.Game_Id == v3.Game_Id && s.Packet_Id == v3.Packet_Id
                                              select s).ToList().LastOrDefault();

                                    int EndNo = Convert.ToInt32(v3.End_No);
                                    int StoppedAt = Convert.ToInt32(v3.Stopped_At);

                                    if (EndNo == StoppedAt)
                                    {
                                        v3.Status = "Active";
                                        v3.End_No = (Convert.ToInt32(v3.End_No)).ToString();
                                        v3.Created_On = Convert.ToDateTime(ShiftInfo1.Date);
                                        v3.Start_No = (Convert.ToInt32(v4.Close_At) + 1).ToString();
                                        v3.ShiftID = ShiftInfo1.ShiftID;
                                        v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                                        v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
                                        v3.Store_Id = ShiftInfo1.StoreId;
                                        v3.EmployeeId = ShiftInfo1.EmployeeId;
                                        context.SaveChanges();

                                        var v2 = (from s in context.Box_Master
                                                  where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
                                                  select s).FirstOrDefault();
                                        if (v2 != null)
                                        {
                                            v2.Status = "Active";
                                            context.SaveChanges();
                                        }

                                        // b.End_No = v3.End_No;
                                    }
                                    else
                                    {
                                        v3.Status = "Active";
                                        v3.End_No = v3.End_No;
                                        v3.Created_On = Convert.ToDateTime(ShiftInfo1.Date);
                                        v3.EmployeeId = ShiftInfo1.EmployeeId;
                                        v3.Start_No = (Convert.ToInt32(v4.Close_At) + 1).ToString();
                                        v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                                        v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
                                        v3.Store_Id = ShiftInfo1.StoreId;
                                        v3.ShiftID = ShiftInfo1.ShiftID;
                                        context.SaveChanges();

                                        var v2 = (from s in context.Box_Master
                                                  where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
                                                  select s).FirstOrDefault();
                                        if (v2 != null)
                                        {
                                            v2.Status = "Active";
                                            context.SaveChanges();
                                        }
                                        //b.End_No = v3.End_No;
                                        //context.tblClose_Box.Attach(b);
                                        //context.tblClose_Box.Remove(b);
                                        //context.SaveChanges();
                                    }
                                }
                            }
                        }
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        var get = (from s in context.tblShifts where s.StoreId == data.StoreId && s.Date == ShiftInfo.Date select s).ToList();
                        if (get.Count > 1)
                        {
                            var j = (from s in context.tblShifts where s.StoreId == data.StoreId select s).ToList().AsEnumerable().Reverse().Skip(1).FirstOrDefault();

                            v1 = (from s in context.tblClose_Box
                                  where s.Store_Id == ShiftInfo.StoreId && s.ShiftID == j.ShiftID
                                  select s).ToList();

                            if (v1 != null)
                            {
                                foreach (var b in v1)
                                {
                                    var v3 = (from s in context.tblActivated_Tickets
                                              where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id && s.ShiftID == j.ShiftID
                                              && s.Status == "Close"
                                              select s).FirstOrDefault();

                                    if (v3 != null)
                                    {
                                        var v4 = (from s in context.tblClose_Box
                                                  where s.Game_Id == v3.Game_Id && s.Packet_Id == v3.Packet_Id
                                                  select s).ToList().LastOrDefault();

                                        int EndNo = Convert.ToInt32(v3.End_No);
                                        int StoppedAt = Convert.ToInt32(v3.Stopped_At);

                                        if (EndNo == StoppedAt)
                                        {
                                            v3.Status = "Active";
                                            v3.End_No = (Convert.ToInt32(v3.End_No)).ToString();
                                            v3.Created_On = Convert.ToDateTime(ShiftInfo1.Date);
                                            v3.Start_No = (Convert.ToInt32(v4.Close_At) + 1).ToString();
                                            v3.ShiftID = ShiftInfo1.ShiftID;
                                            v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                                            v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
                                            v3.Store_Id = ShiftInfo1.StoreId;
                                            v3.EmployeeId = data.EmployeeId;
                                            context.SaveChanges();

                                            var v2 = (from s in context.Box_Master
                                                      where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
                                                      select s).FirstOrDefault();
                                            if (v2 != null)
                                            {
                                                v2.Status = "Active";
                                                context.SaveChanges();
                                            }

                                            // b.End_No = v3.End_No;
                                        }
                                        else
                                        {
                                            v3.Status = "Active";
                                            v3.End_No = v3.End_No;
                                            v3.Created_On = Convert.ToDateTime(ShiftInfo1.Date);
                                            v3.EmployeeId = data.EmployeeId;
                                            v3.Start_No = (Convert.ToInt32(v4.Close_At) + 1).ToString();
                                            v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                                            v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
                                            v3.Store_Id = ShiftInfo1.StoreId;
                                            v3.ShiftID = ShiftInfo1.ShiftID;
                                            context.SaveChanges();

                                            var v2 = (from s in context.Box_Master
                                                      where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
                                                      select s).FirstOrDefault();
                                            if (v2 != null)
                                            {
                                                v2.Status = "Active";
                                                context.SaveChanges();
                                            }
                                            //b.End_No = v3.End_No;
                                            //context.tblClose_Box.Attach(b);
                                            //context.tblClose_Box.Remove(b);
                                            //context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            var j = (from s in context.tblShifts where s.StoreId == data.StoreId select s).ToList().AsEnumerable().Reverse().Skip(1).FirstOrDefault();
                            if(j!=null)
                            {
                                v1 = (from s in context.tblClose_Box
                                      where s.Store_Id == ShiftInfo.StoreId && s.ShiftID == j.ShiftID
                                      select s).ToList();
                            }
                            if (v1 != null && j != null)
                            {
                                foreach (var b in v1)
                                {
                                    var v3 = (from s in context.tblActivated_Tickets
                                              where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id && s.ShiftID == j.ShiftID
                                              && s.Status == "Close"
                                              select s).FirstOrDefault();

                                    if (v3 != null)
                                    {
                                        var v4 = (from s in context.tblClose_Box
                                                  where s.Game_Id == v3.Game_Id && s.Packet_Id == v3.Packet_Id
                                                  select s).ToList().LastOrDefault();

                                        int EndNo = Convert.ToInt32(v3.End_No);
                                        int StoppedAt = Convert.ToInt32(v3.Stopped_At);

                                        if (EndNo == StoppedAt)
                                        {
                                            v3.Status = "Active";
                                            v3.End_No = (Convert.ToInt32(v3.End_No)).ToString();
                                            v3.Created_On = Convert.ToDateTime(ShiftInfo1.Date);
                                            v3.Start_No = (Convert.ToInt32(v4.Close_At) + 1).ToString();
                                            v3.ShiftID = ShiftInfo1.ShiftID;
                                            v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                                            v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
                                            v3.Store_Id = ShiftInfo1.StoreId;
                                            v3.EmployeeId = data.EmployeeId;
                                            context.SaveChanges();

                                            var v2 = (from s in context.Box_Master
                                                      where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
                                                      select s).FirstOrDefault();
                                            if (v2 != null)
                                            {
                                                v2.Status = "Active";
                                                context.SaveChanges();
                                            }

                                            // b.End_No = v3.End_No;
                                        }
                                        else
                                        {
                                            v3.Status = "Active";
                                            v3.End_No = v3.End_No;
                                            v3.Created_On = Convert.ToDateTime(ShiftInfo1.Date);
                                            v3.EmployeeId = data.EmployeeId;
                                            v3.Start_No = (Convert.ToInt32(v4.Close_At) + 1).ToString();
                                            v3.Count = (Convert.ToInt32(v3.End_No) - Convert.ToInt32(v3.Start_No) + 1).ToString();
                                            v3.Total_Price = Convert.ToInt32(v3.Count) * v3.Price;
                                            v3.Store_Id = ShiftInfo1.StoreId;
                                            v3.ShiftID = ShiftInfo1.ShiftID;
                                            context.SaveChanges();

                                            var v2 = (from s in context.Box_Master
                                                      where s.Box_No == b.Box_No && s.Store_Id == b.Store_Id
                                                      select s).FirstOrDefault();
                                            if (v2 != null)
                                            {
                                                v2.Status = "Active";
                                                context.SaveChanges();
                                            }
                                            //b.End_No = v3.End_No;
                                            //context.tblClose_Box.Attach(b);
                                            //context.tblClose_Box.Remove(b);
                                            //context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                    }

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
        }

        [Route("api/Shift/ShiftLogout")]
        public HttpResponseMessage ShiftLogout([FromBody] Shift data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var ShiftInfo = (from s in context.tblShifts where s.StoreId == data.StoreId && s.EmployeeId == data.EmployeeId && s.Date == System.DateTime.Today select s).ToList().LastOrDefault();

                if (ShiftInfo != null)
                {
                    if (ShiftInfo.IsClose == false || ShiftInfo.IsClose == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        [Route("api/Shift/NewGetShiftDetails")]
        public HttpResponseMessage NewGetShiftDetails([FromBody] Shift data)
        {
            ShiftDetails = new ObservableCollection<Shift>();
            context = new LotteryBlankDatabaseEntities();
            
            if(data.EmployeeId == 0)
            {
                var ShiftDetail = (from s in context.tblShifts where s.StoreId == data.StoreId select s).ToList();
                foreach (var v in ShiftDetail)
                {
                    var x = (from s in context.tblTerminal_Data1 where s.Store_Id == data.StoreId && s.Date == v.Date 
                             && s.ShiftID == v.ShiftID select s).ToList().FirstOrDefault(); 
                    if(x != null)
                    { 
                    ShiftDetails.Add(new Shift
                        {
                            ShiftId = v.ShiftID,
                            EmployeeId = Convert.ToInt32(v.EmployeeId),
                            Date = Convert.ToDateTime(v.Date),
                            CloseDate = Convert.ToDateTime(v.CloseDate),
                            EndTime = v.EndTime,
                            IsClose = Convert.ToBoolean(v.IsClose)
                        });
                    }
                }
            }
            else
            {
                var ShiftDetail = (from s in context.tblShifts where s.StoreId == data.StoreId select s).ToList();
                foreach (var v in ShiftDetail)
                {
                    ShiftDetails.Add(new Shift
                        {
                            ShiftId = v.ShiftID,
                            EmployeeId = Convert.ToInt32(v.EmployeeId),
                            Date = Convert.ToDateTime(v.Date),
                            CloseDate = Convert.ToDateTime(v.CloseDate),
                            EndTime = v.EndTime,
                            IsLastShift = v.IsLastShift,
                            IsClose = Convert.ToBoolean(v.IsClose),
                           
                            IsReportGenerated = v.IsReportGenerated

                    });
                }
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, ShiftDetails);
        }

    }
}

