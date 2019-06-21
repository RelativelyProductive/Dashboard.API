using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Models.Interfaces;

namespace RP.Dashboard.API.Business.Infrastructure
{
	public class CustomApiResult : IActionResult
	{
		private readonly IDataResponse _result;

		public CustomApiResult(IDataResponse result)
		{
			_result = result;
		}

		public async Task ExecuteResultAsync(ActionContext context)
		{
			var status = StatusCodes.Status200OK;

			switch (_result.StatusCode)
			{
				case 400:
					status = StatusCodes.Status400BadRequest;
					break;

				case 401:
					status = StatusCodes.Status401Unauthorized;
					break;

				case 404:
					status = StatusCodes.Status404NotFound;
					break;

				case 429:
					status = StatusCodes.Status429TooManyRequests;
					break;

				case 500:
					status = StatusCodes.Status500InternalServerError;
					break;
			}

			var result = new ObjectResult(_result)
			{
				StatusCode = status
			};

			await result.ExecuteResultAsync(context);
		}
	}
}