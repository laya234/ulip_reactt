using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ULIP_proj.Migrations
{
    public partial class Fonee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "KYCVerified",
                table: "Policies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "KYCVerifiedAt",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MedicalVerified",
                table: "Policies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PolicyDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VerifiedBy = table.Column<int>(type: "int", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyDocuments", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_PolicyDocuments_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PolicyDocuments_Users_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDocuments_PolicyId",
                table: "PolicyDocuments",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDocuments_VerifiedBy",
                table: "PolicyDocuments",
                column: "VerifiedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyDocuments");

            migrationBuilder.DropColumn(
                name: "KYCVerified",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "KYCVerifiedAt",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "MedicalVerified",
                table: "Policies");
        }
    }
}
