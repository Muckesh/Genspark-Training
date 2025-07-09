using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApi.Migrations
{
    /// <inheritdoc />
    public partial class testing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                table: "Inquiries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Inquiries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InquiryReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InquiryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorType = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryReplies_Inquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "Inquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_AgentId",
                table: "Inquiries",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryReplies_InquiryId",
                table: "InquiryReplies",
                column: "InquiryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_Agents_AgentId",
                table: "Inquiries",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_Agents_AgentId",
                table: "Inquiries");

            migrationBuilder.DropTable(
                name: "InquiryReplies");

            migrationBuilder.DropIndex(
                name: "IX_Inquiries_AgentId",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Inquiries");
        }
    }
}
