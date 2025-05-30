using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.TensãoVertical.Factory;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Entities.Cálculos.TensãoVertical
{
    public class CálculoTensãoVertical: Cálculo, ICálculoTensãoVertical
    {
        private readonly Geometria _geometria;
        private readonly Trajetória _trajetória;

        private readonly double _pressAtmPsi = 14.69595;
        private readonly double _convPressure = 1.422334;
        private readonly double _waterOvPressure;

        private CálculoTensãoVertical(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada entradas, IPerfisSaída perfilTensãoVertical, Trajetória trajetória, ILitologia litologia, Geometria geometria, DadosGerais dadosGerais) : base(nome, grupoCálculo, entradas, perfilTensãoVertical, trajetória, litologia)
        {
            _geometria = geometria;
            _trajetória = trajetória;
            _waterOvPressure = geometria.OffShore.LaminaDagua * dadosGerais.Area.DensidadeAguaMar;
        }

        public static void RegisterCálculoTensõesCtor()
        {
            CálculoTensãoVerticalFactory.RegisterCálculoTensãoVerticalCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, geometria, dadosGerais) => new CálculoTensãoVertical(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, geometria, dadosGerais));
        }

        public override void Execute(bool chamadaPelaPipeline)
        {
            double profundidadeInicialDeSedimentos = 0.0;
            switch (_geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicialDeSedimentos = _geometria.MesaRotativa + _geometria.OffShore.LaminaDagua;
                    break;
                case CategoriaPoço.OnShore:
                    profundidadeInicialDeSedimentos =
                        _geometria.MesaRotativa + _geometria.OnShore.AlturaDeAntePoço;// + _geometria.DadosOnShore.LençolFreático;
                    break;
            }

            CalcularTvert(profundidadeInicialDeSedimentos);
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        private void CalcularTvert(double profundidadeInicialDeSedimentos)
        {
            var rhob = PerfisEntrada.Perfis.First();
            var tvert = PerfisSaída.Perfis.First();
            tvert.Clear();
            var profundidades = rhob.GetProfundidades();// pontos.Select(ponto => ponto.Pm).ToArray();

            double[] pontosTvert = new double[profundidades.Count];
            double profundidadePmInicial = profundidades[0].Valor;

            ConversorProfundidade.TryGetTVDFromMD(profundidadePmInicial, out var pontoTrajetóriaPV);
            rhob.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidades[0].Valor), out var pontoRhobInicial);

            var tVertInicial = CalcularTensãoVerticalInicial(pontoTrajetóriaPV, profundidadeInicialDeSedimentos, pontoRhobInicial.Valor);
            pontosTvert[0] = tVertInicial;
            tvert.AddPontoEmPm(ConversorProfundidade, profundidadePmInicial, tVertInicial, TipoProfundidade.PM, OrigemPonto.Calculado);

            for (int index = 1; index < profundidades.Count; index++)
            {
                var profundidadePm = profundidades[index];
                var pvAnterior = pontoTrajetóriaPV;

                if (!ConversorProfundidade.TryGetTVDFromMD(profundidadePm.Valor, out pontoTrajetóriaPV))
                    continue;

                if (rhob.TryGetPontoEmPm(ConversorProfundidade, profundidadePm, out var pontoRhob))
                {
                    var tvertAnterior = pontosTvert[index - 1];
                    var valorTVert = CalcularTensãoVerticalPorPv(pontoTrajetóriaPV, pvAnterior, tvertAnterior, pontoRhob.Valor);
                    pontosTvert[index] = valorTVert;
                    tvert.AddPontoEmPm(ConversorProfundidade, profundidadePm, valorTVert, TipoProfundidade.PM, OrigemPonto.Calculado);
                }
            }
        }

        private double CalcularTensãoVerticalInicial(double pv, double profundidadeInicialDeSedimentos, double rhobInicial)
        {
            return _pressAtmPsi + _convPressure * _waterOvPressure + _convPressure * (pv - profundidadeInicialDeSedimentos) * rhobInicial;
        }

        private double CalcularTensãoVerticalPorPv(double pv, double pvAnterior, double tvertAnterior, double rhob)
        {
            return tvertAnterior + _convPressure * (pv - pvAnterior) * rhob;
        }
    }
}
