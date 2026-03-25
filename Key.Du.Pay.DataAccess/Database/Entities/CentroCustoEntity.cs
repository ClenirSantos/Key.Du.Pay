using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Key.Du.Pay.DataAccess.Database.Entities
{
    public class CentroCustoEntity
    {
        [Key]
        [Column("CC_ID")]
        public int Id { get; set; }

        [Column("CC_DS")]
        public required string Descricao { get; set; }
    }
}
