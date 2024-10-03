using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddValidationsExistentsRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "details",
                table: "volunteer",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<float>(
                name: "weight",
                table: "pets",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<float>(
                name: "height",
                table: "pets",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "details",
                table: "volunteer",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "weight",
                table: "pets",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<double>(
                name: "height",
                table: "pets",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);
        }
    }
}
