using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace LotteryNewAPI.Models
{
    public class Activate_Ticket
    {
        public string Game_Id { get; set; }
        public string Packet_No { get; set; }
        public int? Box_No { get; set; }
        public string Status { get; set; }
        public string Ticket_Name { get; set; }
        public int? Price { get; set; }
        public DateTime Created_Date { get; set; }

        public DateTime Activation_Date { get; set; }
        public string First_Ticket { get; set; }
        public string Start_No { get; set; }
        public string End_No { get; set; }
        public string Stopped_At { get; set; }
        public string Count { get; set; }
        public int? EmployeeID { get; set; }
        public string State { get; set; }
        public int? Total_Price { get; set; }
        public int Store_Id { get; set; }
        public int? ShiftID { get; set; }
        public string CloseTime { get; set; } 
        public int ChangeToBox { get; set; }
        
        public int SettlementDays { get; set; }

        // public SolidColorBrush BackColor { get; set; }



    }
}