using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Key.Du.Pay.CrossCutting.Security
{
    public class JwtManager : IJwtManager
    {
        private readonly IConfiguration _configuration;
        //private readonly ILogger _logger;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtManager(IConfiguration configuration)
        {
            _configuration = configuration;
           // _logger = logger;
          //  _httpContextAccessor = httpContextAccessor;
        }
        public string GenerateToken(ResponsavelFinanceiroVM user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtToken").Value ?? throw new Exception("JWT TOKEN INVALIDO - CONFIGURE UM TOKEN VÁLIDO EM APPSETTINGS"));


            var tokenDescription = new SecurityTokenDescriptor
            {

                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Descricao ?? "Desconhecido"),
                    new Claim(ClaimTypes.Role, user.TipoUsuario.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);

        }


        //public UsuarioVM GetPrincipal(string token)
        //{
        //    try
        //    {
        //        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        //        IJsonSerializer serializer = new JsonNetSerializer();
        //        IDateTimeProvider provider = new UtcDateTimeProvider();
        //        IJwtValidator validator = new JwtValidator(serializer, provider);
        //        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        //        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

        //        var obj = decoder.DecodeToObject<UsuarioVM>(token, _configuration.GetSection("JwtToken").Value, verify: false);
        //        return obj;
        //    }
        //    catch (TokenExpiredException ex)
        //    {
        //        return null;
        //    }
        //}


        //public string GetTokenFromHeader()
        //{
        //    try
        //    {
        //        if (_httpContextAccessor.HttpContext.Request != null)
        //        {
        //            var req = _httpContextAccessor.HttpContext.Request;
        //            string bearerToken = req.Headers["Authorization"];

        //            if (bearerToken != null)
        //            {
        //                var token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
        //                return token;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        //public bool IsValid()
        //{
        //    return IsValid(GetTokenFromHeader());
        //}

        //public bool IsValid(string token)
        //{
        //    if (string.IsNullOrEmpty(token))
        //        return false;

        //    var clain = GetPrincipal(token);
        //    if (clain == null)
        //        return false;

        //    return true;
        //}
    }
}
