using Microsoft.AspNetCore.Mvc;

namespace RubbishCam.Api.Controllers;

public class ExtendedControllerBase : ControllerBase
{
	#region 4xx
	[NonAction]
	public BadRequestObjectResult BadRequest( string detail )
	{
		var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 400, detail: detail );
		return BadRequest( problemDetails );
	}

	[NonAction]
	public ObjectResult Unauthorized( string detail )
	{
		var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 401, detail: detail );
		return Unauthorized( problemDetails );
	}

	[NonAction]
	public NotFoundObjectResult NotFound( string detail )
	{
		var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 404, detail: detail );
		return NotFound( problemDetails );
	}

	[NonAction]
	public ConflictObjectResult Conflict( string detail )
	{
		var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 409, detail: detail );
		return Conflict( problemDetails );
	}

	#endregion

	#region 5xx
	[NonAction]
	public ObjectResult InternalServerError( string detail )
	{
		var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 500, detail: detail );
		return new ObjectResult( problemDetails )
		{
			StatusCode = 500
		};
	}

	[NonAction]
	public ObjectResult ServiceUnavailable( string detail )
	{
		var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 503, detail: detail );
		return new ObjectResult( problemDetails )
		{
			StatusCode = 503
		};
	}

	#endregion
}
