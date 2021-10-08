using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.ObservedArea
{
	public class GetObservedAreaDto
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[Range( -90, 90 )]
		public double Latitude { get; set; }
		[Required]
		[Range( -180, 180 )]
		public double Longitude { get; set; }
		//in kilometers
		[Required]
		public double Radius { get; set; }

		[Required]
		[MaxLength( 24 )]
		public string UserIdentifier { get; set; }

		public static Expression<Func<ObservedAreaModel, GetObservedAreaDto>> FromObservedAreaModel => _fromObservedAreaModel;
		private static readonly Expression<Func<ObservedAreaModel, GetObservedAreaDto>> _fromObservedAreaModel = ( ObservedAreaModel area ) => new GetObservedAreaDto()
		{
			Id = area.Id,
			Latitude = area.Latitude,
			Longitude = area.Longitude,
			Radius = area.Radius * Functions.kmPerDeg,
			UserIdentifier = area.UserIdentifier
		};

		private static readonly Func<ObservedAreaModel, GetObservedAreaDto> _fromObservedAreaModelDelegate = _fromObservedAreaModel.Compile();
		public static explicit operator GetObservedAreaDto( ObservedAreaModel s )
		{
			return _fromObservedAreaModelDelegate( s );
		}
	}
}
