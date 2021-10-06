using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Website.Server.Controllers
{
	[Route( "api/[controller]" )]
	[ApiController]
	public class EnvironmentController : ControllerBase
	{
		[HttpGet( "{name}" )]
		public async Task<IActionResult> GetVariable( string name )
		{
			await Task.CompletedTask;

			string value = Environment.GetEnvironmentVariable( $"CLIENT:{name}" );
			if ( value is null )
			{
				return NotFound();
			}

			return Ok( value );
		}

		[HttpGet]
		public async Task<IActionResult> GetVariables()
		{
			await Task.CompletedTask;

			var vars = Environment.GetEnvironmentVariables()
				.Cast<DictionaryEntry>()
				.Where( x => ( x.Key as string ).StartsWith( "CLIENT_" ) )
				.ToDictionary( x =>
					{
						var value = x.Key as string;
						return value["CLIENT_".Length..];
					}, 
					x => x.Value as string );

			return Ok( vars );
		}
	}
}
