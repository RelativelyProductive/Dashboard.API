using System.Collections.Generic;
using RP.Dashboard.API.Models.Data.WEB;

namespace RP.Dashboard.API.Models.Interfaces.ApiWrapper
{
	public interface IWorkspaceObjectCustomResponse : IStandardCustomResponse
	{
		UserWorkspace Data { get; set; }
	}

	public interface IWorkspacesObjectCustomResponse : IStandardCustomResponse
	{
		IEnumerable<UserWorkspace> Data { get; set; }
	}
}