using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Business.Clients;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Infrastructure;
using RP.Dashboard.API.Business.Services;
using RP.Dashboard.API.Models.Data.WEB;
using RP.Dashboard.API.Models.Interfaces.ApiWrapper;

namespace RP.Dashboard.API.Controllers
{
	[Route("dash/TogglAccount")]
	[ApiController]
	[Authorize]
	public class TogglAccountController : Controller
	{
		private readonly UserService _userService;
		private readonly TogglHttpClient _togglHttpClient;
		private readonly ResponseHelper _responseHelper;

		public TogglAccountController(UserService userService, TogglHttpClient togglHttpClient,
		ResponseHelper responseHelper)
		{
			_userService = userService;
			_togglHttpClient = togglHttpClient;
			_responseHelper = responseHelper;
		}

		// GET dash/TogglAccount/GetAllWorkspaceIds/{userId}
		[HttpGet("GetAllWorkspaceIds")]
		[ProducesResponseType(typeof(IWorkspacesObjectCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		public async Task<CustomApiResult> GetAllWorkspaceIds()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var usr = await _userService.GetByNameIdentifier(nameIdentifier);

			// if no user, die
			if (usr == null || string.IsNullOrWhiteSpace(usr.TogglApiKey))
				return new CustomApiResult(_responseHelper.NotFound("No user found"));

			var workspaceList = new List<UserWorkspace>();

			// Call to Toggl Api
			var result = await _togglHttpClient.GetAsync("https://www.toggl.com/api/v8/workspaces", usr.TogglApiKey);

			if (result == null || string.IsNullOrWhiteSpace(result))
				return new CustomApiResult(_responseHelper.NotFound("No workspace found"));

			// Parse the response to an object
			dynamic workspaceResultJson = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

			if (workspaceResultJson == null || workspaceResultJson[0] == null)
				return new CustomApiResult(_responseHelper.NotFound("No workspace found"));

			// We got some ID;s
			foreach (var item in workspaceResultJson)
			{
				workspaceList.Add(new UserWorkspace
				{
					WorkspaceId = item.id.ToString(),
					WorkspaceName = item.name.ToString()
				});
			}

			return new CustomApiResult(_responseHelper.ValidResponse(workspaceList.ToArray()));
		}

		// GET dash/TogglAccount/GetAllClients/{userId}
		[HttpGet("GetAllClients")]
		[ProducesResponseType(typeof(IClientsObjectCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		public async Task<CustomApiResult> GetAllClients()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var usr = await _userService.GetByNameIdentifier(nameIdentifier);

			// if no user, die
			if (usr == null || string.IsNullOrWhiteSpace(usr.TogglApiKey))
				return new CustomApiResult(_responseHelper.NotFound("No user found"));

			var userClientList = new List<UserClient>();

			// Call to Toggl Api
			var result = await _togglHttpClient.GetAsync($"https://www.toggl.com/api/v8/workspaces/{usr.TogglWorkspaceID}/clients", usr.TogglApiKey);

			if (result == null || string.IsNullOrWhiteSpace(result))
				return new CustomApiResult(_responseHelper.NotFound("No clients found"));

			// Parse the response to an object
			dynamic userClientsJsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

			if (userClientsJsonResult == null)
				return new CustomApiResult(_responseHelper.NotFound("No clients found"));

			// Build the result object
			foreach (var userClient in userClientsJsonResult)
			{
				userClientList.Add(new UserClient
				{
					ClientId = userClient.id,
					ClientName = userClient.name
				});
			}

			return new CustomApiResult(_responseHelper.ValidResponse(userClientList.ToArray()));
		}

		// GET dash/TogglAccount/GetAllProjects/{userId}
		[HttpGet("GetAllProjects")]
		[ProducesResponseType(typeof(IProjectsObjectCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		public async Task<CustomApiResult> GetAllProjects()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var usr = await _userService.GetByNameIdentifier(nameIdentifier);

			// if no user, die
			if (usr == null || string.IsNullOrWhiteSpace(usr.TogglApiKey))
				return new CustomApiResult(_responseHelper.NotFound("No user found"));

			var userProjectList = new List<UserProject>();

			// Call to Toggl Api
			var result = await _togglHttpClient.GetAsync($"https://www.toggl.com/api/v8/workspaces/{usr.TogglWorkspaceID}/projects", usr.TogglApiKey);

			if (result == null || string.IsNullOrWhiteSpace(result))
				return new CustomApiResult(_responseHelper.NotFound("No projects found"));

			// Parse the response to an object
			dynamic userProjectsJsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

			if (userProjectsJsonResult == null)
				return new CustomApiResult(_responseHelper.NotFound("No projects found"));

			// Build the result object
			foreach (var userProject in userProjectsJsonResult)
			{
				userProjectList.Add(new UserProject
				{
					ProjectId = userProject.id,
					ProjectName = userProject.name
				});
			}

			return new CustomApiResult(_responseHelper.ValidResponse(userProjectList.ToArray()));
		}

		// GET dash/TogglAccount/GetAllTags/{userId}
		[HttpGet("GetAllTags")]
		[ProducesResponseType(typeof(ITagsObjectCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		public async Task<CustomApiResult> GetAllTags()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var usr = await _userService.GetByNameIdentifier(nameIdentifier);

			// if no user, die
			if (usr == null || string.IsNullOrWhiteSpace(usr.TogglApiKey))
				return new CustomApiResult(_responseHelper.NotFound("No user found"));

			var userTagsList = new List<UserTag>();

			// Call to Toggl Api
			var result = await _togglHttpClient.GetAsync($"https://www.toggl.com/api/v8/workspaces/{usr.TogglWorkspaceID}/tags", usr.TogglApiKey);

			if (result == null || string.IsNullOrWhiteSpace(result))
				return new CustomApiResult(_responseHelper.NotFound("No tags found"));

			// Parse the response to an object
			dynamic userProjectsJsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

			if (userProjectsJsonResult == null)
				return new CustomApiResult(_responseHelper.NotFound("No tags found"));

			// Build the result object
			foreach (var userProject in userProjectsJsonResult)
			{
				userTagsList.Add(new UserTag
				{
					TagId = userProject.id,
					TagName = userProject.name
				});
			}

			return new CustomApiResult(_responseHelper.ValidResponse(userTagsList.ToArray()));
		}
	}
}