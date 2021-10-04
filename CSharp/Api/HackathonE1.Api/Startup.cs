using HackathonE1.Api.Data;
using HackathonE1.Api.Helpers;
using HackathonE1.Api.Hubs.Notification;
using HackathonE1.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api
{
	public class Startup
	{
		public Startup( IConfiguration configuration )
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices( IServiceCollection services )
		{

			_ = services.AddControllers();
			_ = services.AddSignalR();

			_ = services.AddSwaggerGen( c =>
			   {
				   c.SwaggerDoc( "v1", new OpenApiInfo { Title = "HackathonE1.Api", Version = "v1" } );
				   c.AddSecurityDefinition( "Bearer", new OpenApiSecurityScheme
				   {
					   Description = "JWT Authorization header. Example: 'Authorization: Bearer {token}'",
					   Name = "Authorization",
					   In = ParameterLocation.Header,
					   Type = SecuritySchemeType.ApiKey
				   } );
			   } );

			_ = services.AddAuthentication( JwtHelper.ConfigureAuthOptions )
				.AddJwtBearer( options => JwtHelper.ConfigureJwtOptions( options, Configuration ) );


			var connectionString = GetConnectionString();
			_ = services.AddDbContext<AppDbContext>( options =>
			{
				_ = options.UseNpgsql( connectionString );
				_ = options.UseLoggerFactory( LoggerFactory.Create( builder => builder.AddConsole() ) );
			} );

			_ = services.AddSingleton<JwtHelper>();

			_ = services.AddScoped<IJwtService, JwtService>();
			_ = services.AddScoped<IUsersService, UsersService>();
			_ = services.AddScoped<IIssuesService, IssuesService>();
			_ = services.AddScoped<IIssueTypesService, IssueTypesService>();
			_ = services.AddScoped<INotificationsService, NotificationsService>();
			_ = services.AddScoped<IObservedAreasService, ObservedAreasService>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			app.UseStaticFiles();

			if ( env.IsDevelopment() )
			{
				_ = app.UseDeveloperExceptionPage();
			}

			_ = app.UseSwagger();
			_ = app.UseSwaggerUI( c =>
			{
				c.SwaggerEndpoint( "/swagger/v1/swagger.json", "HackathonE1.Api v1" );
				c.InjectStylesheet( "/swagger-ui/SwaggerDark.css" );
			} );

			_ = app.UseHttpsRedirection();

			_ = app.UseRouting();

			_ = app.UseAuthorization();
			_ = app.UseAuthentication();

			_ = app.UseEndpoints( endpoints =>
			   {
				   _ = endpoints.MapControllers();
				   _ = endpoints.MapHub<NotificationsHub>( "/notifications" );
			   } );
		}

		private string GetConnectionString()
		{
			string url = Environment.GetEnvironmentVariable( "DATABASE_URL" );
			if ( string.IsNullOrEmpty( url ) )
			{
				string str = Configuration.GetConnectionString( "postgresConnection" );
				if ( string.IsNullOrEmpty( str ) )
				{
					return null;
				}

				return str;
			}

			Uri uri = new( url );

			var host = uri.Host;
			var database = uri.Segments[1].Trim( '/' );

			var userinfo = uri.UserInfo.Split( ':' );
			var username = userinfo[0];
			var password = userinfo[1];

			return $"Host={host};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
		}

	}
}
