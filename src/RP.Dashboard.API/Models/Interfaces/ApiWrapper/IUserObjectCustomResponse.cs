using System.Collections.Generic;
using RP.Dashboard.API.Models.Data.DB;
using RP.Dashboard.API.Models.Interfaces.ApiWrapper;

namespace RP.Dashboard.API.Models.ApiWrapper
{
	public interface IUserObjectCustomResponse : IStandardCustomResponse
	{
		User Data { get; set; }
	}

	public interface IUsersObjectCustomResponse : IStandardCustomResponse
	{
		IEnumerable<User> Data { get; set; }
	}
}