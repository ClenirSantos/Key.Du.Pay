using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class PagamentoVM
    {
        public int Id { get; set; }

        public CobrancaVM? Cobranca { get; set; }

        public int CobrancaID { get; set; }

        public DateTime DataPagamento { get; set; }

        public decimal Valor { get; set; }
    }
}
