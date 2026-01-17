using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demo.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accessories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accessories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Accessories",
                columns: new[] { "Id", "Category", "Description", "ImageUrl", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Collars", "Durable leather collar with brass buckle", null, "Leather Collar", 24.99m, 50 },
                    { 2, "Toys", "Bouncy ball with bell inside for interactive play", null, "Interactive Toy Ball", 12.99m, 100 },
                    { 3, "Bedding", "Comfortable orthopedic dog bed with memory foam", null, "Soft Dog Bed", 49.99m, 30 },
                    { 4, "Toys", "Braided rope toy for tug of war", null, "Rope Toy", 9.99m, 75 },
                    { 5, "Grooming", "Slicker brush for removing loose fur and mats", null, "Grooming Brush", 14.99m, 60 },
                    { 6, "Feeding", "Stainless steel food and water bowls with rubber base", null, "Pet Food Bowl Set", 19.99m, 80 },
                    { 7, "Leashes", "16ft retractable leash with locking mechanism", null, "Retractable Leash", 22.99m, 45 },
                    { 8, "Housing", "Durable plastic crate for training and travel", null, "Pet Crate", 79.99m, 20 },
                    { 9, "Treats", "Natural rawhide chew sticks (pack of 5)", null, "Chew Stick Pack", 8.99m, 120 },
                    { 10, "Travel", "Ventilated backpack carrier for pets up to 15 lbs", null, "Pet Carrier Backpack", 39.99m, 25 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accessories");
        }
    }
}
