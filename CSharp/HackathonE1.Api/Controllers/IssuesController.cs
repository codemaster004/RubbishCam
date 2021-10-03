using HackathonE1.Api.Services;
using HackathonE1.Domain.Dtos.Issue;
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
	public class IssuesController : ExtendedControllerBase
	{
		private readonly ILogger<IssuesController> _logger;
		private readonly IIssuesService _issuesService;

		public IssuesController( ILogger<IssuesController> logger, IIssuesService issuesService )
		{
			_logger = logger;
			_issuesService = issuesService;
		}

		[HttpGet( "{id}" )]
		public async Task<ActionResult<GetIssueDto>> GetIssue( int id )
		{
			var issue = await _issuesService.GetIssueAsync( id );
			if ( issue is null )
			{
				_logger.LogInformation( $"User {UserName} requested unknown issue {id}." );
				return NotFound( ProblemConstants.IssueNotFound );
			}

			_logger.LogInformation( $"User {UserName} requested issue {id}." );
			return issue;
		}

		[HttpPost]
		public async Task<ActionResult<GetIssueDto>> CreateIssue( [FromBody] CreateIssueDto issueDto )
		{
			var issue = await _issuesService.CreateIssueAsync( issueDto );
			if ( issue is null )
			{
				_logger.LogInformation( $"User {UserName} attempted recreating existing issue." );
				return Conflict( ProblemConstants.IssueExists );
			}

			_logger.LogInformation( $"User {UserName} created issue {issue.Id}." );
			return CreatedAtAction( nameof( GetIssue ), new { issue.Id }, issue );
		}

		[HttpDelete( "{id}" )]
		public async Task<IActionResult> DeleteIssue( int id )
		{
			var deleted = await _issuesService.DeleteIssueAsync( id );
			if ( !deleted )
			{
				_logger.LogInformation( $"User {UserName} failed deleting issue {id}: issue not found." );
				return NotFound( ProblemConstants.IssueNotFound );
			}

			_logger.LogInformation( $"User {UserName} deleted issue {id}." );
			return NoContent();
		}

	}
}
