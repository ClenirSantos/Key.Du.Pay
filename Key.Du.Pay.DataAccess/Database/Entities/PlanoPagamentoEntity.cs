using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Key.Du.Pay.DataAccess.Database.Entities
{
    public class PlanoPagamentoEntity
    {

        public virtual ICollection<CobrancaEntity> CobrancaEntities { get; set; }

        public PlanoPagamentoEntity() => CobrancaEntities = new HashSet<CobrancaEntity>();

        [Key]
        [Column("PP_ID")] 
        public int Id { get; set; }

        public required CentroCustoEntity CentroCusto { get; set; }

        [Required]
        [Column("CC_ID")]
        [ForeignKey(nameof(CentroCusto))]
        public int CentroCustoID { get; set; }

        public required ResponsavelFinanceiroEntity ResponsavelFinanceiro { get; set; }

        [Required]
        [Column("RF_ID")]
        [ForeignKey(nameof(ResponsavelFinanceiro))]
        public int ResponsavelFinanceiroID { get; set; }




    }
}
