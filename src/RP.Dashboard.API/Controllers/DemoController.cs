using Microsoft.AspNetCore.Mvc;
using RP.Dashboard.API.Business.Helpers;

namespace RP.Dashboard.API.Controllers
{
	[Route("/")]
	public class DemoController : Controller
	{
		private readonly ConfigurationHelper _configuration;

		public DemoController(ConfigurationHelper configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public ActionResult Index()
		{
			// DEV ONLY
			if (_configuration.IsDevEnvironment())
			{
				return View("index");
			}

			return View("up");
		}
	}
}