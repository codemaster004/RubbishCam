using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HackathonE1.Api
{
	public class Program
	{
		public static void Main( string[] args )
		{
			CreateHostBuilder( args ).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 _ = webBuilder.UseStartup<Startup>();

					 _ = webBuilder.UseKestrel( options =>
					 {
						 options.Listen( IPAddress.Any, GetKestrelPort()/*, options => options.UseHttps()*/ );
					 } );

					 _ = webBuilder.UseIIS();
				 } );


		public static int GetKestrelPort()
		{
			if ( int.TryParse( Environment.GetEnvironmentVariable( "PORT" ), out int port ) )
			{
				return port;
			}
			if ( int.TryParse( Environment.GetEnvironmentVariable( "ASPNETCORE_URLS" ), out port ) )
			{
				return port;
			}

			return 5001;
		}

	}
}
