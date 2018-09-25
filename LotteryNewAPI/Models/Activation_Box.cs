using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Activation_Box
    {
        public int? Box_No { get; set; }

        public int? Box_Id { get; set; }

        public string Box_Status { get; set; }
        public string State { get; set; }
        public int ShiftID { get; set; }

        public int Store_Id { get; set; }

        public int ChangeToBox { get; set; }
    }
}