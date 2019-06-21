using System;
using System.Diagnostics.CodeAnalysis;

namespace RP.Dashboard.API.Models.Data.WEB
{
	[ExcludeFromCodeCoverage]
	public class TrackedProjectReportEntry
	{
		public DateTime Date { get; set; }

		public bool GoalMet { get; set; }

		public string GoalTrackedDurationInMs { get; set; }

		public string GoalTrackedDurationParsed { get; set; }
	}
}