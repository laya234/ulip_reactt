using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ULIP_proj.Migrations
{
    public partial class Fone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Users_UserId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Premiums_Policies_PolicyId",
                table: "Premiums");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Policies_PolicyId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "PolicyFunds");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Policies");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PoliciesSold",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommissionEarned",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "TransactionType",
                table: "Transactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionReference",
                table: "Premiums",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Premiums",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "Premiums",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "Premiums",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CommissionPaid",
                table: "Premiums",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "PremiumFrequency",
                table: "Policies",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PolicyStatus",
                table: "Policies",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedDate",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "Policies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPaid",
                table: "Policies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProposedDate",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedDate",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Policies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SurrenderRequestDate",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SurrenderRequested",
                table: "Policies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SurrenderStatus",
                table: "Policies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SurrenderValue",
                table: "Policies",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RiskLevel",
                table: "Funds",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "FundType",
                table: "Funds",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "Approvals",
                columns: table => new
                {
                    ApprovalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    RequestedBy = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequestReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApprovalComments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvals", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_Approvals_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Approvals_Users_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QuotedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sales_Users_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ApprovedBy",
                table: "Approvals",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_RequestedBy",
                table: "Approvals",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_AgentId",
                table: "Sales",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PolicyId",
                table: "Sales",
                column: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Users_UserId",
                table: "Policies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Premiums_Policies_PolicyId",
                table: "Premiums",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Policies_PolicyId",
                table: "Transactions",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Users_UserId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Premiums_Policies_PolicyId",
                table: "Premiums");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Policies_PolicyId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Approvals");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PoliciesSold",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalCommissionEarned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "Premiums");

            migrationBuilder.DropColumn(
                name: "CommissionPaid",
                table: "Premiums");

            migrationBuilder.DropColumn(
                name: "AcceptedDate",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "CommissionPaid",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "ProposedDate",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "RejectedDate",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "SurrenderRequestDate",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "SurrenderRequested",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "SurrenderStatus",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "SurrenderValue",
                table: "Policies");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "Transactions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionReference",
                table: "Premiums",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Premiums",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Premiums",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PremiumFrequency",
                table: "Policies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PolicyStatus",
                table: "Policies",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Policies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "RiskLevel",
                table: "Funds",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FundType",
                table: "Funds",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "PolicyFunds",
                columns: table => new
                {
                    PolicyFundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundId = table.Column<int>(type: "int", nullable: false),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Units = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyFunds", x => x.PolicyFundId);
                    table.ForeignKey(
                        name: "FK_PolicyFunds_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "FundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyFunds_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyFunds_FundId",
                table: "PolicyFunds",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyFunds_PolicyId_FundId",
                table: "PolicyFunds",
                columns: new[] { "PolicyId", "FundId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Users_UserId",
                table: "Policies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Premiums_Policies_PolicyId",
                table: "Premiums",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Policies_PolicyId",
                table: "Transactions",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
