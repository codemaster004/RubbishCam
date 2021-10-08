using HackathonE1.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Controllers
{
	[ApiController]
	[Route( "api/[controller]" )]
	public class ImageDetectionController : ExtendedControllerBase
	{
		private readonly ILogger<ImageDetectionController> _logger;
		private readonly IImageDetectionService _imageService;

		public ImageDetectionController( ILogger<ImageDetectionController> logger, IImageDetectionService imageService )
		{
			_logger = logger;
			_imageService = imageService;
		}

		[HttpPost]
		public async Task<IActionResult> CheckImage( IFormFile file )
		{
			using MemoryStream stream = new();

			//var file = files.FirstOrDefault();

			file.CopyTo( stream );
			stream.Position = 0;
			var arr = stream.ToArray();

			var result = await _imageService.CheckImageAsync( arr, file.ContentType );

			return Ok();
		}

	}
}
