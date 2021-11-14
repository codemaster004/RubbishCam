using RubbishCam.Domain.Models;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Friendship;

#nullable disable warnings

public class GetFriendshipDto
{
	public int Id { get; set; }
	public string InitiatorUuid { get; set; }
	public string TargetUuid { get; set; }
	public bool Accepted { get; set; }
	public bool Rejected { get; set; }

#nullable restore

	public static Expression<Func<FriendshipModel, GetFriendshipDto>> FromFriendshipExp { get; set; } = friendship => new GetFriendshipDto()
	{
		Id = friendship.Id,
		InitiatorUuid = friendship.InitiatorUuid,
		TargetUuid = friendship.TargetUuid,
		Accepted = friendship.Accepted,
		Rejected = friendship.Rejected,
	};

	private static readonly Func<FriendshipModel, GetFriendshipDto> fromFriendshipFunc = FromFriendshipExp.Compile();
	public static GetFriendshipDto FromFriendship( FriendshipModel user )
	{
		return fromFriendshipFunc( user );
	}

}
