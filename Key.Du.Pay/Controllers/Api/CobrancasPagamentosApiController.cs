using Key.Du.Pay.BusinessLogic.Pagamento;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Key.Du.Pay.UserInterface.Controllers.Api
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/cobrancas")]
    public class CobrancasPagamentosApiController : ControllerBase
    {
        private readonly IPagamentoBll _pagamentoBll;

        public CobrancasPagamentosApiController(IPagamentoBll pagamentoBll)
        {
            _pagamentoBll = pagamentoBll;
        }

        [HttpPost("{id:int}/pagamentos")]
        public async Task<ActionResult<PagamentoVM>> Post(int id, [FromBody] PagamentoApiRequestDto dto)
        {
            try
            {
                var valor = dto.PagamentoIntegral
                    ? await _pagamentoBll.ObterSaldoDevedorAsync(id)
                    : dto.Valor;
                var vm = new PagamentoVM
                {
                    CobrancaID = id,
                    Valor = valor,
                    DataPagamento = dto.DataPagamento
                };
                var criado = await _pagamentoBll.RegistrarAsync(vm);
                return StatusCode(StatusCodes.Status201Created, criado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
