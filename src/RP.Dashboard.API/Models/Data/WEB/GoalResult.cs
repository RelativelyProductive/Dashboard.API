using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RP.Dashboard.API.Models.Data.WEB
{
	[ExcludeFromCodeCoverage]
	public class GoalResult
	{
		public IEnumerable<TrackedProjectReportEntry> TrackedEntryDetails { get; set; }

		public string GoalFriendlyName { get; set; }

		public int GoalId { get; set; }
	}
}