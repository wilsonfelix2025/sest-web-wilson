using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades
{
    public class SincronizadorProfundidades
    {
        private const double StepMínimoPermitido = 1;
        private readonly GrupoCálculo _grupoDeCálculo;
        private readonly IConversorProfundidade _conversorProfundidade;
        private readonly ILitologia _litologia;
        private readonly List<Queue<double>> _profundidadesDeEntrada = new List<Queue<double>>();
        private Queue<double> _profundidadeDeReferência = new Queue<double>();
        private List<IReadOnlyList<Profundidade>> _profundidadesEntrada;
        

        public SincronizadorProfundidades(IList<PerfilBase> perfisEntrada, IConversorProfundidade conversorProfundidade, ILitologia litologia, GrupoCálculo grupoDeCálculo = GrupoCálculo.Indefinido)
        {
            _grupoDeCálculo = grupoDeCálculo;
            _conversorProfundidade = conversorProfundidade;
            _litologia = litologia;
            PrepararProfundidades(perfisEntrada);
        }

        #region Propriedades

        public double ProfundidadeInicial { get; private set; } = double.NegativeInfinity;

        public double ProfundidadeFinal { get; private set; } = double.PositiveInfinity;

        #endregion

        #region Methods

        private void PrepararProfundidades(IList<PerfilBase> perfisEntrada)
        {
            var profundidadesEntradas = new List<IReadOnlyList<Profundidade>>();

            foreach (var perfil in perfisEntrada)
            {
                if (!perfil.ContémPontos())
                {
                    _profundidadesEntrada = new List<IReadOnlyList<Profundidade>>();
                    return;
                }

                if (perfil.Mnemonico == nameof(DIAM_BROCA))
                {
                    PrepararDiamBroca(perfil, profundidadesEntradas);
                    continue;
                }
                else if (perfil.TemTrendCompactação && _grupoDeCálculo == GrupoCálculo.PressãoPoros)
                {
                    PrepararTrendCompactação(perfil, profundidadesEntradas);
                }

                profundidadesEntradas.Add(perfil.GetProfundidades());
            }

            if (_grupoDeCálculo == GrupoCálculo.Perfis && _litologia != null && _litologia.ContémPontos())
            {
                profundidadesEntradas.Add(new List<Profundidade> { _litologia.PmTopo, _litologia.PmBase });
            }

            _profundidadesEntrada = profundidadesEntradas;
        }

        private void PrepararTrendCompactação(PerfilBase perfil, List<IReadOnlyList<Profundidade>> profundidadesEntradas)
        {
            if (_conversorProfundidade == null)
                throw new ArgumentException("Necessário trajetória.");

            var trend = perfil.Trend;
            var profundidades = trend.ObterProfundidadesPm(_conversorProfundidade).ToList();
            profundidadesEntradas.Add(profundidades);
        }

        private void PrepararDiamBroca(PerfilBase perfil, List<IReadOnlyList<Profundidade>> profundidadesEntradas)
        {
            if (_conversorProfundidade == null || !_conversorProfundidade.ContémDados())
                throw new ArgumentException("Necessário trajetória.");

            if (!perfil.TryGetPontosEmPv(_conversorProfundidade, _conversorProfundidade.PvInicial,
                _conversorProfundidade.PvFinal, out var pontosNoTrecho, true))
                return;

            var profundidades = pontosNoTrecho.Select(p => p.Pm).ToList();
            profundidadesEntradas.Add(profundidades);
        }

        public double[] GetProfundidadeDeReferência()
        {
            try
            {
                if (_profundidadeDeReferência.Count == 0)
                    CalcularProfundidadeDeReferência();

                return _profundidadeDeReferência.ToArray();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Falha ao obter profundidade de referência do cálculo", exception);
            }
        }

        private void CalcularProfundidadeDeReferência()
        {
            ObterLimitesOperacionaisDasProfundidades();

            if (UsarStepFixo())
            {
                var step = CalcularStepFixo();
                CalcularProfundidadeDeReferênciaComStepFixo(step);
            }
            else
            {
                InicializarFilasDeProfundidadesDeEntrada();
                CalcularProfundidadeDeReferênciaComStepVariável();
            }
        }

        private void CalcularProfundidadeDeReferênciaComStepFixo(double step)
        {
            if (step < StepMínimoPermitido) step = StepMínimoPermitido;

            var profundidade = ProfundidadeInicial;

            while (profundidade <= ProfundidadeFinal)
            {
                _profundidadeDeReferência.Enqueue(profundidade);
                profundidade += step;
            }
        }

        private void CalcularProfundidadeDeReferênciaComStepVariável()
        {
            if (_profundidadesDeEntrada.Count == 0) return;

            var últimaProfundidadeEnfileirada = 0.0;
            var profundidadeDeReferência = new Queue<double>();

            while (ExisteProfundidadeNãoAvaliadaEmTodasEntradas())
            {
                var profundidade = ObterMenorPrimeiroElementoDasFilasDeEntrada();
                var stepDentroDosLimites = StepDentroDoLimitePermitido(últimaProfundidadeEnfileirada, profundidade);

                if (últimaProfundidadeEnfileirada == 0.0 || stepDentroDosLimites)
                {
                    profundidadeDeReferência.Enqueue(profundidade);
                    últimaProfundidadeEnfileirada = profundidade;
                }

                RemoverProfundidadeAvaliadaDasEntradas(profundidade);
            }

            _profundidadeDeReferência = profundidadeDeReferência;
        }

        private void RemoverProfundidadeAvaliadaDasEntradas(double profundidade)
        {
            foreach (var profundidadeDeEntrada in _profundidadesDeEntrada)
            {
                var primeiro = profundidadeDeEntrada.Peek();

                if (Math.Abs(primeiro - profundidade) < 0.001) profundidadeDeEntrada.Dequeue();
            }
        }

        private bool StepDentroDoLimitePermitido(double profundidadeAnterior, double profundidadeAtual)
        {
            return (decimal)profundidadeAtual - (decimal)profundidadeAnterior >= (decimal)StepMínimoPermitido;
        }

        private double ObterMenorPrimeiroElementoDasFilasDeEntrada()
        {
            var primeirosElementos = new List<double>();
            foreach (var profundidadeDeEntrada in _profundidadesDeEntrada)
            {
                var fila = profundidadeDeEntrada;
                if (fila.Count == 0)
                    throw new InvalidOperationException(
                        "Tentativa de obter elementos de uma fila vazia durante o cálculo da profundidade de referência.");
                var primeiroElemento = fila.Peek();
                primeirosElementos.Add(primeiroElemento);
            }

            return primeirosElementos.Min();
        }

        private bool ExisteProfundidadeNãoAvaliadaEmTodasEntradas()
        {
            return _profundidadesDeEntrada.All(pe => pe.Count != 0);
        }

        private bool UsarStepFixo()
        {
            return _grupoDeCálculo == GrupoCálculo.Indefinido;
        }

        private void InicializarFilasDeProfundidadesDeEntrada()
        {
            if (!PodeCalcular()) return;

            for (var perfilIndex = 0; perfilIndex < _profundidadesEntrada.Count; perfilIndex++)
            {
                var profundidades = _profundidadesEntrada[perfilIndex];

                if (profundidades == null || profundidades.Count == 0) continue;

                var valorDasProfundidades = new SortedList<Profundidade, double>();
                var lastProfIndex = profundidades.Count - 1; //último index

                for (var profIndex = lastProfIndex; profIndex >= 0; profIndex--)
                {
                    var prof = profundidades[profIndex];

                    if (prof.Valor < ProfundidadeInicial) break;
                    valorDasProfundidades.Add(prof, prof.Valor);
                }

                var listaDeProfundidades = valorDasProfundidades.Values.ToList();
                var profundidadesDoPerfilDeEntrada = new Queue<double>(listaDeProfundidades);
                _profundidadesDeEntrada.Add(profundidadesDoPerfilDeEntrada);
            }
        }

        private bool PodeCalcular()
        {
            if (ProfundidadeInicial <= ProfundidadeFinal) return true;
            return false;
        }

        private void ObterLimitesOperacionaisDasProfundidades()
        {
            ObterLimitesOperacionaisEmPm();
        }

        private void ObterLimitesOperacionaisEmPm()
        {
            for (var perfilIndex = 0; perfilIndex < _profundidadesEntrada.Count; perfilIndex++)
            {
                var profundidades = _profundidadesEntrada[perfilIndex];

                var qtdPontos = profundidades.Count;
                if (qtdPontos == 0)
                {
                    ProfundidadeInicial = 0;
                    ProfundidadeFinal = 0;
                    return;
                }

                var profundidadeDoPrimeiroPonto = profundidades[0];
                var profundidadeDoÚltimoPonto = profundidades[qtdPontos - 1];

                if (profundidadeDoPrimeiroPonto.Valor > ProfundidadeInicial)
                    ProfundidadeInicial = profundidadeDoPrimeiroPonto.Valor;
                if (profundidadeDoÚltimoPonto.Valor < ProfundidadeFinal) 
                    ProfundidadeFinal = profundidadeDoÚltimoPonto.Valor;
            }
        }

        private double CalcularStepFixo()
        {
            if (!PodeCalcular())
                throw new InvalidOperationException(
                    $"Profundidade de início do cálculo ({ProfundidadeInicial}) deve ser menor ou igual à profundidade final do cálculo ({ProfundidadeFinal})");

            var step = double.MaxValue;

            for (var perfilIndex = 0; perfilIndex < _profundidadesEntrada.Count; perfilIndex++)
            {
                var profundidades = _profundidadesEntrada[perfilIndex];
                var qtdPontos = profundidades.Count;

                if (qtdPontos < 2) continue;
                for (var index = 1; index < qtdPontos; index++)
                {
                    var stepCorr = profundidades[index].Valor - profundidades[index - 1].Valor;

                    if (stepCorr < step) step = stepCorr;
                }
            }

            if (step == double.MaxValue) throw new InvalidOperationException("Erro ao calcular o step fixo");

            return step;
        }

        #endregion
    }
}
