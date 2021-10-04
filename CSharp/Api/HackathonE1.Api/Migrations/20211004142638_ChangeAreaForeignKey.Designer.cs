﻿// <auto-generated />
using HackathonE1.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HackathonE1.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211004142638_ChangeAreaForeignKey")]
    partial class ChangeAreaForeignKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("HackathonE1.Domain.Models.IssueModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("Issues");
                });

            modelBuilder.Entity("HackathonE1.Domain.Models.IssueTypeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.HasKey("Id");

                    b.ToTable("IssueTypes");
                });

            modelBuilder.Entity("HackathonE1.Domain.Models.ObservedAreaModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Radius")
                        .HasColumnType("double precision");

                    b.Property<string>("UserIdentifier")
                        .IsRequired()
                        .HasColumnType("character varying(24)");

                    b.HasKey("Id");

                    b.HasIndex("UserIdentifier");

                    b.ToTable("ObservedAreas");
                });

            modelBuilder.Entity("HackathonE1.Domain.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(88)
                        .HasColumnType("character varying(88)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Identifier")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HackathonE1.Domain.Models.IssueModel", b =>
                {
                    b.HasOne("HackathonE1.Domain.Models.IssueTypeModel", "Type")
                        .WithMany("Issues")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");
                });

            modelBuilder.Entity("HackathonE1.Domain.Models.ObservedAreaModel", b =>
                {
                    b.HasOne("HackathonE1.Domain.Models.UserModel", "User")
                        .WithMany("ObservedAreas")
                        .HasForeignKey("UserIdentifier")
                        .HasPrincipalKey("Identifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("HackathonE1.Domain.Models.IssueTypeModel", b =>
                {
                    b.Navigation("Issues");
                });

            modelBuilder.Entity("HackathonE1.Domain.Models.UserModel", b =>
                {
                    b.Navigation("ObservedAreas");
                });
#pragma warning restore 612, 618
        }
    }
}
