using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Business.Constants;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Infrastructure;
using RP.Dashboard.API.Business.Services;
using RP.Dashboard.API.Models.Data.DB;
using RP.Dashboard.API.Models.Data.WEB;
using RP.Dashboard.API.Models.Interfaces.ApiWrapper;

namespace RP.Dashboard.API.Controllers
{
	[Route("dash/DailyGoal")]
	[ApiController]
	[Authorize]
	public class DailyGoalController : Controller
	{
		private readonly UserService _userService;
		private readonly GoalService _goalService;
		private readonly TogglHelper _TogglHelper;
		private readonly IValidator<Goal> _createGoalValidator;
		private readonly ResponseHelper _responseHelper;

		public DailyGoalController(UserService userService, GoalService goalService,
			TogglHelper TogglHelper, IValidator<Goal> createGoalValidator, ResponseHelper responseHelper)
		{
			_userService = userService;
			_goalService = goalService;
			_TogglHelper = TogglHelper;
			_createGoalValidator = createGoalValidator;
			_responseHelper = responseHelper;
		}

		// POST dash/DailyGoal/Create
		[HttpPost("create")]
		[ProducesResponseType(typeof(IStandardCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 400)]
		public async Task<CustomApiResult> Create([FromBody] Goal value)
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var user = await _userService.GetByNameIdentifier(nameIdentifier);

			value.UserId = user.Id;

			// Checking if all the metadata related to goal is available to create
			var validationResults = _createGoalValidator.Validate(value);
			if (validationResults.IsValid)
			{
				if (!await _goalService.IsGoalCreated(value.UserId, value.DailyGoalTogglID))
				{
					// We dont allow create with custom ID
					value.Id = 0;

					await _goalService.Create(value);
				}

				return new CustomApiResult(_responseHelper.ValidResponse("Goal created"));
			}
			else
			{
				return new CustomApiResult(_responseHelper.BadRequest(validationResults.ToString()));
			}
		}

		// POST dash/DailyGoal/Update
		[HttpPost("Update")]
		[ProducesResponseType(typeof(IStandardCustomResponse), 200)]
		public async Task<CustomApiResult> Update([FromBody] Goal value)
		{
			//NOTE
			//userId is immutable

			// We do this manually as this endpoint will handle all updates to the user obj
			// and we want to be able to send in only the changed fields data
			var savedGoal = await _goalService.Get(value.Id);

			// These are the only fields we allow changing
			if (!string.IsNullOrWhiteSpace(value.GoalFriendlyName))
				savedGoal.GoalFriendlyName = value.GoalFriendlyName;

			if (!string.IsNullOrWhiteSpace(value.DailyGoalTogglID))
				savedGoal.DailyGoalTogglID = value.DailyGoalTogglID;

			if ((value.DailyGoalTimeInMs != int.MinValue || value.DailyGoalTimeInMs != int.MaxValue) && value.DailyGoalTimeInMs > 0)
				savedGoal.DailyGoalTimeInMs = value.DailyGoalTimeInMs;

			if (value.DailyGoalTrackStyle != TrackTypeConstants.TrackType.None)
				savedGoal.DailyGoalTrackStyle = value.DailyGoalTrackStyle;

			await _goalService.Update(savedGoal);

			return new CustomApiResult(_responseHelper.ValidResponse());
		}

		// GET dash/DailyGoal/GetGoalResults/{userId}
		[HttpGet("GetGoals")]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		[ProducesResponseType(typeof(IGoalsObjectCustomResponse), 200)]
		public async Task<CustomApiResult> GetGoals()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var user = await _userService.GetByNameIdentifier(nameIdentifier);

			var goals = await _goalService.GetForUserId(user.Id);

			if (goals?.Count() > 0)
			{
				return new CustomApiResult(_responseHelper.ValidResponse(goals.ToArray()));
			}

			return new CustomApiResult(_responseHelper.NotFound("No Results found"));
		}

		// GET dash/DailyGoal/GetGoalResultsForUser/{userId}
		[HttpGet("GetGoalResults")]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		[ProducesResponseType(typeof(IGoalsObjectCustomResponse), 200)]
		public async Task<CustomApiResult> GetGoalResults()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var usr = await _userService.GetByNameIdentifier(nameIdentifier);
			// If no user, die
			if (usr == null || string.IsNullOrWhiteSpace(usr.TogglApiKey))
				return new CustomApiResult(_responseHelper.NotFound("No user found"));

			// Get their saved goals
			var goals = await _goalService.GetForUserId(usr.Id);
			if (goals?.Count() > 0)
			{
				var goalResults = new List<GoalResult>();

				var _now = DateTime.Now;

				// calculate start of week
				int diff = (7 + (_now.DayOfWeek - DayOfWeek.Monday)) % 7;
				var _startOfWeek = _now.AddDays(-1 * diff).Date;
				var _endOfWeek = _startOfWeek.AddDays(6);

				// Hit toggl and retrieve our result objects for each goal
				foreach (var userGoal in goals)
					goalResults.Add(_TogglHelper.GetGoalResultForProject(userGoal, usr, _startOfWeek,
						_endOfWeek));

				// All done, return
				return new CustomApiResult(_responseHelper.ValidResponse(goalResults.ToArray()));
			}

			return new CustomApiResult(_responseHelper.NotFound("No results found"));
		}

		// DELETE dash/DailyGoal/{goalId}
		[HttpDelete("{goalId}")]
		[ProducesResponseType(typeof(IStandardCustomResponse), 200)]
		public async Task<CustomApiResult> Delete(int goalId)
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var usr = await _userService.GetByNameIdentifier(nameIdentifier);
			// If no user, die
			if (usr == null || string.IsNullOrWhiteSpace(usr.TogglApiKey))
				return new CustomApiResult(_responseHelper.NotFound("No user found"));

			var goal = await _goalService.Get(goalId);

			if (goal.UserId != usr.Id)
				return new CustomApiResult(_responseHelper.Unauthorized("You cannot delete that goal"));

			await _goalService.RemoveAsync(goalId);

			return new CustomApiResult(_responseHelper.ValidResponse());
		}
	}
}