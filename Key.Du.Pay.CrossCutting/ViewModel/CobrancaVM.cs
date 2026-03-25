using Key.Du.Pay.CrossCutting.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class CobrancaVM
    {
        public int Id { get; set; }


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


        public PlanoPagamentoVM? PlanoPagamento { get; set; }

        public int PlanoPagamentoID { get; set; }

        public bool EstaVencida =>
            Status != EnumCobrancaStatus.Paga
            && Status != EnumCobrancaStatus.Cancelada
            && DataVencimento.Date < DateTime.UtcNow.Date;
    }
}
