using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.IssueType
{
	public class CreateIssueTypeDto
	{
		[Required]
		[MaxLength( 24 )]
		public string Name { get; set; }
		[MaxLength( 256 )]
		public string Description { get; set; }


		public IssueTypeModel ToIssueTypeModel()
		{
			return new()
			{
				Name = this.Name,
				Description = this.Description
			};
		}
	}
}
