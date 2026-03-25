using Key.Du.Pay.CrossCutting.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Key.Du.Pay.DataAccess.Database.Entities
{
    public class ResponsavelFinanceiroEntity
    {

        [Key]
        [Column("RF_ID")]
        public int Id { get; set; }

        [Column("RF_DS")]
        public required string Descricao { get; set; }

        [Column("RF_DT_CADASTRO")]
        public DateTime DataCadastro { get; set; }

        [Column("RF_ST_ADIMPLENCIA")]
        public EnumAdimplencia Adimplente { get; set; }

        [Column("RF_TP_USUARIO")]
        public EnumTipoUsuario TipoUsuario { get; set; }

    }
}
