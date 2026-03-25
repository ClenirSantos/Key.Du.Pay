using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Key.Du.Pay.UserInterface.Controllers.Api
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/centros-de-custo")]
    public class CentrosCustoApiController : ControllerBase
    {
        private readonly ICentroCustoBll _bll;

        public CentrosCustoApiController(ICentroCustoBll bll)
        {
            _bll = bll;
        }

        [HttpGet]
        public async Task<ActionResult<List<CentroCustoVM>>> Get()
        {
            return Ok(await _bll.ListarAsync());
        }

        [HttpPost]
        public async Task<ActionResult<CentroCustoVM>> Post([FromBody] CentroCustoVM body)
        {
            var criado = await _bll.CriarAsync(body);
            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CentroCustoVM>> GetById(int id)
        {
            var c = await _bll.ObterPorIdAsync(id);
            return c == null ? NotFound() : Ok(c);
        }
    }
}
