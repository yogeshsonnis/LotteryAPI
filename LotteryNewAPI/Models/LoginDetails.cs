using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class LoginDetails
    {
        public int EmployeeId { get; set; }

        public int ShiftId { get; set; }

        public DateTime? Date { get; set; }

        public string State { get; set; }



    }
}