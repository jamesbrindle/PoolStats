﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PoolStats.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PoolStatsDBEntities1 : DbContext
    {
        public PoolStatsDBEntities1()
            : base("name=PoolStatsDBEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<FourPlayer> FourPlayers { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<TwoPlayer> TwoPlayers { get; set; }
        public virtual DbSet<Pin> Pins { get; set; }
    }
}
