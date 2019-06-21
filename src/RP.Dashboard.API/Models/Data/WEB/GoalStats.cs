using System;
using System.Diagnostics.CodeAnalysis;

namespace RP.Dashboard.API.Models.Data.WEB
{
	[ExcludeFromCodeCoverage]
	public class GoalStats
	{
		public int GoalsAchieved { get; set; }

		public int GoalsAttempted { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }
	}
}