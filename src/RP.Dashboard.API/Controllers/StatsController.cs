using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Infrastructure;
using RP.Dashboard.API.Business.Services;
using RP.Dashboard.API.Models.ApiWrapper;
using RP.Dashboard.API.Models.Data.WEB;

namespace RP.Dashboard.API.Controllers
{
	[Route("dash/stats")]
	[ApiController]
	[Authorize]
	public class StatsController : Controller
	{
		private readonly UserService _userService;
		private readonly GoalService _goalService;
		private readonly TogglHelper _TogglHelper;
		private readonly ResponseHelper _responseHelper;

		public StatsController(UserService userService, GoalService goalService,
			TogglHelper TogglHelper, ResponseHelper responseHelper)
		{
			_userService = userService;
			_goalService = goalService;
			_TogglHelper = TogglHelper;
			_responseHelper = responseHelper;
		}

		// GET dash/stats/GetGoalsMetForUser/
		[HttpGet("GetGoalsMet")]
		[ProducesResponseType(typeof(IGoalStatsObjectCustomResponse), 200)]
		public async Task<CustomApiResult> GetGoalsMet(DateTime? startDate = null, DateTime? endDate = null)
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var usr = await _userService.GetByNameIdentifier(nameIdentifier);

			// If no user, die
			if (usr == null || string.IsNullOrWhiteSpace(usr.TogglApiKey))
				return new CustomApiResult(_responseHelper.NotFound("User not found"));

			// Deal with date defaults
			if (startDate == null || startDate == DateTime.MinValue || startDate == DateTime.MaxValue)
				startDate = DateTime.UtcNow.AddDays(-7);

			if (endDate == null || endDate == DateTime.MinValue || endDate == DateTime.MaxValue)
				endDate = DateTime.UtcNow;

			// Make sure they are the correct way round
			if (startDate > endDate)
			{
				var temp = startDate;
				startDate = endDate;
				endDate = temp;
			}

			// Get their saved goals
			var goals = await _goalService.GetForUserId(usr.Id);
			if (goals == null || goals.Count() < 1)
				return new CustomApiResult(_responseHelper.NotFound("No goals found"));

			var goalResults = new List<GoalResult>();

			// Hit toggl and retrieve our result objects for each goal
			foreach (var userGoal in goals)
				goalResults.Add(_TogglHelper.GetGoalResultForProject(userGoal, usr, (DateTime)startDate,
						(DateTime)endDate));

			var totalGoalsMet = 0;
			var totalGoalsSet = 0;
			foreach (var goal in goalResults)
			{
				totalGoalsMet += goal.TrackedEntryDetails.Count(e => e.GoalMet == true);
				totalGoalsSet += goal.TrackedEntryDetails.Count();
			}

			// All done, return
			return new CustomApiResult(_responseHelper.ValidResponse(new GoalStats
			{
				GoalsAchieved = totalGoalsMet,
				GoalsAttempted = totalGoalsSet,
				StartDate = startDate,
				EndDate = endDate
			}));
		}

		// GET dash/stats/GetGoalsMetForAllUsers/
		[HttpGet("GetGoalsMetForAllUsers")]
		[ProducesResponseType(typeof(IGoalStatsObjectCustomResponse), 200)]
		public async Task<CustomApiResult> GetGoalsMetForAllUsers(DateTime? startDate, DateTime? endDate)
		{
			// Deal with date defaults
			if (startDate == null || startDate == DateTime.MinValue || startDate == DateTime.MaxValue)
				startDate = DateTime.UtcNow.AddDays(-7);

			if (endDate == null || endDate == DateTime.MinValue || endDate == DateTime.MaxValue)
				endDate = DateTime.UtcNow;

			// Make sure they are the correct way round
			if (startDate > endDate)
			{
				var temp = startDate;
				startDate = endDate;
				endDate = temp;
			}

			var totalGoalsMet = 0;
			var totalGoalsSet = 0;
			// get all the users in the system
			foreach (var user in await _userService.Get())
			{
				// Check if they have a toggl account
				if (!string.IsNullOrWhiteSpace(user.TogglApiKey))
				{
					var _goals = await _goalService.GetForUserId(user.Id);
					// check the user has at least one goal
					if (_goals.Count() > 0)
					{
						foreach (var userGoal in _goals)
						{
							// Get results from toggl and add them all up.
							var goalResults = _TogglHelper.GetGoalResultForProject(userGoal, user, (DateTime)startDate,
							(DateTime)endDate);
							totalGoalsMet += goalResults.TrackedEntryDetails.Count(e => e.GoalMet == true);
							totalGoalsSet += goalResults.TrackedEntryDetails.Count();

							// Rest to make toggl happy!!
							Thread.Sleep(1000);
						}
					}
				}
			}

			// All done, return
			return new CustomApiResult(_responseHelper.ValidResponse(new GoalStats
			{
				GoalsAchieved = totalGoalsMet,
				GoalsAttempted = totalGoalsSet,
				StartDate = startDate,
				EndDate = endDate
			}));
		}

		// TODO

		// Controller that shows total amount of time tracked (easy to get with user report api)
	}
}