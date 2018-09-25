using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class SoldOut_TicketInfo
    {
        public string Game_Id { get; set; }
        public string Packet_No { get; set; }
        public int? Box_No { get; set; }
        public string Status { get; set; }
        public string Ticket_Name { get; set; }
        public int? Price { get; set; }
        public string Start_No { get; set; }
        public string End_No { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime Modified_Date { get; set; }
        public int? EmployeeID { get; set; }
        public int? Count { get; set; }
        public int? ShiftID { get; set; }
        public int? No_of_Tickets_Sold { get; set; }
        public string State { get; set; }
        public string Partial_Packet { get; set; }
        public int? Total_Price { get; set; }
        public int Store_Id { get; set; }     
        
        public string CloseTime { get; set; }
        public bool ShiftReportGenerate { get; set; }

    }
}