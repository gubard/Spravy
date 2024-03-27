﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spravy.PasswordGenerator.Db.Contexts;

#nullable disable

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations
{
    [DbContext(typeof(UserSecretDbContext))]
    partial class UserSecretDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("Spravy.PasswordGenerator.Db.Models.UserSecretEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Secret")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserSecrets", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
