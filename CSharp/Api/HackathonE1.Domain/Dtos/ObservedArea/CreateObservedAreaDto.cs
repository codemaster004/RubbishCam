using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Dtos.ObservedArea
{
	public class CreateObservedAreaDto
	{
		[Required]
		[Range( -90, 90 )]
		public double Latitude { get; set; }
		[Required]
		[Range( -180, 180 )]
		public double Longitude { get; set; }
		//in kilometers
		[Required]
		public double Radius { get; set; }

		public ObservedAreaModel ToObservedAreaModel()
		{
			return new()
			{
				Latitude = this.Latitude,
				Longitude = this.Longitude,
				Radius = this.Radius * Functions.degPerKm
			};
		}

	}
}
