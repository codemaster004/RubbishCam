using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Models;

public class FriendshipModel
{
	public FriendshipModel( string initiatorUuid, string targetUuid )
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
