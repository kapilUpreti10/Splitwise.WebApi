using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Splitwise.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class seeddataadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "user-001", 0, "Kathmandu", "concurrency-stamp-user-001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "kapil@example.com", true, false, null, "Kapil Upreti", "KAPIL@EXAMPLE.COM", "KAPIL", "AQAAAAIAAYagAAAAEAZx6oWb1/m9EvmbWMcQZvQYteRtB4w2AqiaNoqO+Eqfc2x/mpr1Mk1S0A3OOxza0g==", null, false, "security-stamp-user-001", false, "kapil" },
                    { "user-002", 0, "Mahendranagar", "concurrency-stamp-user-002", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "niraj@example.com", true, false, null, "Niraj Karki", "NIRAJ@EXAMPLE.COM", "NIRAJ", "AQAAAAIAAYagAAAAEKbVcYcgbeTr5H12+wl7GsK3jhMTYnvxt7NYbGgSJRvbVbvvHJsgm29TLgKCkWarXw==", null, false, "security-stamp-user-002", false, "Niraj" },
                    { "user-003", 0, "Argakhanchi", "concurrency-stamp-user-003", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "pratap@example.com", true, false, null, "Pratap Kunwar", "PRATAP@EXAMPLE.COM", "PRATAP", "AQAAAAIAAYagAAAAEMVeU+XMblpEyNLe8wlsQwvZiR1twdlvhi//vM1XVb2eaRfX0uF8H3ti0NYsvHwRTA==", null, false, "security-stamp-user-003", false, "Pratap" },
                    { "user-004", 0, "Butwal", "concurrency-stamp-user-004", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "pariskar@example.com", true, false, null, "Pariskar Poudel", "PARISKAR@EXAMPLE.COM", "PARISKAR", "AQAAAAIAAYagAAAAEPKhSgXj7sWfG0Iqhy++5f6LUicpWcMJuFGbRVWtBGajmFsUWZyJRDna4fuwEaDn5A==", null, false, "security-stamp-user-004", false, "Pariskar" },
                    { "user-005", 0, "Dang", "concurrency-stamp-user-005", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "parbat@example.com", true, false, null, "Parbat Pandey", "PARBAT@EXAMPLE.COM", "PARBAT", "AQAAAAIAAYagAAAAEDc2EU4wGZbdZLQxbpdxc8HwLQV7u0foLPO71WpCv190whBUZnLAC01feheDq3RcRA==", null, false, "security-stamp-user-005", false, "Parbat" }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "Name" },
                values: new object[] { 1, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "user-001", "entrepreneurship friday class may 30", "Khimchi" });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "CreatedAt", "GroupId", "PaidBy", "TotalAmount" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "user-001", 1500m },
                    { 2, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1, "user-002", 2000m }
                });

            migrationBuilder.InsertData(
                table: "ExpenseSplits",
                columns: new[] { "Id", "ExpenseId", "IndivudialAmount", "UserId" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), 1, 500m, "user-005" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), 1, 200m, "user-002" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), 1, 500m, "user-003" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), 1, 300m, "user-004" },
                    { new Guid("20000000-0000-0000-0000-000000000001"), 2, 400m, "user-001" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), 2, 400m, "user-002" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), 2, 800m, "user-004" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), 2, 400m, "user-005" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId_UserId",
                table: "GroupMembers",
                columns: new[] { "GroupId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupMembers_GroupId_UserId",
                table: "GroupMembers");

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ExpenseSplits",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-003");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-004");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-005");

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-002");

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-001");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers",
                column: "GroupId");
        }
    }
}
