using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using RP.Dashboard.API.Business.Clients;
using RP.Dashboard.API.Business.Constants;
using RP.Dashboard.API.Models.Data.DB;
using RP.Dashboard.API.Models.Data.WEB;

namespace RP.Dashboard.API.Business.Helpers
{
	public class TogglHelper
	{
		private TogglHttpClient _togglHttpClient { get; }

		public TogglHelper(TogglHttpClient togglHttpClient)
		{
			_togglHttpClient = togglHttpClient;
		}

		public virtual GoalResult GetGoalResultForProject(Goal userGoal, User usr, DateTime startDate, DateTime endDate)
		{
			string filterJson = "";

			// set filter based on the track type of each goal
			// 1 - Client
			if (userGoal.DailyGoalTrackStyle == TrackTypeConstants.TrackType.Client)
				filterJson = $"\"client_ids\" : [ " + userGoal.DailyGoalTogglID + "]";

			// 2 - Project
			if (userGoal.DailyGoalTrackStyle == TrackTypeConstants.TrackType.Project)
				filterJson = $"\"project_ids\" : [ " + userGoal.DailyGoalTogglID + "]";

			// 3 - Task AKA description
			if (userGoal.DailyGoalTrackStyle == TrackTypeConstants.TrackType.Task)
				filterJson = $"\"description\" : \"{ userGoal.DailyGoalTogglID }\"";

			// 4 - Tag
			if (userGoal.DailyGoalTrackStyle == TrackTypeConstants.TrackType.Tag)
				filterJson = $"\"tag_ids\" : [ " + userGoal.DailyGoalTogglID + "]";

			// Setup the JSON body to post
			var requestJsonBody = @"{
		                          ""start_date"": """ + startDate.ToString("yyyy-MM-dd") + @""",
		                          ""end_date"": """ + endDate.ToString("yyyy-MM-dd") + @""",
		                          " + filterJson + @",
		                          ""grouping"": ""projects"",
		                          ""with_graph"": true
		                      }";

			// Go Go Gadget Toggl API call for this goal
			var response = _togglHttpClient.PostAsync($"https://www.toggl.com/reports/api/v3/workspace/{usr.TogglWorkspaceID}/search/time_entries/totals",
					usr.TogglApiKey, new StringContent(requestJsonBody, Encoding.UTF8, "application/json")).Result;

			// Parse the response to an object
			dynamic reportResultsJson = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

			if (reportResultsJson?.graph == null)
				return null;

			var trackProjectEntries = new List<TrackedProjectReportEntry>();

			// Get the daily entries for this goal
			var daySpan = (endDate - startDate).Days;
			for (int i = daySpan; i >= 0; i--)
			{
				double entryInS = 0D;
				if (reportResultsJson.graph[i] != null && reportResultsJson.graph[i].seconds.ToString() != "null")
				{
					double.TryParse(reportResultsJson.graph[i].seconds.ToString(), out entryInS);
				}

				var t = TimeSpan.FromSeconds(entryInS);
				var totalDurationForDayParsed = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
										t.Hours,
										t.Minutes,
										t.Seconds);

				// Add to goal entry list
				trackProjectEntries.Add(new TrackedProjectReportEntry
				{
					Date = endDate.AddDays(i - daySpan),
					GoalMet = entryInS >= (userGoal.DailyGoalTimeInMs / 1000),
					GoalTrackedDurationInMs = (entryInS * 1000).ToString(),
					GoalTrackedDurationParsed = totalDurationForDayParsed
				});
			}

			//  All results parsed for this goal, return final object
			return new GoalResult
			{
				GoalId = userGoal.Id,
				GoalFriendlyName = userGoal.GoalFriendlyName,
				TrackedEntryDetails = trackProjectEntries?.OrderBy(d => d.Date)
			};
		}
	}
}