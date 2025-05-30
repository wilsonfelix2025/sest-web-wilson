
using System;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Validadores;

namespace SestWeb.Domain.MontagemPerfis
{
    public class MontadorDeLitologia
    {
        protected internal Poço PoçoCorrelação;
        protected internal Poço PoçoTrabalho;
        protected internal double PvTopoCorrelação;
        protected internal double PvBaseCorrelação;
        protected internal double PvTopoTrabalho;
        protected internal double PvBaseTrabalho;

        public MontadorDeLitologia()
        {
            
        }

        public MontadorDeLitologia(Poço poçoCorrelação, Poço poçoTrabalho, double pvTopoCorrelação,
            double pvBaseCorrelação, double pvTopoTrabalho, double pvBaseTrabalho)
        {
            PoçoCorrelação = poçoCorrelação;
            PoçoTrabalho = poçoTrabalho;
            PvTopoCorrelação = pvTopoCorrelação;
            PvBaseCorrelação = pvBaseCorrelação;
            PvTopoTrabalho = pvTopoTrabalho;
            PvBaseTrabalho = pvBaseTrabalho;
        }

        public Litologia MontarLitologia(bool litologiaVazia)
        {
            var validator = new MontarLitologiaValidator();
            var result = validator.Validate(this);

            if (result.IsValid == false)
            {
                throw new Exception(string.Join(',', result.Errors));
            }

            SortedList<Profundidade, PontoLitologia> elementosLitologiaCorrelação = new SortedList<Profundidade, PontoLitologia>();

            // sempre pegará trechos devido à validação anterior
            if (litologiaVazia == false && !TryGetElementosLitologiaCorrelação(PoçoCorrelação, new Profundidade(PvTopoCorrelação),
                new Profundidade(PvBaseCorrelação),
                out elementosLitologiaCorrelação))
                return null;
            
            var trechoCorrelaçãoSize = PvBaseCorrelação - PvTopoCorrelação;
            var trechoTrabalhoSize = PvBaseTrabalho - PvTopoTrabalho;

            var profTopoTrechoTrabalho = new Profundidade(PvTopoTrabalho);
            var profBaseTrechoTrabalho = new Profundidade(PvBaseTrabalho - 0.01);
            var pvNextTopoTrechoTrabalho = PvTopoTrabalho;
            Profundidade profNextTopoTrechoTrabalho;
            var lito = PoçoTrabalho.ObterLitologiaPadrão();

            if (litologiaVazia)
            {
                var tipoRochaOutros = TipoRocha.OUT.Mnemonico;
                lito.AddPontoEmPv(PoçoTrabalho.Trajetória, profTopoTrechoTrabalho, tipoRochaOutros, TipoProfundidade.PV, OrigemPonto.Montado);
                lito.AddPontoEmPv(PoçoTrabalho.Trajetória, profBaseTrechoTrabalho, tipoRochaOutros, TipoProfundidade.PV, OrigemPonto.Montado);
                return lito;
            }

            var elementosCorrelação = elementosLitologiaCorrelação.Values;

            if (!elementosCorrelação.Any())
                return null;

            SortedList<Profundidade, PontoLitologia> elementosLitologiaTrabalho = new SortedList<Profundidade, PontoLitologia>();


            elementosLitologiaTrabalho.Add(profTopoTrechoTrabalho, new PontoLitologia(profTopoTrechoTrabalho, profTopoTrechoTrabalho, elementosCorrelação.First().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, PoçoTrabalho.Trajetória));
            if (lito.ContémPontos() && lito.UltimoPonto.Pv == profTopoTrechoTrabalho)
                lito.RemovePonto(lito.UltimoPonto);
            lito.AddPontoEmPv(PoçoTrabalho.Trajetória, profTopoTrechoTrabalho, elementosCorrelação.First().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado);
            elementosLitologiaTrabalho.Add(profBaseTrechoTrabalho, new PontoLitologia(profBaseTrechoTrabalho, profBaseTrechoTrabalho, elementosCorrelação.Last().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, PoçoTrabalho.Trajetória));
            lito.AddPontoEmPv(PoçoTrabalho.Trajetória, profBaseTrechoTrabalho, elementosCorrelação.Last().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado);

            if (elementosCorrelação.Count == 1)
            {
                return null;
            }

            for (var index = 0; index < elementosCorrelação.Count - 1; index++)
            {
                var nextElementoCorrelação = elementosCorrelação[index + 1];

                var nextTopoCorrelação = nextElementoCorrelação.Pv.Valor;

                var deltaCorrelação = (decimal)nextTopoCorrelação - (decimal)PvTopoCorrelação;
                var deltaTrabalho = deltaCorrelação * (decimal)trechoTrabalhoSize / (decimal)trechoCorrelaçãoSize;

                if (deltaTrabalho < (decimal)0.009)
                    continue;

                profNextTopoTrechoTrabalho = new Profundidade(((nextTopoCorrelação - PvTopoCorrelação) * (PvBaseTrabalho - PvTopoTrabalho)) / (PvBaseCorrelação - PvTopoCorrelação) + PvTopoTrabalho);

                if (!elementosLitologiaTrabalho.ContainsKey(profNextTopoTrechoTrabalho))
                {
                    if (profNextTopoTrechoTrabalho > profBaseTrechoTrabalho)
                        if (profNextTopoTrechoTrabalho.Valor > profBaseTrechoTrabalho.Valor + 0.1)
                            break;

                    elementosLitologiaTrabalho.Add(profNextTopoTrechoTrabalho, new PontoLitologia(profNextTopoTrechoTrabalho, profNextTopoTrechoTrabalho, nextElementoCorrelação.TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, PoçoTrabalho.Trajetória));
                    lito.AddPontoEmPv(PoçoTrabalho.Trajetória, profNextTopoTrechoTrabalho, nextElementoCorrelação.TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado);
                }
            }

            return lito;
        }

        private bool TryGetElementosLitologiaCorrelação(Poço poçoCorrelação, Profundidade profTopoCorrelação,
            Profundidade profBaseCorrelação, out SortedList<Profundidade, PontoLitologia> elementosLitologiaCorrelação)
        {
            elementosLitologiaCorrelação = new SortedList<Profundidade, PontoLitologia>();

            var litologiaCorrelação = poçoCorrelação.ObterLitologiaPadrão();

            if (!litologiaCorrelação.TryGetPontosEmPv(poçoCorrelação.Trajetória, profTopoCorrelação,
                profBaseCorrelação, out var pontosLitoCorrelação, true, true))
                return false;

            for (int index = 0; index < pontosLitoCorrelação.Count; index++)
            {
                var ponto = pontosLitoCorrelação[index];
                elementosLitologiaCorrelação.Add(ponto.Pv,ponto);
            }

            NormalizarTrechosLitologiaCorrelação(poçoCorrelação, profTopoCorrelação, profBaseCorrelação,
                ref elementosLitologiaCorrelação);

            return true;
        }

        private void NormalizarTrechosLitologiaCorrelação(Poço poçoCorrelação, Profundidade profTopoCorrelação,
            Profundidade profBaseCorrelação, ref SortedList<Profundidade, PontoLitologia> elementosLitologiaCorrelação)
        {
            TryGetTrechosLimítrofes(poçoCorrelação, profTopoCorrelação, profBaseCorrelação,
                out IList<PontoLitologia> trechoLimítrofeSuperiorCorrelação,
                out IList<PontoLitologia> trechoLimítrofeInferiorCorrelação);


            if (trechoLimítrofeInferiorCorrelação == null || trechoLimítrofeSuperiorCorrelação == null)
                return;

            var topoTrechoLimítrofeSuperiorCorrelação = trechoLimítrofeSuperiorCorrelação.First().Pv;
            var baseTrechoLimítrofeSuperiorCorrelação = trechoLimítrofeSuperiorCorrelação.Last().Pv;

            var topoTrechoLimítrofeInferiorCorrelação = trechoLimítrofeInferiorCorrelação.First().Pv;
            var baseTrechoLimítrofeInferiorCorrelação = trechoLimítrofeInferiorCorrelação.Last().Pv;

            // Ambos os pontos limítrofes caíram no mesmo trecho litológico
            if (topoTrechoLimítrofeSuperiorCorrelação.Equals(topoTrechoLimítrofeInferiorCorrelação) &&
                baseTrechoLimítrofeSuperiorCorrelação.Equals(baseTrechoLimítrofeInferiorCorrelação) &&
                !elementosLitologiaCorrelação.Any())
            {
                if (!elementosLitologiaCorrelação.ContainsKey(profTopoCorrelação))
                    elementosLitologiaCorrelação.Add(profTopoCorrelação, new PontoLitologia(profTopoCorrelação, profTopoCorrelação, trechoLimítrofeSuperiorCorrelação.First().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, poçoCorrelação.Trajetória));

                if (!elementosLitologiaCorrelação.ContainsKey(profBaseCorrelação))
                    elementosLitologiaCorrelação.Add(profBaseCorrelação, new PontoLitologia(profBaseCorrelação, profBaseCorrelação, trechoLimítrofeInferiorCorrelação.Last().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, poçoCorrelação.Trajetória));
                return;
            }

            if (topoTrechoLimítrofeSuperiorCorrelação < profTopoCorrelação)
            {

                if (!elementosLitologiaCorrelação.ContainsKey(profTopoCorrelação))
                    elementosLitologiaCorrelação.Add(profTopoCorrelação, new PontoLitologia(profTopoCorrelação, profTopoCorrelação, trechoLimítrofeSuperiorCorrelação.First().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, poçoCorrelação.Trajetória));
            }

            if (topoTrechoLimítrofeInferiorCorrelação < profBaseCorrelação)
            {
        
                if (elementosLitologiaCorrelação.ContainsKey(topoTrechoLimítrofeInferiorCorrelação))
                    elementosLitologiaCorrelação.Remove(topoTrechoLimítrofeInferiorCorrelação);

                elementosLitologiaCorrelação.Add(topoTrechoLimítrofeInferiorCorrelação, new PontoLitologia(topoTrechoLimítrofeInferiorCorrelação, topoTrechoLimítrofeInferiorCorrelação, trechoLimítrofeInferiorCorrelação.First().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, poçoCorrelação.Trajetória));

                if (!elementosLitologiaCorrelação.ContainsKey(profBaseCorrelação))
                    elementosLitologiaCorrelação.Add(profBaseCorrelação, new PontoLitologia(profBaseCorrelação, profBaseCorrelação, trechoLimítrofeInferiorCorrelação.Last().TipoRocha.Mnemonico, TipoProfundidade.PV, OrigemPonto.Montado, poçoCorrelação.Trajetória));
            }
        }

        private void TryGetTrechosLimítrofes(Poço poçoCorrelação, Profundidade profTopoCorrelação, Profundidade profBaseCorrelação, out IList<PontoLitologia> trechoLimítrofeSuperiorCorrelação, out IList<PontoLitologia> trechoLimítrofeInferiorCorrelação)
        {
            var gotLimimítrofeSuperior = poçoCorrelação.ObterLitologiaPadrão().TryGetPontosEmPv(poçoCorrelação.Trajetória, profTopoCorrelação, profBaseCorrelação,
                out trechoLimítrofeSuperiorCorrelação);

            if (!gotLimimítrofeSuperior)
            {
                trechoLimítrofeSuperiorCorrelação = null;
                trechoLimítrofeInferiorCorrelação = null;
            }

            var lito = poçoCorrelação.ObterLitologiaPadrão();

            var gotLimimítrofeInferior = lito.TryGetPontosEmPv(poçoCorrelação.Trajetória, profTopoCorrelação, profBaseCorrelação,
                out trechoLimítrofeInferiorCorrelação);
          
        }

    }
}
