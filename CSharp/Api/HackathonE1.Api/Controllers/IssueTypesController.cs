using HackathonE1.Api.Services;
using HackathonE1.Domain.Dtos.IssueType;
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
	public class IssueTypesController : ExtendedControllerBase
	{
		private readonly ILogger<IssueTypesController> _logger;
		private readonly IIssueTypesService _issueTypesService;

		public IssueTypesController( ILogger<IssueTypesController> logger, IIssueTypesService issueTypesService )
		{
			_logger = logger;
			_issueTypesService = issueTypesService;
		}

		[HttpGet]
		public async Task<ActionResult<GetIssueTypeDto[]>> GetIssueTypes()
		{
			_logger.LogInformation( $"User {UserName} requested issue types." );
			return await _issueTypesService.GetIssueTypesAsync();
		}

		[HttpGet( "{id}" )]
		public async Task<ActionResult<GetIssueTypeDto>> GetIssueType( int id )
		{
			var issueType = await _issueTypesService.GetIssueTypeAsync( id );
			if ( issueType is null )
			{
				_logger.LogInformation( $"User {UserName} requested unknown issue type {id}." );
				return NotFound( ProblemConstants.IssueNotFound );
			}

			_logger.LogInformation( $"User {UserName} requested issue type {id}." );
			return issueType;
		}

		[HttpPost]
		public async Task<ActionResult<GetIssueTypeDto>> CreateIssueType( [FromBody] CreateIssueTypeDto issueTypeDto )
		{
			var issueType = await _issueTypesService.CreateIssueTypeAsync( issueTypeDto );
			if ( issueType is null )
			{
				_logger.LogInformation( $"User {UserName} attempted recreating existing issue type." );
				return Conflict( ProblemConstants.IssueTypeExists );
			}

			_logger.LogInformation( $"User {UserName} created issue type {issueType.Id}." );
			return CreatedAtAction( nameof( GetIssueType ), new { issueType.Id }, issueType );
		}

		[HttpDelete( "{id}" )]
		[Authorize( Roles = "Admin" )]
		public async Task<IActionResult> DeleteIssueType( int id )
		{
			var deleted = await _issueTypesService.DeleteIssueTypeAsync( id );
			if ( !deleted )
			{
				_logger.LogInformation( $"User {UserName} failed deleting issue type {id}: issue type not found." );
				return NotFound( ProblemConstants.IssueTypeNotFound );
			}

			_logger.LogInformation( $"User {UserName} deleted issue type {id}." );
			return NoContent();
		}
	}
}
