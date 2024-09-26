using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorIdentityApp.Data.Migrations
{
	/// <inheritdoc />
	public partial class PredefinedRules : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName) VALUES ('913e3f9c-1247-4a2c-8523-34ac1e136190', NULL, 'Admin', 'ADMIN')");
			migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName) VALUES ('f411d2fa-6839-4f1a-92fe-a047afe57472', NULL, 'Editor', 'EDITOR')");
			migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName) VALUES ('d326f21c-9a2b-4e50-a28a-83fa6a4b5a16', NULL, 'Viewer', 'VIEWER')");
			migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName) VALUES ('9114ab3f-546e-4042-9323-d53e23a7c9e1', NULL, 'Support', 'SUPPORT')");

		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
