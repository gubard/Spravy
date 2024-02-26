﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spravy.Authentication.Db.Contexts;

#nullable disable

namespace Spravy.Authentication.Db.Sqlite.Migrator.Migrations
{
    [DbContext(typeof(SpravyDbAuthenticationDbContext))]
    partial class SqliteSpravyAuthenticationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("Spravy.Authentication.Db.Models.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("HashMethod")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Login")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Salt")
                        .HasColumnType("TEXT");

                    b.Property<string>("VerificationCodeHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("VerificationCodeMethod")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
