using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Relations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		List<FriendshipRelation> initiatedFriendships,
		List<FriendshipRelation> targetingFriendships,
		List<UserModel> initiatedFriends,
		List<UserModel> targetingFriends,
		List<PointModel> points,
		List<GroupModel> groups,
		List<GroupMembershipRelation> groupsR,
		List<ChallengeModel> challenges,
		List<UserChallengeRelation> challengesR )
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

		Points = points;

		Groups = groups;
		GroupsR = groupsR;

		Challenges = challenges;
		ChallengesR = challengesR;
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
			  new(),
			  new(),
			  new(),
			  new(),
			  new(),
			  new() )
	{
	}

	#region userdata

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

	#endregion


	public List<TokenModel> Tokens { get; set; }

	public List<RoleModel> Roles { get; set; }

	#region friends

	[InverseProperty( nameof( FriendshipRelation.Initiator ) )]
	public List<FriendshipRelation> InitiatedFriendships { get; set; }

	[InverseProperty( nameof( FriendshipRelation.Target ) )]
	public List<FriendshipRelation> TargetingFriendships { get; set; }

	public List<UserModel> InitiatedFriends { get; set; }
	public List<UserModel> TargetingFriends { get; set; }

	[NotMapped]
	public ReadOnlyCollection<FriendshipRelation> Friendships => Enumerable.Concat( InitiatedFriendships, TargetingFriendships ).ToList().AsReadOnly();

	[NotMapped]
	public ReadOnlyCollection<UserModel> Friends => Enumerable.Concat(
		InitiatedFriendships.Where( x => x.Accepted )
		.Select( x => x.Target! ),
		TargetingFriendships.Where( x => x.Accepted )
		.Select( x => x.Target! ) ).ToList().AsReadOnly();
	#endregion

	#region groups

	public List<GroupModel> Groups { get; set; }
	public List<GroupMembershipRelation> GroupsR { get; set; }

	#endregion

	public List<PointModel> Points { get; set; }

	#region challenges

	public List<ChallengeModel> Challenges { get; set; }
	public List<UserChallengeRelation> ChallengesR { get; set; }

	#endregion

}
