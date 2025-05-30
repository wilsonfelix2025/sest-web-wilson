using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public class CálculoTensãoHorizontalMenorK0 : CálculoTensãoHorizontalBase
    {
        public CálculoTensãoHorizontalMenorK0(IList<PerfilBase> entradas, IConversorProfundidade trajetória, Geometria geometria, ILitologia litologia)
            : base(entradas, trajetória, geometria, litologia)
        {
        }

        public Tuple<double, double>[] Calcular()
        {
            double profundidadeInicial;
            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
                    break;
                case CategoriaPoço.OnShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OnShore.AlturaDeAntePoço + Geometria.OnShore.LençolFreático;
                    break;
                default: throw new InvalidOperationException("Categoria de poço não reconhecida!");
            }
            return CalcularTensãoMenor(profundidadeInicial);
        }

        private Tuple<double, double>[] CalcularTensãoMenor(double profundidadeInicial)
        {
            var profundidades = ObterProfundidades();

            var tensãoVertical = Entradas.Single(x => x.Mnemonico == "TVERT");
            var gradPressãoPoros = Entradas.Single(x => x.Mnemonico == "GPORO" || x.Mnemonico == "GPPI");
            var k0 = Entradas.Single(x => x.Mnemonico =="K0");
            var resultado = new ConcurrentBag<Tuple<double, double>>();

            //TODO Parallel para performance
            for (int i = 0; i < profundidades.Length; i++)
            {
                var profundidadePm = profundidades[i];
                var profundidadePv = 0.0;

                if (!Trajetória.TryGetTVDFromMD(profundidadePm, out profundidadePv))
                    return null;

                if (profundidadePv < profundidadeInicial)
                    continue;

                if (tensãoVertical.TryGetPontoEmPm(Trajetória, new Profundidade(profundidadePm), out var pontoTvert, GrupoCálculo.Tensões) &&
                    gradPressãoPoros.TryGetPontoEmPm(Trajetória, new Profundidade(profundidadePm), out var pontoGporo, GrupoCálculo.Tensões) &&
                    k0.TryGetPontoEmPm(Trajetória, new Profundidade(profundidadePm), out var pontoK0, GrupoCálculo.Tensões) &&
                    Litologia.ObterTipoRochaNaProfundidade(profundidadePm, out var tipoLitologia))
                {
                    var grupoLitológico = tipoLitologia.Grupo;
                    var thormin = ObterTensãoK0(profundidadePv, pontoTvert.Valor, pontoGporo.Valor, pontoK0.Valor, grupoLitológico);
                    resultado.Add(new Tuple<double, double>(profundidadePm, thormin));
                }
            }
            return resultado.ToArray();
        }

        private double ObterTensãoK0(double profundidadePv, double tvert, double gporo, double k0, GrupoLitologico grupoLitológico)
        {
            if (grupoLitológico == GrupoLitologico.Evaporitos)
                return tvert;
            var pp = OperaçõesDeConversão.ObterPressão(profundidadePv, gporo);
            return k0 * (tvert - pp) + pp;
        }

    }
}
