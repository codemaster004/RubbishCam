﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RubbishCam.Data;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
	[DbContext( typeof( AppDbContext ) )]
	[Migration( "20211110172431_InitialCreate" )]
	partial class InitialCreate
	{
		protected override void BuildTargetModel( ModelBuilder modelBuilder )
		{
#pragma warning disable 612, 618
			modelBuilder
				.HasAnnotation( "ProductVersion", "6.0.0" )
				.HasAnnotation( "Relational:MaxIdentifierLength", 63 );

			NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns( modelBuilder );
#pragma warning restore 612, 618
		}
	}
}
