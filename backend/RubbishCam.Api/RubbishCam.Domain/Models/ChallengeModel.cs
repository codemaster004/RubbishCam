using RubbishCam.Domain.Models.ChallangeRequirements;
using RubbishCam.Domain.Relations;

namespace RubbishCam.Domain.Models;

public class ChallengeModel
{
	public ChallengeModel( string name,
		string description,
		List<ChallengeRequirementModel> requirements,
		List<UserModel> users,
		List<UserChallengeRelation> usersR )
	{
		Name = name;
		Description = description;
		Requirements = requirements;
		Users = users;
		UsersR = usersR;
	}

	public ChallengeModel( string name,
		string description )
		: this( name,
			  description,
			  new(),
			  new(),
			  new())
	{
	}

	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }

	public List<ChallengeRequirementModel> Requirements { get; set; }

	public List<UserModel> Users { get; set; }
	public List<UserChallengeRelation> UsersR { get; set; }
}
