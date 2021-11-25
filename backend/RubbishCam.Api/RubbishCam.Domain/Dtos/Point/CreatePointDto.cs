using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Domain.Dtos.Point;

#nullable disable warnings

public record CreatePointDto
{
	[Required]
	public double Longitude { get; set; }
	[Required]
	public double Latitude { get; set; }
	[Required( AllowEmptyStrings = false )]
	[StringLength( 50 )]
	public string Type { get; set; }
	[Required]
	public int Value { get; set; }
	[Required]
	public DateTimeOffset DateScored { get; set; }

#nullable restore

	public PointModel ToPoint( string uuid )
	{
		return new PointModel(
			type: Type,
			value: Value,
			userUuid: uuid,
			longitude: Longitude,
			latitude: Latitude,
			dateScored: DateScored );
	}

}
