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
		public int Id { get; set; }
		[Required]
		[Range( -90, 90 )]
		public double Latitude { get; set; }
		[Required]
		[Range( -180, 180 )]
		public double Longitude { get; set; }
		[Required]
		public double Radius { get; set; }

		[Required]
		[ForeignKey( nameof( User ) )]
		public int UserId { get; set; }
		public UserModel User { get; set; }
	}
}
