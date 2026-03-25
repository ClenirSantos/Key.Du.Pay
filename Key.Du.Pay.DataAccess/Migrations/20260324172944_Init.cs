using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Key.Du.Pay.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CentroCusto",
                columns: table => new
                {
                    CC_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CC_DS = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentroCusto", x => x.CC_ID);
                });

            migrationBuilder.CreateTable(
                name: "ResponsavelFinanceiro",
                columns: table => new
                {
                    RF_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RF_DS = table.Column<string>(type: "text", nullable: false),
                    RF_DT_CADASTRO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RF_ST_ADIMPLENCIA = table.Column<int>(type: "integer", nullable: false),
                    RF_TP_USUARIO = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsavelFinanceiro", x => x.RF_ID);
                });

            migrationBuilder.CreateTable(
                name: "PlanoPagamento",
                columns: table => new
                {
                    PP_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CC_ID = table.Column<int>(type: "integer", nullable: false),
                    RF_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoPagamento", x => x.PP_ID);
                    table.ForeignKey(
                        name: "FK_PlanoPagamento_CentroCusto_CC_ID",
                        column: x => x.CC_ID,
                        principalTable: "CentroCusto",
                        principalColumn: "CC_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanoPagamento_ResponsavelFinanceiro_RF_ID",
                        column: x => x.RF_ID,
                        principalTable: "ResponsavelFinanceiro",
                        principalColumn: "RF_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cobranca",
                columns: table => new
                {
                    CO_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CO_DS = table.Column<string>(type: "text", nullable: false),
                    CO_DT_VENCIMENTO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CO_MT_PAGAMENTO = table.Column<int>(type: "integer", nullable: false),
                    CO_VL = table.Column<decimal>(type: "numeric", nullable: false),
                    CO_ST = table.Column<int>(type: "integer", nullable: false),
                    CO_CD_PAGAMENTO = table.Column<double>(type: "double precision", nullable: false),
                    PP_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cobranca", x => x.CO_ID);
                    table.ForeignKey(
                        name: "FK_Cobranca_PlanoPagamento_PP_ID",
                        column: x => x.PP_ID,
                        principalTable: "PlanoPagamento",
                        principalColumn: "PP_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagamento",
                columns: table => new
                {
                    PA_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CO_ID = table.Column<int>(type: "integer", nullable: false),
                    PA_DT_PAGAMENTO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PA_VL_PAGAMENTO = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamento", x => x.PA_ID);
                    table.ForeignKey(
                        name: "FK_Pagamento_Cobranca_CO_ID",
                        column: x => x.CO_ID,
                        principalTable: "Cobranca",
                        principalColumn: "CO_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CentroCusto",
                columns: new[] { "CC_ID", "CC_DS" },
                values: new object[,]
                {
                    { 1, "Matricula" },
                    { 2, "Mensalidade" },
                    { 3, "Material" }
                });

            migrationBuilder.InsertData(
                table: "ResponsavelFinanceiro",
                columns: new[] { "RF_ID", "RF_ST_ADIMPLENCIA", "RF_DT_CADASTRO", "RF_DS", "RF_TP_USUARIO" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 3, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Instituição de Ensino Padre Anchieta", 1 },
                    { 2, 1, new DateTime(2026, 3, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Escola de Educação Infantil Pinguinho de Gente", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cobranca_PP_ID",
                table: "Cobranca",
                column: "PP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_CO_ID",
                table: "Pagamento",
                column: "CO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoPagamento_CC_ID",
                table: "PlanoPagamento",
                column: "CC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoPagamento_RF_ID",
                table: "PlanoPagamento",
                column: "RF_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagamento");

            migrationBuilder.DropTable(
                name: "Cobranca");

            migrationBuilder.DropTable(
                name: "PlanoPagamento");

            migrationBuilder.DropTable(
                name: "CentroCusto");

            migrationBuilder.DropTable(
                name: "ResponsavelFinanceiro");
        }
    }
}
