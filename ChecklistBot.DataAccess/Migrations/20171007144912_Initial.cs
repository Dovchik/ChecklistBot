using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ChecklistBot.DataAccess.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckListItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckListId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckListItem_CheckList_CheckListId",
                        column: x => x.CheckListId,
                        principalTable: "CheckList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkCheckList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckListId = table.Column<int>(type: "INTEGER", nullable: false),
                    InlineMessageId = table.Column<string>(type: "TEXT", nullable: true),
                    MessageId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCheckList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkCheckList_CheckList_CheckListId",
                        column: x => x.CheckListId,
                        principalTable: "CheckList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkCheckListItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckListItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    WorkCheckListId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCheckListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkCheckListItem_CheckListItem_CheckListItemId",
                        column: x => x.CheckListItemId,
                        principalTable: "CheckListItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkCheckListItem_WorkCheckList_WorkCheckListId",
                        column: x => x.WorkCheckListId,
                        principalTable: "WorkCheckList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckListItem_CheckListId",
                table: "CheckListItem",
                column: "CheckListId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCheckList_CheckListId",
                table: "WorkCheckList",
                column: "CheckListId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCheckListItem_CheckListItemId",
                table: "WorkCheckListItem",
                column: "CheckListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCheckListItem_WorkCheckListId",
                table: "WorkCheckListItem",
                column: "WorkCheckListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkCheckListItem");

            migrationBuilder.DropTable(
                name: "CheckListItem");

            migrationBuilder.DropTable(
                name: "WorkCheckList");

            migrationBuilder.DropTable(
                name: "CheckList");
        }
    }
}
