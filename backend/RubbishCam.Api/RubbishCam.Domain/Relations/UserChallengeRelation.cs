using RubbishCam.Domain.Models;

namespace RubbishCam.Domain.Relations;

public class UserChallengeRelation
{
	public string UserUuid { get; set; }
	public int ChallengeId { get; set; }
	public UserModel? User { get; set; }
	public ChallengeModel? Challange { get; set; }
	public DateTimeOffset DateStarted { get; set; }
	public DateTimeOffset? DateCompleted { get; set; }
	
	public List<PointModel> RelatedPoints { get; set; }
}
