using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNedov.IsiMarkets.EtlPipeline.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    unique_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fname = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    lname = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    cust_email = table.Column<string>(type: "nvarchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    iname = table.Column<string>(type: "nvarchar(24)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    unique_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    iname = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    idesc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_configurations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    unique_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_configurations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transction_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    iname = table.Column<string>(type: "nvarchar(24)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transction_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    unique_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    unitprice = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    location = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    customers_id = table.Column<int>(type: "int", nullable: false),
                    products_id = table.Column<int>(type: "int", nullable: false),
                    payment_methods_id = table.Column<int>(type: "int", nullable: false),
                    transction_statuses_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_customers_transactions_customers_customers_id",
                        column: x => x.customers_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customers_transactions_payment_methods_payment_methods_id",
                        column: x => x.payment_methods_id,
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customers_transactions_products_products_id",
                        column: x => x.products_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customers_transactions_transction_statuses_transction_statuses_id",
                        column: x => x.transction_statuses_id,
                        principalTable: "transction_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_fname_lname_cust_email",
                table: "customers",
                columns: new[] { "fname", "lname", "cust_email" });

            migrationBuilder.CreateIndex(
                name: "IX_customers_transactions_customers_id",
                table: "customers_transactions",
                column: "customers_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_transactions_payment_methods_id",
                table: "customers_transactions",
                column: "payment_methods_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_transactions_products_id",
                table: "customers_transactions",
                column: "products_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_transactions_transction_statuses_id",
                table: "customers_transactions",
                column: "transction_statuses_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_iname",
                table: "products",
                column: "iname");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_addnewproduct
                    @unique_id uniqueidentifier,
                    @iname nvarchar(64),
                    @idesc nvarchar(max),
                    @new_id INT OUTPUT
                AS
                BEGIN
                    INSERT INTO products (unique_id, iname, idesc)
	                VALUES(@unique_id, @iname, @idesc);
                    SET @new_id = SCOPE_IDENTITY();
                END;
                GO

                CREATE PROCEDURE sp_updateproduct
                    @unique_id uniqueidentifier,
                    @iname nvarchar(64),
                    @idesc nvarchar(max),
                    @updated_id INT OUTPUT
                AS
                BEGIN
                UPDATE products
                   SET iname = @iname
                      ,idesc = @idesc
                   WHERE unique_id = @unique_id;
                SET @updated_id = (SELECT id FROM products WHERE unique_id = @unique_id);
                END;
                GO

                CREATE PROCEDURE sp_addnewcustomer
                    @unique_id uniqueidentifier,
                    @fname nvarchar(32),
	                @lname nvarchar(32),
                    @cust_email nvarchar(64),
                    @new_id int OUTPUT  
                AS
                BEGIN
                    INSERT INTO customers (unique_id, fname, lname, cust_email)
	                VALUES(@unique_id, @fname, @lname, @cust_email);
                    SET @new_id = SCOPE_IDENTITY();
                END;
                GO

                CREATE PROCEDURE sp_updatecustomer
                    @unique_id uniqueidentifier,
                    @fname nvarchar(32),
	                @lname nvarchar(32),
                    @updated_id int OUTPUT
                AS
                BEGIN
                UPDATE customers
                   SET 
                       fname = @fname
                      ,lname = @lname
                   WHERE unique_id = @unique_id
                END;
                SET @updated_id = (SELECT id FROM customers WHERE unique_id = @unique_id);
                GO
                
                CREATE PROCEDURE sp_updatetransaction
                    @unique_id UNIQUEIDENTIFIER,
                    @unitprice DECIMAL(19, 4),
                    @quantity DECIMAL(19, 4),
                    @discount DECIMAL(19, 4),
                    @total_price DECIMAL(19, 4),
                    @timestamp DATETIME2,
                    @location NVARCHAR(128),
                    @customers_id INT,
                    @products_id INT,
                    @payment_methods_id INT,
                    @transction_statuses_id INT,
                    @updated_id int OUTPUT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE customers_transactions
                    SET 
                        unitprice = @unitprice,
                        quantity = @quantity,
                        discount = @discount,
                        total_price = @total_price,
                        timestamp = @timestamp,
                        location = @location,
                        customers_id = @customers_id,
                        products_id = @products_id,
                        payment_methods_id = @payment_methods_id,
                        transction_statuses_id = @transction_statuses_id
                    WHERE unique_id = @unique_id;
                    SET @updated_id = (SELECT id FROM customers_transactions WHERE unique_id = @unique_id);
                END;
                GO

                CREATE PROCEDURE sp_addnewtransaction
                    @unique_id UNIQUEIDENTIFIER,
                    @unitprice DECIMAL(19, 4),
                    @quantity DECIMAL(19, 4),
                    @timestamp DATETIME2,
                    @discount DECIMAL(19, 4),
                    @total_price DECIMAL(19, 4),
                    @location NVARCHAR(128),
                    @products_id INT,
                    @customers_id INT,
                    @payment_methods_id INT,
                    @transction_statuses_id INT,
                    @new_id INT OUTPUT
                AS
                BEGIN
                    INSERT INTO customers_transactions (
                        unique_id, unitprice, quantity, timestamp, discount, total_price, location, 
                        products_id, customers_id, payment_methods_id, transction_statuses_id
                    )
                    VALUES (
                        @unique_id, @unitprice, @quantity, @timestamp, @discount, @total_price, @location, 
                        @products_id, @customers_id, @payment_methods_id, @transction_statuses_id
                    );
                    SET @new_id = SCOPE_IDENTITY();
                END;
                GO
                
                BEGIN
                INSERT INTO payment_methods(iname) VALUES
                    ('Cash'),
                    ('CreditCard'),
                    ('DebitCard'),
                    ('DigitalWallet'),
                    ('Voucher');

                    INSERT INTO transction_statuses(iname) VALUES
                    ('Completed'),
                    ('Cancelled'),
                    ('Refunded'),
                    ('Pending')
                END;
                GO

                CREATE PROCEDURE sp_getproducts
                    @skip INT,
                    @take INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        unique_id,
                        iname,
                        idesc
                    FROM 
                        products
                    ORDER BY 
                        unique_id
                    OFFSET @skip ROWS
                    FETCH NEXT @take ROWS ONLY;
                END;
                GO

                CREATE PROCEDURE sp_getcustomers
                    @skip INT,
                    @take INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT *
                    FROM customers
                    ORDER BY lname, fname
                    OFFSET @skip ROWS
                    FETCH NEXT @take ROWS ONLY;
                END
                GO

                CREATE PROCEDURE sp_getcustomertransactions
                    @customerId UNIQUEIDENTIFIER,
                    @skip INT,
                    @take INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT *
                    FROM customers_transactions
                    WHERE unique_id = @customerId
                    ORDER BY timestamp
                    OFFSET @skip ROWS
                    FETCH NEXT @take ROWS ONLY;
                END
                GO
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers_transactions");

            migrationBuilder.DropTable(
                name: "system_configurations");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "transction_statuses");
        }
    }
}
