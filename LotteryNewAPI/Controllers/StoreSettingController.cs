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
    public class StoreSettingController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        ObservableCollection<Store_Info> StoreColl { get; set; }

        [Route("api/StoreSetting/Save_NewStore")]
        public HttpResponseMessage Save_NewStore([FromBody] Store_Info data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var v = context.tblStore_Info.Create();
                v.Store_Name = data.StoreName;
                v.Open_Time = data.OpenTime;
                v.Close_Time = data.CloseTime;
                v.No_Of_Boxes = data.NoOfBoxes;
                context.tblStore_Info.Add(v);
                context.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/StoreSetting/GetNewStoreHistory")]
        public IEnumerable<Store_Info> GetNewStoreHistory()
        {
            StoreColl = new ObservableCollection<Store_Info>();
            context = new LotteryBlankDatabaseEntities();
            var data = (from s in context.tblStore_Info  select s).ToList();
            foreach (var v in data)
            {
                StoreColl.Add(new Store_Info
                {

                    StoreName=v.Store_Name,
                    OpenTime=v.Open_Time,
                    CloseTime=v.Close_Time,
                    NoOfBoxes=v.No_Of_Boxes,
                    StoreID=v.Store_Id,
                    EmailId1 = v.EmailId1,
                    EmailId2 = v.EmailId2,
                    EmailId3 = v.EmailId3,
                    StoreStatus = v.Store_Status
                });
            }

            return StoreColl;
         }

        [Route("api/StoreSetting/NewGetStoreHistory")]
        public HttpResponseMessage NewGetStoreHistory([FromBody] Store_Info data)
        {
            StoreColl = new ObservableCollection<Store_Info>();
            context = new LotteryBlankDatabaseEntities();
            var v = (from s in context.tblStoreEmployeeInfoes where s.EmployeeId == data.EmployeeID && s.Employee_Address == data.StoreAddress  select s).ToList();
            foreach (var i in v)
            {
                var j = (from s in context.tblEmployee_Details where s.EmployeeId == data.EmployeeID select s).ToList().FirstOrDefault();
                var k= (from s in context.tblStore_Info where s.Store_Id == i.StoreId && s.Store_Address == i.Store_Address select s).ToList().FirstOrDefault();

                if (j != null)
                {
                    if (j.IsAssignStore == true)
                    {
                        if(k!=null)
                        {
                            StoreColl.Add(new Store_Info
                            {
                                StoreName = k.Store_Name,
                                EmployeeID = i.EmployeeId,
                                StoreID = k.Store_Id,
                                StoreAddress = k.Store_Address,
                                NoOfBoxes = k.No_Of_Boxes,
                                EmailId1 = k.EmailId1,
                                EmailId2 = k.EmailId2,
                                EmailId3 = k.EmailId3,
                                //Email_On_Off = k.Email_On_Off,
                                SettlementDays = Convert.ToInt32(k.Settlement_Day),
                                AutoSettle = Convert.ToBoolean(k.Auto_Settle),
                                OpenTime = k.Open_Time,
                                CloseTime = k.Close_Time,
                                StoreStatus = k.Store_Status,
                                Email1_On_Off = k.Email1_On_Off,
                                Email2_On_Off=k.Email2_On_Off,
                                Email3_On_Off=k.Email3_On_Off,
                            });
                        }
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, StoreColl);
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("api/StoreSetting/OnGeneralSetting")]
        public HttpResponseMessage OnGeneralSetting([FromBody] Store_Info data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                int i;
                int boxcount = Convert.ToInt32(data.NoOfBoxes);
                var v = (from s in context.tblStore_Info where s.Store_Id == data.StoreID select s).FirstOrDefault();
                v.Open_Time = data.OpenTime;
                v.Close_Time = data.CloseTime;
                v.Settlement_Day = data.SettlementDays;
                v.Auto_Settle = data.AutoSettle;
                if (v.Store_Status == "New Active")
                {
                    v.EmailId1 = data.EmailId1;
                    if(data.EmailId2 != "" || data.EmailId3!="")
                    {
                        if(data.EmailId2 == "")
                        {
                            v.EmailId2 = data.EmailId3;
                        }
                        else
                        {
                            v.EmailId2 = data.EmailId2;
                            v.EmailId3 = data.EmailId3;
                        }
                    }
                    if (v.No_Of_Boxes == null)
                    {
                        for (i = Convert.ToInt32(0 + 1); i <= Convert.ToInt32(data.NoOfBoxes); i++)
                        {
                            var v1 = context.Box_Master.Create();
                            v1.Box_No = i;
                            v1.Status = "Empty";
                            v1.Store_Id = data.StoreID;
                            context.Box_Master.Add(v1);
                        }
                        v.Store_Status = "Active";
                        v.No_Of_Boxes = data.NoOfBoxes;
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (v.No_Of_Boxes == data.NoOfBoxes)
                    {
                        v.No_Of_Boxes = data.NoOfBoxes;
                        context.SaveChanges();
                    }
                    else if (v.No_Of_Boxes < data.NoOfBoxes)
                    {
                        for (i = Convert.ToInt32(v.No_Of_Boxes + 1); i <= Convert.ToInt32(data.NoOfBoxes); i++)
                        {
                            var v1 = context.Box_Master.Create();
                            v1.Box_No = i;
                            v1.Status = "Empty";
                            v1.Store_Id = data.StoreID;
                            context.Box_Master.Add(v1);
                        }
                        v.No_Of_Boxes = data.NoOfBoxes;
                        context.SaveChanges();
                    }
                    else
                    {
                        var v2 = (from s in context.Box_Master where s.Store_Id == data.StoreID && s.Status == "Active" || s.Status == "Soldout" || s.Status == "Close" select s).ToList().LastOrDefault();
                        if (v2 != null)
                        {
                            if (v2.Box_No > data.NoOfBoxes)
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound);
                            }
                            else
                            {
                                for (i = Convert.ToInt32(v.No_Of_Boxes); i > Convert.ToInt32(data.NoOfBoxes); i--)
                                {
                                    var v1 = (from s in context.Box_Master where s.Box_No == i && s.Store_Id == data.StoreID select s).FirstOrDefault();
                                    //v1.Box_No = i;
                                    //v1.Status = "Empty";
                                    //v1.Store_Id = data.StoreID;
                                    context.Box_Master.Remove(v1);
                                    context.SaveChanges();
                                }
                                v.No_Of_Boxes = data.NoOfBoxes;
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            for (i = Convert.ToInt32(v.No_Of_Boxes); i > Convert.ToInt32(data.NoOfBoxes); i--)
                            {
                                var v1 = (from s in context.Box_Master where s.Box_No == i && s.Store_Id == data.StoreID select s).FirstOrDefault();
                                //v1.Box_No = i;
                                //v1.Status = "Empty";
                                //v1.Store_Id = data.StoreID;
                                context.Box_Master.Remove(v1);
                                context.SaveChanges();
                            }
                            v.No_Of_Boxes = data.NoOfBoxes;
                            context.SaveChanges();
                        }
                    }
                }
              
            }
            return Request.CreateResponse(HttpStatusCode.OK, StoreColl);
        }

        [Route("api/StoreSetting/OnEmailSetting")]
        public HttpResponseMessage OnEmailSetting([FromBody] Store_Info data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var v = (from s in context.tblStore_Info where s.Store_Id == data.StoreID select s).FirstOrDefault();
                if(data.Email1_On_Off != null)
                {
                    //v.Email_On_Off = data.Email_On_Off;
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }


    }
}


       
