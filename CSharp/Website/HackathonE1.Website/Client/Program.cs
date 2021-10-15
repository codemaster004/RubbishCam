using HackathonE1.Website.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Website.Client
{
	public class Program
	{
		private static EnvVars environmentVars;

		public static async Task Main( string[] args )
		{
			var builder = WebAssemblyHostBuilder.CreateDefault( args );
			builder.RootComponents.Add<App>( "#app" );

			_ = builder.Services.AddAuthorizationCore();
			_ = builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
			_ = builder.Services.AddScoped<ILoginManager>( provider => provider.GetService<AuthenticationStateProvider>() as JwtAuthenticationStateProvider );

			try
			{
				var http = new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) };
				environmentVars = new() { Variables = await http.GetFromJsonAsync<Dictionary<string, string>>( "/api/Environment" ) };
			}
			catch ( Exception )
			{
				environmentVars = new() { Variables = new() };
			}


			_ = builder.Services.AddSingleton( environmentVars );
			_ = builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

			_ = builder.Services.AddHttpClient( "api", http =>
			{
				http.BaseAddress = new( environmentVars["API_PATH"] );
			} );


			await builder.Build().RunAsync();
		}
	}
}
