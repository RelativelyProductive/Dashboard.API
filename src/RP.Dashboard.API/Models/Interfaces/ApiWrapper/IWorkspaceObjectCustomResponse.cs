using System.Collections.Generic;
using RP.Dashboard.API.Models.Data.WEB;

namespace RP.Dashboard.API.Models.Interfaces.ApiWrapper
{
	public interface IProjectObjectCustomResponse : IStandardCustomResponse
	{
		UserProject Data { get; set; }
	}

	public interface IProjectsObjectCustomResponse : IStandardCustomResponse
	{
		IEnumerable<UserProject> Data { get; set; }
	}
}