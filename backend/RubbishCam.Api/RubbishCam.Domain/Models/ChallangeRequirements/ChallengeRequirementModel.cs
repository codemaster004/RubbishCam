namespace RubbishCam.Domain.Models.ChallangeRequirements;

public abstract class ChallengeRequirementModel
{
	public ChallengeRequirementModel( string name )
	{
		Name = name;
	}
	public int Id { get; set; }
	public string Name { get; set; }
}