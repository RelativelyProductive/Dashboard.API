using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Business.Constants;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Infrastructure;
using RP.Dashboard.API.Business.Services;
using RP.Dashboard.API.Models.ApiWrapper;
using RP.Dashboard.API.Models.Data.DB;
using RP.Dashboard.API.Models.Interfaces.ApiWrapper;

namespace RP.Dashboard.API.Controllers
{
	[Route("dash/[controller]")]
	[ApiController]
	[Authorize]
	public class UsersController : Controller
	{
		private readonly ConfigurationHelper _configuration;
		private readonly UserService _userService;
		private readonly CryptHelper _cryptHelper;
		private readonly ResponseHelper _responseHelper;

		public UsersController(ConfigurationHelper configuration, UserService userService,
		CryptHelper cryptHelper, ResponseHelper responseHelper)
		{
			_configuration = configuration;
			_userService = userService;
			_cryptHelper = cryptHelper;
			_responseHelper = responseHelper;
		}

		// GET dash/users
		[HttpGet]
		[ProducesResponseType(typeof(IUsersObjectCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		public async Task<CustomApiResult> Get()
		{
			// THIS IS FOR DEV ONLY
			if (_configuration.IsDevEnvironment())
			{
				var users = await _userService.Get();

				if (users != null && users.Count > 0)
				{
					foreach (var user in users)
					{
						// We never send the user toggl api key over the wire
						// we do send blank to indicate that it is empty/unset though

						if (!string.IsNullOrWhiteSpace(user.TogglApiKey))
							user.TogglApiKey = PlaceholderConstants.TogglApiKeyPlaceholder;
					}
				}

				return new CustomApiResult(_responseHelper.ValidResponse(users.ToArray()));
			}

			return new CustomApiResult(_responseHelper.NotFound("No users found"));
		}

		// GET dash/users/getself
		[HttpGet("GetSelf")]
		[ProducesResponseType(typeof(IUserObjectCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 404)]
		public async Task<CustomApiResult> GetSelf()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var user = await _userService.GetByNameIdentifier(nameIdentifier);

			if (user == null)
				return new CustomApiResult(_responseHelper.NotFound("User not found"));

			// We never send the user toggl api key over the wire
			user.TogglApiKey = PlaceholderConstants.TogglApiKeyPlaceholder;

			return new CustomApiResult(_responseHelper.ValidResponse(user));
		}

		// POST dash/users/register
		[HttpPost("register")]
		[ProducesResponseType(typeof(IStandardCustomResponse), 200)]
		[ProducesResponseType(typeof(IStandardCustomResponse), 400)]
		public async Task<CustomApiResult> Register([FromBody] User value)
		{
			if (!await _userService.IsEmailRegistered(value.Email))
			{
				// Always encrypted
				if (!string.IsNullOrWhiteSpace(value.TogglApiKey))
					value.TogglApiKey = _cryptHelper.EncryptString(value.TogglApiKey);

				string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

				value.NameIdentifier = nameIdentifier;

				// Create
				await _userService.Create(value);

				var user = await _userService.GetByNameIdentifier(nameIdentifier);

				return new CustomApiResult(_responseHelper.ValidResponse(user));

				// return new CustomApiResult(_responseHelper.ValidResponse("User created successfully"));
			}

			return new CustomApiResult(_responseHelper.BadRequest("User exists"));
		}

		// POST dash/users/update
		[HttpPost("update")]
		[ProducesResponseType(typeof(IStandardCustomResponse), 200)]
		public async Task<CustomApiResult> Update([FromBody] User value)
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			// We do this manually as this endpoint will handle all updates to the user obj
			// and we want to be able to send in only the changed fields data
			var savedUser = await _userService.GetByNameIdentifier(nameIdentifier);

			// These are the only fields we allow changing
			if (!string.IsNullOrWhiteSpace(value.Email))
				savedUser.Email = value.Email;

			if (!string.IsNullOrWhiteSpace(value.FirstName))
				savedUser.FirstName = value.FirstName;

			if (!string.IsNullOrWhiteSpace(value.LastName))
				savedUser.LastName = value.LastName;

			if (!string.IsNullOrWhiteSpace(value.TogglApiKey) && value.TogglApiKey != PlaceholderConstants.TogglApiKeyPlaceholder)
			{
				// Always encrypt at rest
				savedUser.TogglApiKey = _cryptHelper.EncryptString(value.TogglApiKey);
			}

			if (!string.IsNullOrWhiteSpace(value.TogglWorkspaceID))
				savedUser.TogglWorkspaceID = value.TogglWorkspaceID;

			await _userService.Update(savedUser);

			return new CustomApiResult(_responseHelper.ValidResponse("User updated successfully"));
		}

		// POST dash/users/createusersecret
		[HttpPost("CreateUserSecret")]
		[ProducesResponseType(typeof(IStandardStringObjectCustomResponse), 200)]
		public async Task<CustomApiResult> CreateUserSecret()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var user = await _userService.GetByNameIdentifier(nameIdentifier);

			if (user == null)
				return new CustomApiResult(_responseHelper.NotFound("User not found"));

			user.UserSecret = Guid.NewGuid().ToString().Replace("-", "");

			await _userService.Update(user);

			return new CustomApiResult(_responseHelper.ValidResponse(user.UserSecret, "User secret created"));
		}

		// DELETE dash/users
		[HttpDelete]
		[ProducesResponseType(typeof(IStandardCustomResponse), 200)]
		public async Task<CustomApiResult> Delete()
		{
			string nameIdentifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var savedUser = await _userService.GetByNameIdentifier(nameIdentifier);

			await _userService.Remove(savedUser.Id);

			return new CustomApiResult(_responseHelper.ValidResponse());
		}
	}
}