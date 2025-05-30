using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Composto
{
    public class PerfilVetorial
    {
        private const double ValorDeInicialização = 0.0;

        private PerfilBase _perfil;
        private double[] _profundidadeDeReferência;
        private readonly GrupoCálculo _grupoDeCálculo;
        private readonly IConversorProfundidade _conversorProfundidade;

        public PerfilVetorial(IConversorProfundidade conversorProfundidade, PerfilBase perfil, double[] profundidadeDeReferência, GrupoCálculo grupoDeCálculo)

        {
            _perfil = perfil;
            _profundidadeDeReferência = profundidadeDeReferência;
            _grupoDeCálculo = grupoDeCálculo;
            _conversorProfundidade = conversorProfundidade;
            CarregarValoresDosPontos(profundidadeDeReferência);
        }

        #region Propriedades

        public PerfilBase Perfil
        {
            get => _perfil;
            private set => _perfil = value;
        }

        public double[] ValoresDosPontos { get; private set; }

        #endregion

        public void CarregarValoresDosPontos(double[] profundidadeDeReferência)
        {
            _profundidadeDeReferência = profundidadeDeReferência;

            if (PerfilSemPontos()) //perfil de saída
            {
                InicializarValoresDosPontos();
                return;
            }

            // Assume-se que perfil já esteja sincronizado, portanto com pontos, na mesma quantidade da profundidade de referência, inclusive o perfil de saída.
            if (!PerfilEstáSincronizado())
                throw new ArgumentException($"Perfil {_perfil.Nome} não sincronizado.");

            CarregarPontos();
        }

        private bool PerfilEstáSincronizado()
        {
            return _perfil != null && _profundidadeDeReferência != null;
        }

        private void InicializarValoresDosPontos()
        {
            var qtdPontos = _profundidadeDeReferência.Length;
            ValoresDosPontos = new double[qtdPontos];
            Parallel.For(0, qtdPontos, index => { ValoresDosPontos[index] = ValorDeInicialização; });
        }

        private bool PerfilSemPontos()
        {
            return _perfil == null || _perfil.Count == 0;
        }

        private void CarregarPontos()
        {
            var qtdPontos = _profundidadeDeReferência.Length;
            ValoresDosPontos = new double[qtdPontos];

            for (int index = 0; index < qtdPontos; index++)
            {
                var valor = _profundidadeDeReferência[index];
                var pontoObtido =
                    _perfil.TryGetPontoEmPm(_conversorProfundidade,new Profundidade(valor), out Ponto ponto, _grupoDeCálculo);
                if (pontoObtido)
                    ValoresDosPontos[index] = ponto.Valor;
                else
                    ValoresDosPontos[index] = 0;
            }
        }

        public PerfilBase CriarPerfilComPontosCalculados(bool recalculo, bool ignorarValoresNegativos)
        {
            _perfil.Clear();

            for (int index = 0; index < ValoresDosPontos.Length; index++)
            {
                if (ignorarValoresNegativos && ValoresDosPontos[index] < 0) continue; 

                _perfil.AddPontoEmPm(_conversorProfundidade, _profundidadeDeReferência[index], ValoresDosPontos[index], TipoProfundidade.PM,
                    OrigemPonto.Calculado);
            }

            return _perfil;
        }

        public void Reset()
        {
            _perfil.Clear();
            ApagarValoresDosPontos();
        }

        public void ApagarValoresDosPontos()
        {
            ValoresDosPontos = null;
        }
    }
}
