using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Key.Du.Pay.UserInterface.Controllers.Api
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/responsaveis")]
    public class ResponsaveisApiController : ControllerBase
    {
        private readonly IResponsavelFinanceiroBll _responsavelBll;
        private readonly IPlanoPagamentoBll _planoBll;
        private readonly ICobrancaBll _cobrancaBll;

        public ResponsaveisApiController(
            IResponsavelFinanceiroBll responsavelBll,
            IPlanoPagamentoBll planoBll,
            ICobrancaBll cobrancaBll)
        {
            _responsavelBll = responsavelBll;
            _planoBll = planoBll;
            _cobrancaBll = cobrancaBll;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponsavelFinanceiroVM), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] ResponsavelFinanceiroVM body)
        {
            var criado = await _responsavelBll.CriarAsync(body);
            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponsavelFinanceiroVM>> GetById(int id)
        {
            var r = await _responsavelBll.ObterPorIdAsync(id);
            return r == null ? NotFound() : Ok(r);
        }

        [HttpGet("{id:int}/planos-de-pagamento")]
        public async Task<ActionResult<List<PlanoPagamentoVM>>> GetPlanos(int id)
        {
            var list = await _planoBll.ListarPorResponsavelAsync(id);
            return Ok(list);
        }

        [HttpGet("{id:int}/cobrancas")]
        public async Task<ActionResult<List<CobrancaVM>>> GetCobrancas(int id)
        {
            var list = await _cobrancaBll.ListarPorResponsavelAsync(id);
            return Ok(list);
        }

        [HttpGet("{id:int}/cobrancas/quantidade")]
        public async Task<ActionResult<int>> GetCobrancasQuantidade(int id)
        {
            var q = await _cobrancaBll.QuantidadePorResponsavelAsync(id);
            return Ok(q);
        }
    }
}
