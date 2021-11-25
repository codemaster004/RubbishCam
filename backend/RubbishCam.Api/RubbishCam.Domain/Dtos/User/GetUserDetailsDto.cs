using RubbishCam.Domain.Dtos.Roles;
using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.User;

#nullable disable warnings

public record GetUserDetailsDto
{
	[Required]
	[StringLength( 24 )]
	public string Uuid { get; init; }

	[Required]
	[StringLength( 50 )]
	public string FirstName { get; init; }

	[Required]
	[StringLength( 50 )]
	public string LastName { get; init; }

	[Required]
	[StringLength( 32 )]
	public string UserName { get; init; }


	public List<GetRoleDto> Roles { get; init; }

#nullable restore

	public static Expression<Func<UserModel, GetUserDetailsDto>> FromUserExp { get; } = user => new GetUserDetailsDto()
	{
		Uuid = user.Uuid,
		FirstName = user.FirstName,
		LastName = user.LastName,
		UserName = user.UserName,
		Roles = user.Roles.AsQueryable().Select( GetRoleDto.FromRoleExp ).ToList()
	};

	private static readonly Func<UserModel, GetUserDetailsDto> fromUserFunc = FromUserExp.Compile();
	public static GetUserDetailsDto FromUser( UserModel user )
	{
		return fromUserFunc( user );
	}

}
