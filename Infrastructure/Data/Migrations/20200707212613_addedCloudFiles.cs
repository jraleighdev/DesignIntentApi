using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class addedCloudFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CloudFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    ThreeDfile = table.Column<bool>(nullable: false),
                    DrawingFile = table.Column<bool>(nullable: false),
                    ForgeUrn = table.Column<string>(nullable: true),
                    ForgeBucket = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PartNumber = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StockNumber = table.Column<string>(nullable: true),
                    Qty = table.Column<int>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    CloudFileId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bills_CloudFiles_CloudFileId",
                        column: x => x.CloudFileId,
                        principalTable: "CloudFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_CloudFileId",
                table: "Bills",
                column: "CloudFileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "CloudFiles");
        }
    }
}
