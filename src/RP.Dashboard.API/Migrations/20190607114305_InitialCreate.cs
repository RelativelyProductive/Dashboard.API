using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RP.Dashboard.API.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Goals",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<int>(nullable: false),
					GoalFriendlyName = table.Column<string>(nullable: false),
					DailyGoalTimeInMs = table.Column<int>(nullable: false),
					DailyGoalTogglID = table.Column<string>(nullable: false),
					DailyGoalTrackStyle = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Goals", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					NameIdentifier = table.Column<string>(nullable: true),
					FirstName = table.Column<string>(nullable: true),
					LastName = table.Column<string>(nullable: true),
					Email = table.Column<string>(nullable: false),
					TogglApiKey = table.Column<string>(nullable: true),
					TogglWorkspaceID = table.Column<string>(nullable: true),
					UserSecret = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Goals");

			migrationBuilder.DropTable(
				name: "Users");
		}
	}
}