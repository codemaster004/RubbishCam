using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HackathonE1.Domain
{
	public class Functions
	{
		// Circumference of earth (in km)
		private const double earthCirc = 40_075.014;
		public const double kmPerDeg = earthCirc / 360;
		public const double degPerKm = 360 / earthCirc;

	}
}
