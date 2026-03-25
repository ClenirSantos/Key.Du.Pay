using Key.Du.Pay.CrossCutting.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class ResponsavelFinanceiroVM
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }

        public DateTime DataCadastro { get; set; }

        public EnumAdimplencia Adimplente { get; set; }

        public EnumTipoUsuario TipoUsuario { get; set; }

    }
}
