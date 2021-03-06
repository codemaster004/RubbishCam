namespace RubbishCam.Domain.Models.ChallangeRequirements;

public class CollectXItemsRequirement : ChallengeRequirementModel
{
	public CollectXItemsRequirement( string name )
		: base( name )
	{
	}

	public int GarbageTypeId { get; set; }
	public GarbageTypeModel? GarbageType { get; set; }
	public int RequiredAmount { get; set; }
}
