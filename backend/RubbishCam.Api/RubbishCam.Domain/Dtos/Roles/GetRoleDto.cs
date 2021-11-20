using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Roles;

#nullable disable warnings

public record GetRoleDto
{
	public int Id { get; set; }

	[Required]
	[StringLength( 24 )]
	public string Name { get; set; }

#nullable restore

	public static Expression<Func<RoleModel, GetRoleDto>> FromRoleExp { get; set; } = role => new GetRoleDto()
	{
		Id = role.Id,
		Name = role.Name,
	};

	private static readonly Func<RoleModel, GetRoleDto> fromRoleFunc = FromRoleExp.Compile();
	public static GetRoleDto FromUser( RoleModel role )
	{
		return fromRoleFunc( role );
	}

}