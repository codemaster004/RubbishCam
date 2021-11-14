﻿using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Dtos.User;

#nullable disable warnings

public record CreateUserDto
{
	[Required]
	[StringLength( 50 )]
	public string FirstName { get; set; }

	[Required]
	[StringLength( 50 )]
	public string LastName { get; set; }

	[Required]
	public string Password { get; set; }

	[Required]
	[StringLength( 32 )]
	public string UserName { get; set; }

#nullable restore

	public async Task<UserModel> ToUserAsync( Func<string, Task<string>> hashFunction, Func<Task<string>> generateUuid )
	{
		string passwordHash = await hashFunction( Password );
		string uuid = await generateUuid();

		return new UserModel(
			uuid: uuid,
			firstName: FirstName,
			lastName: LastName,
			passwordHash: passwordHash,
			userName: UserName,
			tokens: new(),
			roles: new(),
			initiatedFriendships: new(),
			targetingFriendships: new(),
			initiatedFriends: new(),
			targetingFriends: new() );
	}
}