﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using rise.Data;

namespace risedashboard.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190430175814_RollHundred")]
    partial class RollHundred
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

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
                        .ValueGeneratedOnAdd();

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

            modelBuilder.Entity("rise.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("rise.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<DateTime>("LastMessage");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<int>("MessageCount");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("Photo_Url");

                    b.Property<string>("PublicKey");

                    b.Property<string>("Role");

                    b.Property<string>("Secret");

                    b.Property<string>("SecurityStamp");

                    b.Property<long>("TelegramId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("rise.Models.CoinQuote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Exchange");

                    b.Property<double>("Price");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<double>("USDPrice");

                    b.Property<double>("Volume");

                    b.HasKey("Id");

                    b.HasIndex("Exchange", "TimeStamp");

                    b.ToTable("CoinQuotes");
                });

            modelBuilder.Entity("rise.Models.DelegateForm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Contact");

                    b.Property<string>("Contact_type");

                    b.Property<bool>("Fees_covered");

                    b.Property<double>("Min_payout");

                    b.Property<string>("Notes");

                    b.Property<string>("Payout_address");

                    b.Property<int>("Payout_interval");

                    b.Property<double>("Share");

                    b.HasKey("Id");

                    b.ToTable("DelegateForms");
                });

            modelBuilder.Entity("rise.Models.IPData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("city");

                    b.Property<string>("continent_code");

                    b.Property<string>("continent_name");

                    b.Property<string>("country_code");

                    b.Property<string>("country_name");

                    b.Property<string>("ip");

                    b.Property<double>("latitude");

                    b.Property<int?>("locationId");

                    b.Property<double>("longitude");

                    b.Property<string>("region_code");

                    b.Property<string>("region_name");

                    b.Property<string>("type");

                    b.Property<string>("zip");

                    b.HasKey("Id");

                    b.HasIndex("locationId");

                    b.ToTable("IPData");
                });

            modelBuilder.Entity("rise.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("LocationId");

                    b.Property<string>("code");

                    b.Property<string>("name");

                    b.Property<string>("native");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("rise.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("calling_code");

                    b.Property<string>("capital");

                    b.Property<string>("country_flag");

                    b.Property<string>("country_flag_emoji");

                    b.Property<string>("country_flag_emoji_unicode");

                    b.Property<int?>("geoname_id");

                    b.Property<bool>("is_eu");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("rise.Models.RollHundredRecord", b =>
                {
                    b.Property<int>("RollHundredRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<double>("AmountPaid");

                    b.Property<int>("LuckyNumber");

                    b.Property<double>("Multiplier");

                    b.Property<int>("Options");

                    b.Property<int>("PickedNumber");

                    b.Property<DateTime>("Time");

                    b.Property<bool>("TransactionResult");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<bool>("Winner");

                    b.HasKey("RollHundredRecordId");

                    b.HasIndex("UserId");

                    b.ToTable("RollHundredRecords");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("rise.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("rise.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("rise.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("rise.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("rise.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("rise.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("rise.Models.IPData", b =>
                {
                    b.HasOne("rise.Models.Location", "location")
                        .WithMany()
                        .HasForeignKey("locationId");
                });

            modelBuilder.Entity("rise.Models.Language", b =>
                {
                    b.HasOne("rise.Models.Location")
                        .WithMany("languages")
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("rise.Models.RollHundredRecord", b =>
                {
                    b.HasOne("rise.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
