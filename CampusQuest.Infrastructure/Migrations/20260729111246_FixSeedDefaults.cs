using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CampusQuest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Badges_BadgeId1",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMissions_Missions_MissionId1",
                table: "UserMissions");

            migrationBuilder.DropIndex(
                name: "IX_UserMissions_MissionId1",
                table: "UserMissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges");

            migrationBuilder.DropIndex(
                name: "IX_UserBadges_BadgeId1",
                table: "UserBadges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Badges",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "MissionId1",
                table: "UserMissions");

            migrationBuilder.DropColumn(
                name: "BadgeId1",
                table: "UserBadges");

            migrationBuilder.RenameTable(
                name: "UserBadges",
                newName: "UserBadge");

            migrationBuilder.RenameTable(
                name: "Badges",
                newName: "Badge");

            migrationBuilder.RenameIndex(
                name: "IX_UserBadges_UserId",
                table: "UserBadge",
                newName: "IX_UserBadge_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadge",
                newName: "IX_UserBadge_BadgeId");

            migrationBuilder.AddColumn<string>(
                name: "QrCodeSecret",
                table: "Missions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBadge",
                table: "UserBadge",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Badge",
                table: "Badge",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Missions",
                columns: new[] { "Id", "CreatedAt", "Description", "Latitude", "Longitude", "QrCodeSecret", "RadiusInMeters", "Title", "XpReward" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kütüphanede sessizce çalış ve QR kodu okut.", 37.059199999999997, 35.355200000000004, "CAMPUSQUEST_KUTUPHANE", 20.0, "Merkezi Kütüphane", 100 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Günün menüsünü tat ve enerjini topla.", 37.058, 35.353999999999999, "CAMPUSQUEST_YEMEKHANE", 20.0, "Öğrenci Yemekhanesi", 150 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kampüsün yönetim merkezini ziyaret et.", 37.061, 35.356999999999999, "CAMPUSQUEST_REKTORLUK", 20.0, "Rektörlük Binası", 200 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Geleceğin teknolojilerini yerinde incele.", 37.0565, 35.351999999999997, "CAMPUSQUEST_MUHENDISLIK", 20.0, "Mühendislik Fakültesi", 120 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sağlam kafanın sağlam vücutta bulunduğunu kanıtla.", 37.063000000000002, 35.359000000000002, "CAMPUSQUEST_SPOR", 20.0, "Spor Salonu", 100 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadge_Badge_BadgeId",
                table: "UserBadge",
                column: "BadgeId",
                principalTable: "Badge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadge_Users_UserId",
                table: "UserBadge",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadge_Badge_BadgeId",
                table: "UserBadge");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBadge_Users_UserId",
                table: "UserBadge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBadge",
                table: "UserBadge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Badge",
                table: "Badge");

            migrationBuilder.DeleteData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DropColumn(
                name: "QrCodeSecret",
                table: "Missions");

            migrationBuilder.RenameTable(
                name: "UserBadge",
                newName: "UserBadges");

            migrationBuilder.RenameTable(
                name: "Badge",
                newName: "Badges");

            migrationBuilder.RenameIndex(
                name: "IX_UserBadge_UserId",
                table: "UserBadges",
                newName: "IX_UserBadges_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBadge_BadgeId",
                table: "UserBadges",
                newName: "IX_UserBadges_BadgeId");

            migrationBuilder.AddColumn<Guid>(
                name: "MissionId1",
                table: "UserMissions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BadgeId1",
                table: "UserBadges",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Badges",
                table: "Badges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserMissions_MissionId1",
                table: "UserMissions",
                column: "MissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeId1",
                table: "UserBadges",
                column: "BadgeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges",
                column: "BadgeId",
                principalTable: "Badges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Badges_BadgeId1",
                table: "UserBadges",
                column: "BadgeId1",
                principalTable: "Badges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMissions_Missions_MissionId1",
                table: "UserMissions",
                column: "MissionId1",
                principalTable: "Missions",
                principalColumn: "Id");
        }
    }
}
