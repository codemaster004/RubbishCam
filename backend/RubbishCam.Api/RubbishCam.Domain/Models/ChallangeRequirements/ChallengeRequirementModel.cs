namespace RubbishCam.Domain.Models.ChallengeRequirements;

public abstract class ChallengeRequirementModel
{
	public ChallengeRequirementModel( string name )
	{
		Name = name;
	}
	public int Id { get; set; }
	public string Name { get; set; }
}