using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class BarCodeFormat
    {
        public string State { get; set; }
        public int? GameIDFrom { get; set; }
        public int? PacketIDFrom { get; set; }
        public int? SequenceNoFrom { get; set; }
        public int? GameIDTo { get; set; }
        public int? PacketIDTo { get; set; }
        public int? SequenceIDTo { get; set; }
        public int? BarCodeLength { get; set; }
    }
}