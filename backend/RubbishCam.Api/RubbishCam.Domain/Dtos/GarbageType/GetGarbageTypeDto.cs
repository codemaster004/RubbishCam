using RubbishCam.Domain.Models;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.GarbageType;

#nullable disable warnings

public record GetGarbageTypeDto
{
	public int Id { get; init; }
	public int PointsPerItem { get; init; }
	public string Name { get; init; }

#nullable restore warnings

	public static Expression<Func<GarbageTypeModel, GetGarbageTypeDto>> FromGarbageTypeExp { get; } = garbageType => new GetGarbageTypeDto()
	{
		Id = garbageType.Id,
		Name = garbageType.Name,
		PointsPerItem = garbageType.PointsPerItem,
	};

	private static readonly Func<GarbageTypeModel, GetGarbageTypeDto> fromGarbageTypeFunc = FromGarbageTypeExp.Compile();
	public static GetGarbageTypeDto FromGarbageType( GarbageTypeModel point )
	{
		return fromGarbageTypeFunc( point );
	}

}
