using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryNewAPI.Models
{
    public class Terminal_Details
    {
        public string ScratchSells { get; set; }
        public string ScratchPayout { get; set; }
        public string OnlineSells { get; set; }
        public string OnlinePayout { get; set; }
        public string Loan { get; set; }
        public string CashOnHand { get; set; }
        public int Credit { get; set; }
        public int Debit { get; set; }
        public int TopUp { get; set; }
        public int TopUPCancel { get; set; }
        public string TotalSells { get; set; }
        public string TotalPayout { get; set; }
        public string TotalStockInventory { get; set; }
        public string TotalActiveInventory { get; set; }

        public string NetCash { get; set; }
        public string Short1 { get; set; }
        public string Over { get; set; }

        public string ShortOver { get; set; }
        public int? Store_Id { get; set; }
        public int EmployeeID { get; set; }
        public string IssuedInventory { get; set; }
        public string InstockInventory { get; set; }
        public string ActiveInventory { get; set; }

        public string CountTerminalActiveReceive { get; set; }

        public int? CountRecevied { get; set; }
        public int? CountActive { get; set; }

        public int ShortoverStock { get; set; }
        public int ShortoverActive { get; set; }

        public int? TotalActiveReceviedStock { get; set; }
        public string Total { get; set; }
        public int? ShiftID { get; set; }
        public DateTime Date { get; set; }

        public string CloseTime { get; set; }

        public DateTime HamburgerFromDateOk { get; set; }

        public DateTime HamburgerToDateOk { get; set; }
    }
}