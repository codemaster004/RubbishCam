using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Models
{
	public class IssueTypeModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength( 24 )]
		public string Name { get; set; }
		[MaxLength( 256 )]
		public string Description { get; set; }

		public List<IssueModel> Issues { get; set; }
	}
}
