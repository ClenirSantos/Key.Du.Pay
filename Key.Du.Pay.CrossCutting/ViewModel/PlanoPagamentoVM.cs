using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class PlanoPagamentoVM
    {
        public virtual ICollection<CobrancaVM>? CobrancaEntities { get; set; }

        public int Id { get; set; }

        public required CentroCustoVM CentroCusto { get; set; }

        public int CentroCustoID { get; set; }

        public required ResponsavelFinanceiroVM ResponsavelFinanceiro { get; set; }

        public int ResponsavelFinanceiroID { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal ValorTotalCancelado { get; set; }
    }
}
