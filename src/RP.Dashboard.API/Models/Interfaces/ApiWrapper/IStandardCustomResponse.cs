namespace RP.Dashboard.API.Models.Interfaces.ApiWrapper
{
	public interface IStandardCustomResponse
	{
		bool IsOk { get; set; }

		string Message { get; set; }
	}

	public interface IStandardStringObjectCustomResponse
	{
		bool IsOk { get; set; }

		string Message { get; set; }

		string Data { get; set; }
	}
}