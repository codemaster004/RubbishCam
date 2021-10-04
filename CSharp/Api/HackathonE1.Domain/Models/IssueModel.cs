using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Models
{
	public class IssueModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[Range( -90, 90 )]
		public double Latitude { get; set; }
		[Required]
		[Range( -180, 180 )]
		public double Longitude { get; set; }

		[Required]
		[ForeignKey( nameof( Type ) )]
		public int TypeId { get; set; }
		public IssueTypeModel Type { get; set; }
	}
}
