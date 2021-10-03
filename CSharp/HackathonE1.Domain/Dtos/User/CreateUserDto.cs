﻿using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.User
{
	public class CreateUserDto
	{
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

		[Required]
		[MaxLength( 88 )]
		public string PasswordHash { get; set; }


		public UserModel ToUserModel()
		{
			return new()
			{
				FirstName = this.FirstName,
				LastName = this.LastName,
				Email = this.Email,
				PasswordHash = this.PasswordHash
			};
		}
	}
}
