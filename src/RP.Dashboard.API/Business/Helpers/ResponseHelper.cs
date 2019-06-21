using RP.Dashboard.API.Models.Data.WEB;

namespace RP.Dashboard.API.Business.Helpers
{
	public class ResponseHelper
	{
		public virtual DataResponse ValidResponse(object data, string message = "")
		{
			return new DataResponse
			{
				IsOk = true,
				StatusCode = 200,
				Message = message,
				Data = data
			};
		}

		public virtual DataResponse ValidResponse(string message = "")
		{
			return new DataResponse
			{
				IsOk = true,
				StatusCode = 200,
				Message = message
			};
		}

		public virtual DataResponse BadRequest(string message = "")
		{
			return new DataResponse
			{
				IsOk = false,
				StatusCode = 400,
				Message = message,
			};
		}

		public virtual DataResponse Unauthorized(string message = "")
		{
			return new DataResponse
			{
				IsOk = false,
				StatusCode = 401,
				Message = message,
			};
		}

		public virtual DataResponse NotFound(string message = "")
		{
			return new DataResponse
			{
				IsOk = false,
				StatusCode = 404,
				Message = message,
			};
		}

		public virtual DataResponse RateLimited(string message = "")
		{
			return new DataResponse
			{
				IsOk = false,
				StatusCode = 429,
				Message = message,
			};
		}
	}
}