using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Extensions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Point;

namespace RubbishCam.Api.Controllers;

[Authorize]
[ApiController]
[Route( "api/[controller]" )]
public class PointsController : ExtendedControllerBase
{
	private readonly IPointsService _pointsService;

	public PointsController( IPointsService pointsService )
	{
		_pointsService = pointsService;
	}

	[HttpGet]
	public async Task<ActionResult<GetPointDetailsDto[]>> GetPoints()
	{
		string? uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		return await _pointsService.GetPointsAsync( uuid );
	}

	[HttpGet( "{id}" )]
	public async Task<ActionResult<GetPointDetailsDto[]>> GetPoint( int id )
	{
		await Task.CompletedTask;
		string? uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		throw new NotImplementedException();
	}

	[HttpGet( "sum" )]
	public async Task<ActionResult<int>> SumPoints()
	{
		string? uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		return await _pointsService.SumPointsAsync( uuid );
	}

	[HttpPost]
	public async Task<ActionResult<GetPointDetailsDto>> AddPoint( [FromBody] CreatePointDto dto )
	{
		string? uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		var point = await _pointsService.CreatePoint( dto, uuid );

		return CreatedAtAction( nameof( GetPoint ), new { id = point.Id }, point );
	}



}
