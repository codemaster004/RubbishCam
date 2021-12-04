using RubbishCam.Domain.Models;

namespace RubbishCam.Domain.Relations;

public class GroupPointRelation
{
	public int GroupId { get; set; }
	public GroupModel? Group { get; set; }
	public int PointId { get; set; }
	public PointModel? Point { get; set; }
}
