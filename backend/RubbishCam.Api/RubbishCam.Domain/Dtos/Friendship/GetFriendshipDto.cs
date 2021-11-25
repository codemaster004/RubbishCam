using RubbishCam.Domain.Models;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Friendship;

#nullable disable warnings

public record GetFriendshipDto
{
	public int Id { get; init; }
	public string InitiatorUuid { get; init; }
	public string TargetUuid { get; init; }
	public bool Accepted { get; init; }
	public bool Rejected { get; init; }

#nullable restore

	public static Expression<Func<FriendshipModel, GetFriendshipDto>> FromFriendshipExp { get; } = friendship => new GetFriendshipDto()
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
