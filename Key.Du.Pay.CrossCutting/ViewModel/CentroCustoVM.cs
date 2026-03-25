using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class CentroCustoVM
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }
    }
}
