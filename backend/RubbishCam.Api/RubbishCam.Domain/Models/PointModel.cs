using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Domain.Models;

public class PointModel
{
	public PointModel( string type, int value, string userUuid, double longitude, double latitude, DateTimeOffset dateScored )
	{
		Type = type;
		Value = value;
		UserUuid = userUuid;
		Longitude = longitude;
		Latitude = latitude;
		DateScored = dateScored;
	}

	public int Id { get; set; }
	[Required]
	public double Longitude { get; set; }
	[Required]
	public double Latitude { get; set; }
	[Required]
	[StringLength( 50 )]
	public string Type { get; set; } //todo: extract to separate table, alongside with PointModel.Value
	[Required]
	public int Value { get; set; }
	[Required]
	public DateTimeOffset DateScored { get; set; }
	[Required]
	public string UserUuid { get; set; }
	public UserModel? User { get; set; }
}
