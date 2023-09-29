﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spravy.EventBus.Db.Contexts;

#nullable disable

namespace Spravy.EventBus.Db.Sqlite.Migrator.Migrations
{
    [DbContext(typeof(SpravyDbEventBusDbContext))]
    partial class SpravyEventBusDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Spravy.EventBus.Db.Models.EventEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<Guid>("EventId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Event", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
