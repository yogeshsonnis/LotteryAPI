using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Store_Info
    {
       public string StoreName { get; set; }
       public int StoreHours { get; set; }
       public string OpenTime { get; set; }
       public string CloseTime { get; set; }
       public int? NoOfBoxes { get; set; }
       public int? StoreID { get; set; }
       public int? EmployeeID { get; set; }
       public string StoreAddress { get; set; }

        public bool IsAssignStore { get; set; }

        public string EmailId1 { get; set; }
        public string EmailId2 { get; set; }
        public string EmailId3 { get; set; }
        public string Password { get; set; }
        public string StoreStatus { get; set; }
        public string NewPassword { get; set; }
        public int Index { get; set; }
        public int SettlementDays { get; set; }
        public bool AutoSettle { get; set; }
        public bool? Email1_On_Off { get; set; }
        public bool? Email2_On_Off { get; set; }
        public bool? Email3_On_Off { get; set; }
        public bool? IsEmail_On_Off { get; set; }

    }
}