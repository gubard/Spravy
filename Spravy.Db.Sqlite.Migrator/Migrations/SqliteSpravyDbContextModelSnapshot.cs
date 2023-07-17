﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spravy.Db.Sqlite.Migrator;

#nullable disable

namespace Spravy.Db.Sqlite.Migrator.Migrations
{
    [DbContext(typeof(SqliteSpravyDbContext))]
    partial class SqliteSpravyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.8");

            modelBuilder.Entity("Spravy.Db.Models.ToDoItemEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<uint>("CompletedCount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<uint>("FailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<uint>("OrderIndex")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<uint>("SkippedCount")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("TypeOfPeriodicity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("ToDoItem", (string)null);
                });

            modelBuilder.Entity("Spravy.Db.Models.ToDoItemEntity", b =>
                {
                    b.HasOne("Spravy.Db.Models.ToDoItemEntity", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });
#pragma warning restore 612, 618
        }
    }
}
