using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Point;

#nullable disable warnings

public record GetPointDetailsDto
{
	public int Id { get; init; }
	[Required]
	public double Longitude { get; init; }
	[Required]
	public double Latitude { get; init; }
	[Required]
	[StringLength( 50 )]
	public string Type { get; init; }
	[Required]
	public int Value { get; init; }
	[Required]
	public DateTimeOffset DateScored { get; init; }
	[Required]
	public string UserUuid { get; init; }

#nullable restore

	public static Expression<Func<PointModel, GetPointDetailsDto>> FromPointExp { get; } = point => new GetPointDetailsDto()
	{
		Id = point.Id,
		Latitude = point.Latitude,
		Longitude = point.Longitude,
		Type = point.Type,
		Value = point.Value,
		DateScored = point.DateScored,
		UserUuid = point.UserUuid,
	};

	private static readonly Func<PointModel, GetPointDetailsDto> fromPointFunc = FromPointExp.Compile();
	public static GetPointDetailsDto FromPoint( PointModel point )
	{
		return fromPointFunc( point );
	}
}
