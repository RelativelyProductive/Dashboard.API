using System.Collections.Generic;
using RP.Dashboard.API.Models.Data.WEB;

namespace RP.Dashboard.API.Models.Interfaces.ApiWrapper
{
	public interface IClientObjectCustomResponse : IStandardCustomResponse
	{
		UserClient Data { get; set; }
	}

	public interface IClientsObjectCustomResponse : IStandardCustomResponse
	{
		IEnumerable<UserClient> Data { get; set; }
	}
}