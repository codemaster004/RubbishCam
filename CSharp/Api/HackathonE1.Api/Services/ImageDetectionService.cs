using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface IImageDetectionService
	{
		Task<bool> CheckImageAsync( byte[] content, string mediatype );
	}

	public class ImageDetectionService : IImageDetectionService, IDisposable
	{
		private readonly HttpClient _http;
		private readonly string apiPath;
		private readonly ILogger<ImageDetectionService> _logger;

		public ImageDetectionService( IConfiguration configuration, ILogger<ImageDetectionService> logger )
		{
			apiPath = Environment.GetEnvironmentVariable( "IMG_AI_API_PATH" ) ?? configuration["ExternalServicesConfig:ImgAiApiPath"];

			if ( string.IsNullOrEmpty( apiPath ) )
			{
				_logger.LogError( "Image ai api path not provided." );
			}

			_http = new();
			_http.BaseAddress = new Uri( apiPath );
			_logger = logger;
		}

		public async Task<bool> CheckImageAsync( byte[] content, string mediatype )
		{
			if ( string.IsNullOrEmpty( apiPath ) )
			{
				_logger.LogError( "Image ai sending failed: api path or password not provided." );
				return false;
			}

			using var fileContent = new ByteArrayContent( content );
			fileContent.Headers.ContentType = new MediaTypeHeaderValue( mediatype );

			using var formData = new MultipartFormDataContent
			{
				{ fileContent, "file" }
			};

			var resp = await _http.PostAsync( "/upload", formData );

			return resp.IsSuccessStatusCode;
		}

		public void Dispose()
		{
			( _http as IDisposable ).Dispose();
			GC.SuppressFinalize( this );
		}
	}
}
