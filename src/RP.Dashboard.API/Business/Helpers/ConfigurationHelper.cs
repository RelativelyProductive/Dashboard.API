using Microsoft.Extensions.Configuration;

namespace RP.Dashboard.API.Business.Helpers
{
	public class ConfigurationHelper
	{
		private readonly IConfiguration _configuration;

		public ConfigurationHelper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public virtual string GetCryptoKey()
		{
			return _configuration.GetValue<string>("Crypt:key");
		}

		public virtual string GetCryptoVector()
		{
			return _configuration.GetValue<string>("Crypt:vector");
		}

		public virtual bool IsDevEnvironment()
		{
			string result = _configuration.GetValue<string>("Environment");
			if (!string.IsNullOrWhiteSpace(result) && result == "DEV")
				return true;

			return false;
		}

		public virtual string GetAuthZeroClientId()
		{
			return _configuration.GetValue<string>("Auth0:ClientId");
		}

		public virtual string GetAuthZeroSecret()
		{
			return _configuration.GetValue<string>("Auth0:ClientSecret");
		}

		public virtual string GetAuthZeroAuthority()
		{
			return _configuration.GetValue<string>("Auth0:Authority");
		}

		public virtual string GetAuthZeroAudience()
		{
			return _configuration.GetValue<string>("Auth0:Audience");
		}
	}
}