﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Data;

#nullable disable

namespace Project.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240709074831_add_user_migration")]
    partial class add_user_migration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Project.Models.Favorite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Movie_id")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_poster")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Project.Models.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Movie_id")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_poster")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Rating_value")
                        .HasColumnType("double");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("Project.Models.Recommendation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Movie_id")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_poster")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Recommendations");
                });

            modelBuilder.Entity("Project.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Movie_id")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_poster")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Project.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Project.Models.Watchlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Movie_id")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_poster")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Movie_title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Watchlists");
                });
#pragma warning restore 612, 618
        }
    }
}
