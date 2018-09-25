using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Login
    {
        public string Name { get; set; }
        public string Contactno { get; set; }
        public string Address { get; set; }
        public DateTime Dob { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Employeeid { get; set; }
        public int? StoreId { get; set; }

        public string EmailId { get; set; }
        public string EmailId1 { get; set; }
        public string EmailId2 { get; set; }
        public string EmailId3 { get; set; }

        public string NewPassword { get; set; }

       // public string EmailId1 { get; set; }
       // public string EmailId2 { get; set; }
        //public string EmailId3 { get; set; }

        public string Role { get; set; }

        public int Index { get; set; }

        public bool IsManager { get; set; }

        public bool IsEmployee { get; set; }
      
        public bool IsAssignStore { get; set; }
        public int ShiftId { get; set; }
        public string StoreAddress { get; set; }
        public bool? IsRememberMe { get; set; }
    }
}