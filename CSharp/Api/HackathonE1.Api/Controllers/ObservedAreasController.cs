using HackathonE1.Api.Data;
using HackathonE1.Api.Services;
using HackathonE1.Domain.Dtos.ObservedArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
		private readonly IObservedAreasService _areasService;

		public ObservedAreasController( IObservedAreasService areasService )
		{
			_areasService = areasService;
		}

		[HttpGet()]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetObservedAreaDto[]>> GetAreas()
		{
			return await _areasService.GetAreasAsync();
		}

		[HttpGet( "owned" )]
		public async Task<ActionResult<GetObservedAreaDto[]>> GetOwnedAreas()
		{
			return await _areasService.GetUserAreasAsync( UserName );
		}

		[HttpGet( "{id}" )]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetObservedAreaDto>> GetArea( int id )
		{
			var area = await _areasService.GetAreaAsync( id );
			if ( area is null )
			{
				return NotFound( ProblemConstants.AreaNotFound );
			}

			return area;
		}

		[HttpGet( "owned/{id}" )]
		public async Task<ActionResult<GetObservedAreaDto>> GetOwnedArea( int id )
		{
			var area = await _areasService.GetAreaAsync( id, UserName );
			if ( area is null )
			{
				return NotFound( ProblemConstants.AreaNotObserved );
			}

			return area;
		}


		[HttpPost]
		public async Task<ActionResult<GetObservedAreaDto>> Observe( CreateObservedAreaDto areaDto )
		{
			var area = await _areasService.ObserveAreaAsync( areaDto, UserName );
			if ( area is null )
			{
				return Conflict( ProblemConstants.AreaObserved );
			}

			return CreatedAtAction( nameof( GetOwnedArea ), new { area.Id }, area );
		}


		[HttpDelete( "{id}" )]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetObservedAreaDto>> DeleteArea( int id )
		{
			var deleted = await _areasService.DeleteAreaAsync( id );
			if ( !deleted )
			{
				return NotFound( ProblemConstants.AreaNotFound );
			}

			return NoContent();
		}

		[HttpDelete( "owned/{id}" )]
		public async Task<ActionResult<GetObservedAreaDto>> DeleteOwnedArea( int id )
		{
			var deleted = await _areasService.DeleteAreaAsync( id, UserName );
			if ( !deleted )
			{
				return NotFound( ProblemConstants.AreaNotObserved );
			}

			return NoContent();
		}

	}
}
