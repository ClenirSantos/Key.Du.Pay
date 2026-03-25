# Key.Du.Pay

Aplicação .NET para gestão de planos de pagamento, cobranças e pagamentos (PostgreSQL + EF Core).

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 14+ (ou compatível)

## Configuração do banco

1. Crie um banco vazio no PostgreSQL.
2. Ajuste a connection string em `Key.Du.Pay/appsettings.json` (ou `appsettings.Development.json`):

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=keydupay;Username=postgres;Password=sua_senha"
}
```

3. Aplicar migrations (na pasta da solução):

```bash
dotnet ef database update --project Key.Du.Pay.DataAccess --startup-project Key.Du.Pay
```

## Executar o projeto

```bash
dotnet run --project Key.Du.Pay
```

- Interface web (Razor): `https://localhost:<porta>/`
- Swagger: `https://localhost:<porta>/swagger`

## API REST (exemplos com curl)

Substitua `BASE` pela URL base (ex.: `https://localhost:7xxx`) e os IDs pelos existentes no seu banco.

### Criar responsável financeiro

```bash
curl -X POST "%BASE%/api/responsaveis" ^
  -H "Content-Type: application/json" ^
  -d "{\"descricao\":\"Escola Exemplo\",\"dataCadastro\":\"2026-03-24T00:00:00Z\",\"adimplente\":1,\"tipoUsuario\":1}"
```

### Listar centros de custo

```bash
curl "%BASE%/api/centros-de-custo"
```

### Criar plano de pagamento (payload conforme desafio)

```bash
curl -X POST "%BASE%/api/planos-de-pagamento" ^
  -H "Content-Type: application/json" ^
  -d "{\"responsavelId\":1,\"centroDeCusto\":1,\"cobrancas\":[{\"valor\":500.00,\"dataVencimento\":\"2025-03-10T00:00:00Z\",\"metodoPagamento\":\"BOLETO\"},{\"valor\":500.00,\"dataVencimento\":\"2025-04-10T00:00:00Z\",\"metodoPagamento\":\"PIX\"}]}"
```

### Detalhes e total do plano

```bash
curl "%BASE%/api/planos-de-pagamento/1"
curl "%BASE%/api/planos-de-pagamento/1/total"
```

### Planos e cobranças por responsável

```bash
curl "%BASE%/api/responsaveis/1/planos-de-pagamento"
curl "%BASE%/api/responsaveis/1/cobrancas"
curl "%BASE%/api/responsaveis/1/cobrancas/quantidade"
```

### Registrar pagamento de uma cobrança

```bash
curl -X POST "%BASE%/api/cobrancas/1/pagamentos" ^
  -H "Content-Type: application/json" ^
  -d "{\"valor\":500.00,\"dataPagamento\":\"2026-03-24T12:00:00Z\"}"
```

Quitação integral pelo saldo (equivalente ao valor total em aberto): use `"pagamentoIntegral": true` (o campo `valor` é ignorado).

Regras de negócio: valor igual ao saldo devedor → cobrança **Paga**; valor menor → **Paga parcialmente** e o responsável financeiro associado ao plano passa a **risco de inadimplência**; pagamentos subsequentes que quitarem o saldo restauram o responsável para **adimplente** se estiver em risco.

Os endpoints da API estão marcados com `[AllowAnonymous]` para facilitar testes; em produção, restrinja com autenticação (JWT já configurado no projeto).

## Estrutura

- `Key.Du.Pay` — Razor Pages, Swagger, controllers REST
- `Key.Du.Pay.BusinessLogic` — regras de negócio (BLL)
- `Key.Du.Pay.DataAccess` — EF Core, repositórios
- `Key.Du.Pay.CrossCutting` — view models e enums
- `Key.Du.Pay.CrossCutting.DepencyInjection` — registro de DI
