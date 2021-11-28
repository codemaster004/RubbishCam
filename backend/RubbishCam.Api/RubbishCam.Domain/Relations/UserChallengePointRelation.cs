using RubbishCam.Domain.Models;

namespace RubbishCam.Domain.Relations;

public class UserChallengePointRelation
{
	public int PointId { get; set; }
	public int UserChallengeId { get; set; }
	public PointModel? Point { get; set; }
	public UserChallengeRelation? UserChallenge { get; set; }
}
