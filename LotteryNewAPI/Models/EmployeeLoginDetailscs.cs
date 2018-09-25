using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class EmployeeLoginDetailscs
    {
        public string username { get; set; }
        public  string state { get; set; }
        public int employeeID { get; set; }
        public int totalLengthofBarcode { get; set; }
        public int fromGameIDRange { get; set; }
        public int toGameIDRange { get; set; }
        public int fromPacketIDRange { get; set; }
        public int toPacketIDRange { get; set; }
        public int toSeqenceNo { get; set; }
        public int fromSeqenceNo { get; set; }
    }
}