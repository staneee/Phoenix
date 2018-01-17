using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OneBlog.Data.Migrations
{
    public partial class data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Posts",
                newName: "ReadCount");

            migrationBuilder.RenameColumn(
                name: "CommentDate",
                table: "Comments",
                newName: "CreateDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "ReadCount",
                table: "Posts",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Comments",
                newName: "CommentDate");
        }
    }
}
