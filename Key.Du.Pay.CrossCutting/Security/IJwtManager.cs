using Key.Du.Pay.CrossCutting.ViewModel;

namespace Key.Du.Pay.CrossCutting.Security
{
    public interface IJwtManager
    {
        string GenerateToken(ResponsavelFinanceiroVM user);
        //UsuarioVM GetPrincipal(string token);
        //string GetTokenFromHeader();
        //bool IsValid();
        //bool IsValid(string token);
    }
}
