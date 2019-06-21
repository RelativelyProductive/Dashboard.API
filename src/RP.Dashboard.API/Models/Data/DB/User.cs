using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RP.Dashboard.API.Models.Data.DB
{
	[ExcludeFromCodeCoverage]
	public class User
	{
		public int Id { get; set; }

		public string NameIdentifier { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		[Required]
		public string Email { get; set; }

		public string TogglApiKey { get; set; }

		public string TogglWorkspaceID { get; set; }

		public string UserSecret { get; set; }
	}
}