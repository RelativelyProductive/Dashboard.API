using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using RP.Dashboard.API.Business.Clients;
using RP.Dashboard.API.Business.Constants;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Models.Data.DB;
using Xunit;

namespace RP.Dashboard.API.Tests
{
	public class TogglHelperTests
	{
		// Set up of shared details for tests
		private readonly User _emptyUser = new User();

		private readonly Goal _emptyGoal = new Goal();

		private readonly DateTime _startDate = new DateTime(2019, 05, 05);

		private readonly DateTime _endDate = new DateTime(2019, 05, 11);

		private readonly User _validUser = new User
		{
			Id = 1,
			TogglApiKey = "12345asdfgh",
			TogglWorkspaceID = "12345678",
			FirstName = "Test",
			LastName = "User",
			Email = "TestUser@RelativelyProductive.com"
		};

		private readonly Goal _validGoal = new Goal
		{
			Id = 99,
			UserId = 1,
			DailyGoalTimeInMs = 9001,
			DailyGoalTogglID = "12345",
			DailyGoalTrackStyle = TrackTypeConstants.TrackType.Project,
			GoalFriendlyName = "Test Daily Goal"
		};

		#region GoalResultForProject

		[Fact]
		public void GivenEmptyObjectCombinations_WhenGetGoalResultForProjectIsEnvoked_ThenReturnsNull()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockConfigurationHelper = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			var mockCryptHelper = new Mock<CryptHelper>(mockConfigurationHelper.Object);
			var mockTogglHttpClient = new Mock<TogglHttpClient>(mockCryptHelper.Object);
			mockTogglHttpClient.Setup(s => s.PostAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(string.Empty);

			var sut = new TogglHelper(mockTogglHttpClient.Object);

			// Act
			var result = sut.GetGoalResultForProject(_emptyGoal, _emptyUser, _startDate, _endDate);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void GivenEmptyUserObjectWithValidGoalObject_WhenGetGoalResultForProjectIsEnvoked_ThenReturnsNull()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockConfigurationHelper = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			var mockCryptHelper = new Mock<CryptHelper>(mockConfigurationHelper.Object);
			var mockTogglHttpClient = new Mock<TogglHttpClient>(mockCryptHelper.Object);
			mockTogglHttpClient.Setup(s => s.PostAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(string.Empty);

			var sut = new TogglHelper(mockTogglHttpClient.Object);

			// Act
			var result = sut.GetGoalResultForProject(_validGoal, _emptyUser, _startDate, _endDate);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void GivenEmptyGoalObjectWithValidUserObject_WhenGetGoalResultForProjectIsEnvoked_ThenReturnsNull()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockConfigurationHelper = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			var mockCryptHelper = new Mock<CryptHelper>(mockConfigurationHelper.Object);
			var mockTogglHttpClient = new Mock<TogglHttpClient>(mockCryptHelper.Object);
			mockTogglHttpClient.Setup(s => s.PostAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(string.Empty);

			var sut = new TogglHelper(mockTogglHttpClient.Object);

			// Act
			var result = sut.GetGoalResultForProject(_emptyGoal, _validUser, _startDate, _endDate);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void GivenValidObjectsWithNoTogglResultData_WhenGetGoalResultForProjectIsEnvoked_ThenReturnsEmptyResults()
		{
			// Arrange
			var mockConfiguration = new Mock<IConfiguration>();
			var mockConfigurationHelper = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			var mockCryptHelper = new Mock<CryptHelper>(mockConfigurationHelper.Object);
			var mockTogglHttpClient = new Mock<TogglHttpClient>(mockCryptHelper.Object);
			mockTogglHttpClient.Setup(s => s.PostAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync("");

			var sut = new TogglHelper(mockTogglHttpClient.Object);

			// Act
			var result = sut.GetGoalResultForProject(_emptyGoal, _validUser, _startDate, _endDate);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void GivenValidObjectsWithTogglResultData_WhenGetGoalResultForProjectIsEnvoked_ThenReturnsParsedResults()
		{
			// Arrange
			var validJsonResponseString = "{  \"seconds\": 12360,  \"rates\": null,  \"graph\": [    {      \"seconds\": 0,      \"by_rate\": null    },    {      \"seconds\": 1560,      \"by_rate\": null    },    {      \"seconds\": 0,      \"by_rate\": null    },    {      \"seconds\": 3600,      \"by_rate\": null    },    {      \"seconds\": 0,      \"by_rate\": null    },    {      \"seconds\": 7200,      \"by_rate\": null    },    {      \"seconds\": 0,      \"by_rate\": null    }  ],  \"resolution\": \"day\"}";

			var mockConfiguration = new Mock<IConfiguration>();
			var mockConfigurationHelper = new Mock<ConfigurationHelper>(mockConfiguration.Object);
			var mockCryptHelper = new Mock<CryptHelper>(mockConfigurationHelper.Object);
			var mockTogglHttpClient = new Mock<TogglHttpClient>(mockCryptHelper.Object);
			mockTogglHttpClient.Setup(s => s.PostAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(validJsonResponseString);

			var sut = new TogglHelper(mockTogglHttpClient.Object);

			// Act
			var result = sut.GetGoalResultForProject(_validGoal, _validUser, _startDate, _endDate);
			var resultTrackiedlist = result.TrackedEntryDetails.ToList();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(_validGoal.Id, result.GoalId);
			Assert.Equal(_validGoal.GoalFriendlyName, result.GoalFriendlyName);

			// We have all tracked entries
			Assert.NotEmpty(result.TrackedEntryDetails);
			Assert.Equal(7, result.TrackedEntryDetails.Count());

			// Using our sample result the 2nd entry should match the following
			Assert.True(resultTrackiedlist[1].GoalMet);
			Assert.Equal("1560000", resultTrackiedlist[1].GoalTrackedDurationInMs);
			Assert.Equal("00h:26m:00s", resultTrackiedlist[1].GoalTrackedDurationParsed);
			Assert.Equal(DateTime.Parse("2019-05-06T08:30:39.9120605+01:00").ToLongDateString(), resultTrackiedlist[1].Date.ToLongDateString());

			// And the last should match these
			Assert.False(resultTrackiedlist[6].GoalMet);
			Assert.Equal("0", resultTrackiedlist[6].GoalTrackedDurationInMs);
			Assert.Equal("00h:00m:00s", resultTrackiedlist[6].GoalTrackedDurationParsed);
			Assert.Equal(DateTime.Parse("2019-05-11T08:30:39.9120605+01:00").ToLongDateString(), resultTrackiedlist[6].Date.ToLongDateString());
		}

		#endregion GoalResultForProject
	}
}