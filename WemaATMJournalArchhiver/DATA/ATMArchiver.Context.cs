﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WemaATMJournalArchhiver.DATA
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ATMArchiverEntities : DbContext
    {
        public ATMArchiverEntities()
            : base("name=ATMArchiverEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AJ_AuditTrail> AJ_AuditTrail { get; set; }
        public DbSet<AJ_UsersTable> AJ_UsersTable { get; set; }
        public DbSet<atm_ip_address> atm_ip_address { get; set; }
        public DbSet<new_atm_log_path> new_atm_log_path { get; set; }
        public DbSet<runtime_errors> runtime_errors { get; set; }
        public DbSet<WemaBranch> WemaBranches { get; set; }
    }
}
