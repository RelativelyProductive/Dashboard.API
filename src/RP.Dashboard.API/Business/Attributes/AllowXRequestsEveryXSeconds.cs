using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Infrastructure;

namespace RP.Dashboard.API.Business.Attributes
{
	/// <summary>
	/// SECURE: Decorates any MVC route that needs to have client requests limited over time.
	/// </summary>
	/// <remarks>
	/// Uses the current System.Web.Caching.Cache to store each client request to the decorated route.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method)]
	public class AllowXRequestsEveryXSecondsAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// A unique name for this Action Throttle.
		/// </summary>
		/// <remarks>
		/// We'll be inserting a Cache record based on this ActionName and client IP, e.g. "Name-192.168.0.1:9001"
		/// </remarks>
		private string _actionName { get; }

		/// <summary>
		/// The number of seconds clients must wait before executing this decorated route again.
		/// </summary>
		private int _seconds { get; }

		/// <summary>
		/// The number of requests to allow per client in the given number of seconds
		/// </summary>
		private int _requests { get; }

		public IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions { });

		private readonly ResponseHelper _responseHelper = new ResponseHelper();

		public AllowXRequestsEveryXSecondsAttribute(string actionName, int seconds, int requests)
		{
			_actionName = actionName;
			_seconds = seconds;
			_requests = requests;
		}

		public override void OnActionExecuting(ActionExecutingContext c)
		{
			if (c == null) throw new ArgumentException("ActionExecutingContext not specified");

			var key = string.Concat("AllowXRequestsEveryXSeconds-", _actionName, "-", c.HttpContext.Request.Host.Value);

			// Set cache options.
			var cacheEntryOptions = new MemoryCacheEntryOptions()
				// Keep in cache for this time, reset time if accessed.
				.SetSlidingExpiration(TimeSpan.FromSeconds(_seconds));

			var allowExecute = false;
			var cacheEntry = 0;

			if (!_cache.TryGetValue(key, out cacheEntry))
			{
				// No data in cache, save data for the first time
				_cache.Set(key, cacheEntry, cacheEntryOptions);

				allowExecute = true;
			}
			else
			{
				if (cacheEntry++ <= _requests)
				{
					// Save data in cache.
					_cache.Set(key, cacheEntry, cacheEntryOptions);

					allowExecute = true;
				}
			}

			if (!allowExecute)
			{
				// see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
				//c.HttpContext.Response.TrySkipIisCustomErrors = true; //to prevent iis from showing default 409 page

				c.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
				c.Result = new CustomApiResult(_responseHelper.RateLimited("You have been rate limited. Please try again later."));
			}
		}
	}
}