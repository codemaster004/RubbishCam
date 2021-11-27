using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Domain.Models;

public class GarbageTypeModel
{
	public GarbageTypeModel( string name, List<PointModel> relatedPoints )
	{
		Name = name;
		RelatedPoints = relatedPoints;
	}
	public GarbageTypeModel( string name )
		: this( name, new() )
	{
	}

	public int Id { get; set; }
	public int PointsPerItem { get; set; }
	[StringLength( 32 )]
	public string Name { get; set; }

	public List<PointModel> RelatedPoints { get; set; }
}
