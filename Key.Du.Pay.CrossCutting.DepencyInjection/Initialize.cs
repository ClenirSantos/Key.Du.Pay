using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.BusinessLogic.Pagamento;
using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.Security;
using Key.Du.Pay.DataAccess;
using Key.Du.Pay.DataAccess.Database.Entities;
using Key.Du.Pay.DataAccess.Database.Repositories;
using Key.Du.Pay.DataAccess.Database.Repositories.CentroCusto;
using Key.Du.Pay.DataAccess.Database.Repositories.Cobranca;
using Key.Du.Pay.DataAccess.Database.Repositories.Pagamento;
using Key.Du.Pay.DataAccess.Database.Repositories.PlanoPagamento;
using Key.Du.Pay.DataAccess.Database.Repositories.ResponsavelFinanceiro;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Key.Du.Pay.CrossCutting.DepencyInjection
{
    public static class Initialize
    {
        public static void InjectDependencies(IServiceCollection services, IConfiguration configuration)
        {



            services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


            //BUSINESS LOGIC
            services.AddScoped<IJwtManager, JwtManager>();
            services.AddScoped<ICentroCustoBll, CentroCustoBll>();
            services.AddScoped<ICobrancaBll, CobrancaBll>();
            services.AddScoped<IPagamentoBll, PagamentoBll>();
            services.AddScoped<IPlanoPagamentoBll, PlanoPagamentoBll>();
            services.AddScoped<IResponsavelFinanceiroBll, ResponsavelFinanceiroBll>();


            //REPOSITORYS
            services.AddScoped<ICentroCustoRepository, CentroCustoRepository>();
            services.AddScoped<ICobrancaRepository, CobrancaRepository>();
            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<IPlanoPagamentoRepository, PlanoPagamentoRepository>();
            services.AddScoped<IResponsavelFinanceiroRepository, ResponsavelFinanceiroRepository>();


        }
    }
}
