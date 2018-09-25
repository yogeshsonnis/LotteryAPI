using LotteryNewAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using iTextSharp.text.pdf;
using System.Reflection;
using System.Net.Mail;

namespace LotteryNewAPI.Controllers
{
    public class SendmailController : ApiController
    {
        LotteryBlankDatabaseEntities context;

        public ObservableCollection<SoldOut_TicketInfo> LotteryHistory { get; set; }
        public ObservableCollection<SoldOut_TicketInfo> SoldOutHistory { get; set; }
        public ObservableCollection<Terminal_Details> GetTerminalDataCollection { get; set; }

        public ObservableCollection<Activate_Ticket> LotteryHistoryActivate { get; set; }

        public ObservableCollection<Return_TicketInfo> LotteryHistoryReturn { get; set; }
        public bool ShiftReportGenerate = false;
        public List<Activate_Ticket> ABC1 { get; set; }
        public List<Activate_Ticket> ABC2 { get; set; }
        public List<Activate_Ticket> ABC3 { get; set; }
        Activate_Ticket activeObj = new Activate_Ticket();
        Terminal_Details terminalClsObj = new Terminal_Details();
        SoldOut_TicketInfo soldOutObj = new SoldOut_TicketInfo();
        Return_TicketInfo ReturnObj = new Return_TicketInfo();

        static int pdfStoreId;
        static DateTime pdfDate;
        int totPrice = 0;
        DateTime sdate;
        int temp = 0;
        int endno;
        string pdfEndTime;
        string pdfEmpName;
        string pdfLoc;
        static int pdfEmpId;
        static int pdfShiftId;
        int sid;
        int shiftcount = 0;
        private const string topPadding = "1mm";
        private const string FONT_FACE = "Times New Roman";
        private MigraDoc.DocumentObjectModel.Color FONT_COLOR = MigraDoc.DocumentObjectModel.Colors.Black;

        [Route("api/Sendmail/Send_Mail")]
        public HttpResponseMessage Send_Mail([FromBody] SoldOut_TicketInfo data)
        {
            int temp = 1;
            context = new LotteryBlankDatabaseEntities();
            var shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
            var k = (from s in context.tblStore_Info where s.Store_Id == data.Store_Id select s).ToList().FirstOrDefault();
            ShiftReportGenerate = data.ShiftReportGenerate;
            //string filename = "ABC";
            //string reportFileName = filename + ".pdf";
            context = new LotteryBlankDatabaseEntities();
            var renderer = new PdfDocumentRenderer();
            pdfStoreId = data.Store_Id;
            pdfDate = data.Created_Date;
            //pdfDate = Convert.ToDateTime(shift.Date);
            pdfEndTime = data.CloseTime;

            if (ShiftReportGenerate == true)
            {
                pdfEmpId = Convert.ToInt32(data.EmployeeID);
                if(data.ShiftID==0)
                {
                    pdfShiftId = shift.ShiftID;
                }
                else
                {
                    pdfShiftId = Convert.ToInt32(data.ShiftID);
                }
                
            }
            renderer.Document = ShiftReportCreateDocument();
            renderer.RenderDocument();
            MemoryStream fileStream = new MemoryStream();
            renderer.PdfDocument.Save(fileStream,false);
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("chaudharivikcy56@gmail.com");
           
            if (k.EmailId1!=null && k.Email1_On_Off==true)
            {
                mail.To.Add(k.EmailId1);
            }
            if (k.EmailId2 != null && k.Email2_On_Off == true)
            {
                mail.To.Add(k.EmailId2);
            }
            if (k.EmailId3 != null && k.Email1_On_Off == true)
            {
                mail.To.Add(k.EmailId3);
            }

            //mail.To.Add(k.EmailId2);
            //mail.To.Add(k.EmailId3);
            mail.Subject = "Shift Report";
            mail.Body = "PFA,";
            mail.Attachments.Add(new System.Net.Mail.Attachment(fileStream, "ShiftReport.pdf"));
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("chaudharivicky56@gmail.com", "sohamVkey10");
            SmtpServer.EnableSsl = true;
            foreach (var i in mail.To)
            {
                if(i.Address != null)
                {
                    SmtpServer.Send(mail);
                    HttpResponseMessage result1 = new HttpResponseMessage(HttpStatusCode.OK);
                    return result1;
                }
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NotFound);
            return result;
            // return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/Sendmail/NewDailyReport_Mail")]
        public HttpResponseMessage DailyReport_Mail([FromBody] SoldOut_TicketInfo data)
        {
            context = new LotteryBlankDatabaseEntities();
            var k = (from s in context.tblStore_Info where s.Store_Id == data.Store_Id select s).ToList().FirstOrDefault();
            var renderer = new PdfDocumentRenderer();

            if (data.EmployeeID == 1)
            {
                var shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                pdfStoreId = data.Store_Id;
                pdfDate = Convert.ToDateTime(shift.Date);
            }
            else
            {
                pdfStoreId = data.Store_Id;
                pdfDate = data.Created_Date;
            }


            renderer.Document = CreateDocument();

            renderer.RenderDocument();

            MemoryStream fileStream = new MemoryStream();
            renderer.PdfDocument.Save(fileStream, false);
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("chaudharivikcy56@gmail.com");
            //mail.To.Add(k.EmailId1);
            //mail.To.Add(k.EmailId2);
            //mail.To.Add(k.EmailId3);
            if (k.EmailId1 != null && k.Email1_On_Off == true)
            {
                mail.To.Add(k.EmailId1);
            }
            if (k.EmailId2 != null && k.Email2_On_Off == true)
            {
                mail.To.Add(k.EmailId2);
            }
            if (k.EmailId3 != null && k.Email3_On_Off == true)
            {
                mail.To.Add(k.EmailId3);
            }
            mail.Subject = "Daily Report";
            mail.Body = "PFA,";
            mail.Attachments.Add(new System.Net.Mail.Attachment(fileStream, "DailyReport.pdf"));
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("chaudharivicky56@gmail.com", "sohamVkey10");
            SmtpServer.EnableSsl = true;
            foreach (var i in mail.To)
            {
                if (i.Address != null)
                {
                    SmtpServer.Send(mail);
                    HttpResponseMessage result1 = new HttpResponseMessage(HttpStatusCode.OK);
                    return result1;
                }
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NotFound);
            return result;

        }

        public Document ShiftReportCreateDocument()
        {
            var document = new MigraDoc.DocumentObjectModel.Document();
            document.Info.Title = "Shift Report";
            //document.Info.Author = "Yogesh";

            DefineStyles(document);

            ShiftReportCreateReportPage(document);

            return document;
        }
        private void ShiftReportCreateReportPage(Document document)
        {
            try
            {
                int count = 1;

                MigraDoc.DocumentObjectModel.Section section = document.AddSection();

                section.PageSetup.PageFormat = PageFormat.A4;
                section.PageSetup.TopMargin = "1.5cm";
                section.PageSetup.LeftMargin = "1cm";

                ParagraphFormat formatBold = new ParagraphFormat();
                formatBold.Font.Size = 7;
                formatBold.Font.Bold = true;

                ParagraphFormat formatNormal = new ParagraphFormat();
                formatNormal.Font.Size = 7;
                formatNormal.Font.Bold = false;

                if (ShiftReportGenerate == true)
                {
                    count = pdfShiftId;
                }

                ShiftReportAddHeaderToReport(section, formatBold, formatNormal, count);

                AddLineSpace(section, TabLeader.Spaces);

                formatNormal.Font.Size = 7;
   

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void ShiftReportAddHeaderToReport(Section section, ParagraphFormat formatBold, ParagraphFormat formatNormal, int count)
        {
            MigraDoc.DocumentObjectModel.Shapes.Image labLogo = new MigraDoc.DocumentObjectModel.Shapes.Image();
            context = new LotteryBlankDatabaseEntities();
            section.Headers.Primary = null;
            Paragraph space13 = section.AddParagraph();
            Paragraph space18 = section.AddParagraph();

            Paragraph title1 = section.AddParagraph("Shift Report " + count.ToString());
            title1.Format.Font.Size = 9;
            title1.Format.Font.Bold = true;
            title1.Format.Alignment = ParagraphAlignment.Center;
            Paragraph space19 = section.AddParagraph();
            Paragraph space20 = section.AddParagraph();

            // Create header
            var headerTable = section.Headers.FirstPage.Section.AddTable();

            Column headerColumnLocationName = headerTable.AddColumn("3cm");
            headerColumnLocationName.Format.Alignment = ParagraphAlignment.Left;

            Column headerColLocNameValue = headerTable.AddColumn("10cm");
            headerColLocNameValue.Format.Alignment = ParagraphAlignment.Left;

            Column headerColumnUserLogin = headerTable.AddColumn("3cm");
            headerColumnUserLogin.Format.Alignment = ParagraphAlignment.Right;

            Column headerColumnUserLoginValue = headerTable.AddColumn("3cm");
            headerColumnUserLoginValue.Format.Alignment = ParagraphAlignment.Right;

            Row HeaderRow = headerTable.AddRow();
            Row HeaderRow1 = headerTable.AddRow();
            Row HeaderRow2 = headerTable.AddRow();

            ShiftReportgetHeaderDetails(pdfStoreId, pdfDate);

            HeaderRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[0].AddParagraph("Location Name").Format = formatBold.Clone();

            HeaderRow.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[1].AddParagraph(":  " + pdfLoc).Format = formatBold.Clone();

            HeaderRow.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[2].AddParagraph("User Login").Format = formatBold.Clone();

            HeaderRow.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[3].AddParagraph(":  " + pdfEmpName).Format = formatBold.Clone();

            HeaderRow1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[0].AddParagraph("Address").Format = formatBold.Clone();

            HeaderRow1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[1].AddParagraph(":  " + pdfLoc).Format = formatBold.Clone();

            HeaderRow1.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[2].AddParagraph("Date").Format = formatBold.Clone();

            HeaderRow1.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[3].AddParagraph(":  " + pdfDate.ToString("MM/dd/yyyy")).Format = formatBold.Clone();

            HeaderRow2.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow2.Cells[2].AddParagraph("Time").Format = formatBold.Clone();

            HeaderRow2.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow2.Cells[3].AddParagraph(":  " + pdfEndTime).Format = formatBold.Clone();

            totPrice = 0;
            Paragraph paragraph = section.AddParagraph();
            paragraph.AddText("1. Activity During Shift");
            paragraph.Format.Font.Size = 8;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Left;

            //section.AddParagraph("1. Activity During Shift").Format.Alignment = ParagraphAlignment.Left;

            var eventTable = section.AddTable();
            eventTable.Style = "Table";
            eventTable.Borders.Width = 0.25;
            eventTable.Borders.Color = Colors.Black;
            eventTable.Rows.Height = 10;
            eventTable.Format.Font.Size = 7;

            //eventTable.Borders.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;

            Column columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            Row headers = eventTable.AddRow();

            headers.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            headers.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            headers.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            headers.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            headers.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            headers.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            headers.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            headers.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            headers.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            headers.Cells[7].AddParagraph("Count").Format = formatBold.Clone();
            headers.Cells[7].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            headers.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;



            var e = (from s in context.tblShifts
                     where s.StoreId == pdfStoreId && s.Date == pdfDate.Date && s.ShiftID == pdfShiftId
                     select s).ToList().FirstOrDefault();

            if (e.EndTime == null)
            {
                soldOutObj = new SoldOut_TicketInfo();
                soldOutObj.Store_Id = pdfStoreId;
                soldOutObj.Created_Date = pdfDate;
                soldOutObj.ShiftID = 0;
                soldOutObj.EmployeeID = pdfEmpId;
            }
            else
            {
                soldOutObj = new SoldOut_TicketInfo();
                soldOutObj.Store_Id = pdfStoreId;
                soldOutObj.Created_Date = pdfDate;
                soldOutObj.ShiftID = pdfShiftId;
                soldOutObj.EmployeeID = pdfEmpId;
                soldOutObj.CloseTime = e.EndTime;
            }


            NewGetAllHistory(soldOutObj);

            for (int i = 0; i < SoldOutHistory.Count; i++)
            {
                Row row1 = eventTable.AddRow();
                row1.Cells[0].AddParagraph(SoldOutHistory[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(SoldOutHistory[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(SoldOutHistory[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(SoldOutHistory[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(SoldOutHistory[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(SoldOutHistory[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(SoldOutHistory[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(SoldOutHistory[i].Count.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(SoldOutHistory[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;

                totPrice = totPrice + Convert.ToInt32(SoldOutHistory[i].Total_Price);
            }

            Paragraph paragraph1 = section.AddParagraph();
            paragraph1.AddText(@"Total Amount  :  " + totPrice);
            paragraph1.Format.Font.Size = 7;
            paragraph1.Format.Font.Bold = true;
            paragraph1.Format.Alignment = ParagraphAlignment.Right;


            Table scratchSell = section.AddTable();
            scratchSell.Borders.Width = 0.5;

            Column col1 = scratchSell.AddColumn("4cm");
            col1.Format.Alignment = ParagraphAlignment.Left;

            Column col2 = scratchSell.AddColumn("4cm");
            col1.Format.Alignment = ParagraphAlignment.Center;

            Row row7 = scratchSell.AddRow();
            row7.HeightRule = RowHeightRule.Exactly;
            row7.Height = 12;
            row7.Borders.Bottom.Visible = false;
            row7.TopPadding = 2;

            Row row2 = scratchSell.AddRow();
            row2.HeightRule = RowHeightRule.Exactly;
            row2.Height = 10;
            row2.Borders.Top.Visible = false;
            row2.Borders.Bottom.Visible = false;

            Row row3 = scratchSell.AddRow();
            row3.HeightRule = RowHeightRule.Exactly;
            row3.Height = 10;
            row3.Borders.Top.Visible = false;
            row3.Borders.Bottom.Visible = false;

            Row row4 = scratchSell.AddRow();
            row4.HeightRule = RowHeightRule.Exactly;
            row4.Height = 10;
            row4.Borders.Top.Visible = false;
            row4.Borders.Bottom.Visible = false;

            Row row5 = scratchSell.AddRow();
            row5.HeightRule = RowHeightRule.Exactly;
            row5.Height = 10;
            row5.Borders.Top.Visible = false;
            row5.Borders.Bottom.Visible = false;

            Row row6 = scratchSell.AddRow();
            row6.HeightRule = RowHeightRule.Exactly;
            row6.Height = 10;
            row6.Borders.Top.Visible = false;
            row6.BottomPadding = 6;


            var f = (from s in context.tblShifts
                     where s.StoreId == pdfStoreId && s.Date == pdfDate.Date && s.ShiftID == pdfShiftId
                     select s).ToList().FirstOrDefault();

            if (f.EndTime == null)
            {
                terminalClsObj = new Terminal_Details();
                terminalClsObj.Store_Id = pdfStoreId;
                terminalClsObj.ShiftID = 0;
                terminalClsObj.EmployeeID = 1;
            }
            else
            {
                terminalClsObj = new Terminal_Details();
                terminalClsObj.Store_Id = pdfStoreId;
                terminalClsObj.EmployeeID = pdfEmpId;
                terminalClsObj.ShiftID = pdfShiftId;
                terminalClsObj.CloseTime = f.EndTime;
                terminalClsObj.Date = pdfDate;
            }

            NewGetTerminalDataHistory(terminalClsObj);
            var result = GetTerminalDataCollection.Where(x => x.ShiftID == pdfShiftId && x.Date == pdfDate.Date).LastOrDefault();


            Paragraph scratch = row7.Cells[0].AddParagraph("SCRATCH SELLS  :  " + result.ScratchSells);
            scratch.Format.LineSpacing = Unit.FromMillimeter(3);
            row7.Cells[0].Format.Font.Size = 7;
            row7.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row7.Cells[0].Borders.Right.Visible = false;

            row7.Cells[1].AddParagraph(@" CREDIT  :  " + result.Credit);
            row7.Cells[1].Format.Font.Size = 7;
            row7.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row7.Cells[1].Borders.Left.Visible = false;

            row2.Cells[0].AddParagraph("ONLINE SELLS  :  " + result.OnlineSells);
            row2.Cells[0].Format.Font.Size = 7;
            row2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[0].Borders.Right.Visible = false;

            row2.Cells[1].AddParagraph(@" DEBIT  :  " + result.Debit);
            row2.Cells[1].Format.Font.Size = 7;
            row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[1].Borders.Left.Visible = false;

            row3.Cells[0].AddParagraph("SCRATCH PAYOUT  :  " + result.ScratchPayout);
            row3.Cells[0].Format.Font.Size = 7;
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[0].Borders.Right.Visible = false;

            row3.Cells[1].AddParagraph(@" TOP UP  :  " + result.TopUp);
            row3.Cells[1].Format.Font.Size = 7;
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[1].Borders.Left.Visible = false;

            row4.Cells[0].AddParagraph("ONLINE PAYOUT  :  " + result.OnlinePayout);
            row4.Cells[0].Format.Font.Size = 7;
            row4.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[0].Borders.Right.Visible = false;

            row4.Cells[1].AddParagraph(@" TOP UP CANCEL  :  " + result.TopUPCancel);
            row4.Cells[1].Format.Font.Size = 7;
            row4.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[1].Borders.Left.Visible = false;

            row5.Cells[0].AddParagraph("LOAN  :  " + result.Loan);
            row5.Cells[0].Format.Font.Size = 7;
            row5.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row5.Cells[0].Borders.Right.Visible = false;
            row5.Cells[1].Borders.Left.Visible = false;

            row6.Cells[0].AddParagraph("CASH ON HAND  :  " + result.CashOnHand);
            row6.Cells[0].Format.Font.Size = 7;
            row6.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row6.Cells[0].Borders.Right.Visible = false;
            row6.Cells[1].Borders.Left.Visible = false;

            //Sell Summery Section

            Paragraph space = section.AddParagraph();
            Paragraph p2 = section.AddParagraph();
            p2.AddText(" 2. Sell Summery");
            p2.Format.Font.Size = 8;
            p2.Format.Font.Bold = true;
            p2.Format.Alignment = ParagraphAlignment.Left;

            Table sellSummerytab = section.AddTable();
            sellSummerytab.Format.Font.Size = 7;
            sellSummerytab.Format.Alignment = ParagraphAlignment.Left;
            sellSummerytab.Borders.Width = 0.5;
            sellSummerytab.Rows.Height = 10;

            Column sellSummeryDetFirstCol = sellSummerytab.AddColumn("3cm");
            Column sellSummeryDetCol = sellSummerytab.AddColumn("2cm");


            Row shiftRow = sellSummerytab.AddRow();
            Row scratchSellsRow = sellSummerytab.AddRow();
            Row OnlineSellsRow = sellSummerytab.AddRow();
            Row TotalSellsRow = sellSummerytab.AddRow();
            Row ScratchOfPayoutRow = sellSummerytab.AddRow();
            Row OnlinePayoutRow = sellSummerytab.AddRow();
            Row TotalPayoutRow = sellSummerytab.AddRow();
            Row LoanRow = sellSummerytab.AddRow();

            shiftRow.Cells[0].Borders.Right.Visible = false;
            shiftRow.Cells[0].Borders.Bottom.Visible = false;

            scratchSellsRow.Cells[0].AddParagraph("Scratch Off Sells");
            scratchSellsRow.Cells[0].Borders.Right.Visible = false;
            scratchSellsRow.Cells[0].Borders.Bottom.Visible = false;

            OnlineSellsRow.Cells[0].AddParagraph("Online Sells");
            OnlineSellsRow.Cells[0].Borders.Right.Visible = false;
            OnlineSellsRow.Cells[0].Borders.Bottom.Visible = false;

            TotalSellsRow.Cells[0].AddParagraph("Total Sells");
            TotalSellsRow.Cells[0].Borders.Right.Visible = false;
            TotalSellsRow.Cells[0].Borders.Bottom.Visible = false;

            ScratchOfPayoutRow.Cells[0].AddParagraph("Scratch of Payout");
            ScratchOfPayoutRow.Cells[0].Borders.Right.Visible = false;
            ScratchOfPayoutRow.Cells[0].Borders.Bottom.Visible = false;

            OnlinePayoutRow.Cells[0].AddParagraph("Online Payout");
            OnlinePayoutRow.Cells[0].Borders.Right.Visible = false;
            OnlinePayoutRow.Cells[0].Borders.Bottom.Visible = false;

            TotalPayoutRow.Cells[0].AddParagraph("Total Payout");
            TotalPayoutRow.Cells[0].Borders.Right.Visible = false;
            TotalPayoutRow.Cells[0].Borders.Bottom.Visible = false;

            LoanRow.Cells[0].AddParagraph("Loan");
            LoanRow.Cells[0].Borders.Right.Visible = false;

            shiftRow.Cells[1].AddParagraph("Shift" + pdfShiftId);
            shiftRow.Cells[1].Borders.Left.Visible = false;
            shiftRow.Cells[1].Borders.Bottom.Visible = false;

            scratchSellsRow.Cells[1].AddParagraph(result.ScratchSells);
            scratchSellsRow.Cells[1].Borders.Left.Visible = false;
            scratchSellsRow.Cells[1].Borders.Bottom.Visible = false;

            OnlineSellsRow.Cells[1].AddParagraph(result.OnlineSells);
            OnlineSellsRow.Cells[1].Borders.Left.Visible = false;
            OnlineSellsRow.Cells[1].Borders.Bottom.Visible = false;

            TotalSellsRow.Cells[1].AddParagraph(result.TotalSells);
            TotalSellsRow.Cells[1].Borders.Left.Visible = false;
            TotalSellsRow.Cells[1].Borders.Bottom.Visible = false;

            ScratchOfPayoutRow.Cells[1].AddParagraph(result.ScratchPayout);
            ScratchOfPayoutRow.Cells[1].Borders.Left.Visible = false;
            ScratchOfPayoutRow.Cells[1].Borders.Bottom.Visible = false;

            OnlinePayoutRow.Cells[1].AddParagraph(result.OnlinePayout);
            OnlinePayoutRow.Cells[1].Borders.Left.Visible = false;
            OnlinePayoutRow.Cells[1].Borders.Bottom.Visible = false;

            TotalPayoutRow.Cells[1].AddParagraph(result.TotalPayout);
            TotalPayoutRow.Cells[1].Borders.Left.Visible = false;
            TotalPayoutRow.Cells[1].Borders.Bottom.Visible = false;

            LoanRow.Cells[1].AddParagraph(result.Loan);
            LoanRow.Cells[1].Borders.Top.Visible = false;
            LoanRow.Cells[1].Borders.Left.Visible = false;



            //// section I hope card details - I

            Paragraph space1 = section.AddParagraph();

            Table cardDetTableFirst = section.AddTable();

            cardDetTableFirst.Format.Font.Size = 7;
            cardDetTableFirst.Format.Alignment = ParagraphAlignment.Left;
            cardDetTableFirst.Borders.Width = 0.5;
            cardDetTableFirst.Rows.Height = 10;


            Column cardDetTableFirstCol = cardDetTableFirst.AddColumn("3cm");
            Column cardDetTableCol = cardDetTableFirst.AddColumn("2cm");

            Row CardDetRow = cardDetTableFirst.AddRow();
            Row CardDetShiftRow = cardDetTableFirst.AddRow();
            Row CardDetCreditRow = cardDetTableFirst.AddRow();
            Row CardDetDebitRow = cardDetTableFirst.AddRow();
            Row CardDetTopupRow = cardDetTableFirst.AddRow();
            Row CardDetTopupCRow = cardDetTableFirst.AddRow();

            CardDetRow.Cells[0].AddParagraph("I HOPE CARD DETAILS");
            CardDetRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetRow.Cells[0].Borders.Right.Visible = false;

            CardDetRow.Cells[1].Borders.Bottom.Visible = false;
            CardDetRow.Cells[1].Borders.Left.Visible = false;

            CardDetShiftRow.Cells[0].Borders.Right.Visible = false;
            CardDetShiftRow.Cells[0].Borders.Bottom.Visible = false;

            CardDetCreditRow.Cells[0].AddParagraph("Credit");
            CardDetCreditRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetCreditRow.Cells[0].Borders.Right.Visible = false;

            CardDetDebitRow.Cells[0].AddParagraph("Debit");
            CardDetDebitRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetDebitRow.Cells[0].Borders.Right.Visible = false;

            CardDetTopupRow.Cells[0].AddParagraph("Topup");
            CardDetTopupRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetTopupRow.Cells[0].Borders.Right.Visible = false;

            CardDetTopupCRow.Cells[0].AddParagraph("Topup Cancel");
            CardDetTopupCRow.Cells[0].Borders.Right.Visible = false;

            CardDetShiftRow.Cells[1].AddParagraph("Shift " + pdfShiftId);
            CardDetShiftRow.Cells[1].Borders.Left.Visible = false;
            CardDetShiftRow.Cells[1].Borders.Bottom.Visible = false;

            CardDetCreditRow.Cells[1].AddParagraph(result.Credit.ToString());
            CardDetCreditRow.Cells[1].Borders.Left.Visible = false;
            CardDetCreditRow.Cells[1].Borders.Bottom.Visible = false;

            CardDetDebitRow.Cells[1].AddParagraph(result.Debit.ToString());
            CardDetDebitRow.Cells[1].Borders.Left.Visible = false;
            CardDetDebitRow.Cells[1].Borders.Bottom.Visible = false;

            CardDetTopupRow.Cells[1].AddParagraph(result.TopUp.ToString());
            CardDetTopupRow.Cells[1].Borders.Left.Visible = false;
            CardDetTopupRow.Cells[1].Borders.Bottom.Visible = false;

            CardDetTopupCRow.Cells[1].AddParagraph(result.TopUPCancel.ToString());
            CardDetTopupCRow.Cells[1].Borders.Left.Visible = false;
            CardDetTopupCRow.Cells[1].Borders.Top.Visible = false;


            //// I Hope card detaild section - II

            Paragraph space2 = section.AddParagraph();

            Table cardDetTableSecond = section.AddTable();
            cardDetTableSecond.Format.Font.Size = 7;
            cardDetTableSecond.Format.Alignment = ParagraphAlignment.Left;
            cardDetTableSecond.Borders.Width = 0.5;
            cardDetTableSecond.Rows.Height = 10;

            Column cardDetTableSecondCol1 = cardDetTableSecond.AddColumn("3cm");
            Column cardDetTableSecondCol = cardDetTableSecond.AddColumn("2cm");


            Row cardDetTableSecondshiftRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondNetCashRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondCashOnHRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondShortRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondOverRow = cardDetTableSecond.AddRow();



            cardDetTableSecondshiftRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondshiftRow.Cells[0].Borders.Bottom.Visible = false;

            cardDetTableSecondNetCashRow.Cells[0].AddParagraph("Net Cash");
            cardDetTableSecondNetCashRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondNetCashRow.Cells[0].Borders.Bottom.Visible = false;

            cardDetTableSecondCashOnHRow.Cells[0].AddParagraph("Cash On Hand");
            cardDetTableSecondCashOnHRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondCashOnHRow.Cells[0].Borders.Bottom.Visible = false;

            cardDetTableSecondShortRow.Cells[0].AddParagraph("Short");
            cardDetTableSecondShortRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondCashOnHRow.Cells[0].Borders.Bottom.Visible = false;


            cardDetTableSecondOverRow.Cells[0].AddParagraph("Over");
            cardDetTableSecondOverRow.Cells[0].Borders.Right.Visible = false;


            cardDetTableSecondshiftRow.Cells[1].AddParagraph("Shift" + pdfShiftId);
            cardDetTableSecondshiftRow.Cells[1].Borders.Left.Visible = false;
            cardDetTableSecondshiftRow.Cells[1].Borders.Bottom.Visible = false;

            cardDetTableSecondNetCashRow.Cells[1].AddParagraph(result.NetCash.ToString());
            cardDetTableSecondNetCashRow.Cells[1].Borders.Left.Visible = false;
            cardDetTableSecondNetCashRow.Cells[1].Borders.Bottom.Visible = false;

            cardDetTableSecondCashOnHRow.Cells[1].AddParagraph(result.CashOnHand.ToString());
            cardDetTableSecondCashOnHRow.Cells[1].Borders.Left.Visible = false;
            cardDetTableSecondCashOnHRow.Cells[1].Borders.Bottom.Visible = false;

            cardDetTableSecondShortRow.Cells[1].AddParagraph(result.Short1.ToString());
            cardDetTableSecondShortRow.Cells[1].Borders.Left.Visible = false;
            cardDetTableSecondShortRow.Cells[1].Borders.Bottom.Visible = false;
            cardDetTableSecondShortRow.Cells[0].Borders.Bottom.Visible = false;

            cardDetTableSecondOverRow.Cells[1].AddParagraph(result.Over.ToString());
            cardDetTableSecondOverRow.Cells[1].Borders.Left.Visible = false;



            // section 3 - Active and stock info
            // A . Data from lottery app

            Paragraph space3 = section.AddParagraph();
            Paragraph space4 = section.AddParagraph();
            Paragraph p3 = section.AddParagraph();
            p3.AddText(" 3. Active and Stock Info");
            p3.Format.Font.Size = 8;
            p3.Format.Font.Bold = true;
            p3.Format.Alignment = ParagraphAlignment.Left;

            Paragraph lotteryData = section.AddParagraph("A. Data From Lottery App");
            lotteryData.Format.Font.Size = 7;
            lotteryData.Format.Alignment = ParagraphAlignment.Left;

            Table dataFromLottery = section.AddTable();
            dataFromLottery.Format.Font.Size = 7;
            dataFromLottery.Format.Alignment = ParagraphAlignment.Left;
            dataFromLottery.Borders.Width = 0.5;
            dataFromLottery.Rows.Height = 10;

            Column dataFromLotteryFirstCol = dataFromLottery.AddColumn("3cm");
            Column dataFromLotterySecondCol = dataFromLottery.AddColumn("2cm");


            Row dataFromLotteryShiftRow = dataFromLottery.AddRow();
            Row dataFromLotteryStockRow = dataFromLottery.AddRow();
            Row dataFromLotteryActiveRow = dataFromLottery.AddRow();
            Row dataFromLotteryTotalRow = dataFromLottery.AddRow();


            dataFromLotteryShiftRow.Cells[0].Borders.Right.Visible = false;
            dataFromLotteryShiftRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromLotteryStockRow.Cells[0].AddParagraph("Stock");
            dataFromLotteryStockRow.Cells[0].Borders.Right.Visible = false;
            dataFromLotteryStockRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromLotteryActiveRow.Cells[0].AddParagraph("Active");
            dataFromLotteryActiveRow.Cells[0].Borders.Right.Visible = false;
            dataFromLotteryActiveRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromLotteryTotalRow.Cells[0].AddParagraph("Total");
            dataFromLotteryTotalRow.Cells[0].Borders.Right.Visible = false;

            dataFromLotteryShiftRow.Cells[1].AddParagraph("Shift" + pdfShiftId);
            dataFromLotteryShiftRow.Cells[1].Borders.Left.Visible = false;
            dataFromLotteryShiftRow.Cells[1].Borders.Bottom.Visible = false;

            dataFromLotteryStockRow.Cells[1].AddParagraph(result.CountRecevied.ToString());
            dataFromLotteryStockRow.Cells[1].Borders.Left.Visible = false;
            dataFromLotteryStockRow.Cells[1].Borders.Bottom.Visible = false;

            dataFromLotteryActiveRow.Cells[1].AddParagraph(result.CountActive.ToString());
            dataFromLotteryActiveRow.Cells[1].Borders.Left.Visible = false;
            dataFromLotteryActiveRow.Cells[1].Borders.Bottom.Visible = false;

            dataFromLotteryTotalRow.Cells[1].AddParagraph(result.TotalActiveReceviedStock.ToString());
            dataFromLotteryTotalRow.Cells[1].Borders.Top.Visible = false;
            dataFromLotteryTotalRow.Cells[1].Borders.Left.Visible = false;



            // B. Data From Terrminal

            Paragraph terminalData = section.AddParagraph("B. Data From Terminal");
            terminalData.Format.Font.Size = 7;
            terminalData.Format.Alignment = ParagraphAlignment.Left;

            Table dataFromTerminal = section.AddTable();
            dataFromTerminal.Format.Font.Size = 7;
            dataFromTerminal.Format.Alignment = ParagraphAlignment.Left;
            dataFromTerminal.Borders.Width = 0.5;
            dataFromTerminal.Rows.Height = 10;

            Column dataFromTerminalFirstCol = dataFromTerminal.AddColumn("3cm");
            Column dataFromTerminalSecondCol = dataFromTerminal.AddColumn("2cm");


            Row dataFromTerminalShiftRow = dataFromTerminal.AddRow();
            Row dataFromTerminalStockRow = dataFromTerminal.AddRow();
            Row dataFromTerminalActiveRow = dataFromTerminal.AddRow();
            Row dataFromTerminalTotalRow = dataFromTerminal.AddRow();


            dataFromTerminalShiftRow.Cells[0].Borders.Right.Visible = false;
            dataFromTerminalShiftRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromTerminalStockRow.Cells[0].AddParagraph("Stock");
            dataFromTerminalStockRow.Cells[0].Borders.Right.Visible = false;
            dataFromTerminalStockRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromTerminalActiveRow.Cells[0].AddParagraph("Active");
            dataFromTerminalActiveRow.Cells[0].Borders.Right.Visible = false;
            dataFromTerminalActiveRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromTerminalTotalRow.Cells[0].AddParagraph("Total");
            dataFromTerminalTotalRow.Cells[0].Borders.Right.Visible = false;

            dataFromTerminalShiftRow.Cells[1].AddParagraph("Shift" + pdfShiftId);
            dataFromTerminalShiftRow.Cells[1].Borders.Left.Visible = false;
            dataFromTerminalShiftRow.Cells[1].Borders.Bottom.Visible = false;

            dataFromTerminalStockRow.Cells[1].AddParagraph(result.InstockInventory.ToString());
            dataFromTerminalStockRow.Cells[1].Borders.Left.Visible = false;
            dataFromTerminalStockRow.Cells[1].Borders.Bottom.Visible = false;


            dataFromTerminalActiveRow.Cells[1].AddParagraph(result.ActiveInventory.ToString());
            dataFromTerminalActiveRow.Cells[1].Borders.Left.Visible = false;
            dataFromTerminalActiveRow.Cells[1].Borders.Bottom.Visible = false;


            dataFromTerminalTotalRow.Cells[1].AddParagraph(result.CountTerminalActiveReceive.ToString());
            dataFromTerminalTotalRow.Cells[1].Borders.Top.Visible = false;
            dataFromTerminalTotalRow.Cells[1].Borders.Left.Visible = false;



            //// C. Shortover

            Paragraph ShortoverData = section.AddParagraph("C. Short Over");
            ShortoverData.Format.Font.Size = 7;
            ShortoverData.Format.Alignment = ParagraphAlignment.Left;

            Table ShortOverTable = section.AddTable();
            ShortOverTable.Format.Font.Size = 7;
            ShortOverTable.Format.Alignment = ParagraphAlignment.Left;
            ShortOverTable.Borders.Width = 0.5;
            ShortOverTable.Rows.Height = 10;

            Column ShortOverTableFirstCol = ShortOverTable.AddColumn("3cm");
            Column ShortOverTableSecondCol = ShortOverTable.AddColumn("2cm");


            Row ShortOverTableShiftRow = ShortOverTable.AddRow();
            Row ShortOverTableStockRow = ShortOverTable.AddRow();
            Row ShortOverTableActiveRow = ShortOverTable.AddRow();

            ShortOverTableShiftRow.Cells[0].Borders.Right.Visible = false;
            ShortOverTableShiftRow.Cells[0].Borders.Bottom.Visible = false;

            ShortOverTableStockRow.Cells[0].AddParagraph("Stock");
            ShortOverTableStockRow.Cells[0].Borders.Right.Visible = false;
            ShortOverTableStockRow.Cells[0].Borders.Bottom.Visible = false;

            ShortOverTableActiveRow.Cells[0].AddParagraph("Active");
            ShortOverTableActiveRow.Cells[0].Borders.Right.Visible = false;

            ShortOverTableShiftRow.Cells[1].AddParagraph("Shift" + pdfShiftId);
            ShortOverTableShiftRow.Cells[1].Borders.Left.Visible = false;
            ShortOverTableShiftRow.Cells[1].Borders.Bottom.Visible = false;

            ShortOverTableStockRow.Cells[1].AddParagraph(result.ShortoverStock.ToString());
            ShortOverTableStockRow.Cells[1].Borders.Left.Visible = false;
            ShortOverTableStockRow.Cells[1].Borders.Bottom.Visible = false;

            ShortOverTableActiveRow.Cells[1].AddParagraph(result.ShortoverActive.ToString());
            ShortOverTableActiveRow.Cells[1].Borders.Left.Visible = false;


            //// Section Stock inventory & Remaining Active ticket

            Paragraph space5 = section.AddParagraph();
            Table StockRemaining = section.AddTable();
            StockRemaining.Format.Font.Size = 7;
            StockRemaining.Format.Alignment = ParagraphAlignment.Left;
            StockRemaining.Borders.Width = 0.5;
            StockRemaining.Rows.Height = 10;

            Column StockRemainingFirstCol = StockRemaining.AddColumn("4cm");

            Column StockRemainingSecondCol = StockRemaining.AddColumn("3cm");

            Row StockRemainingRow1 = StockRemaining.AddRow();
            Row StockRemainingRow2 = StockRemaining.AddRow();

            StockRemainingRow1.Cells[0].AddParagraph("Stock Inventory Value");
            StockRemainingRow1.Cells[0].Borders.Bottom.Visible = false;
            StockRemainingRow1.Cells[0].Borders.Right.Visible = false;

            StockRemainingRow2.Cells[0].AddParagraph("Remaining Active Ticket Value");
            StockRemainingRow2.Cells[0].Borders.Top.Visible = false;
            StockRemainingRow2.Cells[0].Borders.Right.Visible = false;

            StockRemainingRow1.Cells[1].AddParagraph(result.TotalStockInventory.ToString());
            StockRemainingRow1.Cells[1].Borders.Bottom.Visible = false;
            StockRemainingRow1.Cells[1].Borders.Left.Visible = false;

            StockRemainingRow2.Cells[1].AddParagraph(result.TotalActiveInventory.ToString());
            StockRemainingRow2.Cells[1].Borders.Top.Visible = false;
            StockRemainingRow2.Cells[1].Borders.Left.Visible = false;

            //section = list of activation

            Paragraph space6 = section.AddParagraph();
            Paragraph space7 = section.AddParagraph();
            Paragraph p4 = section.AddParagraph();
            p4.AddText(" 4. Ticket Info Screen");
            p4.Format.Font.Size = 8;
            p4.Format.Font.Bold = true;
            p4.Format.Alignment = ParagraphAlignment.Left;

            Paragraph ListActivation = section.AddParagraph("A. List Of Activation");
            ListActivation.Format.Font.Size = 7;
            ListActivation.Format.Alignment = ParagraphAlignment.Left;
            Paragraph space8 = section.AddParagraph();

            var ActivationTable = section.AddTable();
            ActivationTable.Style = "Table";
            ActivationTable.Borders.Width = 0.25;
            ActivationTable.Borders.Color = Colors.Black;
            ActivationTable.Rows.Height = 10;
            ActivationTable.Format.Font.Size = 7;
            //eventTable.Borders.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;

            Column ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            Row ActivationTableRows = ActivationTable.AddRow();

            ActivationTableRows.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            ActivationTableRows.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            ActivationTableRows.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            ActivationTableRows.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            ActivationTableRows.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            ActivationTableRows.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            ActivationTableRows.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            ActivationTableRows.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            ActivationTableRows.Cells[7].AddParagraph("Count   ").Format = formatBold.Clone();
            ActivationTableRows.Cells[7].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            ActivationTableRows.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;




            var g = (from s in context.tblShifts
                     where s.StoreId == pdfStoreId && s.Date == pdfDate.Date && s.ShiftID == pdfShiftId
                     select s).ToList().FirstOrDefault();

            if (g.EndTime == null)
            {
                activeObj = new Activate_Ticket();
                activeObj.Store_Id = pdfStoreId;
                activeObj.ShiftID = 0;
            }
            else
            {
                activeObj = new Activate_Ticket();
                activeObj.Store_Id = pdfStoreId;
                activeObj.Created_Date = pdfDate;
                activeObj.EmployeeID = pdfEmpId;
                activeObj.ShiftID = pdfShiftId;
                activeObj.CloseTime = g.EndTime;
            }

            NewGetShiftActivateHistory(activeObj);

            for (int i = 0; i < LotteryHistoryActivate.Count; i++)
            {
                Row row1 = ActivationTable.AddRow();
                row1.Cells[0].AddParagraph(LotteryHistoryActivate[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(LotteryHistoryActivate[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(LotteryHistoryActivate[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(LotteryHistoryActivate[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(LotteryHistoryActivate[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(LotteryHistoryActivate[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(LotteryHistoryActivate[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(LotteryHistoryActivate[i].Count.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(LotteryHistoryActivate[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            }

            //section = list of soldout ticket

            Paragraph space9 = section.AddParagraph();
            Paragraph ListSoldOut = section.AddParagraph("B. List Of SoldOut Ticket");
            ListSoldOut.Format.Font.Size = 7;
            ListSoldOut.Format.Alignment = ParagraphAlignment.Left;

            Paragraph space10 = section.AddParagraph();

            var SoldOutTable = section.AddTable();
            SoldOutTable.Style = "Table";
            SoldOutTable.Borders.Width = 0.25;
            SoldOutTable.Borders.Color = Colors.Black;
            SoldOutTable.Rows.Height = 10;
            SoldOutTable.Format.Font.Size = 7;

            Column SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            Row SoldOutTableRows = SoldOutTable.AddRow();

            SoldOutTableRows.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            SoldOutTableRows.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            SoldOutTableRows.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            SoldOutTableRows.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            SoldOutTableRows.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            SoldOutTableRows.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            SoldOutTableRows.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            SoldOutTableRows.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            SoldOutTableRows.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            SoldOutTableRows.Cells[7].AddParagraph("Count   ").Format = formatBold.Clone();
            SoldOutTableRows.Cells[7].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            SoldOutTableRows.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;



            var h = (from s in context.tblShifts
                     where s.StoreId == pdfStoreId && s.Date == pdfDate.Date && s.ShiftID == pdfShiftId
                     select s).ToList().FirstOrDefault();

            if (h.EndTime == null)
            {
                soldOutObj = new SoldOut_TicketInfo();
                soldOutObj.Store_Id = pdfStoreId;
                soldOutObj.ShiftID = 0;
            }
            else
            {
                soldOutObj = new SoldOut_TicketInfo();
                soldOutObj.Store_Id = pdfStoreId;
                soldOutObj.Created_Date = pdfDate;
                soldOutObj.EmployeeID = pdfEmpId;
                soldOutObj.ShiftID = pdfShiftId;
                soldOutObj.CloseTime = h.EndTime;
            }

            NewGetSoldOutHistory(soldOutObj);

            for (int i = 0; i < LotteryHistory.Count; i++)
            {
                Row row1 = SoldOutTable.AddRow();
                row1.Cells[0].AddParagraph(LotteryHistory[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(LotteryHistory[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(LotteryHistory[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(LotteryHistory[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(LotteryHistory[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(LotteryHistory[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(LotteryHistory[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(LotteryHistory[i].No_of_Tickets_Sold.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(LotteryHistory[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            }

            // section = list of return ticket

            Paragraph space11 = section.AddParagraph();
            Paragraph ListReturn = section.AddParagraph("C. List Of Return Ticket");
            ListReturn.Format.Font.Size = 7;
            ListReturn.Format.Alignment = ParagraphAlignment.Left;

            Paragraph space12 = section.AddParagraph();

            var ReturnTable = section.AddTable();
            ReturnTable.Style = "Table";
            ReturnTable.Borders.Width = 0.25;
            ReturnTable.Borders.Color = Colors.Black;
            ReturnTable.Rows.Height = 10;
            ReturnTable.Format.Font.Size = 7;

            Column ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            Row ReturnTableRows = ReturnTable.AddRow();

            ReturnTableRows.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            ReturnTableRows.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            ReturnTableRows.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            ReturnTableRows.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            ReturnTableRows.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            ReturnTableRows.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            ReturnTableRows.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            ReturnTableRows.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            ReturnTableRows.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            ReturnTableRows.Cells[7].AddParagraph("Count   ").Format = formatBold.Clone();
            ReturnTableRows.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            ReturnTableRows.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;



            var j = (from s in context.tblShifts
                     where s.StoreId == pdfStoreId && s.Date == pdfDate.Date && s.ShiftID == pdfShiftId
                     select s).ToList().FirstOrDefault();

            if (j.EndTime == null)
            {
                ReturnObj = new Return_TicketInfo();
                ReturnObj.Store_Id = pdfStoreId;
                ReturnObj.ShiftID = 0;
            }
            else
            {
                ReturnObj = new Return_TicketInfo();
                ReturnObj.Store_Id = pdfStoreId;
                ReturnObj.Created_Date = pdfDate;
                ReturnObj.EmployeeID = pdfEmpId;
                ReturnObj.ShiftID = pdfShiftId;
                ReturnObj.CloseTime = j.EndTime;
            }

            NewGetReturnHistory(ReturnObj);

            for (int i = 0; i < LotteryHistoryReturn.Count; i++)
            {
                Row row1 = ReturnTable.AddRow();
                row1.Cells[0].AddParagraph(LotteryHistoryReturn[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(LotteryHistoryReturn[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(LotteryHistoryReturn[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(LotteryHistoryReturn[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(LotteryHistoryReturn[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(LotteryHistoryReturn[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(LotteryHistoryReturn[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(LotteryHistoryReturn[i].Count.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(LotteryHistoryReturn[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            }


            var shift = (from s in context.tblShifts where s.StoreId == pdfStoreId && s.Date == pdfDate select s).ToList();

            if (ShiftReportGenerate == false)
            {
                if (shiftcount != shift.Count)
                {
                    int t = shift[shiftcount].ShiftID;
                    var terminal = (from s in context.tblTerminal_Data1
                                    where s.Store_Id == pdfStoreId && s.Date == pdfDate && s.ShiftID == t
                                    select s).ToList();
                    if (terminal.Count != 0)
                    {
                        count = count + 1;
                        ShiftReportAddHeaderToReport(section, formatBold, formatNormal, count);
                    }
                }
            }
            //else
            //{
            //    var terminal = (from s in context.tblTerminal_Data1
            //                    where s.Store_Id == pdfStoreId && s.Date == pdfDate && s.ShiftID == pdfShiftId && s.Employee_Id == pdfEmpId
            //                    select s).ToList();
            //    count = pdfShiftId;
            //    ShiftReportAddHeaderToReport(section, formatBold, formatNormal, count);
            //}



        }
        private void ShiftReportgetHeaderDetails(int pdfStoreId, DateTime pdfDate)
        {
            context = new LotteryBlankDatabaseEntities();
            if (ShiftReportGenerate == false)
            {
                var shift = (from s in context.tblShifts where s.StoreId == pdfStoreId && s.Date == pdfDate.Date select s).ToList();
                shiftcount = shiftcount + 1;
                for (int i = shiftcount; i == shiftcount; i++)
                {
                    int j = i - 1;

                    pdfEndTime = shift[j].EndTime;

                    pdfEmpId = Convert.ToInt32(shift[j].EmployeeId);
                    pdfShiftId = shift[j].ShiftID;
                    var empDet = (from s in context.tblEmployee_Details where s.EmployeeId == pdfEmpId select s).ToList().FirstOrDefault();
                    pdfEmpName = empDet.EmployeeName;
                }
            }
            else
            {
                var shift = (from s in context.tblShifts where s.StoreId == pdfStoreId && s.EmployeeId == pdfEmpId && s.ShiftID == pdfShiftId && s.Date == pdfDate.Date select s).ToList().FirstOrDefault();
                pdfEndTime = shift.EndTime;
                var empDet = (from s in context.tblEmployee_Details where s.EmployeeId == pdfEmpId select s).ToList().FirstOrDefault();
                pdfEmpName = empDet.EmployeeName;
            }

            //temp++;


            var locDet = (from s in context.tblStore_Info where s.Store_Id == pdfStoreId select s).ToList().FirstOrDefault();
            pdfLoc = locDet.Store_Address;
        }
        [Route("api/CloseShift/NewGetShiftActivateHistory")]
        public HttpResponseMessage NewGetShiftActivateHistory([FromBody] Activate_Ticket data)
        {
            LotteryHistoryActivate = new ObservableCollection<Activate_Ticket>();
            context = new LotteryBlankDatabaseEntities();
            if (data.ShiftID == 0)
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                sid = Shift.ShiftID;
                sdate = Convert.ToDateTime(Shift.Date);
            }
            else
            {
                var x = (from s in context.tblShifts
                         where s.StoreId == data.Store_Id && s.Date == data.Created_Date
                         && s.EndTime == data.CloseTime
                         select s).ToList().FirstOrDefault();


                sid = x.ShiftID;
                sdate = data.Created_Date;
            }

            var closeticket = (from s in context.tblClose_Box
                               where s.Store_Id == data.Store_Id && s.ShiftID == sid &&
                               s.Created_On == sdate
                               select s).ToList();
            var soldoutticket = (from s in context.tblSoldouts
                                 where s.Store_Id == data.Store_Id && s.ShiftID == sid
                                 && s.Created_On == sdate
                                 select s).ToList();
            var returnticket = (from s in context.tblReturntickets
                                where s.Store_Id == data.Store_Id && s.ShiftID == sid &&
                                 s.Created_On == sdate
                                select s).ToList();

            foreach (var v in closeticket)
            {
                LotteryHistoryActivate.Add(new Activate_Ticket
                {
                    Game_Id = v.Game_Id,
                    Box_No = v.Box_No,
                    Created_Date = Convert.ToDateTime(v.Created_On),
                    Packet_No = v.Packet_Id,
                    Ticket_Name = v.Ticket_Name,
                    Price = Convert.ToInt32(v.Price),
                    Start_No = v.Start_No,
                    End_No = v.End_No,
                    Status = v.Status,
                    // Stopped_At = v.Close_At,
                    EmployeeID = v.EmployeeId,
                    State = v.State,
                    ShiftID = v.ShiftID,
                    Count = (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1).ToString(),
                    Total_Price = Convert.ToInt32(v.Price) * (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1)
                });
            }

            foreach (var i in soldoutticket)
            {
                if (i.Partial_Packet != "Y")
                {
                    LotteryHistoryActivate.Add(new Activate_Ticket
                    {
                        Game_Id = i.Game_Id,
                        Box_No = i.Box_No,
                        Created_Date = Convert.ToDateTime(i.Created_On),
                        Packet_No = i.Packet_Id,
                        Ticket_Name = i.Ticket_Name,
                        Price = Convert.ToInt32(i.Price),
                        Start_No = i.PackPosition_Open,
                        End_No = i.PackPosition_Close,
                        Status = i.Status,
                        //Stopped_At = i.Close_At,
                        EmployeeID = i.EmployeeId,
                        State = i.State,
                        ShiftID = i.ShiftID,
                        Count = (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1).ToString(),
                        Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1)
                    });
                }
            }

            foreach (var i in soldoutticket)
            {
                if (i.Partial_Packet == "Y")
                {
                    var recieved = (from s in context.tblRecievedTickets
                                    where s.Store_Id == i.Store_Id && s.Game_Id == i.Game_Id && s.Packet_Id == i.Packet_Id
                                    select s).FirstOrDefault();
                    var activated = (from s in context.tblActivated_Tickets
                                     where s.Store_Id == i.Store_Id && s.Game_Id == i.Game_Id && s.Packet_Id == i.Packet_Id
                                     select s).FirstOrDefault();
                    if (recieved != null)
                    {
                        LotteryHistoryActivate.Add(new Activate_Ticket
                        {
                            Game_Id = i.Game_Id,
                            Box_No = i.Box_No,
                            Created_Date = Convert.ToDateTime(i.Created_On),
                            Packet_No = i.Packet_Id,
                            Ticket_Name = i.Ticket_Name,
                            Price = Convert.ToInt32(i.Price),
                            Start_No = i.PackPosition_Open,
                            End_No = recieved.End_No,
                            Status = i.Status,
                            //Stopped_At = i.Close_At,
                            EmployeeID = i.EmployeeId,
                            State = i.State,
                            ShiftID = i.ShiftID,
                            Count = (int.Parse(recieved.End_No) - int.Parse(i.PackPosition_Open) + 1).ToString(),
                            Total_Price = Convert.ToInt32(i.Price) * (int.Parse(recieved.End_No) - int.Parse(i.PackPosition_Open) + 1)
                        });
                    }
                    else if (activated != null)
                    {
                        LotteryHistoryActivate.Add(new Activate_Ticket
                        {
                            Game_Id = i.Game_Id,
                            Box_No = i.Box_No,
                            Created_Date = Convert.ToDateTime(i.Created_On),
                            Packet_No = i.Packet_Id,
                            Ticket_Name = i.Ticket_Name,
                            Price = Convert.ToInt32(i.Price),
                            Start_No = i.PackPosition_Open,
                            End_No = activated.End_No,
                            Status = i.Status,
                            //Stopped_At = i.Close_At,
                            EmployeeID = i.EmployeeId,
                            State = i.State,
                            ShiftID = i.ShiftID,
                            Count = (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1).ToString(),
                            Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1)
                        });
                    }
                    else
                    {
                        LotteryHistoryActivate.Add(new Activate_Ticket
                        {
                            Game_Id = i.Game_Id,
                            Box_No = i.Box_No,
                            Created_Date = Convert.ToDateTime(i.Created_On),
                            Packet_No = i.Packet_Id,
                            Ticket_Name = i.Ticket_Name,
                            Price = Convert.ToInt32(i.Price),
                            Start_No = i.PackPosition_Open,
                            End_No = i.PackPosition_Close,
                            Status = i.Status,
                            //Stopped_At = i.Close_At,
                            EmployeeID = i.EmployeeId,
                            State = i.State,
                            ShiftID = i.ShiftID,
                            Count = (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1).ToString(),
                            Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.PackPosition_Close) - int.Parse(i.PackPosition_Open) + 1)
                        });
                    }

                }
            }

            foreach (var j in returnticket)
            {
                if (j.Box_No != 0)
                {
                    var soldoutpartial = (from s in context.tblSoldouts
                                          where s.Game_Id == j.Game_Id && s.Packet_Id == j.Packet_Id && s.Store_Id == j.Store_Id && s.ShiftID == sid
                                          && s.Created_On == sdate
                                          select s).ToList().FirstOrDefault();
                    if (soldoutpartial == null)
                    {
                        LotteryHistoryActivate.Add(new Activate_Ticket
                        {
                            Game_Id = j.Game_Id,
                            Box_No = j.Box_No,
                            Created_Date = Convert.ToDateTime(j.Created_On),
                            Packet_No = j.Packet_Id,
                            Ticket_Name = j.Ticket_Name,
                            Price = Convert.ToInt32(j.Price),
                            Start_No = j.PackPosition_Open,
                            End_No = j.PackPosition_Close,
                            Status = j.Status,
                            //Stopped_At = i.Close_At,
                            EmployeeID = j.EmplyeeeId,
                            State = j.State,
                            ShiftID = j.ShiftID,
                            Count = (int.Parse(j.PackPosition_Close) - int.Parse(j.PackPosition_Open) + 1).ToString(),
                            Total_Price = Convert.ToInt32(j.Price) * (int.Parse(j.PackPosition_Close) - int.Parse(j.PackPosition_Open) + 1)
                        });
                    }
                }

            }
            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistoryActivate);
        }

        [Route("api/CloseShift/NewGetSoldOutHistory")]
        public HttpResponseMessage NewGetSoldOutHistory([FromBody] SoldOut_TicketInfo data)
        {

            LotteryHistory = new ObservableCollection<SoldOut_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            if (data.ShiftID == 0)
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

                var result = (from s in context.tblSoldouts
                              where s.Status == "SoldOut" && s.Store_Id == data.Store_Id
                              select s).ToList();
                foreach (var v in result)
                {
                    if (v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
                    {
                        LotteryHistory.Add(new SoldOut_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Ticket_Name = v.Ticket_Name,
                            Price = Convert.ToInt32(v.Price),
                            Box_No = Convert.ToInt16(v.Box_No),
                            Status = v.Status,
                            Store_Id = v.Store_Id,
                            ShiftID = v.ShiftID,
                            No_of_Tickets_Sold = v.Total_Tickets,
                            EmployeeID = v.EmployeeId,
                            Modified_Date = Convert.ToDateTime(v.Modified_On),
                            End_No = v.PackPosition_Close,
                            Start_No = v.PackPosition_Open,
                            Partial_Packet = v.Partial_Packet,
                            Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
                        });
                    }
                    else if (Shift.EndTime == null && v.Created_On == Shift.Date && v.ShiftID == Shift.ShiftID)
                    {
                        LotteryHistory.Add(new SoldOut_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Ticket_Name = v.Ticket_Name,
                            Price = Convert.ToInt32(v.Price),
                            Box_No = Convert.ToInt16(v.Box_No),
                            Status = v.Status,
                            Store_Id = v.Store_Id,
                            ShiftID = v.ShiftID,
                            No_of_Tickets_Sold = v.Total_Tickets,
                            EmployeeID = v.EmployeeId,
                            Modified_Date = Convert.ToDateTime(v.Modified_On),
                            End_No = v.PackPosition_Close,
                            Start_No = v.PackPosition_Open,
                            Partial_Packet = v.Partial_Packet,
                            Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
                        });
                    }
                }


            }

            else
            {

                var x = (from s in context.tblShifts
                         where s.StoreId == data.Store_Id && s.Date == data.Created_Date
&& s.EndTime == data.CloseTime
                         select s).ToList();

                foreach (var r in x)
                {
                    var result = (from s in context.tblSoldouts
                                  where s.Status == "SoldOut" && s.Store_Id == data.Store_Id && s.ShiftID == r.ShiftID
                                  && s.Created_On == data.Created_Date
                                  select s).ToList();
                    foreach (var v in result)
                    {
                        //if (v.ShiftID == data.ShiftID && v.Created_On == data.Created_Date)
                        //{
                        LotteryHistory.Add(new SoldOut_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Ticket_Name = v.Ticket_Name,
                            Price = Convert.ToInt32(v.Price),
                            Box_No = Convert.ToInt16(v.Box_No),
                            Status = v.Status,
                            Store_Id = v.Store_Id,
                            ShiftID = v.ShiftID,
                            No_of_Tickets_Sold = v.Total_Tickets,
                            EmployeeID = v.EmployeeId,
                            Modified_Date = Convert.ToDateTime(v.Modified_On),
                            End_No = v.PackPosition_Close,
                            Start_No = v.PackPosition_Open,
                            Partial_Packet = v.Partial_Packet,
                            Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
                        });
                        //}
                    }
                }


            }

            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        }

        [Route("api/CloseShift/NewGetReturnHistory")]
        public HttpResponseMessage NewGetReturnHistory([FromBody] Return_TicketInfo data)
        {
            LotteryHistoryReturn = new ObservableCollection<Return_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            if (data.ShiftID == 0)
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

                var result = (from s in context.tblReturntickets where s.Status == "Return" && s.Store_Id == data.Store_Id select s).ToList();
                if (Shift != null)
                {
                    foreach (var v in result)
                    {
                        if (v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
                        {
                            LotteryHistoryReturn.Add(new Return_TicketInfo
                            {
                                Game_Id = v.Game_Id,
                                Created_Date = Convert.ToDateTime(v.Created_On),
                                Packet_No = v.Packet_Id,
                                Box_No = Convert.ToInt32(v.Box_No),
                                Ticket_Name = v.Ticket_Name,
                                Price = v.Price,
                                Store_Id = v.Store_Id,
                                Start_No = v.PackPosition_Open,
                                End_No = v.PackPosition_Close,
                                Return_At = v.Return_At,
                                EmployeeID = v.EmplyeeeId,
                                ShiftID = v.ShiftID,
                                Count = v.Count,
                                Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
                            });
                        }
                        else if (Shift.EndTime == null && v.ShiftID == Shift.ShiftID && v.Created_On == Shift.Date)
                        {
                            LotteryHistoryReturn.Add(new Return_TicketInfo
                            {
                                Game_Id = v.Game_Id,
                                Created_Date = Convert.ToDateTime(v.Created_On),
                                Packet_No = v.Packet_Id,
                                Box_No = Convert.ToInt32(v.Box_No),
                                Ticket_Name = v.Ticket_Name,
                                Price = v.Price,
                                Store_Id = v.Store_Id,
                                Start_No = v.PackPosition_Open,
                                End_No = v.PackPosition_Close,
                                Return_At = v.Return_At,
                                EmployeeID = v.EmplyeeeId,
                                ShiftID = v.ShiftID,
                                Count = v.Count,
                                Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
                            });
                        }
                    }
                }
            }

            else
            {
                var x = (from s in context.tblShifts
                         where s.StoreId == data.Store_Id && s.Date == data.Created_Date
                         && s.EndTime == data.CloseTime
                         select s).ToList();

                foreach (var r in x)
                {
                    var result = (from s in context.tblReturntickets
                                  where s.Status == "Return" && s.Store_Id == data.Store_Id && s.ShiftID == r.ShiftID
                                  && s.Created_On == data.Created_Date
                                  select s).ToList();

                    foreach (var v in result)
                    {
                        //if (v.ShiftID == data.ShiftID && v.Created_On == data.Created_Date)
                        //{
                        LotteryHistoryReturn.Add(new Return_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Box_No = Convert.ToInt32(v.Box_No),
                            Ticket_Name = v.Ticket_Name,
                            Price = v.Price,
                            Store_Id = v.Store_Id,
                            Start_No = v.PackPosition_Open,
                            End_No = v.PackPosition_Close,
                            Return_At = v.Return_At,
                            EmployeeID = v.EmplyeeeId,
                            ShiftID = v.ShiftID,
                            Count = v.Count,
                            Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
                        });
                        //}

                    }
                }



            }
            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistoryReturn);
        }

        public Document CreateDocument()
        {
            var document = new MigraDoc.DocumentObjectModel.Document();
            document.Info.Title = "Daily Report";
            //document.Info.Author = "Yogesh";

            DefineStyles(document);

            CreateReportPage(document);

            return document;
        }
        private void CreateReportPage(Document document)
        {
            try
            {

                MigraDoc.DocumentObjectModel.Section section = document.AddSection();

                section.PageSetup.PageFormat = PageFormat.A4;
                section.PageSetup.TopMargin = "1.5cm";
                section.PageSetup.LeftMargin = "1cm";

                ParagraphFormat formatBold = new ParagraphFormat();
                formatBold.Font.Size = 7;
                formatBold.Font.Bold = true;

                ParagraphFormat formatNormal = new ParagraphFormat();
                formatNormal.Font.Size = 7;
                formatNormal.Font.Bold = false;

                #region SECTION : «« Header »»

                AddHeaderToReport(section, formatBold, formatNormal);

                AddLineSpace(section, TabLeader.Spaces);

                formatNormal.Font.Size = 7;
                AddHistoryEventsTableToReport(section, formatBold, formatNormal);

                #endregion

                #region "Footer" section

                //AddFooterToReport(section);

                #endregion



            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void AddHistoryEventsTableToReport(Section section, ParagraphFormat formatBold, ParagraphFormat formatNormal)
        {
            totPrice = 0;
            Paragraph paragraph = section.AddParagraph();
            paragraph.AddText("1. Activity During Shift");
            paragraph.Format.Font.Size = 8;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Left;

            //section.AddParagraph("1. Activity During Shift").Format.Alignment = ParagraphAlignment.Left;

            var eventTable = section.AddTable();
            eventTable.Style = "Table";
            eventTable.Borders.Width = 0.25;
            eventTable.Borders.Color = Colors.Black;
            eventTable.Rows.Height = 10;
            eventTable.Format.Font.Size = 7;

            //eventTable.Borders.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;

            Column columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            columnGroup = eventTable.AddColumn("2cm");

            Row headers = eventTable.AddRow();

            headers.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            headers.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            headers.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            headers.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            headers.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            headers.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            headers.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            headers.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            headers.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            headers.Cells[7].AddParagraph("Count").Format = formatBold.Clone();
            headers.Cells[7].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            headers.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            headers.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            headers.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            headers.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            headers.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            activeObj = new Activate_Ticket();
            activeObj.Store_Id = pdfStoreId;
            activeObj.Created_Date = pdfDate;
            activeObj.EmployeeID = 0;
            NewGetDailyReport(activeObj);

            for (int i = 0; i < SoldOutHistory.Count; i++)
            {
                Row row1 = eventTable.AddRow();
                row1.Cells[0].AddParagraph(SoldOutHistory[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(SoldOutHistory[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(SoldOutHistory[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(SoldOutHistory[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(SoldOutHistory[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(SoldOutHistory[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(SoldOutHistory[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(SoldOutHistory[i].Count.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(SoldOutHistory[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;

                totPrice = totPrice + Convert.ToInt32(SoldOutHistory[i].Total_Price);
            }

            Paragraph paragraph1 = section.AddParagraph();
            paragraph1.AddText(@"Total Amount  :  " + totPrice);
            paragraph1.Format.Font.Size = 7;
            paragraph1.Format.Font.Bold = true;
            paragraph1.Format.Alignment = ParagraphAlignment.Right;


            Table scratchSell = section.AddTable();
            scratchSell.Borders.Width = 0.5;

            Column col1 = scratchSell.AddColumn("4cm");
            col1.Format.Alignment = ParagraphAlignment.Left;

            Column col2 = scratchSell.AddColumn("4cm");
            col1.Format.Alignment = ParagraphAlignment.Center;

            Row row7 = scratchSell.AddRow();
            row7.HeightRule = RowHeightRule.Exactly;
            row7.Height = 12;
            row7.Borders.Bottom.Visible = false;
            row7.TopPadding = 2;

            Row row2 = scratchSell.AddRow();
            row2.HeightRule = RowHeightRule.Exactly;
            row2.Height = 10;
            row2.Borders.Top.Visible = false;
            row2.Borders.Bottom.Visible = false;

            Row row3 = scratchSell.AddRow();
            row3.HeightRule = RowHeightRule.Exactly;
            row3.Height = 10;
            row3.Borders.Top.Visible = false;
            row3.Borders.Bottom.Visible = false;

            Row row4 = scratchSell.AddRow();
            row4.HeightRule = RowHeightRule.Exactly;
            row4.Height = 10;
            row4.Borders.Top.Visible = false;
            row4.Borders.Bottom.Visible = false;

            Row row5 = scratchSell.AddRow();
            row5.HeightRule = RowHeightRule.Exactly;
            row5.Height = 10;
            row5.Borders.Top.Visible = false;
            row5.Borders.Bottom.Visible = false;

            Row row6 = scratchSell.AddRow();
            row6.HeightRule = RowHeightRule.Exactly;
            row6.Height = 10;
            row6.Borders.Top.Visible = false;
            row6.BottomPadding = 6;

            terminalClsObj = new Terminal_Details();
            terminalClsObj.Store_Id = pdfStoreId;
            terminalClsObj.Date = pdfDate;
            terminalClsObj.ShiftID = 0;
            NewGetTerminalDataHistory(terminalClsObj);

            int scrs = 0;
            int cre = 0;
            int onls = 0;
            int deb = 0;
            int scrp = 0;
            int topup = 0;
            int onlp = 0;
            int topc = 0;
            int loan = 0;
            int cashonh = 0;
            int short1 = 0;
            int over = 0;
            int totsells = 0;
            int totpayout = 0;
            int netCash = 0;
            // int shortOver = 0;
            int totStock = 0;
            int totActive = 0;

            for (int i = 0; i < GetTerminalDataCollection.Count; i++)
            {
                scrs = scrs + Convert.ToInt32(GetTerminalDataCollection[i].ScratchSells);
                cre = cre + Convert.ToInt32(GetTerminalDataCollection[i].Credit);
                onls = onls + Convert.ToInt32(GetTerminalDataCollection[i].OnlineSells);
                deb = deb + Convert.ToInt32(GetTerminalDataCollection[i].Debit);
                scrp = scrp + Convert.ToInt32(GetTerminalDataCollection[i].ScratchPayout);
                topup = topup + Convert.ToInt32(GetTerminalDataCollection[i].TopUp);
                onlp = onlp + Convert.ToInt32(GetTerminalDataCollection[i].OnlinePayout);
                topc = topc + Convert.ToInt32(GetTerminalDataCollection[i].TopUPCancel);
                loan = loan + Convert.ToInt32(GetTerminalDataCollection[i].Loan);
                cashonh = cashonh + Convert.ToInt32(GetTerminalDataCollection[i].CashOnHand);
                totsells = totsells + Convert.ToInt32(GetTerminalDataCollection[i].TotalSells);
                totpayout = totpayout + Convert.ToInt32(GetTerminalDataCollection[i].TotalPayout);
                netCash = netCash + Convert.ToInt32(GetTerminalDataCollection[i].NetCash);
                // shortOver = shortOver + Convert.ToInt32(GetTerminalDataCollection[i].ShortOver);
                short1 = short1 + Convert.ToInt32(GetTerminalDataCollection[i].Short1);
                over = over + Convert.ToInt32(GetTerminalDataCollection[i].Over);

                totStock = totStock + Convert.ToInt32(GetTerminalDataCollection[i].TotalStockInventory);
                totActive = totActive + Convert.ToInt32(GetTerminalDataCollection[i].TotalActiveInventory);

            }

            Paragraph scratch = row7.Cells[0].AddParagraph("SCRATCH SELLS  :  " + scrs);
            scratch.Format.LineSpacing = Unit.FromMillimeter(3);
            row7.Cells[0].Format.Font.Size = 7;
            row7.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row7.Cells[0].Borders.Right.Visible = false;

            row7.Cells[1].AddParagraph(@" CREDIT  :  " + cre);
            row7.Cells[1].Format.Font.Size = 7;
            row7.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row7.Cells[1].Borders.Left.Visible = false;

            row2.Cells[0].AddParagraph("ONLINE SELLS  :  " + onls);
            row2.Cells[0].Format.Font.Size = 7;
            row2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[0].Borders.Right.Visible = false;

            row2.Cells[1].AddParagraph(@" DEBIT  :  " + deb);
            row2.Cells[1].Format.Font.Size = 7;
            row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[1].Borders.Left.Visible = false;

            row3.Cells[0].AddParagraph("SCRATCH PAYOUT  :  " + scrp);
            row3.Cells[0].Format.Font.Size = 7;
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[0].Borders.Right.Visible = false;

            row3.Cells[1].AddParagraph(@" TOP UP  :  " + topup);
            row3.Cells[1].Format.Font.Size = 7;
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[1].Borders.Left.Visible = false;

            row4.Cells[0].AddParagraph("ONLINE PAYOUT  :  " + onlp);
            row4.Cells[0].Format.Font.Size = 7;
            row4.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[0].Borders.Right.Visible = false;

            row4.Cells[1].AddParagraph(@" TOP UP CANCEL  :  " + topc);
            row4.Cells[1].Format.Font.Size = 7;
            row4.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[1].Borders.Left.Visible = false;



            row5.Cells[0].AddParagraph("LOAN  :  " + loan);
            row5.Cells[0].Format.Font.Size = 7;
            row5.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row5.Cells[0].Borders.Right.Visible = false;
            row5.Cells[1].Borders.Left.Visible = false;

            row6.Cells[0].AddParagraph("CASH ON HAND  :  " + cashonh);
            row6.Cells[0].Format.Font.Size = 7;
            row6.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row6.Cells[0].Borders.Right.Visible = false;

            row6.Cells[1].Borders.Left.Visible = false;

            //    Sell Summery Section

            Paragraph space = section.AddParagraph();
            Paragraph p2 = section.AddParagraph();
            p2.AddText(" 2. Sell Summery");
            p2.Format.Font.Size = 8;
            p2.Format.Font.Bold = true;
            p2.Format.Alignment = ParagraphAlignment.Left;

            Table sellSummerytab = section.AddTable();
            sellSummerytab.Format.Font.Size = 7;
            sellSummerytab.Format.Alignment = ParagraphAlignment.Left;
            sellSummerytab.Borders.Width = 0.5;
            sellSummerytab.Rows.Height = 10;

            Column sellSummeryDetFirstCol = sellSummerytab.AddColumn("3cm");

            for (int i = 1; i < (GetTerminalDataCollection.Count + 2); i++)
            {
                Column sellSummeryDetCol = sellSummerytab.AddColumn("2cm");
            }

            Row shiftRow = sellSummerytab.AddRow();
            Row scratchSellsRow = sellSummerytab.AddRow();
            Row OnlineSellsRow = sellSummerytab.AddRow();
            Row TotalSellsRow = sellSummerytab.AddRow();
            Row ScratchOfPayoutRow = sellSummerytab.AddRow();
            Row OnlinePayoutRow = sellSummerytab.AddRow();
            Row TotalPayoutRow = sellSummerytab.AddRow();
            Row LoanRow = sellSummerytab.AddRow();

            shiftRow.Cells[0].Borders.Right.Visible = false;
            shiftRow.Cells[0].Borders.Bottom.Visible = false;

            scratchSellsRow.Cells[0].AddParagraph("Scratch Off Sells");
            scratchSellsRow.Cells[0].Borders.Right.Visible = false;
            scratchSellsRow.Cells[0].Borders.Bottom.Visible = false;

            OnlineSellsRow.Cells[0].AddParagraph("Online Sells");
            OnlineSellsRow.Cells[0].Borders.Right.Visible = false;
            OnlineSellsRow.Cells[0].Borders.Bottom.Visible = false;

            TotalSellsRow.Cells[0].AddParagraph("Total Sells");
            TotalSellsRow.Cells[0].Borders.Right.Visible = false;
            TotalSellsRow.Cells[0].Borders.Bottom.Visible = false;

            ScratchOfPayoutRow.Cells[0].AddParagraph("Scratch of Payout");
            ScratchOfPayoutRow.Cells[0].Borders.Right.Visible = false;
            ScratchOfPayoutRow.Cells[0].Borders.Bottom.Visible = false;

            OnlinePayoutRow.Cells[0].AddParagraph("Online Payout");
            OnlinePayoutRow.Cells[0].Borders.Right.Visible = false;
            OnlinePayoutRow.Cells[0].Borders.Bottom.Visible = false;

            TotalPayoutRow.Cells[0].AddParagraph("Total Payout");
            TotalPayoutRow.Cells[0].Borders.Right.Visible = false;
            TotalPayoutRow.Cells[0].Borders.Bottom.Visible = false;

            LoanRow.Cells[0].AddParagraph("Loan");
            LoanRow.Cells[0].Borders.Right.Visible = false;


            for (int i = 0, j = 1; i < GetTerminalDataCollection.Count; i++, j++)
            {
                shiftRow.Cells[j].AddParagraph("Shift" + GetTerminalDataCollection[i].ShiftID);
                shiftRow.Cells[j].Borders.Left.Visible = false;
                shiftRow.Cells[j].Borders.Right.Visible = false;
                shiftRow.Cells[j].Borders.Bottom.Visible = false;

                scratchSellsRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].ScratchSells);
                scratchSellsRow.Cells[j].Borders.Visible = false;

                OnlineSellsRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].OnlineSells);
                OnlineSellsRow.Cells[j].Borders.Visible = false;

                TotalSellsRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].TotalSells);
                TotalSellsRow.Cells[j].Borders.Visible = false;

                ScratchOfPayoutRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].ScratchPayout);
                ScratchOfPayoutRow.Cells[j].Borders.Visible = false;

                OnlinePayoutRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].OnlinePayout);
                OnlinePayoutRow.Cells[j].Borders.Visible = false;

                TotalPayoutRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].TotalPayout);
                TotalPayoutRow.Cells[j].Borders.Visible = false;

                LoanRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].Loan);
                LoanRow.Cells[j].Borders.Top.Visible = false;
                LoanRow.Cells[j].Borders.Left.Visible = false;
                LoanRow.Cells[j].Borders.Right.Visible = false;
            }


            shiftRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph("DAILY TOTAL");
            shiftRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            shiftRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            scratchSellsRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(scrs.ToString());
            scratchSellsRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            scratchSellsRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            OnlineSellsRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(onls.ToString());
            OnlineSellsRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            OnlineSellsRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            TotalSellsRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(totsells.ToString());
            TotalSellsRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            TotalSellsRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            ScratchOfPayoutRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(scrp.ToString());
            ScratchOfPayoutRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            ScratchOfPayoutRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            OnlinePayoutRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(onlp.ToString());
            OnlinePayoutRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            OnlinePayoutRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            TotalPayoutRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(totpayout.ToString());
            TotalPayoutRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            TotalPayoutRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            LoanRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(loan.ToString());
            LoanRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;


            // section I hope card details - I

            Paragraph space1 = section.AddParagraph();

            Table cardDetTableFirst = section.AddTable();

            cardDetTableFirst.Format.Font.Size = 7;
            cardDetTableFirst.Format.Alignment = ParagraphAlignment.Left;
            cardDetTableFirst.Borders.Width = 0.5;
            cardDetTableFirst.Rows.Height = 10;


            Column cardDetTableFirstCol = cardDetTableFirst.AddColumn("3cm");

            for (int i = 1; i < (GetTerminalDataCollection.Count + 2); i++)
            {
                Column cardDetTableCol = cardDetTableFirst.AddColumn("2cm");
            }

            Row CardDetRow = cardDetTableFirst.AddRow();
            Row CardDetShiftRow = cardDetTableFirst.AddRow();
            Row CardDetCreditRow = cardDetTableFirst.AddRow();
            Row CardDetDebitRow = cardDetTableFirst.AddRow();
            Row CardDetTopupRow = cardDetTableFirst.AddRow();
            Row CardDetTopupCRow = cardDetTableFirst.AddRow();

            CardDetRow.Cells[0].AddParagraph("I HOPE CARD DETAILS");
            CardDetRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetRow.Cells[0].Borders.Right.Visible = false;

            for (int i = 1; i < (GetTerminalDataCollection.Count + 2); i++)
            {
                CardDetRow.Cells[i].Borders.Left.Visible = false;
                CardDetRow.Cells[i].Borders.Right.Visible = false;
                CardDetRow.Cells[i].Borders.Bottom.Visible = false;
            }
            CardDetRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Right.Visible = true;

            CardDetShiftRow.Cells[0].Borders.Right.Visible = false;
            CardDetShiftRow.Cells[0].Borders.Bottom.Visible = false;

            CardDetCreditRow.Cells[0].AddParagraph("Credit");
            CardDetCreditRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetCreditRow.Cells[0].Borders.Right.Visible = false;

            CardDetDebitRow.Cells[0].AddParagraph("Debit");
            CardDetDebitRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetDebitRow.Cells[0].Borders.Right.Visible = false;

            CardDetTopupRow.Cells[0].AddParagraph("Topup");
            CardDetTopupRow.Cells[0].Borders.Bottom.Visible = false;
            CardDetTopupRow.Cells[0].Borders.Right.Visible = false;

            CardDetTopupCRow.Cells[0].AddParagraph("Topup Cancel");
            CardDetTopupCRow.Cells[0].Borders.Right.Visible = false;

            for (int i = 0, j = 1; i < GetTerminalDataCollection.Count; i++, j++)
            {
                CardDetShiftRow.Cells[j].AddParagraph("Shift " + GetTerminalDataCollection[i].ShiftID);
                CardDetShiftRow.Cells[j].Borders.Visible = false;

                CardDetCreditRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].Credit.ToString());
                CardDetCreditRow.Cells[j].Borders.Visible = false;

                CardDetDebitRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].Debit.ToString());
                CardDetDebitRow.Cells[j].Borders.Visible = false;

                CardDetTopupRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].TopUp.ToString());
                CardDetTopupRow.Cells[j].Borders.Visible = false;

                CardDetTopupCRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].TopUPCancel.ToString());
                CardDetTopupCRow.Cells[j].Borders.Left.Visible = false;
                CardDetTopupCRow.Cells[j].Borders.Right.Visible = false;
                CardDetTopupCRow.Cells[j].Borders.Top.Visible = false;
            }

            CardDetShiftRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph("DAILY TOTAL");
            CardDetShiftRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;
            CardDetShiftRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Top.Visible = false;
            CardDetShiftRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;


            CardDetCreditRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(cre.ToString());
            CardDetCreditRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;
            CardDetCreditRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;

            CardDetDebitRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(deb.ToString());
            CardDetDebitRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;
            CardDetDebitRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;

            CardDetTopupRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(topup.ToString());
            CardDetTopupRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;
            CardDetTopupRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;

            CardDetTopupCRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(topc.ToString());
            CardDetTopupCRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;

            // I Hope card detaild section - II

            Paragraph space2 = section.AddParagraph();

            Table cardDetTableSecond = section.AddTable();
            cardDetTableSecond.Format.Font.Size = 7;
            cardDetTableSecond.Format.Alignment = ParagraphAlignment.Left;
            cardDetTableSecond.Borders.Width = 0.5;
            cardDetTableSecond.Rows.Height = 10;

            Column cardDetTableSecondCol1 = cardDetTableSecond.AddColumn("3cm");

            for (int i = 1; i < (GetTerminalDataCollection.Count + 2); i++)
            {
                Column cardDetTableSecondCol = cardDetTableSecond.AddColumn("2cm");

            }

            Row cardDetTableSecondshiftRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondNetCashRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondCashOnHRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondShortRow = cardDetTableSecond.AddRow();
            Row cardDetTableSecondOverRow = cardDetTableSecond.AddRow();

            cardDetTableSecondshiftRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondshiftRow.Cells[0].Borders.Bottom.Visible = false;


            cardDetTableSecondNetCashRow.Cells[0].AddParagraph("Net Cash");
            cardDetTableSecondNetCashRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondNetCashRow.Cells[0].Borders.Bottom.Visible = false;

            cardDetTableSecondCashOnHRow.Cells[0].AddParagraph("Cash On Hand");
            cardDetTableSecondCashOnHRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondCashOnHRow.Cells[0].Borders.Bottom.Visible = false;

            cardDetTableSecondShortRow.Cells[0].AddParagraph("Short");
            cardDetTableSecondShortRow.Cells[0].Borders.Right.Visible = false;
            cardDetTableSecondShortRow.Cells[0].Borders.Bottom.Visible = false;

            cardDetTableSecondOverRow.Cells[0].AddParagraph("Over");
            cardDetTableSecondOverRow.Cells[0].Borders.Right.Visible = false;

            for (int i = 0, j = 1; i < GetTerminalDataCollection.Count; i++, j++)
            {
                cardDetTableSecondshiftRow.Cells[j].AddParagraph("Shift" + GetTerminalDataCollection[i].ShiftID);
                cardDetTableSecondshiftRow.Cells[j].Borders.Left.Visible = false;
                cardDetTableSecondshiftRow.Cells[j].Borders.Right.Visible = false;
                cardDetTableSecondshiftRow.Cells[j].Borders.Bottom.Visible = false;


                cardDetTableSecondNetCashRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].NetCash.ToString());
                cardDetTableSecondNetCashRow.Cells[j].Borders.Visible = false;

                cardDetTableSecondCashOnHRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].CashOnHand.ToString());
                cardDetTableSecondCashOnHRow.Cells[j].Borders.Visible = false;

                cardDetTableSecondShortRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].Short1.ToString());
                cardDetTableSecondShortRow.Cells[j].Borders.Visible = false;

                cardDetTableSecondOverRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].Over.ToString());
                cardDetTableSecondOverRow.Cells[j].Borders.Top.Visible = false;
                cardDetTableSecondOverRow.Cells[j].Borders.Left.Visible = false;
                cardDetTableSecondOverRow.Cells[j].Borders.Right.Visible = false;

            }

            cardDetTableSecondshiftRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph("DAILY TOTAL");
            cardDetTableSecondshiftRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            cardDetTableSecondshiftRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            cardDetTableSecondNetCashRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(netCash.ToString());
            cardDetTableSecondNetCashRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            cardDetTableSecondNetCashRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            cardDetTableSecondCashOnHRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(cashonh.ToString());
            cardDetTableSecondCashOnHRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            cardDetTableSecondCashOnHRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            cardDetTableSecondShortRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(short1.ToString());
            cardDetTableSecondShortRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;
            cardDetTableSecondShortRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Bottom.Visible = false;

            cardDetTableSecondOverRow.Cells[GetTerminalDataCollection.Count + 1].AddParagraph(over.ToString());
            cardDetTableSecondOverRow.Cells[GetTerminalDataCollection.Count + 1].Borders.Left.Visible = false;

            // section 3 - Active and stock info
            // A . Data from lottery app

            Paragraph space3 = section.AddParagraph();
            Paragraph space4 = section.AddParagraph();
            Paragraph p3 = section.AddParagraph();
            p3.AddText(" 3. Active and Stock Info");
            p3.Format.Font.Size = 8;
            p3.Format.Font.Bold = true;
            p3.Format.Alignment = ParagraphAlignment.Left;

            Paragraph lotteryData = section.AddParagraph("A. Data From Lottery App");
            lotteryData.Format.Font.Size = 7;
            lotteryData.Format.Alignment = ParagraphAlignment.Left;

            Table dataFromLottery = section.AddTable();
            dataFromLottery.Format.Font.Size = 7;
            dataFromLottery.Format.Alignment = ParagraphAlignment.Left;
            dataFromLottery.Borders.Width = 0.5;
            dataFromLottery.Rows.Height = 10;

            Column dataFromLotteryFirstCol = dataFromLottery.AddColumn("3cm");

            for (int i = 1; i < (GetTerminalDataCollection.Count + 1); i++)
            {
                Column dataFromLotterySecondCol = dataFromLottery.AddColumn("2cm");
            }

            Row dataFromLotteryShiftRow = dataFromLottery.AddRow();
            Row dataFromLotteryStockRow = dataFromLottery.AddRow();
            Row dataFromLotteryActiveRow = dataFromLottery.AddRow();
            Row dataFromLotteryTotalRow = dataFromLottery.AddRow();


            dataFromLotteryShiftRow.Cells[0].Borders.Right.Visible = false;
            dataFromLotteryShiftRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromLotteryStockRow.Cells[0].AddParagraph("Stock");
            dataFromLotteryStockRow.Cells[0].Borders.Right.Visible = false;
            dataFromLotteryStockRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromLotteryActiveRow.Cells[0].AddParagraph("Active");
            dataFromLotteryActiveRow.Cells[0].Borders.Right.Visible = false;
            dataFromLotteryActiveRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromLotteryTotalRow.Cells[0].AddParagraph("Total");
            dataFromLotteryTotalRow.Cells[0].Borders.Right.Visible = false;

            for (int i = 0, j = 1; i < GetTerminalDataCollection.Count; i++, j++)
            {
                dataFromLotteryShiftRow.Cells[j].AddParagraph("Shift" + GetTerminalDataCollection[i].ShiftID);
                dataFromLotteryShiftRow.Cells[j].Borders.Left.Visible = false;
                dataFromLotteryShiftRow.Cells[j].Borders.Right.Visible = false;
                dataFromLotteryShiftRow.Cells[j].Borders.Bottom.Visible = false;

                dataFromLotteryStockRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].CountRecevied.ToString());
                dataFromLotteryStockRow.Cells[j].Borders.Visible = false;

                dataFromLotteryActiveRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].CountActive.ToString());
                dataFromLotteryActiveRow.Cells[j].Borders.Visible = false;

                dataFromLotteryTotalRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].TotalActiveReceviedStock.ToString());
                dataFromLotteryTotalRow.Cells[j].Borders.Top.Visible = false;
                dataFromLotteryTotalRow.Cells[j].Borders.Left.Visible = false;
                dataFromLotteryTotalRow.Cells[j].Borders.Right.Visible = false;
            }

            dataFromLotteryStockRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            dataFromLotteryActiveRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            dataFromLotteryTotalRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            dataFromLotteryShiftRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;

            // B. Data From Terrminal

            Paragraph terminalData = section.AddParagraph("B. Data From Terminal");
            terminalData.Format.Font.Size = 7;
            terminalData.Format.Alignment = ParagraphAlignment.Left;

            Table dataFromTerminal = section.AddTable();
            dataFromTerminal.Format.Font.Size = 7;
            dataFromTerminal.Format.Alignment = ParagraphAlignment.Left;
            dataFromTerminal.Borders.Width = 0.5;
            dataFromTerminal.Rows.Height = 10;

            Column dataFromTerminalFirstCol = dataFromTerminal.AddColumn("3cm");

            for (int i = 1; i < (GetTerminalDataCollection.Count + 1); i++)
            {
                Column dataFromTerminalSecondCol = dataFromTerminal.AddColumn("2cm");
            }

            Row dataFromTerminalShiftRow = dataFromTerminal.AddRow();
            Row dataFromTerminalStockRow = dataFromTerminal.AddRow();
            Row dataFromTerminalActiveRow = dataFromTerminal.AddRow();
            Row dataFromTerminalTotalRow = dataFromTerminal.AddRow();


            dataFromTerminalShiftRow.Cells[0].Borders.Right.Visible = false;
            dataFromTerminalShiftRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromTerminalStockRow.Cells[0].AddParagraph("Stock");
            dataFromTerminalStockRow.Cells[0].Borders.Right.Visible = false;
            dataFromTerminalStockRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromTerminalActiveRow.Cells[0].AddParagraph("Active");
            dataFromTerminalActiveRow.Cells[0].Borders.Right.Visible = false;
            dataFromTerminalActiveRow.Cells[0].Borders.Bottom.Visible = false;

            dataFromTerminalTotalRow.Cells[0].AddParagraph("Total");
            dataFromTerminalTotalRow.Cells[0].Borders.Right.Visible = false;

            for (int i = 0, j = 1; i < GetTerminalDataCollection.Count; i++, j++)
            {
                dataFromTerminalShiftRow.Cells[j].AddParagraph("Shift" + GetTerminalDataCollection[i].ShiftID);
                dataFromTerminalShiftRow.Cells[j].Borders.Left.Visible = false;
                dataFromTerminalShiftRow.Cells[j].Borders.Right.Visible = false;
                dataFromTerminalShiftRow.Cells[j].Borders.Bottom.Visible = false;

                dataFromTerminalStockRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].InstockInventory.ToString());
                dataFromTerminalStockRow.Cells[j].Borders.Visible = false;


                dataFromTerminalActiveRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].ActiveInventory.ToString());
                dataFromTerminalActiveRow.Cells[j].Borders.Visible = false;


                dataFromTerminalTotalRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].CountTerminalActiveReceive.ToString());
                dataFromTerminalTotalRow.Cells[j].Borders.Top.Visible = false;
                dataFromTerminalTotalRow.Cells[j].Borders.Left.Visible = false;
                dataFromTerminalTotalRow.Cells[j].Borders.Right.Visible = false;

            }
            dataFromTerminalStockRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            dataFromTerminalActiveRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            dataFromTerminalTotalRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            dataFromTerminalShiftRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;

            // C. Shortover

            Paragraph ShortoverData = section.AddParagraph("C. Short Over");
            ShortoverData.Format.Font.Size = 7;
            ShortoverData.Format.Alignment = ParagraphAlignment.Left;

            Table ShortOverTable = section.AddTable();
            ShortOverTable.Format.Font.Size = 7;
            ShortOverTable.Format.Alignment = ParagraphAlignment.Left;
            ShortOverTable.Borders.Width = 0.5;
            ShortOverTable.Rows.Height = 10;

            Column ShortOverTableFirstCol = ShortOverTable.AddColumn("3cm");

            for (int i = 1; i < (GetTerminalDataCollection.Count + 1); i++)
            {
                Column ShortOverTableSecondCol = ShortOverTable.AddColumn("2cm");
            }

            Row ShortOverTableShiftRow = ShortOverTable.AddRow();
            Row ShortOverTableStockRow = ShortOverTable.AddRow();
            Row ShortOverTableActiveRow = ShortOverTable.AddRow();

            ShortOverTableShiftRow.Cells[0].Borders.Right.Visible = false;
            ShortOverTableShiftRow.Cells[0].Borders.Bottom.Visible = false;

            ShortOverTableStockRow.Cells[0].AddParagraph("Stock");
            ShortOverTableStockRow.Cells[0].Borders.Right.Visible = false;
            ShortOverTableStockRow.Cells[0].Borders.Bottom.Visible = false;

            ShortOverTableActiveRow.Cells[0].AddParagraph("Active");
            ShortOverTableActiveRow.Cells[0].Borders.Right.Visible = false;

            for (int i = 0, j = 1; i < GetTerminalDataCollection.Count; i++, j++)
            {
                ShortOverTableShiftRow.Cells[j].AddParagraph("Shift" + GetTerminalDataCollection[i].ShiftID);
                ShortOverTableShiftRow.Cells[j].Borders.Left.Visible = false;
                ShortOverTableShiftRow.Cells[j].Borders.Right.Visible = false;
                ShortOverTableShiftRow.Cells[j].Borders.Bottom.Visible = false;

                ShortOverTableStockRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].ShortoverStock.ToString());
                ShortOverTableStockRow.Cells[j].Borders.Visible = false;

                ShortOverTableActiveRow.Cells[j].AddParagraph(GetTerminalDataCollection[i].ShortoverActive.ToString());
                ShortOverTableActiveRow.Cells[j].Borders.Top.Visible = false;
                ShortOverTableActiveRow.Cells[j].Borders.Left.Visible = false;
                ShortOverTableActiveRow.Cells[j].Borders.Right.Visible = false;

            }
            ShortOverTableStockRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            ShortOverTableShiftRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;
            ShortOverTableActiveRow.Cells[GetTerminalDataCollection.Count].Borders.Right.Visible = true;

            // Section Stock inventory & Remaining Active ticket

            Paragraph space5 = section.AddParagraph();
            Table StockRemaining = section.AddTable();
            StockRemaining.Format.Font.Size = 7;
            StockRemaining.Format.Alignment = ParagraphAlignment.Left;
            StockRemaining.Borders.Width = 0.5;
            StockRemaining.Rows.Height = 10;

            Column StockRemainingFirstCol = StockRemaining.AddColumn("4cm");

            Column StockRemainingSecondCol = StockRemaining.AddColumn("3cm");

            Row StockRemainingRow1 = StockRemaining.AddRow();
            Row StockRemainingRow2 = StockRemaining.AddRow();

            StockRemainingRow1.Cells[0].AddParagraph("Stock Inventory Value");
            StockRemainingRow1.Cells[0].Borders.Bottom.Visible = false;
            StockRemainingRow1.Cells[0].Borders.Right.Visible = false;

            StockRemainingRow2.Cells[0].AddParagraph("Remaining Active Ticket Value");
            StockRemainingRow2.Cells[0].Borders.Top.Visible = false;
            StockRemainingRow2.Cells[0].Borders.Right.Visible = false;

            StockRemainingRow1.Cells[1].AddParagraph(totStock.ToString());
            StockRemainingRow1.Cells[1].Borders.Bottom.Visible = false;
            StockRemainingRow1.Cells[1].Borders.Left.Visible = false;

            StockRemainingRow2.Cells[1].AddParagraph(totActive.ToString());
            StockRemainingRow2.Cells[1].Borders.Top.Visible = false;
            StockRemainingRow2.Cells[1].Borders.Left.Visible = false;

            Paragraph space6 = section.AddParagraph();
            Paragraph space7 = section.AddParagraph();
            Paragraph p4 = section.AddParagraph();
            p4.AddText(" 4. Ticket Info Screen");
            p4.Format.Font.Size = 8;
            p4.Format.Font.Bold = true;
            p4.Format.Alignment = ParagraphAlignment.Left;

            Paragraph ListActivation = section.AddParagraph("A. List Of Activation");
            ListActivation.Format.Font.Size = 7;
            ListActivation.Format.Alignment = ParagraphAlignment.Left;
            Paragraph space8 = section.AddParagraph();

            var ActivationTable = section.AddTable();
            ActivationTable.Style = "Table";
            ActivationTable.Borders.Width = 0.25;
            ActivationTable.Borders.Color = Colors.Black;
            ActivationTable.Rows.Height = 10;
            ActivationTable.Format.Font.Size = 7;
            //eventTable.Borders.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;

            Column ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");


            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            ActivationTablecolumnGroup = ActivationTable.AddColumn("2cm");

            Row ActivationTableRows = ActivationTable.AddRow();

            ActivationTableRows.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            ActivationTableRows.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            ActivationTableRows.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            ActivationTableRows.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            ActivationTableRows.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            ActivationTableRows.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            ActivationTableRows.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            ActivationTableRows.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            ActivationTableRows.Cells[7].AddParagraph("Count   ").Format = formatBold.Clone();
            ActivationTableRows.Cells[7].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ActivationTableRows.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            ActivationTableRows.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            ActivationTableRows.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ActivationTableRows.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ActivationTableRows.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            activeObj = new Activate_Ticket();
            activeObj.Store_Id = pdfStoreId;
            activeObj.Created_Date = pdfDate;
            activeObj.EmployeeID = 0;
            NewGetDailyActivateHistory(activeObj);

            for (int i = 0; i < LotteryHistoryActivate.Count; i++)
            {
                Row row1 = ActivationTable.AddRow();
                row1.Cells[0].AddParagraph(LotteryHistoryActivate[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(LotteryHistoryActivate[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(LotteryHistoryActivate[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(LotteryHistoryActivate[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(LotteryHistoryActivate[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(LotteryHistoryActivate[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(LotteryHistoryActivate[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(LotteryHistoryActivate[i].Count.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(LotteryHistoryActivate[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            }


            Paragraph space9 = section.AddParagraph();
            Paragraph ListSoldOut = section.AddParagraph("B. List Of SoldOut Ticket");
            ListSoldOut.Format.Font.Size = 7;
            ListSoldOut.Format.Alignment = ParagraphAlignment.Left;

            Paragraph space10 = section.AddParagraph();

            var SoldOutTable = section.AddTable();
            SoldOutTable.Style = "Table";
            SoldOutTable.Borders.Width = 0.25;
            SoldOutTable.Borders.Color = Colors.Black;
            SoldOutTable.Rows.Height = 10;
            SoldOutTable.Format.Font.Size = 7;

            Column SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            SoldOutTablecolumnGroup = SoldOutTable.AddColumn("2cm");

            Row SoldOutTableRows = SoldOutTable.AddRow();

            SoldOutTableRows.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            SoldOutTableRows.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            SoldOutTableRows.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            SoldOutTableRows.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            SoldOutTableRows.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            SoldOutTableRows.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            SoldOutTableRows.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            SoldOutTableRows.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            SoldOutTableRows.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            SoldOutTableRows.Cells[7].AddParagraph("Count   ").Format = formatBold.Clone();
            SoldOutTableRows.Cells[7].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            SoldOutTableRows.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            SoldOutTableRows.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            SoldOutTableRows.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            SoldOutTableRows.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            SoldOutTableRows.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            soldOutObj = new SoldOut_TicketInfo();
            soldOutObj.Store_Id = pdfStoreId;
            soldOutObj.Created_Date = pdfDate;
            soldOutObj.EmployeeID = 0;
            NewGetDailySoldOutHistory(soldOutObj);

            for (int i = 0; i < LotteryHistory.Count; i++)
            {
                Row row1 = SoldOutTable.AddRow();
                row1.Cells[0].AddParagraph(LotteryHistory[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(LotteryHistory[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(LotteryHistory[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(LotteryHistory[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(LotteryHistory[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(LotteryHistory[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(LotteryHistory[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(LotteryHistory[i].No_of_Tickets_Sold.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(LotteryHistory[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            }

            Paragraph space11 = section.AddParagraph();
            Paragraph ListReturn = section.AddParagraph("C. List Of Return Ticket");
            ListReturn.Format.Font.Size = 7;
            ListReturn.Format.Alignment = ParagraphAlignment.Left;

            Paragraph space12 = section.AddParagraph();

            var ReturnTable = section.AddTable();
            ReturnTable.Style = "Table";
            ReturnTable.Borders.Width = 0.25;
            ReturnTable.Borders.Color = Colors.Black;
            ReturnTable.Rows.Height = 10;
            ReturnTable.Format.Font.Size = 7;

            Column ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            ReturnTablecolumnGroup = ReturnTable.AddColumn("2cm");

            Row ReturnTableRows = ReturnTable.AddRow();

            ReturnTableRows.Cells[0].AddParagraph("Box No").Format = formatBold.Clone();
            ReturnTableRows.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[0].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            ReturnTableRows.Cells[1].AddParagraph("Game Id ").Format = formatBold.Clone();
            ReturnTableRows.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[1].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[2].AddParagraph("Packet Id").Format = formatBold.Clone();
            ReturnTableRows.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[2].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[3].AddParagraph("Ticket Name").Format = formatBold.Clone();
            ReturnTableRows.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[3].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[4].AddParagraph("Start No.").Format = formatBold.Clone();
            ReturnTableRows.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[4].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[5].AddParagraph("End No. ").Format = formatBold.Clone();
            ReturnTableRows.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[5].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[6].AddParagraph("Value").Format = formatBold.Clone();
            ReturnTableRows.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[6].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;


            ReturnTableRows.Cells[7].AddParagraph("Count   ").Format = formatBold.Clone();
            ReturnTableRows.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[7].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnTableRows.Cells[8].AddParagraph("Total").Format = formatBold.Clone();
            ReturnTableRows.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            ReturnTableRows.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.DarkGray;
            ReturnTableRows.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            ReturnTableRows.Cells[8].Borders.Color = MigraDoc.DocumentObjectModel.Colors.Gray;

            ReturnObj = new Return_TicketInfo();
            ReturnObj.Store_Id = pdfStoreId;
            ReturnObj.Created_Date = pdfDate;
            ReturnObj.EmployeeID = 0;
            NewGetDailyReturnHistory(ReturnObj);

            for (int i = 0; i < LotteryHistoryReturn.Count; i++)
            {
                Row row1 = ReturnTable.AddRow();
                row1.Cells[0].AddParagraph(LotteryHistoryReturn[i].Box_No.ToString()).Format = formatNormal.Clone();
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[1].AddParagraph(LotteryHistoryReturn[i].Game_Id).Format = formatNormal.Clone();
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[2].AddParagraph(LotteryHistoryReturn[i].Packet_No).Format = formatNormal.Clone();
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[3].AddParagraph(LotteryHistoryReturn[i].Ticket_Name).Format = formatNormal.Clone();
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[4].AddParagraph(LotteryHistoryReturn[i].Start_No).Format = formatNormal.Clone();
                row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[5].AddParagraph(LotteryHistoryReturn[i].End_No).Format = formatNormal.Clone();
                row1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[6].AddParagraph(LotteryHistoryReturn[i].Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[7].AddParagraph(LotteryHistoryReturn[i].Count.ToString()).Format = formatNormal.Clone();
                row1.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                row1.Cells[8].AddParagraph(LotteryHistoryReturn[i].Total_Price.ToString()).Format = formatNormal.Clone();
                row1.Cells[8].Format.Alignment = ParagraphAlignment.Left;
            }

        }

        private void AddLineSpace(Section section, TabLeader tabLeader)
        {
            var paragraph = section.AddParagraph();
            paragraph.Format.TabStops.ClearAll();
            paragraph.Format.TabStops.AddTabStop("16cm", TabAlignment.Right, tabLeader);
            paragraph.AddTab();
        }

        private void AddHeaderToReport(Section section, ParagraphFormat formatBold, ParagraphFormat formatNormal)
        {
            MigraDoc.DocumentObjectModel.Shapes.Image labLogo = new MigraDoc.DocumentObjectModel.Shapes.Image();

            section.Headers.Primary = null;
            section.Headers.Primary = null;
            Paragraph space13 = section.AddParagraph();
            Paragraph space18 = section.AddParagraph();

            Paragraph title1 = section.AddParagraph("Daily Report ");
            title1.Format.Font.Size = 9;
            title1.Format.Font.Bold = true;
            title1.Format.Alignment = ParagraphAlignment.Center;
            Paragraph space19 = section.AddParagraph();
            Paragraph space20 = section.AddParagraph();


            // Create header
            var headerTable = section.Headers.FirstPage.Section.AddTable();

            Column headerColumnLocationName = headerTable.AddColumn("3cm");
            headerColumnLocationName.Format.Alignment = ParagraphAlignment.Left;

            Column headerColLocNameValue = headerTable.AddColumn("10cm");
            headerColLocNameValue.Format.Alignment = ParagraphAlignment.Left;

            Column headerColumnUserLogin = headerTable.AddColumn("3cm");
            headerColumnUserLogin.Format.Alignment = ParagraphAlignment.Right;

            Column headerColumnUserLoginValue = headerTable.AddColumn("3cm");
            headerColumnUserLoginValue.Format.Alignment = ParagraphAlignment.Right;


            Row HeaderRow = headerTable.AddRow();
            Row HeaderRow1 = headerTable.AddRow();
            Row HeaderRow2 = headerTable.AddRow();

            getHeaderDetails(pdfStoreId, pdfDate);
            HeaderRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[0].AddParagraph("Location Name").Format = formatBold.Clone();

            HeaderRow.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[1].AddParagraph(":  " + pdfLoc).Format = formatBold.Clone();

            HeaderRow.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[2].AddParagraph("User Login").Format = formatBold.Clone();

            HeaderRow.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow.Cells[3].AddParagraph(":  " + pdfEmpName).Format = formatBold.Clone();

            HeaderRow1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[0].AddParagraph("Address").Format = formatBold.Clone();

            HeaderRow1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[1].AddParagraph(":  " + pdfLoc).Format = formatBold.Clone();

            HeaderRow1.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[2].AddParagraph("Date").Format = formatBold.Clone();

            HeaderRow1.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow1.Cells[3].AddParagraph(":  " + pdfDate.ToString("MM/dd/yyyy")).Format = formatBold.Clone();

            HeaderRow2.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow2.Cells[2].AddParagraph("Time").Format = formatBold.Clone();

            HeaderRow2.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            HeaderRow2.Cells[3].AddParagraph(":" + pdfEndTime).Format = formatBold.Clone();

        }

        private void getHeaderDetails(int pdfStoreId, DateTime pdfDate)
        {
            context = new LotteryBlankDatabaseEntities();
            soldOutObj = new SoldOut_TicketInfo();
            soldOutObj.Store_Id = pdfStoreId;
            soldOutObj.Created_Date = pdfDate;
            soldOutObj.ShiftID = 1;
            NewGetDailySoldOutHistory(soldOutObj);
        }

        private void DefineStyles(Document document)
        {
            MigraDoc.DocumentObjectModel.Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            //System.Drawing.Color fontColor = ColorConverter.ConvertFromString("#FFDFD991") as System.Drawing.Color;
            style.Font.Color = FONT_COLOR;
            style.Font.Name = FONT_FACE;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            //style = document.Styles[StyleNames.Footer];
            //style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = FONT_FACE;
        }
        
        [Route("api/CloseShift/NewGetTerminalDataHistory")]
        public HttpResponseMessage NewGetTerminalDataHistory([FromBody] Terminal_Details data)
        {

            GetTerminalDataCollection = new ObservableCollection<Terminal_Details>();
            context = new LotteryBlankDatabaseEntities();
            var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
            if (Shift != null)
            {
                if (data.ShiftID == 0 && data.EmployeeID != 0) // Shift Report & Daily Report
                {
                    var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == Shift.Date select s).ToList();

                    foreach (var r in x)
                    {
                        var result = (from s in context.tblTerminal_Data1
                                      where s.Store_Id == data.Store_Id && s.Date == Shift.Date
                                      && s.ShiftID == r.ShiftID
                                      select s).ToList();
                        foreach (var v in result)
                        {
                            int short1 = 0;
                            int over = 0;
                            if (int.Parse(v.Net_Cash) > int.Parse(v.Cash_On_Hand))
                            {
                                short1 = (Convert.ToInt32(v.Cash_On_Hand) - Convert.ToInt32(v.Net_Cash));
                            }
                            else
                            {
                                over = (Convert.ToInt32(v.Net_Cash) - Convert.ToInt32(v.Cash_On_Hand));
                            }
                            GetTerminalDataCollection.Add(new Terminal_Details
                            {
                                ScratchSells = v.Scratch_Sells,
                                OnlineSells = v.Online_Sells,
                                TotalSells = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells)).ToString(),
                                ScratchPayout = v.Scratch_Payout,
                                OnlinePayout = v.Online_Payout,
                                TotalPayout = (int.Parse(v.Online_Payout) + int.Parse(v.Scratch_Payout)).ToString(),
                                Loan = v.Loan,
                                Credit = v.Credit,
                                Debit = v.Debit,
                                TopUp = v.Top_Up,
                                TopUPCancel = v.Top_Up_Cancel,
                                IssuedInventory = v.Issued_Inventory,
                                InstockInventory = v.InStock_Inventory,
                                TotalStockInventory = v.Total_Stock_Inventory,
                                TotalActiveInventory = v.Total_Active_Inventory,
                                ActiveInventory = v.Active_Inventory,
                                CashOnHand = v.Cash_On_Hand,
                                ShiftID = v.ShiftID,
                                EmployeeID = Convert.ToInt32(v.Employee_Id),
                                Total = v.Total,
                                NetCash = v.Net_Cash,
                                Short1 = short1.ToString(),
                                Over = over.ToString(),
                                //ShortOver = b;
                                //ActualCash = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells) - int.Parse(v.Scratch_Payout) - int.Parse(v.Online_Payout) - int.Parse(v.Loan)).ToString(),
                                //ShortOver = ((Convert.ToInt32(v.Scratch_Sells) + Convert.ToInt32(v.Online_Sells) - Convert.ToInt32(v.Scratch_Payout) - Convert.ToInt32(v.Online_Payout) - int.Parse(v.Loan)) - Convert.ToInt32(v.Scratch_Sells)).ToString(),
                                CountTerminalActiveReceive = (int.Parse(v.InStock_Inventory) + int.Parse(v.Active_Inventory)).ToString(),
                                CountActive = v.CountActive,
                                CountRecevied = v.CountRecevied,
                                TotalActiveReceviedStock = v.CountActive + v.CountRecevied,
                                ShortoverStock = Convert.ToInt32(v.CountRecevied) - Convert.ToInt32(v.InStock_Inventory),
                                ShortoverActive = Convert.ToInt32(v.CountActive) - Convert.ToInt32(v.Active_Inventory),
                                Date = Convert.ToDateTime(v.Date)
                            });
                        }

                    }

                }
                else if (data.ShiftID == 0 && data.EmployeeID == 0) // Hamburger Daily report
                {
                    var x = (from s in context.tblShifts
                             where s.StoreId == data.Store_Id && s.Date == data.Date
                             select s).ToList();

                    foreach (var r in x)
                    {
                        var result = (from s in context.tblTerminal_Data1
                                      where s.Store_Id == data.Store_Id && s.Date == data.Date && s.ShiftID == r.ShiftID
                                      select s).ToList();
                        foreach (var v in result)
                        {
                            int short1 = 0;
                            int over = 0;
                            if (int.Parse(v.Net_Cash) > int.Parse(v.Cash_On_Hand))
                            {
                                short1 = (Convert.ToInt32(v.Cash_On_Hand) - Convert.ToInt32(v.Net_Cash));
                            }
                            else
                            {
                                over = (Convert.ToInt32(v.Net_Cash) - Convert.ToInt32(v.Cash_On_Hand));
                            }
                            GetTerminalDataCollection.Add(new Terminal_Details
                            {
                                ScratchSells = v.Scratch_Sells,
                                OnlineSells = v.Online_Sells,
                                TotalSells = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells)).ToString(),
                                ScratchPayout = v.Scratch_Payout,
                                OnlinePayout = v.Online_Payout,
                                TotalPayout = (int.Parse(v.Online_Payout) + int.Parse(v.Scratch_Payout)).ToString(),
                                Loan = v.Loan,
                                Credit = v.Credit,
                                Debit = v.Debit,
                                TopUp = v.Top_Up,
                                TopUPCancel = v.Top_Up_Cancel,
                                IssuedInventory = v.Issued_Inventory,
                                InstockInventory = v.InStock_Inventory,
                                ActiveInventory = v.Active_Inventory,
                                CashOnHand = v.Cash_On_Hand,
                                ShiftID = v.ShiftID,
                                EmployeeID = Convert.ToInt32(v.Employee_Id),
                                Total = v.Total,
                                Short1 = short1.ToString(),
                                Over = over.ToString(),
                                //ActualCash = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells) - int.Parse(v.Scratch_Payout) - int.Parse(v.Online_Payout) - int.Parse(v.Loan)).ToString(),
                                // ShortOver = ((Convert.ToInt32(v.Scratch_Sells) + Convert.ToInt32(v.Online_Sells) - Convert.ToInt32(v.Scratch_Payout) - Convert.ToInt32(v.Online_Payout) - int.Parse(v.Loan)) - Convert.ToInt32(v.Scratch_Sells)).ToString(),
                                CountTerminalActiveReceive = (int.Parse(v.InStock_Inventory) + int.Parse(v.Active_Inventory)).ToString(),
                                CountActive = v.CountActive,
                                CountRecevied = v.CountRecevied,
                                TotalActiveReceviedStock = v.CountActive + v.CountRecevied,
                                ShortoverStock = Convert.ToInt32(v.CountRecevied) - Convert.ToInt32(v.InStock_Inventory),
                                ShortoverActive = Convert.ToInt32(v.CountActive) - Convert.ToInt32(v.Active_Inventory),
                                Date = Convert.ToDateTime(v.Date),
                                TotalStockInventory = v.Total_Stock_Inventory,
                                TotalActiveInventory = v.Total_Active_Inventory,
                                NetCash = v.Net_Cash
                            });
                        }
                    }

                }

                else if (data.ShiftID != 0 && data.EmployeeID != 0)  // Hamburger Shift Report
                {
                    var x = (from s in context.tblShifts
                             where s.StoreId == data.Store_Id && s.Date == data.Date
 && s.EndTime == data.CloseTime
                             select s).ToList();

                    foreach (var r in x)
                    {
                        var result = (from s in context.tblTerminal_Data1
                                      where s.Store_Id == data.Store_Id
  && s.ShiftID == r.ShiftID
                                      select s).ToList();
                        foreach (var v in result)
                        {
                            int short1 = 0;
                            int over = 0;
                            if (int.Parse(v.Net_Cash) > int.Parse(v.Cash_On_Hand))
                            {
                                short1 = (Convert.ToInt32(v.Cash_On_Hand) - Convert.ToInt32(v.Net_Cash));
                            }
                            else
                            {
                                over = (Convert.ToInt32(v.Net_Cash) - Convert.ToInt32(v.Cash_On_Hand));
                            }
                            GetTerminalDataCollection.Add(new Terminal_Details
                            {
                                ScratchSells = v.Scratch_Sells,
                                OnlineSells = v.Online_Sells,
                                TotalSells = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells)).ToString(),
                                ScratchPayout = v.Scratch_Payout,
                                OnlinePayout = v.Online_Payout,
                                TotalPayout = (int.Parse(v.Online_Payout) + int.Parse(v.Scratch_Payout)).ToString(),
                                Loan = v.Loan,
                                Credit = v.Credit,
                                Debit = v.Debit,
                                TopUp = v.Top_Up,
                                TopUPCancel = v.Top_Up_Cancel,
                                IssuedInventory = v.Issued_Inventory,
                                InstockInventory = v.InStock_Inventory,
                                ActiveInventory = v.Active_Inventory,
                                CashOnHand = v.Cash_On_Hand,
                                ShiftID = v.ShiftID,
                                EmployeeID = Convert.ToInt32(v.Employee_Id),
                                Total = v.Total,
                                Short1 = short1.ToString(),
                                Over = over.ToString(),
                                //ActualCash = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells) - int.Parse(v.Scratch_Payout) - int.Parse(v.Online_Payout) - int.Parse(v.Loan)).ToString(),
                                // ShortOver = ((Convert.ToInt32(v.Scratch_Sells) + Convert.ToInt32(v.Online_Sells) - Convert.ToInt32(v.Scratch_Payout) - Convert.ToInt32(v.Online_Payout) - int.Parse(v.Loan)) - Convert.ToInt32(v.Scratch_Sells)).ToString(),
                                CountTerminalActiveReceive = (int.Parse(v.InStock_Inventory) + int.Parse(v.Active_Inventory)).ToString(),
                                CountActive = v.CountActive,
                                CountRecevied = v.CountRecevied,
                                TotalActiveReceviedStock = v.CountActive + v.CountRecevied,
                                ShortoverStock = Convert.ToInt32(v.CountRecevied) - Convert.ToInt32(v.InStock_Inventory),
                                ShortoverActive = Convert.ToInt32(v.CountActive) - Convert.ToInt32(v.Active_Inventory),
                                Date = Convert.ToDateTime(v.Date),
                                TotalStockInventory = v.Total_Stock_Inventory,
                                TotalActiveInventory = v.Total_Active_Inventory,
                                NetCash = v.Net_Cash

                            });
                        }
                    }

                }

                else if (data.ShiftID != 0) // Hamburger Daily report ok button
                {
                    var x = (from s in context.tblShifts
                             where s.StoreId == data.Store_Id
                             select s).ToList();
                    foreach (var r in x)
                    {
                        if (r.Date > data.HamburgerFromDateOk && r.Date <= data.HamburgerToDateOk)
                        {
                            var result = (from s in context.tblTerminal_Data1
                                          where s.Store_Id == data.Store_Id && s.Date == r.Date && s.ShiftID == r.ShiftID
                                          select s).ToList();

                            foreach (var v in result)
                            {
                                int short1 = 0;
                                int over = 0;
                                if (int.Parse(v.Net_Cash) > int.Parse(v.Cash_On_Hand))
                                {
                                    short1 = (Convert.ToInt32(v.Cash_On_Hand) - Convert.ToInt32(v.Net_Cash));
                                }
                                else
                                {
                                    over = (Convert.ToInt32(v.Net_Cash) - Convert.ToInt32(v.Cash_On_Hand));
                                }
                                GetTerminalDataCollection.Add(new Terminal_Details
                                {
                                    ScratchSells = v.Scratch_Sells,
                                    OnlineSells = v.Online_Sells,
                                    TotalSells = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells)).ToString(),
                                    ScratchPayout = v.Scratch_Payout,
                                    OnlinePayout = v.Online_Payout,
                                    TotalPayout = (int.Parse(v.Online_Payout) + int.Parse(v.Scratch_Payout)).ToString(),
                                    Loan = v.Loan,
                                    Credit = v.Credit,
                                    Debit = v.Debit,
                                    TopUp = v.Top_Up,
                                    TopUPCancel = v.Top_Up_Cancel,
                                    IssuedInventory = v.Issued_Inventory,
                                    InstockInventory = v.InStock_Inventory,
                                    ActiveInventory = v.Active_Inventory,
                                    CashOnHand = v.Cash_On_Hand,
                                    ShiftID = v.ShiftID,
                                    EmployeeID = Convert.ToInt32(v.Employee_Id),
                                    Total = v.Total,
                                    Short1 = short1.ToString(),
                                    Over = over.ToString(),
                                    //ActualCash = (int.Parse(v.Scratch_Sells) + int.Parse(v.Online_Sells) - int.Parse(v.Scratch_Payout) - int.Parse(v.Online_Payout) - int.Parse(v.Loan)).ToString(),
                                    // ShortOver = ((Convert.ToInt32(v.Scratch_Sells) + Convert.ToInt32(v.Online_Sells) - Convert.ToInt32(v.Scratch_Payout) - Convert.ToInt32(v.Online_Payout) - int.Parse(v.Loan)) - Convert.ToInt32(v.Scratch_Sells)).ToString(),
                                    CountTerminalActiveReceive = (int.Parse(v.InStock_Inventory) + int.Parse(v.Active_Inventory)).ToString(),
                                    CountActive = v.CountActive,
                                    CountRecevied = v.CountRecevied,
                                    TotalActiveReceviedStock = v.CountActive + v.CountRecevied,
                                    ShortoverStock = Convert.ToInt32(v.CountRecevied) - Convert.ToInt32(v.InStock_Inventory),
                                    ShortoverActive = Convert.ToInt32(v.CountActive) - Convert.ToInt32(v.Active_Inventory),
                                    Date = Convert.ToDateTime(v.Date),
                                    TotalStockInventory = v.Total_Stock_Inventory,
                                    TotalActiveInventory = v.Total_Active_Inventory,
                                    NetCash = v.Net_Cash

                                });
                            }
                        }


                    }

                }


            }

            return Request.CreateResponse(HttpStatusCode.OK, GetTerminalDataCollection);
        }

        [Route("api/CloseShift/NewGetDailyActivateHistory")]
        public HttpResponseMessage NewGetDailyActivateHistory([FromBody] Activate_Ticket data)
        {
            LotteryHistoryActivate = new ObservableCollection<Activate_Ticket>();
            context = new LotteryBlankDatabaseEntities();
            if (data.EmployeeID != 0)
            {
                var Shift = (from s in context.tblShifts
                             where s.StoreId == data.Store_Id
                             select s).ToList().LastOrDefault();

                sdate = Convert.ToDateTime(Shift.Date);
            }

            else
            {
                sdate = data.Created_Date;
                temp = 1;
            }

            var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == sdate select s).ToList();

            foreach (var a in x)
            {
                var close = (from s in context.tblClose_Box
                             where s.Store_Id == data.Store_Id
                             && s.Created_On == sdate && s.ShiftID == a.ShiftID
                             select s).ToList();

                var soldret = (from s in context.tblActivated_Tickets
                               where s.Store_Id == data.Store_Id && s.ShiftID == a.ShiftID
                               && s.Created_On == sdate && (s.Status == "SoldOut" || s.Status == "Return")
                               select s).ToList();

                foreach (var v in close)
                {
                    var active = (from s in context.tblActivated_Tickets
                                  where s.Store_Id == v.Store_Id && s.Game_Id == v.Game_Id && s.Packet_Id == v.Packet_Id
                                  && s.Created_On == sdate
                                  select s).ToList().FirstOrDefault();

                    if (temp == 1)
                    {
                        endno = Convert.ToInt32(v.End_No);
                    }
                    else
                    {
                        endno = Convert.ToInt32(active.End_No);
                    }

                    LotteryHistoryActivate.Add(new Activate_Ticket
                    {
                        Game_Id = v.Game_Id,
                        Box_No = v.Box_No,
                        Created_Date = Convert.ToDateTime(v.Created_On),
                        Packet_No = v.Packet_Id,
                        Ticket_Name = v.Ticket_Name,
                        Price = Convert.ToInt32(v.Price),
                        Start_No = v.Start_No,
                        End_No = endno.ToString(),
                        Status = v.Status,
                        Stopped_At = v.Close_At,
                        EmployeeID = v.EmployeeId,
                        State = v.State,
                        ShiftID = v.ShiftID,
                        Count = (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1).ToString(),
                        Total_Price = Convert.ToInt32(v.Price) * (int.Parse(v.End_No) - int.Parse(v.Start_No) + 1)
                    });
                }

                foreach (var i in soldret)
                {
                    LotteryHistoryActivate.Add(new Activate_Ticket
                    {
                        Game_Id = i.Game_Id,
                        Box_No = i.Box_No,
                        Created_Date = Convert.ToDateTime(i.Created_On),
                        Packet_No = i.Packet_Id,
                        Ticket_Name = i.Ticket_Name,
                        Price = Convert.ToInt32(i.Price),
                        Start_No = i.Start_No,
                        End_No = i.End_No,
                        Status = i.Status,
                        Stopped_At = i.Stopped_At,
                        EmployeeID = i.EmployeeId,
                        State = i.State,
                        ShiftID = i.ShiftID,
                        Count = (int.Parse(i.End_No) - int.Parse(i.Start_No) + 1).ToString(),
                        Total_Price = Convert.ToInt32(i.Price) * (int.Parse(i.End_No) - int.Parse(i.Start_No) + 1)
                    });
                }
            }


            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistoryActivate);
        }

        [Route("api/CloseShift/NewGetDailySoldOutHistory")]
        public HttpResponseMessage NewGetDailySoldOutHistory([FromBody] SoldOut_TicketInfo data)
        {
            LotteryHistory = new ObservableCollection<SoldOut_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            if (data.EmployeeID != 0)
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

                if (data.ShiftID != 0)
                {
                    var Shift1 = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == data.Created_Date select s).ToList().LastOrDefault();
                    if (Shift1.IsClose == null)
                    {
                        pdfEndTime = System.DateTime.Now.ToString("hh:mm tt");
                    }
                    else
                    {
                        pdfEndTime = Shift1.EndTime;
                    }
                    var empDet = (from s in context.tblEmployee_Details where s.EmployeeId == Shift1.EmployeeId select s).ToList().FirstOrDefault();
                    pdfEmpName = empDet.EmployeeName;

                    var locDet = (from s in context.tblStore_Info where s.Store_Id == pdfStoreId select s).ToList().FirstOrDefault();
                    pdfLoc = locDet.Store_Address;
                }

                else
                {
                    var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == Shift.Date select s).ToList();

                    foreach (var r in x)
                    {
                        var result = (from s in context.tblSoldouts
                                      where s.Status == "SoldOut" && s.Store_Id == data.Store_Id
                                      && s.Created_On == Shift.Date && s.ShiftID == r.ShiftID
                                      select s).ToList();

                        foreach (var v in result)
                        {
                            LotteryHistory.Add(new SoldOut_TicketInfo
                            {
                                Game_Id = v.Game_Id,
                                Created_Date = Convert.ToDateTime(v.Created_On),
                                Packet_No = v.Packet_Id,
                                Ticket_Name = v.Ticket_Name,
                                Price = Convert.ToInt32(v.Price),
                                Box_No = Convert.ToInt16(v.Box_No),
                                Status = v.Status,
                                Store_Id = v.Store_Id,
                                ShiftID = v.ShiftID,
                                No_of_Tickets_Sold = v.Total_Tickets,
                                EmployeeID = v.EmployeeId,
                                Modified_Date = Convert.ToDateTime(v.Modified_On),
                                End_No = v.PackPosition_Close,
                                Start_No = v.PackPosition_Open,
                                Partial_Packet = v.Partial_Packet,
                                Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
                            });
                        }
                    }

                }

            }
            else
            {

                var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == data.Created_Date select s).ToList();

                foreach (var i in x)
                {
                    var result = (from s in context.tblSoldouts
                                  where s.Status == "SoldOut" && s.Store_Id == data.Store_Id
                                  && s.Created_On == data.Created_Date && s.ShiftID == i.ShiftID
                                  select s).ToList();

                    foreach (var v in result)
                    {
                        LotteryHistory.Add(new SoldOut_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Ticket_Name = v.Ticket_Name,
                            Price = Convert.ToInt32(v.Price),
                            Box_No = Convert.ToInt16(v.Box_No),
                            Status = v.Status,
                            Store_Id = v.Store_Id,
                            ShiftID = v.ShiftID,
                            No_of_Tickets_Sold = v.Total_Tickets,
                            EmployeeID = v.EmployeeId,
                            Modified_Date = Convert.ToDateTime(v.Modified_On),
                            End_No = v.PackPosition_Close,
                            Start_No = v.PackPosition_Open,
                            Partial_Packet = v.Partial_Packet,
                            Total_Price = Convert.ToInt32(v.Price) * v.Total_Tickets
                        });
                    }
                }

            }

            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistory);
        }

        [Route("api/CloseShift/NewGetDailyReturnHistory")]
        public HttpResponseMessage NewGetDailyReturnHistory([FromBody] Return_TicketInfo data)
        {
            LotteryHistoryReturn = new ObservableCollection<Return_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            if (data.EmployeeID != 0)
            {
                var Shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == Shift.Date select s).ToList();

                foreach (var r in x)
                {
                    var result = (from s in context.tblReturntickets
                                  where s.Status == "Return" && s.Created_On == Shift.Date && s.Store_Id == data.Store_Id
                                  && s.ShiftID == r.ShiftID
                                  select s).ToList();
                    foreach (var v in result)
                    {
                        LotteryHistoryReturn.Add(new Return_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Box_No = Convert.ToInt32(v.Box_No),
                            Ticket_Name = v.Ticket_Name,
                            Price = v.Price,
                            Store_Id = v.Store_Id,
                            Start_No = v.PackPosition_Open,
                            End_No = v.PackPosition_Close,
                            Return_At = v.Return_At,
                            EmployeeID = v.EmplyeeeId,
                            ShiftID = v.ShiftID,
                            Count = v.Count,
                            Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
                        });
                    }
                }


            }
            else
            {
                var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == data.Created_Date select s).ToList();
                foreach (var j in x)
                {
                    var result = (from s in context.tblReturntickets
                                  where s.Status == "Return" && s.Created_On == data.Created_Date && s.Store_Id == data.Store_Id
                                  && s.ShiftID == j.ShiftID
                                  select s).ToList();
                    foreach (var v in result)
                    {
                        LotteryHistoryReturn.Add(new Return_TicketInfo
                        {
                            Game_Id = v.Game_Id,
                            Created_Date = Convert.ToDateTime(v.Created_On),
                            Packet_No = v.Packet_Id,
                            Box_No = Convert.ToInt32(v.Box_No),
                            Ticket_Name = v.Ticket_Name,
                            Price = v.Price,
                            Store_Id = v.Store_Id,
                            Start_No = v.PackPosition_Open,
                            End_No = v.PackPosition_Close,
                            Return_At = v.Return_At,
                            EmployeeID = v.EmplyeeeId,
                            ShiftID = v.ShiftID,
                            Count = v.Count,
                            Total_Price = (Convert.ToInt32(v.Price) * v.Count).ToString(),
                        });
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, LotteryHistoryReturn);
        }

        [Route("api/CloseShift/NewGetDailyReport")]
        public HttpResponseMessage NewGetDailyReport([FromBody] Activate_Ticket data)
        {
            SoldOutHistory = new ObservableCollection<SoldOut_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            if (data.EmployeeID != 0)
            {
                var shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();

                var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == shift.Date select s).ToList();

                foreach (var r in x)
                {
                    var SoldOutData = (from s in context.tblSoldouts where s.Status == "SoldOut" && s.Store_Id == data.Store_Id && s.Created_On == shift.Date && s.ShiftID == r.ShiftID select s).ToList();
                    var ReturnData = (from s in context.tblReturntickets where s.Status == "Return" && s.Store_Id == data.Store_Id && s.Created_On == shift.Date && s.ShiftID == r.ShiftID select s).ToList();
                    var CloseData = (from s in context.tblClose_Box where s.Status == "Close" && s.Store_Id == data.Store_Id && s.Created_On == shift.Date && s.ShiftID == r.ShiftID select s).ToList();
                    foreach (var i in SoldOutData)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Total_Tickets,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmployeeId,
                            Store_Id = i.Store_Id,
                            Total_Price = Convert.ToInt32(i.Price) * i.Total_Tickets,
                            ShiftID = i.ShiftID

                        });
                    }
                    foreach (var i in ReturnData)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Count,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmplyeeeId,
                            Store_Id = i.Store_Id,
                            Total_Price = Convert.ToInt32(i.Price) * i.Count,
                            ShiftID = i.ShiftID
                        });
                    }
                    foreach (var i in CloseData)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = (Convert.ToInt32(i.Close_At) + 1).ToString(),
                            Start_No = i.Start_No,
                            Count = (Convert.ToInt32(i.Count)),
                            No_of_Tickets_Sold = (Convert.ToInt32(i.Start_No) + Convert.ToInt32(i.Close_At)) + 1,
                            EmployeeID = i.EmployeeId,
                            Store_Id = i.Store_Id,
                            Total_Price = Convert.ToInt32(i.Price) * Convert.ToInt32(i.Count),
                            ShiftID = i.ShiftID

                        });
                    }
                }

            }
            else
            {


                pdfStoreId = data.Store_Id;
                pdfDate = data.Created_Date;

                var x = (from s in context.tblShifts where s.StoreId == data.Store_Id && s.Date == data.Created_Date select s).ToList();

                foreach (var r in x)
                {
                    var SoldOutData = (from s in context.tblSoldouts where s.Status == "SoldOut" && s.Store_Id == data.Store_Id && s.Created_On == data.Created_Date && s.ShiftID == r.ShiftID select s).ToList();
                    var ReturnData = (from s in context.tblReturntickets where s.Status == "Return" && s.Store_Id == data.Store_Id && s.Created_On == data.Created_Date && s.ShiftID == r.ShiftID select s).ToList();
                    var CloseData = (from s in context.tblClose_Box where s.Status == "Close" && s.Store_Id == data.Store_Id && s.Created_On == data.Created_Date && s.ShiftID == r.ShiftID select s).ToList();
                    foreach (var i in SoldOutData)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Total_Tickets,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmployeeId,
                            Store_Id = i.Store_Id,
                            Total_Price = Convert.ToInt32(i.Price) * i.Total_Tickets,
                            ShiftID = i.ShiftID

                        });
                    }
                    foreach (var i in ReturnData)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Count,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmplyeeeId,
                            Store_Id = i.Store_Id,
                            Total_Price = Convert.ToInt32(i.Price) * i.Count,
                            ShiftID = i.ShiftID
                        });
                    }
                    foreach (var i in CloseData)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = (Convert.ToInt32(i.Close_At) + 1).ToString(),
                            Start_No = i.Start_No,
                            Count = (Convert.ToInt32(i.Count)),
                            No_of_Tickets_Sold = (Convert.ToInt32(i.Start_No) + Convert.ToInt32(i.Close_At)) + 1,
                            EmployeeID = i.EmployeeId,
                            Store_Id = i.Store_Id,
                            Total_Price = Convert.ToInt32(i.Price) * Convert.ToInt32(i.Count),
                            ShiftID = i.ShiftID

                        });
                    }
                }

            }

            return Request.CreateResponse(HttpStatusCode.OK, SoldOutHistory);
        }

        [Route("api/CloseShift/NewGetAllHistory")]
        public HttpResponseMessage NewGetAllHistory([FromBody] SoldOut_TicketInfo data)
        {
            SoldOutHistory = new ObservableCollection<SoldOut_TicketInfo>();
            context = new LotteryBlankDatabaseEntities();
            if (data.ShiftID == 0)
            {
                var shift = (from s in context.tblShifts where s.StoreId == data.Store_Id select s).ToList().LastOrDefault();
                var SoldOutData = (from s in context.tblSoldouts where s.Status == "SoldOut" && s.Store_Id == data.Store_Id && s.Created_On == shift.Date select s).ToList();
                var ReturnData = (from s in context.tblReturntickets where s.Status == "Return" && s.Store_Id == data.Store_Id && s.Created_On == shift.Date select s).ToList();
                var CloseBoxData = (from s in context.tblClose_Box where s.Store_Id == data.Store_Id && s.Created_On == shift.Date select s).ToList();
                var TerminalData = (from s in context.tblTerminal_Data1 where s.Store_Id == data.Store_Id && s.ShiftID == shift.ShiftID select s).ToList().LastOrDefault();

                foreach (var i in SoldOutData)
                {
                    if (i.ShiftID == shift.ShiftID)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Total_Tickets,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmployeeId,
                            Total_Price = Convert.ToInt32(i.Price) * i.Total_Tickets,
                            ShiftID = i.ShiftID

                        });
                    }
                    // SoldOutHistory = new ObservableCollection<SoldOut_TicketInfo>();

                }
                foreach (var i in ReturnData)
                {
                    if (i.ShiftID == shift.ShiftID)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Count,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmplyeeeId,
                            Total_Price = Convert.ToInt32(i.Price) * i.Count,
                            ShiftID = i.ShiftID

                        });
                    }
                    //SoldOutHistory = new ObservableCollection<SoldOut_TicketInfo>();

                }
                foreach (var i in CloseBoxData)
                {
                    if (i.ShiftID == shift.ShiftID)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = i.Price,
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = (Convert.ToInt32(i.Close_At) + 1).ToString(),
                            Start_No = i.Start_No,
                            Count = Convert.ToInt32(i.Count),
                            EmployeeID = i.EmployeeId,
                            Total_Price = i.Price * Convert.ToInt32(i.Count),
                            ShiftID = i.ShiftID
                        });
                    }

                }

                if (shift != null && TerminalData != null)
                {
                    TerminalData.Scratch_Sells = SoldOutHistory.Sum(x => x.Total_Price).ToString();
                    context.SaveChanges();
                }
            }
            else
            {
                var x = (from s in context.tblShifts
                         where s.StoreId == data.Store_Id && s.Date == data.Created_Date
&& s.EndTime == data.CloseTime
                         select s).ToList().FirstOrDefault();


                var SoldOutData = (from s in context.tblSoldouts
                                   where s.Status == "SoldOut"
                                    && s.Store_Id == data.Store_Id && s.Created_On == data.Created_Date
                                   select s).ToList();
                var ReturnData = (from s in context.tblReturntickets
                                  where s.Status == "Return"
                                    && s.Store_Id == data.Store_Id && s.Created_On == data.Created_Date
                                  select s).ToList();
                var CloseBoxData = (from s in context.tblClose_Box
                                    where s.Store_Id == data.Store_Id && s.Created_On == data.Created_Date
                                    select s).ToList();
                var TerminalData = (from s in context.tblTerminal_Data1
                                    where s.Store_Id == data.Store_Id && s.ShiftID == data.ShiftID
                                    select s).ToList().LastOrDefault();

                foreach (var i in SoldOutData)
                {
                    if (i.ShiftID == x.ShiftID)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Total_Tickets,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmployeeId,
                            Total_Price = Convert.ToInt32(i.Price) * i.Total_Tickets,
                            ShiftID = i.ShiftID

                        });
                    }
                    // SoldOutHistory = new ObservableCollection<SoldOut_TicketInfo>();

                }
                foreach (var i in ReturnData)
                {
                    if (i.ShiftID == x.ShiftID)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = Convert.ToInt32(i.Price),
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = i.PackPosition_Close,
                            Start_No = i.PackPosition_Open,
                            Count = i.Count,
                            No_of_Tickets_Sold = (Convert.ToInt32(i.PackPosition_Open) + Convert.ToInt32(i.PackPosition_Close)) + 1,
                            EmployeeID = i.EmplyeeeId,
                            Total_Price = Convert.ToInt32(i.Price) * i.Count,
                            ShiftID = i.ShiftID

                        });
                    }
                    //SoldOutHistory = new ObservableCollection<SoldOut_TicketInfo>();

                }
                foreach (var i in CloseBoxData)
                {
                    if (i.ShiftID == x.ShiftID)
                    {
                        SoldOutHistory.Add(new SoldOut_TicketInfo
                        {
                            Price = i.Price,
                            Box_No = Convert.ToInt32(i.Box_No),
                            Packet_No = i.Packet_Id,
                            Game_Id = i.Game_Id,
                            Status = i.Status,
                            Ticket_Name = i.Ticket_Name,
                            End_No = (Convert.ToInt32(i.Close_At) + 1).ToString(),
                            Start_No = i.Start_No,
                            Count = Convert.ToInt32(i.Count),
                            EmployeeID = i.EmployeeId,
                            Total_Price = i.Price * Convert.ToInt32(i.Count),
                            ShiftID = i.ShiftID
                        });
                    }

                }

                //if (shift != null && TerminalData != null)
                //{
                //    TerminalData.Scratch_Sells = SoldOutHistory.Sum(x => x.Total_Price).ToString();
                //    context.SaveChanges();
                //}
            }
            return Request.CreateResponse(HttpStatusCode.OK, SoldOutHistory);
        }
    }
}
