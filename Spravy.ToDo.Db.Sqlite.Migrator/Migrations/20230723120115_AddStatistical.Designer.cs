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
    [Migration("20230723120115_AddStatistical")]
    partial class AddStatistical
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

                    b.Property<DateTimeOffset>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCurrent")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<uint>("OrderIndex")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("StatisticalId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ValueId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GroupId")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.HasIndex("StatisticalId")
                        .IsUnique();

                    b.HasIndex("ValueId")
                        .IsUnique();

                    b.ToTable("ToDoItem", (string)null);
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemGroupEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ToDoItemGroupEntity");
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemStatisticalEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<uint>("CompletedCount")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("FailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<uint>("SkippedCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ToDoItemStatisticalEntity");
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemValueEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<byte>("TypeOfPeriodicity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ToDoItemValueEntity");
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemEntity", b =>
                {
                    b.HasOne("Spravy.ToDo.Db.Models.ToDoItemGroupEntity", "Group")
                        .WithOne("Item")
                        .HasForeignKey("Spravy.ToDo.Db.Models.ToDoItemEntity", "GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Spravy.ToDo.Db.Models.ToDoItemEntity", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("Spravy.ToDo.Db.Models.ToDoItemStatisticalEntity", "Statistical")
                        .WithOne("Item")
                        .HasForeignKey("Spravy.ToDo.Db.Models.ToDoItemEntity", "StatisticalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Spravy.ToDo.Db.Models.ToDoItemValueEntity", "Value")
                        .WithOne("Item")
                        .HasForeignKey("Spravy.ToDo.Db.Models.ToDoItemEntity", "ValueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Parent");

                    b.Navigation("Statistical");

                    b.Navigation("Value");
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemGroupEntity", b =>
                {
                    b.Navigation("Item")
                        .IsRequired();
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemStatisticalEntity", b =>
                {
                    b.Navigation("Item")
                        .IsRequired();
                });

            modelBuilder.Entity("Spravy.ToDo.Db.Models.ToDoItemValueEntity", b =>
                {
                    b.Navigation("Item")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
