using Key.Du.Pay.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Key.Du.Pay.UserInterface.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidaUsuarioController : ControllerBase
    {
        private readonly IJwtManager _jwtManager;
        public ValidaUsuarioController(IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        [HttpGet]
        [Route("Administrador")]
        [Authorize(Roles = "Administrador")]
        public string Administrador()
        {
            return User?.Identity?.Name ?? "Não Autorizado";
        }

        [HttpGet]
        [Route("Analista")]
        [Authorize(Roles = "Analista")]
        public string Analista()
        {
            return User?.Identity?.Name ?? "Não Autorizado";
        }
    }
}
