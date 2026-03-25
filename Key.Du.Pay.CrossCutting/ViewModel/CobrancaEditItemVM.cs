using Key.Du.Pay.CrossCutting.Enum;
using System;

namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class CobrancaEditItemVM
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public EnumMetodoPagamento MetodoPagamento { get; set; }
        public string? Descricao { get; set; }
        public bool Cancelada { get; set; }
        public EnumCobrancaStatus Status { get; set; }
    }
}
