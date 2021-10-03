using Microsoft.AspNetCore.Mvc;

namespace HackathonE1.Api.Controllers
{
	public class ExtendedControllerBase : ControllerBase
	{
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

		[NonAction]
		public BadRequestObjectResult BadRequest( string detail )
		{
			var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 400, detail: detail );
			return BadRequest( problemDetails );
		}

		[NonAction]
		public ObjectResult InternalServerError( string detail )
		{
			var problemDetails = ProblemDetailsFactory.CreateProblemDetails( HttpContext, 500, detail: detail );
			return new ObjectResult( problemDetails )
			{
				StatusCode = 500
			};
		}

		public string UserName => User.Identity.Name;

	}
}
