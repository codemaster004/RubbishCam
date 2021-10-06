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
		public static async Task Main( string[] args )
		{
			var builder = WebAssemblyHostBuilder.CreateDefault( args );
			builder.RootComponents.Add<App>( "#app" );

			_ = builder.Services.AddAuthorizationCore();
			_ = builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();


			var http = new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) };
			EnvVars env = new() { Variables = await http.GetFromJsonAsync<Dictionary<string, string>>( "/api/Environment" ) };

			_ = builder.Services.AddSingleton( env );

			_ = builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

			await builder.Build().RunAsync();
		}
	}
}
