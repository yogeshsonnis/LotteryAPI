using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Receive_Inventory
    {
        public string Game_Id { get; set; }
        public string Packet_No { get; set; }
        public string Quantity { get; set; }
        public int? Rate { get; set; }
        public string Total_Amount { get; set; }
        public DateTime Created_On { get; set; }
        public DateTime Modified_On { get; set; }
        public string Start_No { get; set; }
        public string End_No { get; set; }
        public string Expire_Date { get; set; }
        public string Status { get; set; }
        public string Ticket_Name { get; set; }
        public int EmployeeID { get; set; }
        public string State { get; set; }
        public int Store_Id { get; set; }
        public int ShiftID { get; set; }
    }
}