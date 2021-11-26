using RubbishCam.Domain.Dtos.User;
using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Group;

#nullable disable warnings

public record GetGroupDetailsDto
{
	public int Id { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public DateTimeOffset TimeCreated { get; set; }

	//todo: public List<PointModel> Points { get; set; }
	public GetUserDto[] Members { get; set; }

#nullable restore

	public static Expression<Func<GroupModel, GetGroupDetailsDto>> FromGroupExp { get; } = group => new GetGroupDetailsDto()
	{
		Id = group.Id,
		Name = group.Name,
		TimeCreated = group.TimeCreated,
		Members = group.Members.AsQueryable().Select( GetUserDto.FromUserExp ).ToArray(),
	};

	private static readonly Func<GroupModel, GetGroupDetailsDto> fromGroupFunc = FromGroupExp.Compile();
	public static GetGroupDetailsDto FromGroup( GroupModel group )
	{
		return fromGroupFunc( group );
	}

}
