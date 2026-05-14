using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IVF_Managment_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase2IvfClinicalCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmbryoObservations_Embryos_EmbryoId",
                table: "EmbryoObservations");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentPhase",
                table: "IvfCycles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                table: "IvfCycles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Embryos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Embryos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "EmbryoId",
                table: "EmbryoObservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EmbryoClinicalInstructions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmbryoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rationale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbryoClinicalInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmbryoClinicalInstructions_Embryos_EmbryoId",
                        column: x => x.EmbryoId,
                        principalTable: "Embryos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmbryoCryopreservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmbryoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cane = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StrawPosition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FreezingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VitrificationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbryoCryopreservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmbryoCryopreservations_Embryos_EmbryoId",
                        column: x => x.EmbryoId,
                        principalTable: "Embryos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmbryoClinicalInstructions_EmbryoId",
                table: "EmbryoClinicalInstructions",
                column: "EmbryoId");

            migrationBuilder.CreateIndex(
                name: "IX_EmbryoCryopreservations_EmbryoId",
                table: "EmbryoCryopreservations",
                column: "EmbryoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmbryoObservations_Embryos_EmbryoId",
                table: "EmbryoObservations",
                column: "EmbryoId",
                principalTable: "Embryos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmbryoObservations_Embryos_EmbryoId",
                table: "EmbryoObservations");

            migrationBuilder.DropTable(
                name: "EmbryoClinicalInstructions");

            migrationBuilder.DropTable(
                name: "EmbryoCryopreservations");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "IvfCycles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Embryos");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentPhase",
                table: "IvfCycles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Embryos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmbryoId",
                table: "EmbryoObservations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_EmbryoObservations_Embryos_EmbryoId",
                table: "EmbryoObservations",
                column: "EmbryoId",
                principalTable: "Embryos",
                principalColumn: "Id");
        }
    }
}
