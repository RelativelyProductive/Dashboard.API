using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RP.Dashboard.API.Business.Constants;

namespace RP.Dashboard.API.Models.Data.DB
{
	[ExcludeFromCodeCoverage]
	public class Goal
	{
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public string GoalFriendlyName { get; set; }

		[Required]
		public int DailyGoalTimeInMs { get; set; }

		[Required]
		public string DailyGoalTogglID { get; set; }

		public TrackTypeConstants.TrackType DailyGoalTrackStyle { get; set; }
	}
}