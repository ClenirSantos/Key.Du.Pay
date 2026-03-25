using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Key.Du.Pay.DataAccess.Database.Entities
{
    public class PagamentoEntity
    {
        [Key]
        [Column("PA_ID")]
        public int Id { get; set; }

        public required CobrancaEntity Cobranca { get; set; }

        [Required]
        [Column("CO_ID")]
        [ForeignKey(nameof(Cobranca))]
        public int CobrancaID { get; set; }


        [Column("PA_DT_PAGAMENTO")]
        public DateTime DataPagamento { get; set; }

        [Column("PA_VL_PAGAMENTO")]
        public decimal Valor { get; set; }

    }
}
