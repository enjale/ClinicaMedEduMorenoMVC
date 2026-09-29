using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaMedEduardoMorenoMVCWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposMedicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Diagnostico",
                table: "ConsultaEnfermedades");

            migrationBuilder.DropColumn(
                name: "Grado",
                table: "Alergias");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Alergias",
                newName: "Codigo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDeteccion",
                table: "ExpedienteEnfermedades",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "VitalTemperatura",
                table: "Consultas",
                type: "decimal(4,1)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "VitalPeso",
                table: "Consultas",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Tratamiento",
                table: "Consultas",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "Consultas",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Consultas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Medico",
                table: "Consultas",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Consultas",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Consultas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Diagnostico",
                table: "Consultas",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Consultas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "VitalPresion",
                table: "Consultas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "ConsultaEnfermedades",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpedienteId",
                table: "Antecedentes",
                type: "int",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_Expedientes_PacienteId",
                table: "Expedientes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpedienteEnfermedades_EnfermedadId",
                table: "ExpedienteEnfermedades",
                column: "EnfermedadId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpedienteEnfermedades_ExpedienteId",
                table: "ExpedienteEnfermedades",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpedienteAlergias_AlergiaId",
                table: "ExpedienteAlergias",
                column: "AlergiaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpedienteAlergias_ExpedienteId",
                table: "ExpedienteAlergias",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Antecedentes_ExpedienteId",
                table: "Antecedentes",
                column: "ExpedienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Antecedentes_Expedientes_ExpedienteId",
                table: "Antecedentes",
                column: "ExpedienteId",
                principalTable: "Expedientes",
                principalColumn: "ExpedienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpedienteAlergias_Alergias_AlergiaId",
                table: "ExpedienteAlergias",
                column: "AlergiaId",
                principalTable: "Alergias",
                principalColumn: "AlergiaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpedienteAlergias_Expedientes_ExpedienteId",
                table: "ExpedienteAlergias",
                column: "ExpedienteId",
                principalTable: "Expedientes",
                principalColumn: "ExpedienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpedienteEnfermedades_Enfermedades_EnfermedadId",
                table: "ExpedienteEnfermedades",
                column: "EnfermedadId",
                principalTable: "Enfermedades",
                principalColumn: "EnfermedadId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpedienteEnfermedades_Expedientes_ExpedienteId",
                table: "ExpedienteEnfermedades",
                column: "ExpedienteId",
                principalTable: "Expedientes",
                principalColumn: "ExpedienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expedientes_Pacientes_PacienteId",
                table: "Expedientes",
                column: "PacienteId",
                principalTable: "Pacientes",
                principalColumn: "PacienteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Antecedentes_Expedientes_ExpedienteId",
                table: "Antecedentes");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpedienteAlergias_Alergias_AlergiaId",
                table: "ExpedienteAlergias");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpedienteAlergias_Expedientes_ExpedienteId",
                table: "ExpedienteAlergias");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpedienteEnfermedades_Enfermedades_EnfermedadId",
                table: "ExpedienteEnfermedades");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpedienteEnfermedades_Expedientes_ExpedienteId",
                table: "ExpedienteEnfermedades");

            migrationBuilder.DropForeignKey(
                name: "FK_Expedientes_Pacientes_PacienteId",
                table: "Expedientes");

            migrationBuilder.DropIndex(
                name: "IX_Expedientes_PacienteId",
                table: "Expedientes");

            migrationBuilder.DropIndex(
                name: "IX_ExpedienteEnfermedades_EnfermedadId",
                table: "ExpedienteEnfermedades");

            migrationBuilder.DropIndex(
                name: "IX_ExpedienteEnfermedades_ExpedienteId",
                table: "ExpedienteEnfermedades");

            migrationBuilder.DropIndex(
                name: "IX_ExpedienteAlergias_AlergiaId",
                table: "ExpedienteAlergias");

            migrationBuilder.DropIndex(
                name: "IX_ExpedienteAlergias_ExpedienteId",
                table: "ExpedienteAlergias");

            migrationBuilder.DropIndex(
                name: "IX_Antecedentes_ExpedienteId",
                table: "Antecedentes");

            migrationBuilder.DropColumn(
                name: "VitalPresion",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "ConsultaEnfermedades");

            migrationBuilder.DropColumn(
                name: "ExpedienteId",
                table: "Antecedentes");


            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "Alergias",
                newName: "Descripcion");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDeteccion",
                table: "ExpedienteEnfermedades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VitalTemperatura",
                table: "Consultas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VitalPeso",
                table: "Consultas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tratamiento",
                table: "Consultas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "Consultas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Consultas",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Medico",
                table: "Consultas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Consultas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Consultas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Diagnostico",
                table: "Consultas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Consultas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "Diagnostico",
                table: "ConsultaEnfermedades",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Grado",
                table: "Alergias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
