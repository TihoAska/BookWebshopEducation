using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookWebshopEducation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companys",
                columns: new[] { "Id", "City", "Country", "Name", "PhoneNumber", "PostalCode", "StreetAddress" },
                values: new object[] { 1, "Djk", "Croatia", "MyCompany", "0991234567", 31400, "Long Street" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companys",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
