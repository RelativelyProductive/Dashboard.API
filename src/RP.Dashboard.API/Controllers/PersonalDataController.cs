using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Business.Attributes;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Infrastructure;
using RP.Dashboard.API.Business.Services;

namespace RP.Dashboard.API.Controllers
{
	[Route("external/[controller]")]
	[ApiController]
	//[Authorize]
	// No auth here
	public class PersonalDataController : Controller
	{
		private readonly ConfigurationHelper _configuration;
		private readonly UserService _userService;
		private readonly ResponseHelper _responseHelper;

		public PersonalDataController(ConfigurationHelper configuration, UserService userService, ResponseHelper responseHelper)
		{
			_configuration = configuration;
			_userService = userService;
			_responseHelper = responseHelper;
		}

		// GET personaldata/GetUserBySecret/{userSecret}
		[HttpGet("GetUserBySecret")]
		[AllowXRequestsEveryXSeconds("GetUserBySecret", 15, 3)]
		public async Task<CustomApiResult> GetUserBySecret(string secret)
		{
			// THIS IS FOR DEV ONLY, for now
			if (_configuration.IsDevEnvironment())
			{
				// Prevent getting all users with no secret set
				if (string.IsNullOrWhiteSpace(secret))
					return new CustomApiResult(_responseHelper.NotFound("No user found"));

				var user = await _userService.GetBySecret(secret);
				if (user != null)
				{
					user.TogglApiKey = null;

					return new CustomApiResult(_responseHelper.ValidResponse(user));
				}
			}

			return new CustomApiResult(_responseHelper.BadRequest("This feature is still in development."));
		}
	}
}