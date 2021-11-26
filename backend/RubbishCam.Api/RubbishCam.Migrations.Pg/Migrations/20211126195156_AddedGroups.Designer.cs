﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RubbishCam.Data;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211126195156_AddedGroups")]
    partial class AddedGroups
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RoleModelUserModel", b =>
                {
                    b.Property<int>("RolesId")
                        .HasColumnType("integer");

                    b.Property<int>("UsersId")
                        .HasColumnType("integer");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("RoleModelUserModel");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.FriendshipModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Accepted")
                        .HasColumnType("boolean");

                    b.Property<string>("InitiatorUuid")
                        .IsRequired()
                        .HasColumnType("character varying(24)");

                    b.Property<bool>("Rejected")
                        .HasColumnType("boolean");

                    b.Property<string>("TargetUuid")
                        .IsRequired()
                        .HasColumnType("character varying(24)");

                    b.HasKey("Id");

                    b.HasIndex("InitiatorUuid");

                    b.HasIndex("TargetUuid");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.GroupModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.PointModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateScored")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("UserUuid")
                        .IsRequired()
                        .HasColumnType("character varying(24)");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserUuid");

                    b.ToTable("Points");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.RoleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.TokenModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Revoked")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserUuid")
                        .IsRequired()
                        .HasColumnType("character varying(24)");

                    b.Property<DateTimeOffset>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("UserUuid");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Uuid")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RubbishCam.Domain.Relations.GroupMembersRelation", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<string>("UserUuid")
                        .HasColumnType("character varying(24)");

                    b.Property<bool>("IsOwner")
                        .HasColumnType("boolean");

                    b.HasKey("GroupId", "UserUuid");

                    b.HasIndex("UserUuid");

                    b.ToTable("GroupsMembers");
                });

            modelBuilder.Entity("RubbishCam.Domain.Relations.GroupPointsRelation", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("PointId")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "PointId");

                    b.HasIndex("PointId");

                    b.ToTable("GroupPointsRelation");
                });

            modelBuilder.Entity("RoleModelUserModel", b =>
                {
                    b.HasOne("RubbishCam.Domain.Models.RoleModel", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RubbishCam.Domain.Models.UserModel", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.FriendshipModel", b =>
                {
                    b.HasOne("RubbishCam.Domain.Models.UserModel", "Initiator")
                        .WithMany("InitiatedFriendships")
                        .HasForeignKey("InitiatorUuid")
                        .HasPrincipalKey("Uuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RubbishCam.Domain.Models.UserModel", "Target")
                        .WithMany("TargetingFriendships")
                        .HasForeignKey("TargetUuid")
                        .HasPrincipalKey("Uuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Initiator");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.PointModel", b =>
                {
                    b.HasOne("RubbishCam.Domain.Models.UserModel", "User")
                        .WithMany("Points")
                        .HasForeignKey("UserUuid")
                        .HasPrincipalKey("Uuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.TokenModel", b =>
                {
                    b.HasOne("RubbishCam.Domain.Models.UserModel", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserUuid")
                        .HasPrincipalKey("Uuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RubbishCam.Domain.Relations.GroupMembersRelation", b =>
                {
                    b.HasOne("RubbishCam.Domain.Models.GroupModel", "Group")
                        .WithMany("MembersR")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RubbishCam.Domain.Models.UserModel", "User")
                        .WithMany("GroupsR")
                        .HasForeignKey("UserUuid")
                        .HasPrincipalKey("Uuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RubbishCam.Domain.Relations.GroupPointsRelation", b =>
                {
                    b.HasOne("RubbishCam.Domain.Models.GroupModel", "Group")
                        .WithMany("PointsR")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RubbishCam.Domain.Models.PointModel", "Point")
                        .WithMany("GroupsR")
                        .HasForeignKey("PointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Point");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.GroupModel", b =>
                {
                    b.Navigation("MembersR");

                    b.Navigation("PointsR");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.PointModel", b =>
                {
                    b.Navigation("GroupsR");
                });

            modelBuilder.Entity("RubbishCam.Domain.Models.UserModel", b =>
                {
                    b.Navigation("GroupsR");

                    b.Navigation("InitiatedFriendships");

                    b.Navigation("Points");

                    b.Navigation("TargetingFriendships");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
