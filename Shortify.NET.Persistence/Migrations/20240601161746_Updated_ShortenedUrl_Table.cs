using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shortify.NET.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Updated_ShortenedUrl_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_Users_UserId",
                table: "ShortenedUrls");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ShortenedUrls",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Tags",
                table: "ShortenedUrls",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ShortenedUrls",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_Users_UserId",
                table: "ShortenedUrls",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_Users_UserId",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ShortenedUrls");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ShortenedUrls",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_Users_UserId",
                table: "ShortenedUrls",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
