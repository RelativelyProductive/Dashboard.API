using RP.Dashboard.API.Models.Data.WEB;
using RP.Dashboard.API.Models.Interfaces.ApiWrapper;

namespace RP.Dashboard.API.Models.ApiWrapper
{
	public interface IGoalStatsObjectCustomResponse : IStandardCustomResponse
	{
		GoalStats Data { get; set; }
	}
}