namespace RP.Dashboard.API.Models.Interfaces
{
	public interface IDataResponse
	{
		int StatusCode { get; set; }

		bool IsOk { get; set; }

		string Message { get; set; }

		object Data { get; set; }
	}
}