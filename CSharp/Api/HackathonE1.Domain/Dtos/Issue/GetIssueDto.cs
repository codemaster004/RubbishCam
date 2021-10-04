using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.Issue
{
	public class GetIssueDto
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
		public int TypeId { get; set; }


		public static Expression<Func<IssueModel, GetIssueDto>> FromIssueModel => _fromIssueModel;
		private static readonly Expression<Func<IssueModel, GetIssueDto>> _fromIssueModel = ( IssueModel issue ) => new GetIssueDto()
		{
			Id = issue.Id,
			Latitude = issue.Latitude,
			Longitude = issue.Longitude,
			TypeId = issue.TypeId
		};

		private static readonly Func<IssueModel, GetIssueDto> _fromIssueModelDelegate = _fromIssueModel.Compile();
		public static explicit operator GetIssueDto( IssueModel s )
		{
			return _fromIssueModelDelegate( s );
		}
	}
}
