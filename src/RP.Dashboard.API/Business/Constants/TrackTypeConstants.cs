using System.Diagnostics.CodeAnalysis;

namespace RP.Dashboard.API.Business.Constants
{
	[ExcludeFromCodeCoverage]
	public class TrackTypeConstants
	{
		public enum TrackType
		{
			None = 0,
			Client = 1,
			Project = 2,
			Task = 3,
			Tag = 4
		}
	}
}