using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.Security;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Key.Du.Pay.UserInterface.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IJwtManager _jwtManager;   
        public LoginController(IJwtManager jwtManager) { 
            _jwtManager = jwtManager;
        }

        [HttpGet]
        [Route("Administrador")]
        public string Administrador()
        {
            ResponsavelFinanceiroVM user = new ResponsavelFinanceiroVM
            {
                Descricao = "Clenir",
                TipoUsuario =  EnumTipoUsuario.Administrador
            };

            return _jwtManager.GenerateToken(user);

        }


        [HttpGet]
        [Route("ResponsavelFinanceiro")]
        public string ResponsavelFinanceiro()
        {
            ResponsavelFinanceiroVM user = new ResponsavelFinanceiroVM
            {
                Descricao = "Clenir",
                TipoUsuario = EnumTipoUsuario.ResponsavelFinanceiro
            };

            return _jwtManager.GenerateToken(user);

        }
    }
}
