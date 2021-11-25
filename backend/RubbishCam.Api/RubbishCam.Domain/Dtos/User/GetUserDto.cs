using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.User;

#nullable disable warnings

public record GetUserDto
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

#nullable restore

	public static Expression<Func<UserModel, GetUserDto>> FromUserExp { get; } = user => new GetUserDto()
	{
		Uuid = user.Uuid,
		FirstName = user.FirstName,
		LastName = user.LastName,
		UserName = user.UserName
	};

	private static readonly Func<UserModel, GetUserDto> fromUserFunc = FromUserExp.Compile();
	public static GetUserDto FromUser( UserModel user )
	{
		return fromUserFunc( user );
	}

}
