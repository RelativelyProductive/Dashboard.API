using System.Collections.Generic;
using RP.Dashboard.API.Models.Data.DB;

namespace RP.Dashboard.API.Models.Interfaces.ApiWrapper
{
	public interface IGoalObjectCustomResponse : IStandardCustomResponse
	{
		Goal Data { get; set; }
	}

	public interface IGoalsObjectCustomResponse : IStandardCustomResponse
	{
		IEnumerable<Goal> Data { get; set; }
	}
}