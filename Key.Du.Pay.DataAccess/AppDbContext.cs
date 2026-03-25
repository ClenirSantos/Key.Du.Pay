using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.DataAccess.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Key.Du.Pay.DataAccess
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }




        public DbSet<CentroCustoEntity> CentroCusto { get; set; }
        public DbSet<CobrancaEntity> Cobranca { get; set; }
        public DbSet<PagamentoEntity> Pagamento { get; set; }
        public DbSet<PlanoPagamentoEntity> PlanoPagamento { get; set; }
        public DbSet<ResponsavelFinanceiroEntity> ResponsavelFinanceiro { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResponsavelFinanceiroEntity>().HasData(
                new ResponsavelFinanceiroEntity {Id = 1, DataCadastro = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc), Adimplente =  EnumAdimplencia.Adimplente, TipoUsuario = EnumTipoUsuario.ResponsavelFinanceiro, Descricao = "Instituição de Ensino Padre Anchieta" },
                new ResponsavelFinanceiroEntity {Id = 2, DataCadastro = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc), Adimplente =  EnumAdimplencia.Adimplente, TipoUsuario = EnumTipoUsuario.ResponsavelFinanceiro, Descricao = "Escola de Educação Infantil Pinguinho de Gente" }
            );


            modelBuilder.Entity<CentroCustoEntity>().HasData(
                new CentroCustoEntity { Id = 1, Descricao = "Matricula" },
                new CentroCustoEntity { Id = 2, Descricao = "Mensalidade" },
                new CentroCustoEntity { Id = 3, Descricao = "Material" }
            );
        }


    //    public class NullableDateTimeAsUtcValueConverter() : ValueConverter<DateTime?, DateTime?>(
    //v => !v.HasValue ? v : ToUtc(v.Value), v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
    //    {
    //        private static DateTime? ToUtc(DateTime v) => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime();
    //    }

    //    public class DateTimeAsUtcValueConverter() : ValueConverter<DateTime, DateTime>(
    //        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));


    //    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    //    {
    //        ArgumentNullException.ThrowIfNull(configurationBuilder);

    //        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeAsUtcValueConverter>();
    //        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeAsUtcValueConverter>();
    //    }

    }
}
