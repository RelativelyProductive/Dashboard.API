using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Infrastructure;

namespace RP.Dashboard.API.Controllers
{
	[Route("dash/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ConfigurationHelper _configuration;
		private readonly ResponseHelper _responseHelper;

		public AuthController(ConfigurationHelper configuration, ResponseHelper responseHelper)
		{
			_configuration = configuration;
			_responseHelper = responseHelper;
		}

		[HttpGet("login")]
		public CustomApiResult Login()
		{
			// DEV ONLY
			if (_configuration.IsDevEnvironment())
			{
				using (var loginClient = new HttpClient())
				{
					var response = loginClient.PostAsync(new Uri(_configuration.GetAuthZeroAuthority() + "oauth/token"),
						new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\": \"" + _configuration.GetAuthZeroClientId() + "\",\"client_secret\": \"" +
						_configuration.GetAuthZeroSecret() + "\",\"audience\": \"" +
						_configuration.GetAuthZeroAudience() + "\"}", Encoding.UTF8, "application/json")).Result;
					if (response.IsSuccessStatusCode)
					{
						// parse the data
						dynamic loginResultJson = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

						if (loginResultJson != null)
						{
							return new CustomApiResult(_responseHelper.ValidResponse(loginResultJson));
						}
					}
				}
			}

			return new CustomApiResult(_responseHelper.BadRequest("This is for DEV"));
		}
	}
}