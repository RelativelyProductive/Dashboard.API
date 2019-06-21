using System.Collections.Generic;
using RP.Dashboard.API.Models.Data.WEB;

namespace RP.Dashboard.API.Models.Interfaces.ApiWrapper
{
	public interface ITagObjectCustomResponse : IStandardCustomResponse
	{
		UserTag Data { get; set; }
	}

	public interface ITagsObjectCustomResponse : IStandardCustomResponse
	{
		IEnumerable<UserTag> Data { get; set; }
	}
}