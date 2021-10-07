using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

	public class EmailService : IEmailService, IDisposable
	{
		private readonly HttpClient _http;
		private readonly string apiPath;
		private readonly string apiPassword;
		private readonly ILogger<EmailService> _logger;

		public EmailService( IConfiguration configuration, ILogger<EmailService> logger )
		{
			apiPath = Environment.GetEnvironmentVariable( "EMAIL_API_PATH" ) ?? configuration["ExternalServicesConfig:EmailApiPath"];
			apiPassword = Environment.GetEnvironmentVariable( "EMAIL_API_PASSWORD" ) ?? configuration["ExternalServicesConfig:EmailApiPassword"];

			if ( string.IsNullOrEmpty( apiPath ) )
			{
				_logger.LogError( "Email api path not provided." );
			}
			if ( string.IsNullOrEmpty( apiPassword ) )
			{
				_logger.LogError( "Email api password not provided." );
			}

			_http = new();
			_http.BaseAddress = new Uri( apiPath );
			_logger = logger;
		}

		public async Task<bool> SendEmailAsync( string reciver, string subject, string content )
		{
			if ( string.IsNullOrEmpty( apiPath ) || string.IsNullOrEmpty( apiPassword ) )
			{
				_logger.LogError( "Email sending failed: api path or password not provided." );
				return false;
			}

			EmailModel email = new()
			{
				Password = apiPassword,
				Receiver = reciver,
				Subject = subject,
				Content = content
			};

			var resp = await _http.PostAsJsonAsync( "/send_mail", email );

			if ( !resp.IsSuccessStatusCode )
			{
				_logger.LogError( "Email sending failed." );
				resp = await _http.PostAsJsonAsync( "/send_mail", email );
			}

			return resp.IsSuccessStatusCode;
		}


		class EmailModel
		{
			[JsonPropertyName( "receiver" )]
			public string Receiver { get; set; }
			[JsonPropertyName( "subject" )]
			public string Subject { get; set; }
			[JsonPropertyName( "content" )]
			public string Content { get; set; }
			[JsonPropertyName( "password" )]
			public string Password { get; set; }
		}

		public void Dispose()
		{
			( _http as IDisposable ).Dispose();
			GC.SuppressFinalize( this );
		}
	}
}
