using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Point;

#nullable disable warnings

public record GetPointDto
{
	public int Id { get; init; }
	[Required]
	public int GarbageTypeId { get; init; }
	[Required]
	public DateTimeOffset DateScored { get; init; }
	[Required]
	public string UserUuid { get; init; }

#nullable restore

	public static Expression<Func<PointModel, GetPointDto>> FromPointExp { get; } = point => new GetPointDto()
	{
		Id = point.Id,
		GarbageTypeId = point.GarbageTypeId,
		DateScored = point.DateScored,
		UserUuid = point.UserUuid,
	};

	private static readonly Func<PointModel, GetPointDto> fromPointFunc = FromPointExp.Compile();
	public static GetPointDto FromPoint( PointModel point )
	{
		return fromPointFunc( point );
	}

}
