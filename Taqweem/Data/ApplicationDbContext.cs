﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taqweem.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Taqweem.ViewModels.ManageViewModels;

namespace Taqweem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Models.TimeZone> TimeZones { get; set; }

        public DbSet<Masjid> Masjids { get; set; }

        public DbSet<SalaahTime> SalaahTimes { get; set; }

        public DbSet<Notice> Notices { get; set; }

        public DbSet<Configuration> Configurations { get; set; }

        public DbSet<PublicHoliday> PublicHolidays { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<CurrencyHistory> CurrencyHistory { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Models.TimeZone>().ToTable("TimeZone");

            builder.Entity<Models.Currency>().ToTable("Currency");

            builder.Entity<Models.CurrencyHistory>().ToTable("CurrencyHistory");

            builder.Entity<Masjid>().ToTable("Masjid");

            builder.Entity<Masjid>().Property(u => u.UID).ValueGeneratedOnAdd();//.UseSqlServerIdentityColumn();

            builder.Entity<Masjid>().Property(p => p.UID).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<SalaahTime>().ToTable("SalaahTime");

            //builder.Entity<SalaahTime>().HasKey(c => c.UID);

            //builder.Entity<SalaahTime>().Property(u => u.UID).UseSqlServerIdentityColumn();

            //builder.Entity<SalaahTime>().Property(p => p.UID).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            builder.Entity<Notice>().ToTable("Notice");

            builder.Entity<PublicHoliday>().ToTable("PublicHoliday");

            builder.Entity<Notice>().Property(u => u.UID).ValueGeneratedOnAdd();// .UseSqlServerIdentityColumn();

            builder.Entity<Notice>().Property(p => p.UID).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Entity<Configuration>().ToTable("Configuration");

            base.OnModelCreating(builder);
            {
                builder.HasDefaultSchema("Taqweem");
            }
        }
    }
}
