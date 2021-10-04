using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.Issue
{
	public class CreateIssueDto
	{
		[Required]
		[Range( -90, 90 )]
		public double Latitude { get; set; }
		[Required]
		[Range( -180, 180 )]
		public double Longitude { get; set; }

		[Required]
		public int TypeId { get; set; }


		public IssueModel ToIssueModel()
		{
			return new()
			{
				Latitude = this.Latitude,
				Longitude = this.Longitude,
				TypeId = this.TypeId
			};
		}
	}
}
