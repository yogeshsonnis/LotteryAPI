using LotteryNewAPI;
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
    public class LotteryController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        public ObservableCollection<Activation_Box> BoxCollection { get; set; }
        public ObservableCollection<LotteryInfo> LotteryCollection { get; set; }

        [Route("api/Lottery/NewGetBoxCollection")]
        public HttpResponseMessage NewGetBoxCollection([FromBody] LotteryInfo data)
        {
            BoxCollection = new ObservableCollection<Activation_Box>();
            context = new LotteryBlankDatabaseEntities();
            var v = (from s in context.Box_Master
                     where s.Store_Id == data.Store_Id && (s.Status == "Empty" || s.Status == "SoldOut" || s.Status == "Deactivated")
                     select s).ToList();
            foreach (var i in v)
            {
                BoxCollection.Add(new Activation_Box
                {
                    Box_No = Convert.ToInt32(i.Box_No)
                });
            }
            //  return BoxCollection;
            return Request.CreateResponse(HttpStatusCode.OK, BoxCollection);
        }

        //[Route("api/Lottery/GetLotteryCollection")]
        //public IEnumerable<LotteryInfo> GetLotteryCollection()
        //{
        //    LotteryCollection = new ObservableCollection<LotteryInfo>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var boxlist = context.Box_Master.ToList();
        //    var activatedticketslist = context.tblActivated_Tickets.ToList();
        //    var result = from bl in boxlist
        //                 join activel in activatedticketslist
        //                 on bl.Box_No equals activel.Box_No into a 
        //                 from b in a.DefaultIfEmpty(new tblActivated_Tickets() )
        //                 select new
        //                 {
        //                     bl.Box_No,
        //                     b.Game_Id,
        //                     b.Price,
        //                     b.Packet_Id,
        //                     bl.Status,
        //                     b.Ticket_Name,
        //                     b.Start_No,
        //                     b.End_No,
        //                     b.Count,
        //                     b.Total_Price
        //                 };

        //    foreach (var i in result)
        //    {
        //        LotteryCollection.Add(new LotteryInfo
        //        {
        //            Price = Convert.ToInt32(i.Price),
        //            Box_No = Convert.ToInt32(i.Box_No),
        //            Packet_No = i.Packet_Id,
        //            Game_Id = i.Game_Id,
        //            Status = i.Status,
        //            Ticket_Name = i.Ticket_Name,
        //            End_No=i.End_No,
        //            Start_No=i.Start_No,
        //            Count=i.Count,
        //            Total_Price=i.Total_Price,
        //        });
        //    }

        //    return LotteryCollection;
        //}

        [Route("api/Lottery/NewGetLotteryCollection")]
        public HttpResponseMessage NewGetLotteryCollection([FromBody] LotteryInfo data)
        {
            LotteryCollection = new ObservableCollection<LotteryInfo>();
            context = new LotteryBlankDatabaseEntities();
            var boxlist = (from s in context.Box_Master where s.Store_Id == data.Store_Id select s).ToList();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
            if (Shift != null)
            {

                var activatedticketslist = (from s in context.tblActivated_Tickets where s.Store_Id == data.Store_Id && s.ShiftID == Shift.ShiftID && (s.Status == "Active" || s.Status == "Close") select s).ToList();
                var settleticketslist = (from s in context.tblSettleTickets where s.Store_Id == data.Store_Id && s.ShiftID == Shift.ShiftID select s).ToList();

                var result = from bl in boxlist
                             join activel in activatedticketslist
                             on bl.Box_No equals activel.Box_No into a
                             from b in a.DefaultIfEmpty(new tblActivated_Tickets())
                             select new
                             {
                                 bl.Box_No,
                                 bl.Store_Id,
                                 b.Game_Id,
                                 b.Price,
                                 b.Packet_Id,
                                 bl.Status,
                                 b.Ticket_Name,
                                 b.Start_No,
                                 b.End_No,
                                 b.Count,
                                 b.Total_Price,
                                 b.Stopped_At,

                             };


                foreach (var i in result)
                {

                    if (settleticketslist != null)
                    {
                        var get = settleticketslist.Where(x => x.Box_No == i.Box_No).FirstOrDefault();
                        if (get != null)
                        {
                            LotteryCollection.Add(new LotteryInfo
                            {
                                Price = Convert.ToInt32(i.Price),
                                Box_No = Convert.ToInt32(i.Box_No),
                                Packet_No = i.Packet_Id,
                                Game_Id = i.Game_Id,
                                Settle_Status = get.Status,
                                Status = i.Status,
                                Ticket_Name = i.Ticket_Name,
                                End_No = i.End_No,
                                Start_No = i.Start_No,
                                Count = i.Count,
                                Total_Price = i.Total_Price,
                                Store_Id = i.Store_Id,
                                Stopped_At = i.Stopped_At
                            });
                        }
                        else
                        {
                            LotteryCollection.Add(new LotteryInfo
                            {
                                Price = Convert.ToInt32(i.Price),
                                Box_No = Convert.ToInt32(i.Box_No),
                                Packet_No = i.Packet_Id,
                                Game_Id = i.Game_Id,
                                Status = i.Status,
                                Ticket_Name = i.Ticket_Name,
                                End_No = i.End_No,
                                Start_No = i.Start_No,
                                Count = i.Count,
                                Total_Price = i.Total_Price,
                                Store_Id = i.Store_Id,
                                Stopped_At = i.Stopped_At
                            });
                        }
                    }
                    else
                    {
                        LotteryCollection.Add(new LotteryInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.End_No,
                            Start_No = i.Start_No,
                            Count = i.Count,
                            Total_Price = i.Total_Price,
                            Store_Id = i.Store_Id,
                            Stopped_At = i.Stopped_At
                        });
                    }
                    
                }

            }
            else
            {
                foreach (var i in boxlist)
                {
                    LotteryCollection.Add(new LotteryInfo
                    {
                        Box_No = Convert.ToInt32(i.Box_No),
                        Status = i.Status
                    });
                }
            }
           

            // return LotteryCollection;

            return Request.CreateResponse(HttpStatusCode.OK, LotteryCollection);
        }



    }
}
