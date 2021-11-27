using RubbishCam.Domain.Relations;
using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Domain.Models;

public class PointModel
{
	public PointModel( int garbageTypeId,
		string userUuid,
		double longitude,
		double latitude,
		DateTimeOffset dateScored,
		List<GroupModel> groups,
		List<GroupPointsRelation> groupsR )
	{
		GarbageTypeId = garbageTypeId;
		UserUuid = userUuid;
		Longitude = longitude;
		Latitude = latitude;
		DateScored = dateScored;
		Groups = groups;
		GroupsR = groupsR;
	}
	public PointModel(
		int garbageTypeId,
		string userUuid,
		double longitude,
		double latitude,
		DateTimeOffset dateScored )
		: this(
			  garbageTypeId,
			  userUuid,
			  longitude,
			  latitude,
			  dateScored,
			  new(),
			  new() )
	{
	}

	public int Id { get; set; }
	[Required]
	public double Longitude { get; set; }
	[Required]
	public double Latitude { get; set; }
	[Required]
	public DateTimeOffset DateScored { get; set; }

	[Required]
	public int GarbageTypeId { get; set; }
	public GarbageTypeModel? GarbageType { get; set; }

	[Required]
	public string UserUuid { get; set; }
	public UserModel? User { get; set; }

	public List<GroupModel> Groups { get; set; }
	public List<GroupPointsRelation> GroupsR { get; set; }
}
