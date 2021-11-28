using RubbishCam.Domain.Models;

namespace RubbishCam.Domain.Relations;

public class UserChallengeRelation
{
	public UserChallengeRelation( string userUuid,
		List<PointModel> relatedPoints,
		List<UserChallengePointRelation> relatedPointsR )
	{
		UserUuid = userUuid;
		RelatedPoints = relatedPoints;
		RelatedPointsR = relatedPointsR;
	}
	public UserChallengeRelation( string userUuid )
		: this( userUuid,
			 new(),
			 new() )
	{
	}

	public int Id { get; set; }
	public string UserUuid { get; set; }
	public int ChallengeId { get; set; }
	public UserModel? User { get; set; }
	public ChallengeModel? Challenge { get; set; }
	public DateTimeOffset DateStarted { get; set; }
	public DateTimeOffset? DateCompleted { get; set; }

	public List<PointModel> RelatedPoints { get; set; }
	public List<UserChallengePointRelation> RelatedPointsR { get; set; }
}
