﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RemoteWorkScheduler.DbContexts;

#nullable disable

namespace RemoteWorkScheduler.Migrations
{
    [DbContext(typeof(RemoteWorkSchedulerContext))]
    [Migration("20240919073318_Migration2_2")]
    partial class Migration2_2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("RemoteWorkScheduler.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("TEXT")
                        .HasColumnName("team_id");

                    b.Property<int>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("INTEGER")
                        .HasColumnName("title");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("t_employees", (string)null);
                });

            modelBuilder.Entity("RemoteWorkScheduler.Entities.RemoteLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT")
                        .HasColumnName("date");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("TEXT")
                        .HasColumnName("employee_id");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("t_remote_logs", (string)null);
                });

            modelBuilder.Entity("RemoteWorkScheduler.Entities.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("t_teams", (string)null);
                });

            modelBuilder.Entity("RemoteWorkScheduler.Entities.Employee", b =>
                {
                    b.HasOne("RemoteWorkScheduler.Entities.Team", "Team")
                        .WithMany("Employees")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("RemoteWorkScheduler.Entities.RemoteLog", b =>
                {
                    b.HasOne("RemoteWorkScheduler.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("RemoteWorkScheduler.Entities.Team", b =>
                {
                    b.Navigation("Employees");
                });
#pragma warning restore 612, 618
        }
    }
}
