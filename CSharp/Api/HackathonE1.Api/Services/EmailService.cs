using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface IEmailService
	{
		Task<bool> SendEmailAsync( string reciver, string subject, string content );
	}

	public class EmailService : IEmailService
	{
		private readonly HttpClient _http;
		private readonly string apiPath;
		private readonly string apiPassword;

		public EmailService( IConfiguration configuration )
		{
			apiPath = Environment.GetEnvironmentVariable( "EMAIL_API_PATH" ) ?? configuration["ExternalServicesConfig:EmailApiPath"];
			apiPassword = Environment.GetEnvironmentVariable( "EMAIL_API_PASSWORD" ) ?? configuration["ExternalServicesConfig:EmailApiPassword"];

			_http = new();
			_http.BaseAddress = new Uri( apiPath );
		}

		public async Task<bool> SendEmailAsync( string reciver, string subject, string content )
		{
			if ( string.IsNullOrEmpty( apiPath ) || string.IsNullOrEmpty( apiPassword ) )
			{
				return false;
			}

			EmailModel email = new()
			{
				Password = apiPassword,
				Receiver = reciver,
				Subject = subject,
				Content = content
			};

			var resp = await _http.PostAsJsonAsync( "/sendMessage", email );

			if ( !resp.IsSuccessStatusCode )
			{
				resp = await _http.PostAsJsonAsync( "/sendMessage", email );
			}

			return resp.IsSuccessStatusCode;
		}


		class EmailModel
		{
			[JsonPropertyName( "reciver" )]
			public string Receiver { get; set; }
			[JsonPropertyName( "subject" )]
			public string Subject { get; set; }
			[JsonPropertyName( "content" )]
			public string Content { get; set; }
			[JsonPropertyName( "password" )]
			public string Password { get; set; }
		}
	}
}
