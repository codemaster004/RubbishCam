using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.User
{
	public class GetUserDto
	{
		[Required( AllowEmptyStrings = false )]
		[MaxLength( 24 )]
		public string Identifier { get; set; }

		[Required]
		[MaxLength( 32 )]
		public string FirstName { get; set; }

		[Required]
		[MaxLength( 32 )]
		public string LastName { get; set; }

		[Required]
		[MaxLength( 32 )]
		[EmailAddress]
		public string Email { get; set; }


		public static Expression<Func<UserModel, GetUserDto>> FromUserModel => _fromUserModel;
		private static readonly Expression<Func<UserModel, GetUserDto>> _fromUserModel = ( UserModel user ) => new GetUserDto()
		{
			Identifier = user.Identifier,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Email = user.Email
		};

		private static readonly Func<UserModel, GetUserDto> _fromUserModelDelegate = _fromUserModel.Compile();
		public static explicit operator GetUserDto( UserModel s )
		{
			return _fromUserModelDelegate( s );
		}
	}
}
