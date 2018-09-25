using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class LotteryInfo
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int Box_No { get; set; }
        public string Packet_No { get; set; }
        public string Game_Id { get; set; }
        public string Status { get; set; }
        public string Ticket_Name { get; set; }
        public string Start_No { get; set; }
        public string End_No { get; set; }
        public string Count { get; set; }
        public string State { get; set; }
        public int? Store_Id { get; set; }
        public int? Employee_Id { get; set; }
        public int? Total_Price { get; set; }
        public string Stopped_At { get; set; }

        public string Settle_Status { get; set; }
        public int? Settle_Box_No { get; set; }

    }
}