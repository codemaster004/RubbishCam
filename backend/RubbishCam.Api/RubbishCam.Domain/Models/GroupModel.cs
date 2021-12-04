using RubbishCam.Domain.Relations;
using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Domain.Models;

public class GroupModel
{
	public GroupModel( string name,
		List<UserModel> members,
		List<PointModel> points,
		List<GroupPointRelation> pointsR,
		List<GroupMembershipRelation> membersR )
	{
		Name = name;
		Members = members;
		Points = points;
		PointsR = pointsR;
		MembersR = membersR;
	}
	public GroupModel( string name )
		: this( name,
			  new(),
			  new(),
			  new(),
			  new() )
	{
	}

	public int Id { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public DateTimeOffset TimeCreated { get; set; }
	public List<PointModel> Points { get; set; }
	public List<GroupPointRelation> PointsR { get; set; }
	public List<UserModel> Members { get; set; }
	public List<GroupMembershipRelation> MembersR { get; set; }

}
