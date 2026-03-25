using Key.Du.Pay.CrossCutting.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Key.Du.Pay.DataAccess.Database.Entities
{
    public class CobrancaEntity
    {
        [Key]
        [Column("CO_ID")]
        public int Id { get; set; }

        [Column("CO_DS")]
        public required string Descricao { get; set; }
        
        [Column("CO_DT_VENCIMENTO")]
        public DateTime DataVencimento { get; set; }

        [Column("CO_MT_PAGAMENTO")]
        public EnumMetodoPagamento MetodoPagamento { get; set; }

        [Column("CO_VL")]
        public decimal Valor { get; set; }

        [Column("CO_ST")]
        public EnumCobrancaStatus Status { get; set; }

        [Column("CO_CD_PAGAMENTO")]
        public double CodigoPagamento { get; set; }


        public required PlanoPagamentoEntity PlanoPagamento { get; set; }

        [Required]
        [Column("PP_ID")]
        [ForeignKey(nameof(PlanoPagamento))]
        public int PlanoPagamentoID { get; set; }
    }
}
