using System.Diagnostics.CodeAnalysis;
using RP.Dashboard.API.Models.Interfaces;

namespace RP.Dashboard.API.Models.Data.WEB
{
	[ExcludeFromCodeCoverage]
	public class DataResponse : IDataResponse
	{
		public bool IsOk { get; set; }

		public int StatusCode { get; set; }

		public string Message { get; set; }

		public object Data { get; set; }
	}
}