using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Master_List_Inventory
    {
        public string Game_Id { get; set; }
     //   public string Packet_No { get; set; }
        public string Count { get; set; }
        public int? Rate { get; set; }
        public DateTime? Date { get; set; }
        public string Start_No { get; set; }
        public string End_No { get; set; }
        public string Ticket_Name { get; set; }
        public string State { get; set; }

        public int Store_Id { get; set; }
        public int Employee_Id { get; set; }
    }
}