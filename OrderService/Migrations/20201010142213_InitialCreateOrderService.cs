using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderService.Migrations
{
    public partial class InitialCreateOrderService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerId = table.Column<long>(nullable: true),
                    amount = table.Column<decimal>(nullable: true),
                    state = table.Column<string>(nullable: false),
                    version = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
