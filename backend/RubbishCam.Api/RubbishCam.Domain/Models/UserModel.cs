using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Models;


[Index( nameof( Uuid ), IsUnique = true )]
[Index( nameof( UserName ), IsUnique = true )]
public class UserModel
{
	public UserModel( string uuid,
		string firstName,
		string lastName,
		string passwordHash,
		string userName,
		List<TokenModel> tokens,
		List<RoleModel> roles,
		List<FriendshipModel> initiatedFriendships,
		List<FriendshipModel> targetingFriendships,
		List<UserModel> initiatedFriends,
		List<UserModel> targetingFriends )
	{
		Uuid = uuid;
		FirstName = firstName;
		LastName = lastName;
		PasswordHash = passwordHash;
		UserName = userName;

		Tokens = tokens;
		Roles = roles;
		InitiatedFriendships = initiatedFriendships;
		TargetingFriendships = targetingFriendships;
		InitiatedFriends = initiatedFriends;
		TargetingFriends = targetingFriends;
	}
	public UserModel( string uuid,
		string firstName,
		string lastName,
		string passwordHash,
		string userName )
		: this( uuid,
			  firstName,
			  lastName,
			  passwordHash,
			  userName,
			  new(),
			  new(),
			  new(),
			  new(),
			  new(),
			  new() )
	{
	}

	[Key]
	public int Id { get; set; }
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
	public string PasswordHash { get; set; }

	[Required]
	[StringLength( 32 )]
	public string UserName { get; set; }


	public List<TokenModel> Tokens { get; set; }

	public List<RoleModel> Roles { get; set; }

	[InverseProperty( nameof( FriendshipModel.Initiator ) )]
	public List<FriendshipModel> InitiatedFriendships { get; set; }

	[InverseProperty( nameof( FriendshipModel.Target ) )]
	public List<FriendshipModel> TargetingFriendships { get; set; }

	public List<UserModel> InitiatedFriends { get; set; }
	public List<UserModel> TargetingFriends { get; set; }

	[NotMapped]
	public ReadOnlyCollection<FriendshipModel> Friendships => Enumerable.Concat( InitiatedFriendships, TargetingFriendships ).ToList().AsReadOnly();

	[NotMapped]
	public ReadOnlyCollection<UserModel> Friends => Enumerable.Concat( InitiatedFriends, TargetingFriends ).ToList().AsReadOnly();

}
