using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Dodajemo kolonu 'Active' u tabelu 'Product' sa tipom 'string' (VARCHAR)
            migrationBuilder.AddColumn<string>(
                name: "Active",
                table: "Product",
                type: "varchar(255)", // Možete navesti željenu dužinu VARCHAR kolone
                nullable: true // Definišemo da kolona može imati NULL vrednost
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Uklanjamo kolonu 'Active' ako treba da poništimo migraciju
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Product"
            );
        }
    }
}
