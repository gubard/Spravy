﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spravy.PasswordGenerator.Db.Contexts;

#nullable disable

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations
{
    [DbContext(typeof(PasswordDbContext))]
    partial class PasswordDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("Spravy.PasswordGenerator.Db.Models.PasswordItemEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomAvailableCharacters")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAvailableLowerLatin")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAvailableNumber")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAvailableSpecialSymbols")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAvailableUpperLatin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ushort>("Length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Regex")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("PasswordItems", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
