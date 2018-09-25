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
    public class BarCodeFormatController : ApiController
    {
        LotteryBlankDatabaseEntities context;

        public ObservableCollection<BarCodeFormat> AllBarcodeFormat { get; set; }

        [Route("api/BarCodeFormat/SaveBarCodeFormat")]
        public HttpResponseMessage SaveBarCodeFormat([FromBody] BarCodeFormat data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var p = (from s in context.tblBarcode_Format where s.State == data.State select s).FirstOrDefault();
                if (p != null)
                {
                    p.GameIDRange_From = data.GameIDFrom;
                    p.TotalLengthofBarcode = data.BarCodeLength;
                    p.GameIDRange_To = data.GameIDTo;
                    p.PacketIDRange_From = data.PacketIDFrom;
                    p.PacketIDRange_To = data.PacketIDTo;
                    p.SequenceNo_From = data.SequenceNoFrom;
                    p.SequenceNo_To = data.SequenceIDTo;
                    p.State = data.State;
                    context.SaveChanges();
                }
                else
                {
                    var v = context.tblBarcode_Format.Create();
                    v.State = data.State;
                    v.TotalLengthofBarcode = data.BarCodeLength;
                    v.GameIDRange_From = data.GameIDFrom;
                    v.GameIDRange_To = data.GameIDTo;
                    v.PacketIDRange_From = data.PacketIDFrom;
                    v.PacketIDRange_To = data.PacketIDTo;
                    v.SequenceNo_From = data.SequenceNoFrom;
                    v.SequenceNo_To = data.SequenceIDTo;
                    context.tblBarcode_Format.Add(v);
                    context.SaveChanges();
                }

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[Route("api/BarCodeFormat/GetBarCodeFormat")]
        //public HttpResponseMessage BarCodeFormat([FromBody] BarCodeFormat data)
        //{
        //    AllBarcodeFormat = new ObservableCollection<BarCodeFormat>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var result = context.tblBarcode_Format.ToList();
        //    if(result!=null)
        //    {
        //        var v = result.Where(x=>x.State == data)
        //    }
        //    foreach (var v in i)
        //    {
        //        AllBarcodeFormat.Add(new BarCodeFormat
        //        {
        //            State = v.State,
        //            GameIDFrom = v.GameIDRange_From,
        //            GameIDTo = v.GameIDRange_To,
        //            PacketIDFrom = v.PacketIDRange_From,
        //            PacketIDTo = v.PacketIDRange_To,
        //            SequenceIDTo = v.SequenceNo_To,
        //            SequenceNoFrom = v.SequenceNo_From,
        //            BarCodeLength = v.TotalLengthofBarcode,
        //        });
        //    }
        //    return AllBarcodeFormat;
        //}

        [Route("api/BarCodeFormat/NewGetBarCodeFormat")]
        public HttpResponseMessage NewGetBarCodeFormat([FromBody] BarCodeFormat data)
        {
            AllBarcodeFormat = new ObservableCollection<BarCodeFormat>();
            context = new LotteryBlankDatabaseEntities();
            var result = context.tblBarcode_Format.ToList();
            if(result!=null)
            {
                var v = result.Where(x => x.State == data.State).ToList();
                foreach(var i in v)
                {
                    AllBarcodeFormat.Add(new BarCodeFormat
                    {
                        State = i.State,
                        GameIDFrom = i.GameIDRange_From,
                        GameIDTo = i.GameIDRange_To,
                        PacketIDFrom = i.PacketIDRange_From,
                        PacketIDTo = i.PacketIDRange_To,
                        SequenceIDTo = i.SequenceNo_To,
                        SequenceNoFrom = i.SequenceNo_From,
                        BarCodeLength = i.TotalLengthofBarcode,
                    });
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, AllBarcodeFormat);
        }


    }
    }

