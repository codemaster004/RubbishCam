using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Models
{
	public class ObservedAreaModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[Range( -90, 90 )]
		public double Latitude { get; set; }
		[Required]
		[Range( -180, 180 )]
		public double Longitude { get; set; }
		//in degrees
		[Required]
		public double Radius { get; set; }

		[Required]
		[ForeignKey( nameof( User ) )]
		[MaxLength( 24 )]
		public string UserIdentifier { get; set; }
		public UserModel User { get; set; }
	}
}
