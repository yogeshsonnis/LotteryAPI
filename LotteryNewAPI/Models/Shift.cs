using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Shift
    {
        public int? ShiftId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public  int StoreId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime CloseDate { get; set; }
        public Boolean? IsLastShift { get; set; }
       


        public Boolean IsClose { get; set; }
        public Boolean? IsReportGenerated { get; set; }

        public int IsCheck { get; set; }
    }
}