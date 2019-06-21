using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RP.Dashboard.API.Business.Helpers;

namespace RP.Dashboard.API.Business.Clients
{
	// We can safely exclude this is it is mainly a wrapper for HttpClient which allows us to mock its usage elsewhere.
	[ExcludeFromCodeCoverage]
	public class TogglHttpClient
	{
		private CryptHelper _cryptHelper { get; }

		public TogglHttpClient(CryptHelper cryptHelper)
		{
			_cryptHelper = cryptHelper;
		}

		public virtual async Task<string> PostAsync(string requestUri, string userTogglApiKey, HttpContent content)
		{
			using (var togglClient = new HttpClient())
			{
				// Default Toggl Headers
				togglClient.DefaultRequestHeaders.Accept.Clear();
				togglClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(_cryptHelper.DecryptString(userTogglApiKey) +
					":api_token"));
				togglClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

				// Send the response to Toggl Api
				var resultResponse = await togglClient.PostAsync(requestUri, content);

				if (!resultResponse.IsSuccessStatusCode)
					return null;

				// Check the response data
				if (resultResponse?.Content?.ReadAsStringAsync()?.Result == null)
					return null;

				return resultResponse.Content.ReadAsStringAsync().Result;
			}
		}

		public virtual async Task<string> GetAsync(string requestUri, string userTogglApiKey)
		{
			using (var togglClient = new HttpClient())
			{
				// Default Toggl Headers
				togglClient.DefaultRequestHeaders.Accept.Clear();
				togglClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(_cryptHelper.DecryptString(userTogglApiKey) +
					":api_token"));
				togglClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

				// Send to toggl API
				var resultResponse = await togglClient.GetAsync(requestUri, new CancellationToken());

				if (!resultResponse.IsSuccessStatusCode)
					return null;

				// Check the response data
				if (resultResponse?.Content?.ReadAsStringAsync()?.Result == null)
					return null;

				return resultResponse.Content.ReadAsStringAsync().Result;
			}
		}
	}
}