using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Close_Box
    {
        public int CloseBox_Id { get; set; }
        public string Game_Id { get; set; }
        public string Packet_Id { get; set; }
        public int? Box_No { get; set; }
        public string Status { get; set; }
        public string Ticket_Name { get; set; }
        public int Price { get; set; }
        public DateTime Created_Date { get; set; }
        public string Start_No { get; set; }
        public string End_No { get; set; }
        public string Close_At { get; set; }
        public string Count { get; set; }
        public int? EmployeeID { get; set; }
        public string State { get; set; }
        public int? Total_Price { get; set; }
        public int Store_Id { get; set; }
        public int ShiftID { get; set; }

    }
}