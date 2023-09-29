﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spravy.ToDo.Db.Sqlite.Migrator;
using Spravy.ToDo.Db.Contexts;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    [DbContext(typeof(SpravyDbToDoDbContext))]
    [Migration("20230811143221_AddLastCompleted")]
    partial class AddLastCompleted
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.9");

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<uint>("CompletedCount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("DaysOfMonth")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("1");

                    b.Property<string>("DaysOfWeek")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("Monday");

                    b.Property<string>("DaysOfYear")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("1.1");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<uint>("FailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCurrent")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("LastCompleted")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<uint>("OrderIndex")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<uint>("SkippedCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("TypeOfPeriodicity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("ToDoItem", (string)null);
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemEntity", b =>
                {
                    b.HasOne("Spravy.ToDo.Db.Models.ToDoItemEntity", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });
#pragma warning restore 612, 618
        }
    }
}
