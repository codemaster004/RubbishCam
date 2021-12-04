using RubbishCam.Domain.Relations;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Group.Membership;

#nullable disable warnings

public record GetGroupMembershipDto
{
	public string UserUuid { get; set; }
	public string UserName { get; set; }
	public int GroupId { get; set; }
	public bool IsOwner { get; set; }

#nullable restore warnings

	public static Expression<Func<GroupMembershipRelation, GetGroupMembershipDto>> FromGroupMembersRelationExp { get; } = groupMember => new GetGroupMembershipDto()
	{
		GroupId = groupMember.GroupId,
		IsOwner = groupMember.IsOwner,
		UserUuid=groupMember.UserUuid,
		UserName=groupMember.User!.UserName
	};

	private static readonly Func<GroupMembershipRelation, GetGroupMembershipDto> fromGroupMembersRelationFunc = FromGroupMembersRelationExp.Compile();
	public static GetGroupMembershipDto FromGroupMembersRelation( GroupMembershipRelation group )
	{
		return fromGroupMembersRelationFunc( group );
	}

}
