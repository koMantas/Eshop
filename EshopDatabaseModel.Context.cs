﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eshop
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EshopDBContext : DbContext
    {
        public EshopDBContext()
            : base("name=EshopDBContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Kategorija> Kategorija { get; set; }
        public virtual DbSet<Pirkejas> Pirkejas { get; set; }
        public virtual DbSet<Preke> Preke { get; set; }
        public virtual DbSet<Uzsakymas> Uzsakymas { get; set; }
        public virtual DbSet<Uzsakymo_info> Uzsakymo_info { get; set; }
    }
}
