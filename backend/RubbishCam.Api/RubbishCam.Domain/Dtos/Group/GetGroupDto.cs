using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Group;

#nullable disable warnings

public record GetGroupDto
{
	public int Id { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public DateTimeOffset TimeCreated { get; set; }

#nullable restore

	public static Expression<Func<GroupModel, GetGroupDto>> FromGroupExp { get; } = group => new GetGroupDto()
	{
		Id = group.Id,
		Name = group.Name,
		TimeCreated = group.TimeCreated,
	};

	private static readonly Func<GroupModel, GetGroupDto> fromGroupFunc = FromGroupExp.Compile();
	public static GetGroupDto FromGroup( GroupModel group )
	{
		return fromGroupFunc( group );
	}
}
