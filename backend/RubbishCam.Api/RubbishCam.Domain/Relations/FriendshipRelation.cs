using RubbishCam.Domain.Models;

namespace RubbishCam.Domain.Relations;

public class FriendshipRelation
{
	public FriendshipRelation( string initiatorUuid, string targetUuid )
	{
		InitiatorUuid = initiatorUuid;
		TargetUuid = targetUuid;
	}

	public int Id { get; set; }
	public string InitiatorUuid { get; set; }
	public UserModel? Initiator { get; set; }
	public string TargetUuid { get; set; }
	public UserModel? Target { get; set; }
	public bool Accepted { get; set; }
	public bool Rejected { get; set; }
}
