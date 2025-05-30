using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public class CálculoTensãoHorizontalMenorModeloElástico
    {
        private readonly IConversorProfundidade Trajetória;
        private readonly ILitologia Litologia;
        private readonly List<PerfilBase> Entradas;
        private readonly Geometria Geometria;
        private const double FatorConversão = 0.1704;

        public CálculoTensãoHorizontalMenorModeloElástico(IList<PerfilBase> entradas, IConversorProfundidade trajetória, ILitologia litologia, Geometria geometria)
        {
            Trajetória = trajetória;          
            Litologia = litologia;
            Entradas = entradas.ToList();
            Geometria = geometria;
        }

        public PerfilBase Calcular(double[] profundidades, string nome)
        {
            double profundidadeInicial = 0.0;
            var perfilThorMin = PerfisFactory.Create("THORmin", nome, Trajetória, Litologia);

            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
                    break;
                case CategoriaPoço.OnShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OnShore.AlturaDeAntePoço + Geometria.OnShore.LençolFreático;
                    break;
            }

            var tensãoVertical = Entradas.Single(x => x.Mnemonico == "TVERT");
            var gradPressãoPoros = Entradas.Single(x => x.Mnemonico == "GPORO" || x.Mnemonico == "GPPI");
            var poisson = Entradas.Single(x => x.Mnemonico == "POISS");

            var pontosThormin = new ConcurrentBag<Ponto>();
            //TODO Paralelizar para performance

            for (int i = 0; i < profundidades.Length; i++)
            {
                var profundidadePm = profundidades[i];
                var profundidadePv = 0.0;

                if (!Trajetória.TryGetTVDFromMD(profundidadePm, out profundidadePv))
                    return null;

                if (profundidadePv < profundidadeInicial)
                    continue;

                if (tensãoVertical.TryGetPontoEmPm(Trajetória, new Profundidade(profundidadePm), out var pontoTvert, GrupoCálculo.Tensões) 
                    && gradPressãoPoros.TryGetPontoEmPm(Trajetória, new Profundidade(profundidadePm), out var pontoGporo, GrupoCálculo.Tensões) 
                    && poisson.TryGetPontoEmPm(Trajetória, new Profundidade(profundidadePm), out var pontoPoiss, GrupoCálculo.Tensões) 
                    && Litologia.ObterTipoRochaNaProfundidade(profundidadePm, out var tipoLitologia))
                {
                    var thormin = CalcularTensãoHorizontalMenor(profundidadePv, pontoTvert.Valor, pontoGporo.Valor,
                        pontoPoiss.Valor, tipoLitologia.Grupo);


                    perfilThorMin.AddPonto(Trajetória, profundidadePm, profundidadePv, thormin, TipoProfundidade.PM, OrigemPonto.Calculado);
                }
            }

            return perfilThorMin;
        }

        private static double CalcularTensãoHorizontalMenor(double profundidadePv, double tensãoVertical, double gPoro, double poisson, GrupoLitologico grupoLitológico)
        {
            if (grupoLitológico == GrupoLitologico.Evaporitos)
                return tensãoVertical;

            var pPoro = OperaçõesDeConversão.ObterPressão(profundidadePv, gPoro);
            var thorMin = (tensãoVertical - pPoro) * (poisson / (1 - poisson)) + pPoro;
            return thorMin;
        }

    }
}
