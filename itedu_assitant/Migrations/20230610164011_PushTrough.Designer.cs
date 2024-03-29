﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using itedu_assitant.DB;

#nullable disable

namespace itedu_assitant.Migrations
{
    [DbContext(typeof(dbcontext))]
    [Migration("20230610164011_PushTrough")]
    partial class PushTrough
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("itedu_assitant.Model.Base.Active", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<int>("IsActiveid")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastInsert")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("changeAmount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("changeAmount"));

                    b.HasKey("id");

                    b.HasIndex("IsActiveid");

                    b.ToTable("ContCurrentActive");
                });

            modelBuilder.Entity("itedu_assitant.Model.Base.AuthCodes", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("createddate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("id");

                    b.ToTable("oauthcode");
                });

            modelBuilder.Entity("itedu_assitant.Model.Base.Groups", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<DateTime>("created_date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("group_id")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("ContUserGroups");
                });

            modelBuilder.Entity("itedu_assitant.Model.Base.Instance", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("ApiToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("idInstance")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("ContUserInstance");
                });

            modelBuilder.Entity("itedu_assitant.Model.Base.ManagerNumbers", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("phoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("ContUserNumbers");
                });

            modelBuilder.Entity("itedu_assitant.Model.Base.Users", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("id"));

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserStatus")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("hasContact")
                        .HasColumnType("boolean");

                    b.Property<int>("userGroupid")
                        .HasColumnType("integer");

                    b.Property<string>("userPhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.HasIndex("userGroupid");

                    b.ToTable("ContUsers");
                });

            modelBuilder.Entity("itedu_assitant.Model.Base.Active", b =>
                {
                    b.HasOne("itedu_assitant.Model.Base.ManagerNumbers", "IsActive")
                        .WithMany()
                        .HasForeignKey("IsActiveid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IsActive");
                });

            modelBuilder.Entity("itedu_assitant.Model.Base.Users", b =>
                {
                    b.HasOne("itedu_assitant.Model.Base.Groups", "userGroup")
                        .WithMany()
                        .HasForeignKey("userGroupid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("userGroup");
                });
#pragma warning restore 612, 618
        }
    }
}
