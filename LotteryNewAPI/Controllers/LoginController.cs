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
    public class LoginController : ApiController
    {
        LotteryBlankDatabaseEntities Context;

        public ObservableCollection<Login> EmployeeDetails { get; set; }

        public ObservableCollection<Shift> ShiftDetails { get; set; }

        [Route("api/Login/Employee_Registration")]

        public HttpResponseMessage Employee_Registration([FromBody] Login data)
        {
            using (Context = new LotteryBlankDatabaseEntities())
            {
                if (data.StoreId != 0 && data.Password != null && data.Name != null)  // For Add User Popup
                {
                    var i = (from s in Context.tblEmployee_Details where s.Username == data.Username select s).ToList().FirstOrDefault();

                    var r = (from s in Context.tblStore_Info where s.Store_Id == data.StoreId select s).ToList().FirstOrDefault();

                    if (i != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        // var y = (from s in Context.tblEmployee_Details where s.StoreId == data.StoreId select s).ToList().LastOrDefault();
                        var v = Context.tblEmployee_Details.Create();
                        v.EmployeeName = data.Name;
                        v.Contact = data.Contactno;
                        v.Username = data.Username;
                        v.Password = data.Password;
                        v.EmailId = data.EmailId;
                        v.StoreId = Convert.ToInt32(data.StoreId);
                        v.IsManager = data.IsManager;
                        v.IsEmployee = data.IsEmployee;
                        v.IsAssignStore = data.IsAssignStore;
                        v.Store_Address = data.Address;
                        v.Address = data.Address;
                        //if (y != null)
                        //{
                        //    v.EmployeeId = y.EmployeeId + 1;
                        //}
                        //else
                        //{
                        //    v.EmployeeId = 1;
                        //}
                        Context.tblEmployee_Details.Add(v);
                        Context.SaveChanges();

                        // var j = (from s in Context.tblStoreEmployeeInfoes where s.Store_Id == data.StoreId select s).ToList().FirstOrDefault();

                        //var k = (from s in Context.tblEmployee_Details where s.Username == data.Username && s.StoreId == data.StoreId select s).ToList().FirstOrDefault();
                        var y = (from s in Context.tblEmployee_Details where s.Username == data.Username select s).ToList().LastOrDefault();
                        var x = Context.tblStoreEmployeeInfoes.Create();
                        x.StoreId = data.StoreId;
                        if (y != null)
                        {
                            x.EmployeeId = y.EmployeeId;
                        }
                        else
                        {
                            x.EmployeeId = 1;
                        }
                        x.Employee_Address = data.Address;
                        x.Store_Address = data.Address;
                        Context.tblStoreEmployeeInfoes.Add(x);
                        Context.SaveChanges();
                    }


                }
                else if (data.StoreId != 0 && data.Username != null && data.Password == null && data.Name == null)  // For Delete User Popup
                {
                    var v = (from s in Context.tblEmployee_Details where s.Username == data.Username && s.StoreId == data.StoreId select s).ToList().FirstOrDefault();
                    var w = (from s in Context.tblStoreEmployeeInfoes where s.EmployeeId == v.EmployeeId && s.StoreId == v.StoreId select s).ToList().FirstOrDefault();
                    if (v != null)
                    {
                        Context.tblEmployee_Details.Remove(v);
                        Context.tblStoreEmployeeInfoes.Remove(w);
                        Context.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }

                else if (data.StoreId != 0 || data.Username != null || data.Password == null) // For Edit user Popup
                {
                    var i = (from s in Context.tblEmployee_Details where s.Username == data.Username && s.Store_Address == data.Address select s).ToList().FirstOrDefault();

                    if (i != null)
                    {
                        //i.Username = data.Username;
                        i.EmailId = data.EmailId;
                        i.Contact = data.Contactno;
                        i.EmployeeName = data.Name;
                        i.IsManager = data.IsManager;
                        i.IsEmployee = data.IsEmployee;
                        i.IsAssignStore = data.IsAssignStore;
                        Context.SaveChanges();
                    }
                }
                else
                {
                    //var v = Context.tblEmployee_Details.Create();  // For Signup Popup
                    //v.EmployeeName = data.Name;
                    //v.Address = data.Address;
                    //v.Contact = data.Contactno;
                    //v.BirthDate = data.Dob;
                    //v.Username = data.Username;
                    //v.Password = data.Password;
                    //Context.tblEmployee_Details.Add(v);
                    //Context.SaveChanges();
                }

            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/Login/Employee_RememberPassword")]

        public HttpResponseMessage Employee_RememberPassword([FromBody] Login data)
        {
            using (Context = new LotteryBlankDatabaseEntities())
            {
                var v = (from s in Context.tblEmployee_Details where s.Username==data.Username && s.Password==data.Password select s).ToList().FirstOrDefault();
                if(v!=null)
                {
                    v.IsRememberMe = data.IsRememberMe;
                    Context.SaveChanges();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);

        }

        //public HttpResponseMessage Employee_Registration([FromBody] Login data)
        //{
        //    using (Context = new LotteryBlankDatabaseEntities())
        //    {
        //        if (data.StoreId != 0 && data.Password != null && data.Name != null)  // For Add User Popup
        //        {
        //            var i = (from s in Context.tblEmployee_Details where s.Username == data.Username && s.StoreId == data.StoreId select s).ToList().FirstOrDefault();

        //            var r = (from s in Context.tblStore_Info where s.Store_Id == data.StoreId select s).ToList().FirstOrDefault();


        //            if (i != null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.NotFound);
        //            }
        //            else
        //            {
        //                var v = Context.tblEmployee_Details.Create();
        //                v.EmployeeName = data.Name;
        //                v.Contact = data.Contactno;
        //                v.Username = data.Username;
        //                v.Password = data.Password;
        //                v.EmailId = data.EmailId;
        //                v.StoreId = Convert.ToInt32(data.StoreId);
        //                v.IsManager = data.IsManager;
        //                v.IsEmployee = data.IsEmployee;
        //                v.IsAssignStore = data.IsAssignStore;
        //                v.Store_Address = r.Store_Address;

        //                Context.tblEmployee_Details.Add(v);
        //                Context.SaveChanges();

        //                var j = (from s in Context.tblStore_Info where s.Store_Id == data.StoreId select s).ToList().FirstOrDefault();

        //                var k = (from s in Context.tblEmployee_Details where s.Username == data.Username && s.StoreId == data.StoreId select s).ToList().FirstOrDefault();

        //                var x = Context.tblStore_Info.Create();
        //                x.Store_Name = j.Store_Name;
        //                x.Open_Time = j.Open_Time;
        //                x.Close_Time = j.Close_Time;
        //                x.Store_Hours = j.Store_Hours;
        //                x.No_Of_Boxes = j.No_Of_Boxes;
        //                x.EmployeeId = k.EmployeeId;
        //                x.Store_Address = j.Store_Address;
        //                x.Store_Id = data.StoreId;

        //                Context.tblStore_Info.Add(x);
        //                Context.SaveChanges();
        //            }


        //        }
        //        else if (data.StoreId != 0 && data.Username != null && data.Password == null && data.Name == null)  // For Delete User Popup
        //        {
        //            var v = (from s in Context.tblEmployee_Details where s.Username == data.Username && s.StoreId == data.StoreId select s).ToList().FirstOrDefault();

        //            if(v != null)
        //            {
        //                Context.tblEmployee_Details.Remove(v);
        //                Context.SaveChanges();
        //                return Request.CreateResponse(HttpStatusCode.OK);
        //            }
        //        }

        //        else if (data.StoreId != 0 || data.Username != null || data.Password == null) // For Edit user Popup
        //        {
        //            var i = (from s in Context.tblEmployee_Details where s.Username == data.Username && s.StoreId == data.StoreId select s).ToList().FirstOrDefault();

        //            if(i != null)
        //            {
        //                //i.Username = data.Username;
        //                i.EmailId = data.EmailId;
        //                i.Contact = data.Contactno;
        //                i.EmployeeName = data.Name;
        //                i.IsManager = data.IsManager;
        //                i.IsEmployee = data.IsEmployee;
        //                i.IsAssignStore = data.IsAssignStore;
        //                Context.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            var v = Context.tblEmployee_Details.Create();  // For Signup Popup
        //            v.EmployeeName = data.Name;
        //            v.Address = data.Address;
        //            v.Contact = data.Contactno;
        //            v.BirthDate = data.Dob;
        //            v.Username = data.Username;
        //            v.Password = data.Password;
        //            Context.tblEmployee_Details.Add(v);
        //            Context.SaveChanges();
        //        }

        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [Route("api/Login/ChangePassword")]
        public HttpResponseMessage ChangePassword([FromBody] Store_Info Data)
        {
            Context = new LotteryBlankDatabaseEntities();
            int flag = 0;
            if (Data.Password != null)  // Change Password
            {
                var v = (from s in Context.tblEmployee_Details where s.StoreId == Data.StoreID && s.EmployeeId == Data.EmployeeID select s).ToList().FirstOrDefault();

                if (v.Password == Data.Password)
                {
                    v.Password = Data.NewPassword;
                    Context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                if (Data.Index != 0 && Data.StoreName != null)  // For Delete Email
                {
                    var i = (from s in Context.tblStore_Info
                             where s.Store_Id == Data.StoreID
                             select s).ToList();

                    if (Data.Index == 1)
                    {
                        foreach (var v in i)
                        {
                            v.EmailId1 = v.EmailId2;
                            v.EmailId2 = v.EmailId3;
                            v.EmailId3 = null;
                            Context.SaveChanges();
                        }

                    }
                    else if (Data.Index == 2)
                    {
                        foreach (var v in i)
                        {
                            v.EmailId2 = v.EmailId3;
                            v.EmailId3 = null;
                            Context.SaveChanges();
                        }
                    }
                    else if (Data.Index == 3)
                    {
                        foreach (var v in i)
                        {
                            v.EmailId3 = null;
                            Context.SaveChanges();
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                else if (Data.Index != 0)   // For Edit Email
                {
                    var i = (from s in Context.tblStore_Info
                             where s.Store_Id == Data.StoreID
                             select s).ToList();


                    if (Data.Index == 1)
                    {
                        foreach (var v in i)
                        {
                            v.EmailId1 = Data.EmailId1;
                            Context.SaveChanges();
                        }
                    }
                    else if (Data.Index == 2)
                    {
                        foreach (var v in i)
                        {
                            v.EmailId2 = Data.EmailId1;
                            Context.SaveChanges();
                        }
                    }
                    else if (Data.Index == 3)
                    {
                        foreach (var v in i)
                        {
                            v.EmailId3 = Data.EmailId1;
                            Context.SaveChanges();
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else  // For Add Email
                {
                    var i = (from s in Context.tblStore_Info
                             where s.Store_Id == Data.StoreID
                             select s).ToList();
                    if (flag == 0)
                    {
                        foreach (var v in i)
                        {
                            if (v.EmailId1 == null)
                            {
                                v.EmailId1 = Data.EmailId1;
                                Context.SaveChanges();
                                flag = 1;
                            }

                        }
                    }

                    if (flag == 0)
                    {
                        foreach (var v in i)
                        {
                            if (v.EmailId2 == null)
                            {
                                v.EmailId2 = Data.EmailId1;
                                Context.SaveChanges();
                                flag = 1;
                            }
                        }
                    }

                    if (flag == 0)
                    {
                        foreach (var v in i)
                        {
                            if (v.EmailId3 == null)
                            {
                                v.EmailId3 = Data.EmailId1;
                                Context.SaveChanges();
                            }
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

            }
        }

        [Route("api/Login/EmailID_ON_Off")]
        public HttpResponseMessage EmailID_ON_Off([FromBody] Store_Info Data)
        {
            Context = new LotteryBlankDatabaseEntities();
            int flag = 0;
            if (Data.Index != 0 && Data.StoreName != null) 
            {
                var i = (from s in Context.tblStore_Info
                         where s.Store_Id == Data.StoreID
                         select s).ToList();
                if (Data.Index == 1)
                {
                    foreach (var v in i)
                    {
                        v.Email1_On_Off = Data.IsEmail_On_Off;
                        Context.SaveChanges();
                    }

                }
                else if (Data.Index == 2)
                {
                    foreach (var v in i)
                    {
                        v.Email2_On_Off = Data.IsEmail_On_Off;
                        Context.SaveChanges();
                    }
                }
                else if (Data.Index == 3)
                {
                    foreach (var v in i)
                    {
                        v.Email3_On_Off = Data.IsEmail_On_Off;
                        Context.SaveChanges();
                    }
                }
            }
           return Request.CreateResponse(HttpStatusCode.OK);
           
        }
        [Route("api/Login/GetEmployeeDetails")]
        public IEnumerable<Login> GetHistory()
        {
            EmployeeDetails = new ObservableCollection<Login>();
            Context = new LotteryBlankDatabaseEntities();
            var data = Context.tblEmployee_Details.ToList();
            foreach (var v in data)
            {
                EmployeeDetails.Add(new Login
                {
                    Name = v.EmployeeName,
                    Dob = Convert.ToDateTime(v.BirthDate),
                    Address = v.Address,
                    Contactno = v.Contact,
                    Username = v.Username,
                    Password = v.Password,
                    Employeeid = Convert.ToInt32(v.EmployeeId),
                    StoreId = v.StoreId,
                    EmailId = v.EmailId,
                    EmailId1 = v.EmailId1,
                    EmailId2 = v.EmailId2,
                   IsRememberMe=v.IsRememberMe
                });
            }
            return EmployeeDetails;
        }

        [Route("api/Login/GetShiftDetails")]
        public IEnumerable<Shift> GetShiftDetails()
        {
            ShiftDetails = new ObservableCollection<Shift>();
            Context = new LotteryBlankDatabaseEntities();
            var data = Context.tblShifts.ToList();
            foreach (var v in data)
            {
                ShiftDetails.Add(new Shift
                {
                    ShiftId = v.ShiftID,
                    EndTime = v.EndTime,
                    Date = Convert.ToDateTime(v.Date),
                    EmployeeId = Convert.ToInt32(v.EmployeeId),
                    IsClose = Convert.ToBoolean(v.IsClose),
                    IsReportGenerated = v.IsReportGenerated
                });
            }
            return ShiftDetails;
        }


        [Route("api/Login/RemoveShift")]
        public HttpResponseMessage RemoveShift([FromBody] Login data)
        {

            Context = new LotteryBlankDatabaseEntities();
            if (data.Password != null)
            {
                var i = (from s in Context.tblEmployee_Details where s.Store_Address == data.StoreAddress && s.EmployeeId == data.Employeeid select s).ToList().FirstOrDefault();

                if (i.Password == data.Password)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                var shift = (from s in Context.tblShifts where s.StoreId == data.StoreId select s).ToList().LastOrDefault();
                var get = (from s in Context.tblShifts where s.StoreId == data.StoreId && s.Date == shift.Date select s).ToList();
                if (get.Count > 1)
                {

                    if (shift.IsReportGenerated == true)   // If last shift is generated
                    {
                        var j = (from s in Context.tblShifts where s.StoreId == data.StoreId select s).ToList().AsEnumerable().Reverse().Skip(1).FirstOrDefault();


                        // **************        remove record for current shift

                        var g = (from s in Context.tblClose_Box
                                 where s.Store_Id == data.StoreId && s.Created_On == shift.Date &&
  s.ShiftID == shift.ShiftID
                                 select s).ToList();
                        foreach (var i in g)                    // Remove records from close box
                        {
                            var h = (from s in Context.tblActivated_Tickets
                                     where s.Store_Id == data.StoreId && s.Created_On == shift.Date &&
                                     s.ShiftID == i.ShiftID && s.Status == "Close" && s.Game_Id == i.Game_Id
                                     && s.Packet_Id == i.Packet_Id
                                     select s).ToList().FirstOrDefault();

                            Context.tblActivated_Tickets.Remove(h);
                            Context.SaveChanges();

                            var v = (from s in Context.Box_Master
                                     where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                     select s).ToList().FirstOrDefault();
                            v.Status = "Empty";
                            Context.SaveChanges();

                            Context.tblClose_Box.Remove(i);
                            Context.SaveChanges();

                        }


                        // remove activate records of current shift which is not soldout / return /Close

                        var activate = (from s in Context.tblActivated_Tickets
                                        where s.Store_Id == data.StoreId
 && s.Created_On == shift.Date && s.ShiftID == shift.ShiftID &&
 s.Status == "Active"
                                        select s).ToList();
                        foreach (var i in activate)
                        {
                            Context.tblActivated_Tickets.Remove(i);
                            Context.SaveChanges();

                            var v = (from s in Context.Box_Master
                                     where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                     select s).ToList().FirstOrDefault();
                            v.Status = "Empty";
                            Context.SaveChanges();
                        }



                        // Remove soldout records from activate table

                        var ActSold = (from s in Context.tblActivated_Tickets
                                       where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                       && s.ShiftID == shift.ShiftID && s.Status == "Soldout"
                                       select s).ToList();

                        foreach (var i in ActSold)
                        {
                            Context.tblActivated_Tickets.Remove(i);
                            Context.SaveChanges();
                        }

                        // Remove soldout records from soldout table

                        var Sold = (from s in Context.tblSoldouts
                                    where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                    && s.ShiftID == shift.ShiftID && s.Status == "Soldout"
                                    select s).ToList();

                        foreach (var i in Sold)
                        {
                            Context.tblSoldouts.Remove(i);
                            Context.SaveChanges();

                            var v = (from s in Context.Box_Master
                                     where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                     select s).ToList().FirstOrDefault();
                            v.Status = "Empty";
                            Context.SaveChanges();
                        }

                        // Remove Return records from Activate table

                        var ActRet = (from s in Context.tblActivated_Tickets
                                      where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                      && s.ShiftID == shift.ShiftID && s.Status == "Return"
                                      select s).ToList();

                        foreach (var i in ActRet)
                        {
                            Context.tblActivated_Tickets.Remove(i);
                            Context.SaveChanges();
                        }


                        // Remove Return records from Return table

                        var Ret = (from s in Context.tblReturntickets
                                   where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                   && s.ShiftID == shift.ShiftID && s.Status == "Return"
                                   select s).ToList();

                        foreach (var i in Ret)
                        {
                            Context.tblReturntickets.Remove(i);
                            Context.SaveChanges();

                            var v = (from s in Context.Box_Master
                                     where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                     select s).ToList().FirstOrDefault();
                            v.Status = "Empty";
                            Context.SaveChanges();
                        }


                        // Remove settle records from settle table

                        var settle = (from s in Context.tblSettleTickets
                                      where s.Store_Id == data.StoreId && s.Created_On == shift.Date
 && s.ShiftID == shift.ShiftID && s.Status == "Settle"
                                      select s).ToList();

                        foreach (var i in settle)
                        {
                            Context.tblSettleTickets.Remove(i);
                            Context.SaveChanges();
                        }


                        // Remove Receive records from Receive table

                        var Receive = (from s in Context.tblRecievedTickets
                                       where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                       && s.ShiftID == shift.ShiftID
                                       select s).ToList();

                        foreach (var i in Receive)
                        {
                            Context.tblRecievedTickets.Remove(i);
                            Context.SaveChanges();
                        }


                        // Remove Deactivate records from Deactivate table

                        var Deact = (from s in Context.tblDeactivateds
                                     where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                     && s.ShiftID == shift.ShiftID
                                     select s).ToList();

                        foreach (var i in Deact)
                        {
                            Context.tblDeactivateds.Remove(i);
                            Context.SaveChanges();

                            var v = (from s in Context.Box_Master
                                     where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                     select s).ToList().FirstOrDefault();
                            v.Status = "Empty";
                            Context.SaveChanges();
                        }


                        //************ activate records of previous shift which is closed

                        var lastShiftClose = (from s in Context.tblClose_Box
                                              where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                              && s.ShiftID == j.ShiftID
                                              select s).ToList();

                        foreach (var i in lastShiftClose)
                        {
                            var v = Context.tblActivated_Tickets.Create();
                            v.Game_Id = i.Game_Id;
                            v.Packet_Id = i.Packet_Id;
                            v.Status = "Active";
                            v.ShiftID = shift.ShiftID;
                            v.Box_No = i.Box_No;
                            v.Ticket_Name = i.Ticket_Name;
                            v.Price = i.Price;
                            v.State = i.State;
                            v.Created_On = Convert.ToDateTime(shift.Date);
                            v.Start_No = (Convert.ToInt32(i.Close_At) + 1).ToString();
                            v.End_No = i.End_No;
                            v.Count = (Convert.ToInt32(i.End_No) - Convert.ToInt32(i.Start_No) + 1).ToString();
                            v.Total_Price = Convert.ToInt32(v.Count) * v.Price;
                            v.EmployeeId = shift.EmployeeId;
                            v.Store_Id = Convert.ToInt32(data.StoreId);
                            Context.tblActivated_Tickets.Add(v);

                            var m = (from s in Context.Box_Master
                                     where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                     select s).ToList().FirstOrDefault();
                            m.Status = "Active";
                            Context.SaveChanges();

                            //Context.tblClose_Box.Remove(i);
                            //Context.SaveChanges();
                        }


                        if (shift != null)
                        {
                            shift.IsClose = null;
                            shift.IsLastShift = null;
                            shift.CloseDate = null;
                            shift.IsReportGenerated = null;
                            shift.EndTime = null;
                            // Context.tblShifts.Remove(shift);
                            Context.SaveChanges();
                        }


                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        var j = (from s in Context.tblShifts where s.StoreId == data.StoreId select s).ToList().AsEnumerable().Reverse().Skip(1).FirstOrDefault();

                        var terminal = (from s in Context.tblTerminal_Data1
                                        where s.Store_Id == data.StoreId && s.Date == shift.Date
    && s.ShiftID == shift.ShiftID
                                        select s).ToList().FirstOrDefault();

                        if (terminal != null)      // If only shift report is generated
                        {
                            // Remove record of current shift

                            var k = (from s in Context.tblClose_Box
                                     where s.Store_Id == data.StoreId && s.Created_On == shift.Date &&
                                     s.ShiftID == shift.ShiftID
                                     select s).ToList();
                            foreach (var i in k)
                            {
                                var h = (from s in Context.tblActivated_Tickets
                                         where s.Store_Id == data.StoreId && s.Created_On == shift.Date &&
                                         s.ShiftID == i.ShiftID && s.Status == "Close" && s.Game_Id == i.Game_Id
                                         && s.Packet_Id == i.Packet_Id
                                         select s).ToList().FirstOrDefault();

                                Context.tblActivated_Tickets.Remove(h);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();

                                Context.tblClose_Box.Remove(i);
                                Context.SaveChanges();

                            }


                            // Remove activate records of current shift which is not soldout/ return/ close

                            var activate = (from s in Context.tblActivated_Tickets
                                            where s.Store_Id == data.StoreId &&
     s.Created_On == shift.Date && s.ShiftID == shift.ShiftID && s.Status == "Active"
                                            select s).ToList();

                            foreach (var i in activate)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }





                            // Remove soldout records from activate table

                            var ActSold = (from s in Context.tblActivated_Tickets
                                           where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                           && s.ShiftID == shift.ShiftID && s.Status == "Soldout"
                                           select s).ToList();

                            foreach (var i in ActSold)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove soldout records from soldout table

                            var Sold = (from s in Context.tblSoldouts
                                        where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                        && s.ShiftID == shift.ShiftID && s.Status == "Soldout"
                                        select s).ToList();

                            foreach (var i in Sold)
                            {
                                Context.tblSoldouts.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }


                            // Remove Return records from Activate table

                            var ActRet = (from s in Context.tblActivated_Tickets
                                          where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                          && s.ShiftID == shift.ShiftID && s.Status == "Return"
                                          select s).ToList();

                            foreach (var i in ActRet)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Return records from Return table

                            var Ret = (from s in Context.tblReturntickets
                                       where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                       && s.ShiftID == shift.ShiftID && s.Status == "Return"
                                       select s).ToList();

                            foreach (var i in Ret)
                            {
                                Context.tblReturntickets.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }


                            // Remove settle records from settle table

                            var settle = (from s in Context.tblSettleTickets
                                          where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                          && s.ShiftID == shift.ShiftID && s.Status == "Settle"
                                          select s).ToList();

                            foreach (var i in settle)
                            {
                                Context.tblSettleTickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Receive records from Receive table

                            var Receive = (from s in Context.tblRecievedTickets
                                           where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                           && s.ShiftID == shift.ShiftID
                                           select s).ToList();

                            foreach (var i in Receive)
                            {
                                Context.tblRecievedTickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Deactivate records from Deactivate table

                            var Deact = (from s in Context.tblDeactivateds
                                         where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                         && s.ShiftID == shift.ShiftID
                                         select s).ToList();

                            foreach (var i in Deact)
                            {
                                Context.tblDeactivateds.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }

                            // Activate records of previous shift which is closed

                            var preshift = (from s in Context.tblClose_Box
                                            where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                            && s.ShiftID == j.ShiftID
                                            select s).ToList();

                            foreach (var i in preshift)
                            {
                                var v = Context.tblActivated_Tickets.Create();
                                v.Game_Id = i.Game_Id;
                                v.Packet_Id = i.Packet_Id;
                                v.Status = "Active";
                                v.ShiftID = shift.ShiftID;
                                v.Box_No = i.Box_No;
                                v.Ticket_Name = i.Ticket_Name;
                                v.Price = i.Price;
                                v.State = i.State;
                                v.Created_On = Convert.ToDateTime(shift.Date);
                                v.Start_No = (Convert.ToInt32(i.Close_At) + 1).ToString();
                                v.End_No = i.End_No;
                                v.Count = (Convert.ToInt32(i.End_No) - Convert.ToInt32(i.Start_No) + 1).ToString();
                                v.Total_Price = Convert.ToInt32(v.Count) * v.Price;
                                v.EmployeeId = shift.EmployeeId;
                                v.Store_Id = Convert.ToInt32(data.StoreId);
                                Context.tblActivated_Tickets.Add(v);

                                var m = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                m.Status = "Active";
                                Context.SaveChanges();

                                //Context.tblClose_Box.Remove(i);
                                //Context.SaveChanges();
                            }

                            //Remove record from terminal table

                            var DelTerminal = (from s in Context.tblTerminal_Data1
                                               where s.Store_Id == data.StoreId &&
        s.Date == shift.Date && s.ShiftID == shift.ShiftID
                                               select s).ToList().FirstOrDefault();

                            Context.tblTerminal_Data1.Remove(DelTerminal);
                            Context.SaveChanges();

                            //update shift table


                            if (shift != null)
                            {
                                shift.IsClose = null;
                                shift.IsLastShift = null;
                                shift.CloseDate = null;
                                shift.IsReportGenerated = null;
                                shift.EndTime = null;
                                // Context.tblShifts.Remove(shift);
                                Context.SaveChanges();
                            }


                            return Request.CreateResponse(HttpStatusCode.OK);

                        }

                        else
                        {

                            // Remove soldout records from activate table


                            var ActSold = (from s in Context.tblActivated_Tickets
                                           where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                           && s.ShiftID == j.ShiftID && s.Status == "Soldout"
                                           select s).ToList();

                            foreach (var i in ActSold)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove soldout records from soldout table

                            var Sold = (from s in Context.tblSoldouts
                                        where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                        && s.ShiftID == j.ShiftID && s.Status == "Soldout"
                                        select s).ToList();

                            foreach (var i in Sold)
                            {
                                Context.tblSoldouts.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();

                            }


                            // Remove Return records from Activate table

                            var ActRet = (from s in Context.tblActivated_Tickets
                                          where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                          && s.ShiftID == j.ShiftID && s.Status == "Return"
                                          select s).ToList();

                            foreach (var i in ActRet)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();
                            }



                            // Remove Return records from Return table

                            var Ret = (from s in Context.tblReturntickets
                                       where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                       && s.ShiftID == j.ShiftID && s.Status == "Return"
                                       select s).ToList();

                            foreach (var i in Ret)
                            {
                                Context.tblReturntickets.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }


                            // Remove settle records from settle table

                            var settle = (from s in Context.tblSettleTickets
                                          where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                          && s.ShiftID == j.ShiftID && s.Status == "Settle"
                                          select s).ToList();

                            foreach (var i in settle)
                            {
                                Context.tblSettleTickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Receive records from Receive table

                            var Receive = (from s in Context.tblRecievedTickets
                                           where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                           && s.ShiftID == j.ShiftID
                                           select s).ToList();

                            foreach (var i in Receive)
                            {
                                Context.tblRecievedTickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Deactivate records from Deactivate table

                            var Deact = (from s in Context.tblDeactivateds
                                         where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                         && s.ShiftID == j.ShiftID
                                         select s).ToList();

                            foreach (var i in Deact)
                            {
                                Context.tblDeactivateds.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }

                            var o = (from s in Context.tblShifts where s.StoreId == data.StoreId select s).ToList().AsEnumerable().Reverse().Skip(2).FirstOrDefault();

                            // Remove records from close box
                            var g = (from s in Context.tblClose_Box
                                     where s.Store_Id == data.StoreId && s.Created_On == j.Date &&
                                    s.ShiftID == j.ShiftID
                                     select s).OrderBy(x => x.Box_No).ToList();
                            foreach (var i in g)
                            {
                                var h = (from s in Context.tblActivated_Tickets
                                         where s.Store_Id == data.StoreId && s.Created_On == j.Date && s.ShiftID == shift.ShiftID
                                         && s.Status == "Active" && s.Game_Id == i.Game_Id
                                         && s.Packet_Id == i.Packet_Id
                                         select s).ToList().FirstOrDefault();


                                if (h != null && o == null)
                                {
                                    Context.tblActivated_Tickets.Remove(h);
                                    Context.SaveChanges();

                                    var c = (from s in Context.tblClose_Box
                                             where s.Store_Id == data.StoreId && s.Created_On == h.Created_On && s.ShiftID == j.ShiftID && s.Packet_Id == h.Packet_Id
                                             select s).ToList().LastOrDefault();
                                    if (c != null)
                                    {

                                        Context.tblClose_Box.Remove(c);
                                        Context.SaveChanges();
                                    }

                                    var v = (from s in Context.Box_Master
                                             where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                             select s).ToList().FirstOrDefault();
                                    v.Status = "Empty";
                                    Context.SaveChanges();
                                }
                                else if (h != null && Convert.ToInt32(i.Start_No) != 000)
                                {
                                    h.Status = "Active";
                                    h.ShiftID = shift.ShiftID;
                                    h.EmployeeId = shift.EmployeeId;
                                    h.Start_No = i.Start_No;
                                    h.ShiftID = j.ShiftID;
                                    Context.SaveChanges();

                                    var v = (from s in Context.Box_Master
                                             where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                             select s).ToList().FirstOrDefault();
                                    v.Status = "Active";
                                    Context.SaveChanges();

                                    var c = (from s in Context.tblClose_Box
                                             where s.Store_Id == data.StoreId && s.Created_On == i.Created_On && s.ShiftID == j.ShiftID && s.Packet_Id == i.Packet_Id
                                             select s).ToList().LastOrDefault();
                                    Context.tblClose_Box.Remove(c);
                                    Context.SaveChanges();


                                }

                                if (o != null)
                                {
                                    var p = (from s in Context.tblClose_Box
                                             where s.Store_Id == data.StoreId && s.Created_On == o.Date && s.ShiftID == o.ShiftID
                                            && s.Game_Id == i.Game_Id
                                             && s.Packet_Id == i.Packet_Id
                                             select s).ToList().FirstOrDefault();


                                    if (p == null)
                                    {
                                        Context.tblActivated_Tickets.Remove(h);
                                        Context.SaveChanges();

                                        var c = (from s in Context.tblClose_Box
                                                 where s.Store_Id == data.StoreId && s.Created_On == h.Created_On && s.ShiftID == j.ShiftID && s.Packet_Id == h.Packet_Id
                                                 select s).ToList().LastOrDefault();
                                        Context.tblClose_Box.Remove(c);
                                        Context.SaveChanges();

                                        var v = (from s in Context.Box_Master
                                                 where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                                 select s).ToList().FirstOrDefault();
                                        v.Status = "Empty";
                                        Context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (Convert.ToInt32(h.Start_No) == 000)
                                    {
                                        Context.tblActivated_Tickets.Remove(h);
                                        Context.SaveChanges();

                                        var c = (from s in Context.tblClose_Box
                                                 where s.Store_Id == data.StoreId && s.Created_On == h.Created_On && s.ShiftID == j.ShiftID && s.Packet_Id == h.Packet_Id
                                                 select s).ToList().LastOrDefault();
                                        if (c != null)
                                        {

                                            Context.tblClose_Box.Remove(c);
                                            Context.SaveChanges();
                                        }

                                        var v = (from s in Context.Box_Master
                                                 where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                                 select s).ToList().FirstOrDefault();
                                        v.Status = "Empty";
                                        Context.SaveChanges();
                                    }
                                }
                                // comment

                                //if (get.Count > j.ShiftID)
                                //{
                                //    shift.ShiftID = j.ShiftID;
                                //    Context.tblShifts.Attach(j);
                                //    Context.tblShifts.Remove(j);
                                //    Context.SaveChanges();
                                //}


                                //Context.tblClose_Box.Remove(i);
                                //Context.SaveChanges();

                            }


                            // remove record from terminal table

                            var terminaldel = (from s in Context.tblTerminal_Data1
                                               where s.Store_Id == data.StoreId &&
           s.Date == j.Date && s.ShiftID == j.ShiftID
                                               select s).ToList().FirstOrDefault();

                            Context.tblTerminal_Data1.Remove(terminaldel);
                            Context.SaveChanges();
                            shift.ShiftID = j.ShiftID;
                            Context.tblShifts.Remove(j);
                            Context.SaveChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }

                    }

                }
                else
                {
                    if (get.Count == 1)
                    {
                        var j = (from s in Context.tblShifts where s.StoreId == data.StoreId select s).ToList().AsEnumerable().Reverse().Skip(1).FirstOrDefault();

                        var terminal = (from s in Context.tblTerminal_Data1
                                        where s.Store_Id == data.StoreId && s.Date == shift.Date
                                        && s.ShiftID == shift.ShiftID
                                        select s).ToList().FirstOrDefault();
                        if (terminal != null)
                        {
                            var k = (from s in Context.tblClose_Box
                                     where s.Store_Id == data.StoreId && s.Created_On == shift.Date &&
                                     s.ShiftID == shift.ShiftID
                                     select s).ToList();
                            foreach (var i in k)
                            {
                                var h = (from s in Context.tblActivated_Tickets
                                         where s.Store_Id == data.StoreId && s.Created_On == shift.Date &&
                                         s.ShiftID == i.ShiftID && s.Status == "Close" && s.Game_Id == i.Game_Id
                                         && s.Packet_Id == i.Packet_Id
                                         select s).ToList().FirstOrDefault();

                                Context.tblActivated_Tickets.Remove(h);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();

                                Context.tblClose_Box.Remove(i);
                                Context.SaveChanges();

                            }


                            // Remove activate records of current shift which is not soldout/ return/ close

                            var activate = (from s in Context.tblActivated_Tickets
                                            where s.Store_Id == data.StoreId &&
                                            s.Created_On == shift.Date && s.ShiftID == shift.ShiftID && s.Status == "Active"
                                            select s).ToList();

                            foreach (var i in activate)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }





                            // Remove soldout records from activate table

                            var ActSold = (from s in Context.tblActivated_Tickets
                                           where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                           && s.ShiftID == shift.ShiftID && s.Status == "Soldout"
                                           select s).ToList();

                            foreach (var i in ActSold)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove soldout records from soldout table

                            var Sold = (from s in Context.tblSoldouts
                                        where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                        && s.ShiftID == shift.ShiftID && s.Status == "Soldout"
                                        select s).ToList();

                            foreach (var i in Sold)
                            {
                                Context.tblSoldouts.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }


                            // Remove Return records from Activate table

                            var ActRet = (from s in Context.tblActivated_Tickets
                                          where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                          && s.ShiftID == shift.ShiftID && s.Status == "Return"
                                          select s).ToList();

                            foreach (var i in ActRet)
                            {
                                Context.tblActivated_Tickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Return records from Return table

                            var Ret = (from s in Context.tblReturntickets
                                       where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                       && s.ShiftID == shift.ShiftID && s.Status == "Return"
                                       select s).ToList();

                            foreach (var i in Ret)
                            {
                                Context.tblReturntickets.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }


                            // Remove settle records from settle table

                            var settle = (from s in Context.tblSettleTickets
                                          where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                          && s.ShiftID == shift.ShiftID && s.Status == "Settle"
                                          select s).ToList();

                            foreach (var i in settle)
                            {
                                Context.tblSettleTickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Receive records from Receive table

                            var Receive = (from s in Context.tblRecievedTickets
                                           where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                           && s.ShiftID == shift.ShiftID
                                           select s).ToList();

                            foreach (var i in Receive)
                            {
                                Context.tblRecievedTickets.Remove(i);
                                Context.SaveChanges();
                            }


                            // Remove Deactivate records from Deactivate table

                            var Deact = (from s in Context.tblDeactivateds
                                         where s.Store_Id == data.StoreId && s.Created_On == shift.Date
                                         && s.ShiftID == shift.ShiftID
                                         select s).ToList();

                            foreach (var i in Deact)
                            {
                                Context.tblDeactivateds.Remove(i);
                                Context.SaveChanges();

                                var v = (from s in Context.Box_Master
                                         where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                         select s).ToList().FirstOrDefault();
                                v.Status = "Empty";
                                Context.SaveChanges();
                            }

                            // Activate records of previous shift which is closed
                            if (j != null)
                            {
                                var preshift = (from s in Context.tblClose_Box
                                                where s.Store_Id == data.StoreId && s.Created_On == j.Date
                                                && s.ShiftID == j.ShiftID
                                                select s).ToList();

                                foreach (var i in preshift)
                                {
                                    var v = Context.tblActivated_Tickets.Create();
                                    v.Game_Id = i.Game_Id;
                                    v.Packet_Id = i.Packet_Id;
                                    v.Status = "Active";
                                    v.ShiftID = shift.ShiftID;
                                    v.Box_No = i.Box_No;
                                    v.Ticket_Name = i.Ticket_Name;
                                    v.Price = i.Price;
                                    v.State = i.State;
                                    v.Created_On = Convert.ToDateTime(shift.Date);
                                    v.Start_No = (Convert.ToInt32(i.Close_At) + 1).ToString();
                                    v.End_No = i.End_No;
                                    v.Count = (Convert.ToInt32(i.End_No) - Convert.ToInt32(i.Start_No) + 1).ToString();
                                    v.Total_Price = Convert.ToInt32(v.Count) * v.Price;
                                    v.EmployeeId = shift.EmployeeId;
                                    v.Store_Id = Convert.ToInt32(data.StoreId);
                                    Context.tblActivated_Tickets.Add(v);

                                    var m = (from s in Context.Box_Master
                                             where s.Store_Id == data.StoreId && s.Box_No == i.Box_No
                                             select s).ToList().FirstOrDefault();
                                    m.Status = "Active";
                                    Context.SaveChanges();

                                    //Context.tblClose_Box.Remove(i);
                                    //Context.SaveChanges();
                                }
                            }




                            //Remove record from terminal table

                            var DelTerminal = (from s in Context.tblTerminal_Data1
                                               where s.Store_Id == data.StoreId &&
                                               s.Date == shift.Date && s.ShiftID == shift.ShiftID
                                               select s).ToList().FirstOrDefault();

                            Context.tblTerminal_Data1.Remove(DelTerminal);
                            Context.SaveChanges();

                            if (shift != null)
                            {
                                shift.IsClose = null;
                                shift.IsLastShift = null;
                                shift.CloseDate = null;
                                shift.IsReportGenerated = null;
                                shift.EndTime = null;
                                Context.SaveChanges();
                            }


                            return Request.CreateResponse(HttpStatusCode.OK);

                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }


                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }

        }

        [Route("api/Login/ResetTracker")]
        public HttpResponseMessage ResetTracker([FromBody] Store_Info data)
        {
            Context = new LotteryBlankDatabaseEntities();

            var v = (from s in Context.Box_Master where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in v)
            {
                i.Status = "Empty";
                Context.SaveChanges();
            }

            var a = (from s in Context.tblActivated_Tickets where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in a)
            {
                Context.tblActivated_Tickets.Remove(i);
                Context.SaveChanges();
            }

            var sold = (from s in Context.tblSoldouts where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in sold)
            {
                Context.tblSoldouts.Remove(i);
                Context.SaveChanges();
            }

            var r = (from s in Context.tblReturntickets where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in r)
            {
                Context.tblReturntickets.Remove(i);
                Context.SaveChanges();
            }

            var d = (from s in Context.tblDeactivateds where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in d)
            {
                Context.tblDeactivateds.Remove(i);
                Context.SaveChanges();
            }

            var settle = (from s in Context.tblSettleTickets where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in settle)
            {
                Context.tblSettleTickets.Remove(i);
                Context.SaveChanges();
            }

            var c = (from s in Context.tblClose_Box where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in c)
            {
                Context.tblClose_Box.Remove(i);
                Context.SaveChanges();
            }

            var t = (from s in Context.tblTerminal_Data1 where s.Store_Id == data.StoreID select s).ToList();

            foreach (var i in t)
            {
                Context.tblTerminal_Data1.Remove(i);
                Context.SaveChanges();
            }


            if (data.StoreName != null || data.StoreStatus != null)
            {
                var re = (from s in Context.tblRecievedTickets where s.Store_Id == data.StoreID select s).ToList();

                foreach (var i in re)
                {
                    Context.tblRecievedTickets.Remove(i);
                    Context.SaveChanges();
                }
            }
            if(data.StoreStatus == "New Active")
            {
                foreach (var i in v)
                {
                    Context.Box_Master.Remove(i);
                    Context.SaveChanges();
                }
                var storeinfo = (from s in Context.tblStore_Info where s.Store_Id == data.StoreID select s).FirstOrDefault();
                storeinfo.Open_Time = null;
                storeinfo.Close_Time = null;
                storeinfo.No_Of_Boxes = null;
                storeinfo.EmailId1 = null;
                storeinfo.EmailId2 = null;
                storeinfo.EmailId3 = null;
                storeinfo.Email1_On_Off = null;
                storeinfo.Email2_On_Off = null;
                storeinfo.Email3_On_Off = null;
                storeinfo.Settlement_Day = null;
                storeinfo.Auto_Settle = null;
                storeinfo.Store_Status = "New Active";
                Context.SaveChanges();
            }

            var shiftrecord = (from s in Context.tblShifts where s.StoreId == data.StoreID select s).ToList();

            foreach (var i in shiftrecord)
            {
                Context.tblShifts.Remove(i);
                Context.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK);

        }

        [Route("api/Login/InsertLoginDetails")]
        public HttpResponseMessage InsertLoginDetails([FromBody] LoginDetails data)
        {
            using (Context = new LotteryBlankDatabaseEntities())
            {
                Context = new LotteryBlankDatabaseEntities();
                var v = Context.tblLogin_Details.Create();
                v.EmployeeId = data.EmployeeId;
                // v.ShiftId = data.ShiftId;
                v.Date = System.DateTime.Now;
                Context.tblLogin_Details.Add(v);
                Context.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[Route("api/Login/GetEmployeeRecords")]
        //public HttpResponseMessage GetEmployeeRecords([FromBody] Login data)
        //{
        //    EmployeeDetails = new ObservableCollection<Login>();
        //    Context = new LotteryBlankDatabaseEntities();
        //    var result = Context.tblEmployee_Details.ToList();
        //    if(result!=null)
        //    {
        //        var v = result.Where(x => x.Username == data.Username && x.Password == data.Password).ToList();
        //        foreach (var i in v)
        //        {
        //            EmployeeDetails.Add(new Login
        //            {
        //                Name = i.EmployeeName,
        //                Dob = i.BirthDate,
        //                Address = i.Address,
        //                Contactno = i.Contact,
        //                Username = i.Username,
        //                Password = i.Password,
        //                Employeeid = i.EmployeeId
        //            });
        //        }
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, EmployeeDetails);
        //}

        [Route("api/Login/NewGetEmpRecords")]
        public HttpResponseMessage NewGetEmpRecords([FromBody] Login data)
        {
            EmployeeDetails = new ObservableCollection<Login>();
            Context = new LotteryBlankDatabaseEntities();

            if (data.Username != null && data.Password != null)
            {
                var result = Context.tblEmployee_Details.ToList();
                if (result != null)
                {
                    var v = result.Where(x => x.Username == data.Username && x.Password == data.Password && x.Store_Address == data.StoreAddress).ToList();
                    if (v != null)
                    {
                        foreach (var i in v)
                        {
                            EmployeeDetails.Add(new Login
                            {
                                Name = i.EmployeeName,
                                Dob = Convert.ToDateTime(i.BirthDate),
                                Address = i.Address,
                                Contactno = i.Contact,
                                Username = i.Username,
                                Password = i.Password,
                                Employeeid = Convert.ToInt32(i.EmployeeId),
                                StoreId = i.StoreId,
                                StoreAddress = i.Store_Address,
                                IsManager = Convert.ToBoolean(i.IsManager),
                            });
                        }
                    }
                }

                if (EmployeeDetails.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, EmployeeDetails);
                }
            }
            else
            {
                //var result = Context.tblEmployee_Details.ToList();

                var v = (from s in Context.tblStoreEmployeeInfoes where s.StoreId == data.StoreId select s).ToList();
                var v1 = (Context.tblEmployee_Details).ToList();
                if (v != null)
                {
                    if (v1 != null)
                    {
                        foreach (var i in v)
                        {
                            var get = v1.Where(x => x.EmployeeId == i.EmployeeId).FirstOrDefault();
                            if(get!=null)
                            {
                                EmployeeDetails.Add(new Login
                                {
                                    Name = get.EmployeeName,
                                    Contactno = get.Contact,
                                    Employeeid = Convert.ToInt32(get.EmployeeId),
                                    EmailId = get.EmailId,
                                    Dob = Convert.ToDateTime(get.BirthDate),
                                    Address = get.Address,
                                    Username = get.Username,
                                    Password = get.Password,
                                    StoreId = get.StoreId,
                                    IsManager = Convert.ToBoolean(get.IsManager),
                                    IsEmployee = Convert.ToBoolean(get.IsEmployee),
                                    IsAssignStore = Convert.ToBoolean(get.IsAssignStore)
                                });
                            }
                        }
                           
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, EmployeeDetails);
                }

                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

            }

        }

    }
}
