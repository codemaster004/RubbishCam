using HackathonE1.Api.Data;
using HackathonE1.Api.Services;
using HackathonE1.Domain.Dtos.ObservedArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Controllers
{
	[ApiController]
	[Authorize]
	[Route( "api/[controller]" )]
	public class ObservedAreasController : ExtendedControllerBase
	{
		private readonly ILogger<ObservedAreasController> _logger;
		private readonly IObservedAreasService _areasService;

		public ObservedAreasController( IObservedAreasService areasService, ILogger<ObservedAreasController> logger )
		{
			_areasService = areasService;
			_logger = logger;
		}

		[HttpGet()]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetObservedAreaDto[]>> GetAreas()
		{
			_logger.LogInformation( $"User {UserName} requested all observed areas." );
			return await _areasService.GetAreasAsync();
		}

		[HttpGet( "owned" )]
		public async Task<ActionResult<GetObservedAreaDto[]>> GetOwnedAreas()
		{
			_logger.LogInformation( $"User {UserName} requested their observed areas." );
			return await _areasService.GetUserAreasAsync( UserName );
		}

		[HttpGet( "{id}" )]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetObservedAreaDto>> GetArea( int id )
		{
			var area = await _areasService.GetAreaAsync( id );
			if ( area is null )
			{
				_logger.LogInformation( $"User {UserName} requested unknown observed area {id}." );
				return NotFound( ProblemConstants.AreaNotFound );
			}

			_logger.LogInformation( $"User {UserName} requested observed area {id}." );
			return area;
		}

		[HttpGet( "owned/{id}" )]
		public async Task<ActionResult<GetObservedAreaDto>> GetOwnedArea( int id )
		{
			var area = await _areasService.GetAreaAsync( id, UserName );
			if ( area is null )
			{
				_logger.LogInformation( $"User {UserName} requested not owned/unknown observed area {id}." );
				return NotFound( ProblemConstants.AreaNotObserved );
			}

			_logger.LogInformation( $"User {UserName} requested their observed area {id}." );
			return area;
		}


		[HttpPost( "owned" )]
		public async Task<ActionResult<GetObservedAreaDto>> Observe( CreateObservedAreaDto areaDto )
		{
			var area = await _areasService.ObserveAreaAsync( areaDto, UserName );
			if ( area is null )
			{
				_logger.LogInformation( $"User {UserName} failed creating observed area: already exists." );
				return Conflict( ProblemConstants.AreaObserved );
			}

			_logger.LogInformation( $"User {UserName} created observed area {area.Id}." );
			return CreatedAtAction( nameof( GetOwnedArea ), new { area.Id }, area );
		}


		[HttpDelete( "{id}" )]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetObservedAreaDto>> DeleteArea( int id )
		{
			var deleted = await _areasService.DeleteAreaAsync( id );
			if ( !deleted )
			{
				_logger.LogInformation( $"User {UserName} requested deleting unknown observed area." );
				return NotFound( ProblemConstants.AreaNotFound );
			}

			_logger.LogInformation( $"User {UserName} deleted observed area {id}." );
			return NoContent();
		}

		[HttpDelete( "owned/{id}" )]
		public async Task<ActionResult<GetObservedAreaDto>> DeleteOwnedArea( int id )
		{
			var deleted = await _areasService.DeleteAreaAsync( id, UserName );
			if ( !deleted )
			{
				_logger.LogInformation( $"User {UserName} requested deleting not their/unknown observed area." );
				return NotFound( ProblemConstants.AreaNotObserved );
			}

			_logger.LogInformation( $"User {UserName} deleted their observed area {id}." );
			return NoContent();
		}

	}
}
