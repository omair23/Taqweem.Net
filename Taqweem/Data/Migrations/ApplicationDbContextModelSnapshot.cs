﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Taqweem.Data;

namespace Taqweem.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Taqweem")
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Taqweem.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int>("ActiveStatus");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(38);

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FullName");

                    b.Property<bool>("IsSuperUser");

                    b.Property<DateTime>("LastLogin");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("MasjidId");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("ShowDetails");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<DateTime?>("UpdatedAt");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("MasjidId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Taqweem.Models.Configuration", b =>
                {
                    b.Property<long>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Parameter");

                    b.Property<string>("Value");

                    b.HasKey("UID");

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("Taqweem.Models.Currency", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38);

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<double>("ConversionRate");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(38);

                    b.Property<string>("Flag");

                    b.Property<string>("FractionalUnit");

                    b.Property<string>("Locations");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberToBasic");

                    b.Property<int>("Style");

                    b.Property<string>("Symbol");

                    b.Property<long>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Currency","World");
                });

            modelBuilder.Entity("Taqweem.Models.CurrencyHistory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38);

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<double>("ConversionRate");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("DateTimeStamp");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(38);

                    b.Property<long>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("CurrencyHistory","World");
                });

            modelBuilder.Entity("Taqweem.Models.Masjid", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38);

                    b.Property<string>("Address");

                    b.Property<bool>("AllowRegistration");

                    b.Property<string>("Contact");

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(38);

                    b.Property<string>("GeneralInfo");

                    b.Property<double>("Height");

                    b.Property<bool>("IsPublicHolidaySpecialTimesEnabled");

                    b.Property<bool>("IsSpecialDayEnabled");

                    b.Property<bool>("JummahFacility");

                    b.Property<int>("JuristMethod");

                    b.Property<bool>("LadiesFacility");

                    b.Property<DateTime>("LastUpdate");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<int>("MaghribAdhaanDelay");

                    b.Property<string>("Name");

                    b.Property<int>("OldSiteId");

                    b.Property<int>("SalaahTimesType");

                    b.Property<int>("SpecialDayNumber");

                    b.Property<string>("TZTimeZone");

                    b.Property<double>("TimeZoneDiff");

                    b.Property<string>("TimeZoneId");

                    b.Property<string>("Town");

                    b.Property<long>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("TimeZoneId");

                    b.ToTable("Masjid");
                });

            modelBuilder.Entity("Taqweem.Models.Notice", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedId")
                        .HasMaxLength(38);

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(38);

                    b.Property<bool>("IsHidden");

                    b.Property<string>("MasjidId");

                    b.Property<string>("NoticeContent");

                    b.Property<long>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("CreatedId");

                    b.HasIndex("MasjidId");

                    b.ToTable("Notice");
                });

            modelBuilder.Entity("Taqweem.Models.PublicHoliday", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38);

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("DayOfHoliday");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(38);

                    b.Property<string>("NameOfHoliday");

                    b.Property<long>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("PublicHoliday","World");
                });

            modelBuilder.Entity("Taqweem.Models.SalaahTime", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(38);

                    b.Property<DateTime>("AsrAdhaan");

                    b.Property<DateTime>("AsrSalaah");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("DayNumber");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(38);

                    b.Property<DateTime>("DhuhrAdhaan");

                    b.Property<DateTime>("DhuhrSalaah");

                    b.Property<DateTime>("FajrAdhaan");

                    b.Property<DateTime>("FajrSalaah");

                    b.Property<bool>("IsATimeChange");

                    b.Property<DateTime>("IshaAdhaan");

                    b.Property<DateTime>("IshaSalaah");

                    b.Property<DateTime>("JumuahAdhaan");

                    b.Property<DateTime>("JumuahSalaah");

                    b.Property<string>("MasjidId");

                    b.Property<DateTime>("SpecialDhuhrAdhaan");

                    b.Property<DateTime>("SpecialDhuhrSalaah");

                    b.Property<DateTime>("TimeDate");

                    b.Property<int>("Type");

                    b.Property<long>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("MasjidId");

                    b.ToTable("SalaahTime");
                });

            modelBuilder.Entity("Taqweem.Models.TimeZone", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DaylightName");

                    b.Property<double>("DefaultUTCDifference");

                    b.Property<string>("DisplayName");

                    b.Property<string>("StandardName");

                    b.Property<bool>("SupportsDaylightSavingTime");

                    b.HasKey("Id");

                    b.ToTable("TimeZone");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Taqweem.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Taqweem.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Taqweem.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Taqweem.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Taqweem.Models.ApplicationUser", b =>
                {
                    b.HasOne("Taqweem.Models.Masjid", "Masjid")
                        .WithMany("Users")
                        .HasForeignKey("MasjidId");
                });

            modelBuilder.Entity("Taqweem.Models.Masjid", b =>
                {
                    b.HasOne("Taqweem.Models.TimeZone", "TimeZone")
                        .WithMany()
                        .HasForeignKey("TimeZoneId");
                });

            modelBuilder.Entity("Taqweem.Models.Notice", b =>
                {
                    b.HasOne("Taqweem.Models.ApplicationUser", "Created")
                        .WithMany()
                        .HasForeignKey("CreatedId");

                    b.HasOne("Taqweem.Models.Masjid", "Masjid")
                        .WithMany("Notices")
                        .HasForeignKey("MasjidId");
                });

            modelBuilder.Entity("Taqweem.Models.SalaahTime", b =>
                {
                    b.HasOne("Taqweem.Models.Masjid")
                        .WithMany("SalaahTimes")
                        .HasForeignKey("MasjidId");
                });
#pragma warning restore 612, 618
        }
    }
}
