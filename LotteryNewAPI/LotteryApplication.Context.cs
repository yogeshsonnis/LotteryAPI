﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LotteryNewAPI
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LotteryBlankDatabaseEntities : DbContext
    {
        public LotteryBlankDatabaseEntities()
            : base("name=LotteryBlankDatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Box_Master> Box_Master { get; set; }
        public virtual DbSet<Existing_Inventory> Existing_Inventory { get; set; }
        public virtual DbSet<New_Inventory> New_Inventory { get; set; }
        public virtual DbSet<tblActivated_Tickets> tblActivated_Tickets { get; set; }
        public virtual DbSet<tblBarcode_Format> tblBarcode_Format { get; set; }
        public virtual DbSet<tblBox> tblBoxes { get; set; }
        public virtual DbSet<tblClose_Box> tblClose_Box { get; set; }
        public virtual DbSet<tblDeactivated> tblDeactivateds { get; set; }
        public virtual DbSet<tblEmployee_Details> tblEmployee_Details { get; set; }
        public virtual DbSet<tblLogin_Details> tblLogin_Details { get; set; }
        public virtual DbSet<tblLottery_Inventory> tblLottery_Inventory { get; set; }
        public virtual DbSet<tblRecievedTicket> tblRecievedTickets { get; set; }
        public virtual DbSet<tblReturnticket> tblReturntickets { get; set; }
        public virtual DbSet<tblSettleTicket> tblSettleTickets { get; set; }
        public virtual DbSet<tblShift> tblShifts { get; set; }
        public virtual DbSet<tblSoldout> tblSoldouts { get; set; }
        public virtual DbSet<tblSoldOut_History> tblSoldOut_History { get; set; }
        public virtual DbSet<tblStore_Info> tblStore_Info { get; set; }
        public virtual DbSet<tblStoreEmployeeInfo> tblStoreEmployeeInfoes { get; set; }
        public virtual DbSet<tblTerminal_Data1> tblTerminal_Data1 { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<TicketMaster> TicketMasters { get; set; }
    }
}
