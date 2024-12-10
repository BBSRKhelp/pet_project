using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "species",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "volunteer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    work_experience = table.Column<byte>(type: "smallint", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    requisites = table.Column<string>(type: "jsonb", nullable: true),
                    social_networks = table.Column<string>(type: "jsonb", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_volunteer", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "breed",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_breed", x => x.id);
                    table.ForeignKey(
                        name: "fk_breed_species_species_id",
                        column: x => x.species_id,
                        principalTable: "species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    coloration = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    weight = table.Column<float>(type: "real", nullable: false, defaultValue: 0f),
                    height = table.Column<float>(type: "real", nullable: false, defaultValue: 0f),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    health_information = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_castrated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_vaccinated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    postalcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    serial_number = table.Column<int>(type: "integer", nullable: false),
                    pet_photo = table.Column<string>(type: "jsonb", nullable: false),
                    requisite = table.Column<string>(type: "jsonb", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    breed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pets", x => x.id);
                    table.ForeignKey(
                        name: "fk_pets_volunteer_volunteer_id",
                        column: x => x.volunteer_id,
                        principalTable: "volunteer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_breed_species_id",
                table: "breed",
                column: "species_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_volunteer_id",
                table: "pets",
                column: "volunteer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "breed");

            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "species");

            migrationBuilder.DropTable(
                name: "volunteer");
        }
    }
}
