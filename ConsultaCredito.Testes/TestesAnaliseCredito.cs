using NSubstitute;
using FluentAssertions;

namespace ConsultaCredito.Testes;

public class TestesAnaliseCredito
{
    private readonly IServicoConsultaCredito _mock;

    private const string CPF_INVALIDO = "123A";
    private const string CPF_ERRO_COMUNICACAO = "76217486300";
    private const string CPF_SEM_PENDENCIAS = "60487583752";
    private const string CPF_INADIMPLENTE = "82226651209";

    public TestesAnaliseCredito()
    {
        _mock = Substitute.For<IServicoConsultaCredito>();

        _mock.ConsultarPendenciasPorCPF(CPF_INVALIDO)
            .Returns((List<Pendencia>)null!);

        _mock.ConsultarPendenciasPorCPF(CPF_ERRO_COMUNICACAO)
            .Returns(s => { throw new Exception("Erro de comunica��o..."); });

        _mock.ConsultarPendenciasPorCPF(CPF_SEM_PENDENCIAS)
            .Returns(new List<Pendencia>());

        var pendencias = new List<Pendencia>()
        {
            new ()
            {
                CPF = CPF_INADIMPLENTE,
                NomePessoa = "Cliente Teste",
                NomeReclamante = "Empresas ACME",
                DescricaoPendencia = "Parcela n�o paga",
                VlPendencia = 900.50
            }
        };
        _mock.ConsultarPendenciasPorCPF(CPF_INADIMPLENTE)
            .Returns(pendencias);
    }

    private StatusConsultaCredito ObterStatusAnaliseCredito(string cpf)
    {
        AnaliseCredito analise = new(_mock);
        return analise.ConsultarSituacaoCPF(cpf);
    }

    [Fact]
    [Trait("CPF Invalido", "Simular")]
    public void TestarCPFInvalidoMoq()
    {
        StatusConsultaCredito status =
            ObterStatusAnaliseCredito(CPF_INVALIDO);
        status.Should().Be(StatusConsultaCredito.ParametroEnvioInvalido,
            "Resultado incorreto para um CPF inv�lido");
    }

    [Fact]
    [Trait("Erro Comunicacao", "Simular")]
    public void TestarErroComunicacaoMoq()
    {
        StatusConsultaCredito status =
            ObterStatusAnaliseCredito(CPF_ERRO_COMUNICACAO);
        status.Should().Be(StatusConsultaCredito.ErroComunicacao,
            "Resultado incorreto para um erro de comunica��o");
    }

    [Fact]
    [Trait("Sem Pendencias", "Simular")]
    public void TestarCPFSemPendenciasMoq()
    {
        StatusConsultaCredito status =
            ObterStatusAnaliseCredito(CPF_SEM_PENDENCIAS);
        status.Should().Be(StatusConsultaCredito.SemPendencias,
            "Resultado incorreto para um CPF sem pend�ncias");
    }

    [Fact]
    [Trait("Inadimplente", "Simular")]
    public void TestarCPFInadimplenteMoq()
    {
        StatusConsultaCredito status =
            ObterStatusAnaliseCredito(CPF_INADIMPLENTE);
        status.Should().Be(StatusConsultaCredito.Inadimplente,
            "Resultado incorreto para um CPF inadimplente");
    }
}