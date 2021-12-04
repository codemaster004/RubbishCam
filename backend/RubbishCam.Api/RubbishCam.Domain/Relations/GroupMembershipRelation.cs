using RubbishCam.Domain.Models;

namespace RubbishCam.Domain.Relations;

public class GroupMembershipRelation
{
	public GroupMembershipRelation( string userUuid )
	{
		UserUuid = userUuid;
	}

	public string UserUuid { get; set; }
	public int GroupId { get; set; }
	public UserModel? User { get; set; }
	public GroupModel? Group { get; set; }
	public bool IsOwner { get; set; }
}
