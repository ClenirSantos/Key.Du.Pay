using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Key.Du.Pay.UserInterface.Controllers.Api
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/planos-de-pagamento")]
    public class PlanosPagamentoApiController : ControllerBase
    {
        private readonly IPlanoPagamentoBll _bll;

        public PlanosPagamentoApiController(IPlanoPagamentoBll bll)
        {
            _bll = bll;
        }

        [HttpPost]
        public async Task<ActionResult<PlanoPagamentoVM>> Post([FromBody] PlanoPagamentoApiRequestDto dto)
        {
            var vm = new PlanoPagamentoCreateVM
            {
                ResponsavelId = dto.ResponsavelId,
                CentroDeCusto = dto.CentroDeCusto,
                Cobrancas = dto.Cobrancas.Select(MapItem).ToList()
            };
            var criado = await _bll.CriarAsync(vm);
            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlanoPagamentoVM>> GetById(int id)
        {
            var p = await _bll.ObterPorIdAsync(id);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpGet("{id:int}/total")]
        public async Task<ActionResult<decimal>> GetTotal(int id)
        {
            try
            {
                return Ok(await _bll.ObterValorTotalAsync(id));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        private static CobrancaPlanoItemVM MapItem(CobrancaItemApiDto x) => new()
        {
            Valor = x.Valor,
            DataVencimento = x.DataVencimento,
            MetodoPagamento = ParseMetodo(x.MetodoPagamento),
            Descricao = null
        };

        private static EnumMetodoPagamento ParseMetodo(string s)
        {
            return s.ToUpperInvariant() switch
            {
                "BOLETO" => EnumMetodoPagamento.Boleto,
                "PIX" => EnumMetodoPagamento.Pix,
                _ => throw new InvalidOperationException($"Método de pagamento inválido: {s}. Use BOLETO ou PIX.")
            };
        }
    }
}
