using LotteryNewAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LotteryNewAPI.Controllers
{
    public class TerminalDetailsController : ApiController
    {
        LotteryBlankDatabaseEntities context;
        public ObservableCollection<Terminal_Details> GetTerminalDataCollection { get; set; }

        [Route("api/TerminalDetails/Save_TeminalData")]
        public HttpResponseMessage Save_TeminalData([FromBody] Terminal_Details data)
        {
            using (context = new LotteryBlankDatabaseEntities())
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var result = (from s in context.tblTerminal_Data1 where s.Store_Id == data.Store_Id && s.ShiftID == Shift.ShiftID && s.Date == Shift.Date select s).ToList().LastOrDefault();
                int ActiveCount = (from s in context.tblActivated_Tickets where s.Store_Id == data.Store_Id && s.EmployeeId == data.EmployeeID && s.ShiftID == Shift.ShiftID && s.Date == System.DateTime.Today select s).Count();
                int ReceivedCount = (from s in context.tblRecievedTickets where s.Store_Id == data.Store_Id && s.EmployeeId == data.EmployeeID select s).Count();
                int? stock = 0;
                var r = (from t in context.tblRecievedTickets
                         where t.IsDelete == "N" && t.Status == "Receive" && t.Store_Id == data.Store_Id
                         select t).ToList();
                foreach (var i in r)
                {
                    stock = stock + i.Total_Price;
                }
                int? active = 0;
                var a = (from t in context.tblActivated_Tickets
                         where t.Status == "Close" && t.Store_Id == data.Store_Id && t.EmployeeId == data.EmployeeID 
                         && t.ShiftID == Shift.ShiftID
                         select t).ToList();
                foreach (var i in a)
                {
                    active = active + (int.Parse(i.End_No) - int.Parse(i.Stopped_At) + 1) * i.Price;
                }


                if (data.ScratchSells != null)

                {
                    result.Scratch_Sells = data.ScratchSells;
                    result.Net_Cash = ((Convert.ToInt32(result.Scratch_Sells) + Convert.ToInt32(data.OnlineSells)) - (Convert.ToInt32(data.ScratchPayout) + Convert.ToInt32(data.OnlinePayout) + Convert.ToInt32(data.Loan))).ToString();

                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }


                if (result != null)
                {
                    result.Scratch_Payout = data.ScratchPayout;
                    result.Online_Payout = data.OnlinePayout;
                    result.Online_Sells = data.OnlineSells;
                    result.Issued_Inventory = data.IssuedInventory;
                    result.InStock_Inventory = data.InstockInventory;
                    result.Active_Inventory = data.ActiveInventory;
                    result.Total = result.Issued_Inventory + result.InStock_Inventory + result.Active_Inventory;
                    result.Loan = data.Loan;
                    result.Cash_On_Hand = data.CashOnHand.ToString();
                    result.Credit = data.Credit;
                    result.Debit = data.Debit;
                    result.Top_Up = data.TopUp;
                    result.Top_Up_Cancel = data.TopUPCancel;
                    result.CountActive = ActiveCount;
                    result.CountRecevied = ReceivedCount;
                    result.Date = Shift.Date;
                    result.Store_Id = data.Store_Id;
                    result.Employee_Id = data.EmployeeID;
                    result.ShiftID = Shift.ShiftID;
                    result.Total_Stock_Inventory = stock.ToString();
                    result.Total_Active_Inventory = active.ToString();
                    result.Scratch_Sells = data.ScratchSells;
                    context.SaveChanges();
                }
               else
                {
                    var v = context.tblTerminal_Data1.Create();
                    v.Scratch_Payout = data.ScratchPayout;
                    v.Online_Payout = data.OnlinePayout;
                    v.Online_Sells = data.OnlineSells;
                    v.Issued_Inventory = data.IssuedInventory;
                    v.InStock_Inventory = data.InstockInventory;
                    v.Active_Inventory = data.ActiveInventory;
                    v.Total = v.Issued_Inventory + v.InStock_Inventory + v.Active_Inventory;
                    v.Loan = data.Loan;
                    v.Scratch_Sells = data.ScratchSells;
                    v.Cash_On_Hand = data.CashOnHand.ToString();
                    v.Credit = data.Credit;
                    v.Debit = data.Debit;
                    v.Top_Up = data.TopUp;
                    v.Top_Up_Cancel = data.TopUPCancel;
                    v.CountActive = ActiveCount;
                    v.CountRecevied = ReceivedCount;
                    v.Date = Shift.Date;
                    v.Store_Id = data.Store_Id;
                    v.ShiftID = Shift.ShiftID;
                    v.Employee_Id = data.EmployeeID;
                    v.Total_Stock_Inventory = stock.ToString();
                    v.Total_Active_Inventory = active.ToString();
                    v.Net_Cash = ((Convert.ToInt32(data.ScratchSells) + Convert.ToInt32(data.OnlineSells)) - (Convert.ToInt32(data.ScratchPayout) + Convert.ToInt32(data.OnlinePayout) + Convert.ToInt32(data.Loan))).ToString();

                    context.tblTerminal_Data1.Add(v);
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                    return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }

        
        //[Route("api/TerminalDetails/NewGetTerminalDataHistory")]
        //public HttpResponseMessage NewGetTerminalDataHistory([FromBody] Terminal_Details data)
        //{
        //    GetTerminalDataCollection = new ObservableCollection<Terminal_Details>();
        //    context = new LotteryBlankDatabaseEntities();
        //    var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
        //    if(Shift != null)
        //    {
        //        if (data.ShiftID == 0 && data.EmployeeID != 0) // Shift Report & Daily Report
        //        {
        //            var result = (from s in context.tblTerminal_Data1 where s.Store_Id == data.Store_Id && s.Date == Shift.Date select s).ToList();
        //            foreach (var v in result)
        //            {
        //                GetTerminalDataCollection.Add(new Terminal_Details
        //                {
        //                    ScratchSells = v.Scratch_Sells,
        //                    OnlineSells = v.Online_Sells,
        //                    TotalSells = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells)).ToString(),
        //                    ScratchPayout = v.Scratch_Payout,
        //                    OnlinePayout = v.Online_Payout,
        //                    TotalPayout = (int.Parse(v.Online_Payout) + int.Parse(v.Scratch_Payout)).ToString(),
        //                    Loan = v.Loan,
        //                    Credit = v.Credit,
        //                    Debit = v.Debit,
        //                    TopUp = v.Top_Up,
        //                    TopUPCancel = v.Top_Up_Cancel,
        //                    IssuedInventory = v.Issued_Inventory,
        //                    InstockInventory = v.InStock_Inventory,
        //                    TotalStockInventory = v.Total_Stock_Inventory,
        //                    TotalActiveInventory = v.Total_Active_Inventory,
        //                    ActiveInventory = v.Active_Inventory,
        //                    CashOnHand = v.Cash_On_Hand,
        //                    ShiftID = v.ShiftID,
        //                    EmployeeID = Convert.ToInt32(v.Employee_Id),
        //                    Total = v.Total,
        //                    //ActualCash = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells) - int.Parse(v.Scratch_Payout) - int.Parse(v.Online_Payout) - int.Parse(v.Loan)).ToString(),
        //                    ShortOver = ((Convert.ToInt32(v.Scratch_Sells) + Convert.ToInt32(v.Online_Sells) - Convert.ToInt32(v.Scratch_Payout) - Convert.ToInt32(v.Online_Payout) - int.Parse(v.Loan)) - Convert.ToInt32(v.Scratch_Sells)).ToString(),
        //                    CountTerminalActiveReceive = (int.Parse(v.InStock_Inventory) + int.Parse(v.Active_Inventory)).ToString(),
        //                    CountActive = v.CountActive,
        //                    CountRecevied = v.CountRecevied,
        //                    TotalActiveReceviedStock = v.CountActive + v.CountRecevied,
        //                    ShortoverStock = Convert.ToInt32(v.CountRecevied) - Convert.ToInt32(v.InStock_Inventory),
        //                    ShortoverActive = Convert.ToInt32(v.CountActive) - Convert.ToInt32(v.Active_Inventory),
        //                    Date = Convert.ToDateTime(v.Date),
        //                    NetCash = v.Net_Cash
                            

        //                });
        //            }
        //        }
        //        else if (data.ShiftID == 0 && data.EmployeeID == 0) // Hamburger Daily report
        //        {
        //            var result = (from s in context.tblTerminal_Data1 where s.Store_Id == data.Store_Id
        //                          && s.Date == data.Date select s).ToList();
        //            foreach (var v in result)
        //            {
        //                GetTerminalDataCollection.Add(new Terminal_Details
        //                {
        //                    ScratchSells = v.Scratch_Sells,
        //                    OnlineSells = v.Online_Sells,
        //                    TotalSells = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells)).ToString(),
        //                    ScratchPayout = v.Scratch_Payout,
        //                    OnlinePayout = v.Online_Payout,
        //                    TotalPayout = (int.Parse(v.Online_Payout) + int.Parse(v.Scratch_Payout)).ToString(),
        //                    Loan = v.Loan,
        //                    Credit = v.Credit,
        //                    Debit = v.Debit,
        //                    TopUp = v.Top_Up,
        //                    TopUPCancel = v.Top_Up_Cancel,
        //                    IssuedInventory = v.Issued_Inventory,
        //                    InstockInventory = v.InStock_Inventory,
        //                    ActiveInventory = v.Active_Inventory,
        //                    CashOnHand = v.Cash_On_Hand,
        //                    ShiftID = v.ShiftID,
        //                    EmployeeID = Convert.ToInt32(v.Employee_Id),
        //                    Total = v.Total,
        //                    //ActualCash = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells) - int.Parse(v.Scratch_Payout) - int.Parse(v.Online_Payout) - int.Parse(v.Loan)).ToString(),
        //                    ShortOver = ((Convert.ToInt32(v.Scratch_Sells) + Convert.ToInt32(v.Online_Sells) - Convert.ToInt32(v.Scratch_Payout) - Convert.ToInt32(v.Online_Payout) - int.Parse(v.Loan)) - Convert.ToInt32(v.Scratch_Sells)).ToString(),
        //                    CountTerminalActiveReceive = (int.Parse(v.InStock_Inventory) + int.Parse(v.Active_Inventory)).ToString(),
        //                    CountActive = v.CountActive,
        //                    CountRecevied = v.CountRecevied,
        //                    TotalActiveReceviedStock = v.CountActive + v.CountRecevied,
        //                    ShortoverStock = Convert.ToInt32(v.CountRecevied) - Convert.ToInt32(v.InStock_Inventory),
        //                    ShortoverActive = Convert.ToInt32(v.CountActive) - Convert.ToInt32(v.Active_Inventory),
        //                    Date = Convert.ToDateTime(v.Date),
        //                    TotalStockInventory = v.Total_Stock_Inventory,
        //                    TotalActiveInventory = v.Total_Active_Inventory,
        //                    NetCash = v.Net_Cash
                           
        //                });
        //            }
        //        }
               
        //        else if(data.ShiftID != 0 )  // Hamburger Shift Report
        //        {
        //            var result = (from s in context.tblTerminal_Data1 where s.Store_Id == data.Store_Id select s).ToList();
        //            foreach (var v in result)
        //            {
        //                GetTerminalDataCollection.Add(new Terminal_Details
        //                {
        //                    ScratchSells = v.Scratch_Sells,
        //                    OnlineSells = v.Online_Sells,
        //                    TotalSells = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells)).ToString(),
        //                    ScratchPayout = v.Scratch_Payout,
        //                    OnlinePayout = v.Online_Payout,
        //                    TotalPayout = (int.Parse(v.Online_Payout) + int.Parse(v.Scratch_Payout)).ToString(),
        //                    Loan = v.Loan,
        //                    Credit = v.Credit,
        //                    Debit = v.Debit,
        //                    TopUp = v.Top_Up,
        //                    TopUPCancel = v.Top_Up_Cancel,
        //                    IssuedInventory = v.Issued_Inventory,
        //                    InstockInventory = v.InStock_Inventory,
        //                    ActiveInventory = v.Active_Inventory,
        //                    CashOnHand = v.Cash_On_Hand,
        //                    ShiftID = v.ShiftID,
        //                    EmployeeID = Convert.ToInt32(v.Employee_Id),
        //                    Total = v.Total,
        //                    //ActualCash = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells) - int.Parse(v.Scratch_Payout) - int.Parse(v.Online_Payout) - int.Parse(v.Loan)).ToString(),
        //                    ShortOver = ((Convert.ToInt32(v.Scratch_Sells) + Convert.ToInt32(v.Online_Sells) - Convert.ToInt32(v.Scratch_Payout) - Convert.ToInt32(v.Online_Payout) - int.Parse(v.Loan)) - Convert.ToInt32(v.Scratch_Sells)).ToString(),
        //                    CountTerminalActiveReceive = (int.Parse(v.InStock_Inventory) + int.Parse(v.Active_Inventory)).ToString(),
        //                    CountActive = v.CountActive,
        //                    CountRecevied = v.CountRecevied,
        //                    TotalActiveReceviedStock = v.CountActive + v.CountRecevied,
        //                    ShortoverStock = Convert.ToInt32(v.CountRecevied) - Convert.ToInt32(v.InStock_Inventory),
        //                    ShortoverActive = Convert.ToInt32(v.CountActive) - Convert.ToInt32(v.Active_Inventory),
        //                    Date = Convert.ToDateTime(v.Date),
        //                    TotalStockInventory = v.Total_Stock_Inventory,
        //                    TotalActiveInventory = v.Total_Active_Inventory,
        //                    NetCash = v.Net_Cash
                            
        //                });
        //            }
        //        }

        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, GetTerminalDataCollection);
        //}
    }
}
