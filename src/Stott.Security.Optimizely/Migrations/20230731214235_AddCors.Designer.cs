﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stott.Security.Optimizely.Entities;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    [DbContext(typeof(CspDataContext))]
    [Migration("20230731214235_AddCors")]
    partial class AddCors
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.AuditHeader", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Actioned")
                        .HasColumnType("datetime2");

                    b.Property<string>("ActionedBy")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OperationType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecordType")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Actioned", "ActionedBy", "RecordType" }, "idx_CspAuditHeader_LookUp");

                    b.ToTable("tbl_CspAuditHeader");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.AuditProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuditHeaderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Field")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NewValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OldValue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "AuditHeaderId" }, "idx_CspAuditProperty_LookUp");

                    b.ToTable("tbl_CspAuditProperty");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.CorsSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AllowCredentials")
                        .HasColumnType("bit");

                    b.Property<string>("AllowHeaders")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AllowMethods")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AllowOrigins")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExposeHeaders")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<int>("MaxAge")
                        .HasColumnType("int");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("tbl_CorsSettings");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.CspSandbox", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAllowDownloadsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowDownloadsWithoutGestureEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowFormsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowModalsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowOrientationLockEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowPointerLockEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowPopupsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowPopupsToEscapeTheSandboxEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowPresentationEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowSameOriginEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowScriptsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowStorageAccessByUserEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowTopNavigationByUserEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowTopNavigationEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAllowTopNavigationToCustomProtocolEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSandboxEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("tbl_CspSandbox");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.CspSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReportOnly")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUpgradeInsecureRequestsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWhitelistEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WhitelistUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("tbl_CspSettings");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.CspSource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Directives")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("tbl_CspSource");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.CspViolationSummary", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BlockedUri")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("Instances")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastReported")
                        .HasColumnType("datetime2");

                    b.Property<string>("ViolatedDirective")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "BlockedUri", "ViolatedDirective" }, "idx_CspViolationSummary_LookUp");

                    b.ToTable("tbl_CspViolationSummary");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.SecurityHeaderSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CrossOriginEmbedderPolicy")
                        .HasColumnType("int");

                    b.Property<int>("CrossOriginOpenerPolicy")
                        .HasColumnType("int");

                    b.Property<int>("CrossOriginResourcePolicy")
                        .HasColumnType("int");

                    b.Property<bool>("ForceHttpRedirect")
                        .HasColumnType("bit");

                    b.Property<int>("FrameOptions")
                        .HasColumnType("int");

                    b.Property<bool>("IsStrictTransportSecurityEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStrictTransportSecuritySubDomainsEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReferrerPolicy")
                        .HasColumnType("int");

                    b.Property<int>("StrictTransportSecurityMaxAge")
                        .HasColumnType("int");

                    b.Property<int>("XContentTypeOptions")
                        .HasColumnType("int");

                    b.Property<int>("XssProtection")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("tbl_CspSecurityHeaderSettings");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.AuditProperty", b =>
                {
                    b.HasOne("Stott.Security.Optimizely.Entities.AuditHeader", "Header")
                        .WithMany("AuditProperties")
                        .HasForeignKey("AuditHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Header");
                });

            modelBuilder.Entity("Stott.Security.Optimizely.Entities.AuditHeader", b =>
                {
                    b.Navigation("AuditProperties");
                });
#pragma warning restore 612, 618
        }
    }
}
