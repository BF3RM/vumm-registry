﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Migrations
{
    [DbContext(typeof(RegistryContext))]
    partial class RegistryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("VUModManagerRegistry.Models.AccessToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("Token")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Token");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("VUModManagerRegistry.Models.Mod", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Mods");
                });

            modelBuilder.Entity("VUModManagerRegistry.Models.ModVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<string>("Dependencies")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<long>("ModId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Tag")
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ModId");

                    b.HasIndex("Name", "Version")
                        .IsUnique();

                    b.ToTable("ModVersions");
                });

            modelBuilder.Entity("VUModManagerRegistry.Models.ModVersion", b =>
                {
                    b.HasOne("VUModManagerRegistry.Models.Mod", null)
                        .WithMany("Versions")
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VUModManagerRegistry.Models.Mod", b =>
                {
                    b.Navigation("Versions");
                });
#pragma warning restore 612, 618
        }
    }
}
