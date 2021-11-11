using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Dtos.User;

#nullable disable warnings

public record GetUserDto
{
	[Required]
	[StringLength( 24 )]
	public string Uuid { get; set; }

	[Required]
	[StringLength( 50 )]
	public string FirstName { get; set; }

	[Required]
	[StringLength( 50 )]
	public string LastName { get; set; }

	[Required]
	[StringLength( 32 )]
	public string UserName { get; set; }

#nullable restore

	public static Expression<Func<UserModel, GetUserDto>> FromUserExp { get; set; } = user => new GetUserDto()
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
