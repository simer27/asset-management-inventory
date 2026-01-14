using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class FixItemDiscardRequestRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemDiscardRequests_AspNetUsers_RequestedById",
                table: "ItemDiscardRequests");

            migrationBuilder.DropIndex(
                name: "IX_ItemDiscardRequests_RequestedById",
                table: "ItemDiscardRequests");

            migrationBuilder.DropColumn(
                name: "RequestedById",
                table: "ItemDiscardRequests");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDiscardRequests_RequestedByUserId",
                table: "ItemDiscardRequests",
                column: "RequestedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDiscardRequests_AspNetUsers_RequestedByUserId",
                table: "ItemDiscardRequests",
                column: "RequestedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemDiscardRequests_AspNetUsers_RequestedByUserId",
                table: "ItemDiscardRequests");

            migrationBuilder.DropIndex(
                name: "IX_ItemDiscardRequests_RequestedByUserId",
                table: "ItemDiscardRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestedById",
                table: "ItemDiscardRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ItemDiscardRequests_RequestedById",
                table: "ItemDiscardRequests",
                column: "RequestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDiscardRequests_AspNetUsers_RequestedById",
                table: "ItemDiscardRequests",
                column: "RequestedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
