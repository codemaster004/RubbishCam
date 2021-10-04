using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.IssueType
{
	public class GetIssueTypeDto
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength( 24 )]
		public string Name { get; set; }
		[MaxLength( 256 )]
		public string Description { get; set; }

		public static Expression<Func<IssueTypeModel, GetIssueTypeDto>> FromIssueTypeModel => _fromIssueTypeModel;
		private static readonly Expression<Func<IssueTypeModel, GetIssueTypeDto>> _fromIssueTypeModel = ( IssueTypeModel issueType ) => new GetIssueTypeDto()
		{
			Id = issueType.Id,
			Name = issueType.Name,
			Description = issueType.Description
		};

		private static readonly Func<IssueTypeModel, GetIssueTypeDto> _fromIssueTypeModelDelegate = _fromIssueTypeModel.Compile();
		public static explicit operator GetIssueTypeDto( IssueTypeModel s )
		{
			return _fromIssueTypeModelDelegate( s );
		}
	}
}
