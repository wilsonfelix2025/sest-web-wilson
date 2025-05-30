using System;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Linq;
using SestWeb.Domain.Helpers;
using SestWeb.Domain.Validadores;

namespace SestWeb.Domain.MontagemPerfis
{

    public class MontadorDePerfil
    {
        protected internal Poço PoçoCorrelação;
        protected internal Poço PoçoTrabalho;
        protected internal IList<PerfilBase> Perfis;
        protected internal List<PerfilBase> PerfisDeTrabalho;
        protected internal double TopoCorrelação;
        protected internal double BaseCorrelação;
        protected internal double TopoTrabalho;
        protected internal double BaseTrabalho;
        protected internal bool RemoverTendência;

        public MontadorDePerfil()
        {

        }

        public MontadorDePerfil(Poço poçoCorrelação, Poço poçoTrabalho, IList<PerfilBase> perfis,
            List<PerfilBase> perfisDeTrabalho, double topoCorrelação,
            double baseCorrelação, double topoTrabalho, double baseTrabalho, bool removerTendência)
        {
            PoçoCorrelação = poçoCorrelação;
            PoçoTrabalho = poçoTrabalho;
            Perfis = perfis;
            PerfisDeTrabalho = perfisDeTrabalho;
            TopoCorrelação = topoCorrelação;
            BaseCorrelação = baseCorrelação;
            TopoTrabalho = topoTrabalho;
            BaseTrabalho = baseTrabalho;
            RemoverTendência = removerTendência;
        }
        /// <summary>
        /// Monta os perfis.
        /// </summary>
        /// <returns>Retorna os novos perfis montados.</returns>
        public List<PerfilBase> MontarPerfis()
        {
            var profTopoCorrelação = new Profundidade(TopoCorrelação);
            var profBaseCorrelação = new Profundidade(BaseCorrelação);

            var novosPerfis = new List<PerfilBase>();

            var validator = new MontarPerfisValidator();
            var result = validator.Validate(this);

            if (result.IsValid == false)
            {
                throw new Exception(string.Join(',', result.Errors));
            }

            for (var perfilIndex = 0; perfilIndex < Perfis.Count; perfilIndex++)
            {
                var perfilId = Perfis[perfilIndex].Id;
                var perfilCorrelacao = Perfis[perfilIndex];

                if (perfilCorrelacao == null || !perfilCorrelacao.ContémPontos())
                    continue;


                var pontosPvTrechoCorrelação = perfilCorrelacao.GetPvPontosNoTrecho(PoçoCorrelação.Trajetória,profTopoCorrelação,
                    profBaseCorrelação, PoçoCorrelação.Trajetória, true);

                var pvTrechoCorrelação = pontosPvTrechoCorrelação.ToList();

                if (!pvTrechoCorrelação.Any())
                    continue;

                var pontosCorrelação = pvTrechoCorrelação;

                var perfilTrabalho = PerfisDeTrabalho.Find(p => p.Mnemonico == perfilCorrelacao.Mnemonico);

                var profTopoTrabalho = new Profundidade(TopoTrabalho);
                var profBaseTrabalho = new Profundidade(BaseTrabalho);

                var trechoCorrelaçãoSize = BaseCorrelação - TopoCorrelação;
                var trechoTrabalhoSize = BaseTrabalho - TopoTrabalho;

                // verificar corretude das profundidades de criação desses pontos
                if (!CriarPrimeiroPontoNoTrechoDoPerfilTrabalho(PoçoTrabalho, perfilTrabalho, profTopoTrabalho, profBaseTrabalho, profTopoCorrelação, profBaseCorrelação, pontosCorrelação, out Ponto pontoTopoTrabalho))
                    continue;

                if (!CriarÚltimoPontoNoTrechoDoPerfilDeTrabalho(PoçoTrabalho, perfilTrabalho, profTopoTrabalho, profBaseTrabalho, profTopoCorrelação, profBaseCorrelação, pontosCorrelação))
                    continue;


                var residuals = new List<double>();
                var newValues = new List<double>();

                if (RemoverTendência)
                {
                    var pvs = new List<double>();
                    var valores = new List<double>();

                    for (int i = 0; i < pontosCorrelação.Count - 1; i++)
                    {
                        pvs.Add(pontosCorrelação[i].Pv.Valor);
                        valores.Add(pontosCorrelação[i].Valor);
                    }

                    FastMath.LinearRegression(valores, pvs, out double rSquared, out double yIntercept, out double slope, out residuals, out newValues);
                }

                var pontosPvTrechoTrabalho = new SortedList<Profundidade, Ponto>();
                pontosPvTrechoTrabalho.Add(pontoTopoTrabalho.Pv, pontoTopoTrabalho);

                AdicionarLimítrofesDosTH(PoçoCorrelação.Trajetória, PoçoTrabalho.Trajetória, profTopoTrabalho, profBaseTrabalho, profTopoCorrelação, profBaseCorrelação, perfilCorrelacao, perfilTrabalho);

                var pvAnterior = pontoTopoTrabalho.Pv;
                for (var pointIndex = 1; pointIndex < pontosCorrelação.Count - 1; pointIndex++)
                {
                    var pontoCorrelação = pontosCorrelação[pointIndex];
                    var pontoCorrelaçãoAnterior = pontosCorrelação[pointIndex - 1];

                    var deltaCorrelação = (decimal)pontoCorrelação.Pv.Valor - (decimal)pontoCorrelaçãoAnterior.Pv.Valor;
                    var deltaTrabalho = deltaCorrelação * (decimal)trechoTrabalhoSize / (decimal)trechoCorrelaçãoSize;

                    if (deltaTrabalho < (decimal)0.009)
                        continue;


                    var profPontoTrabalho = new Profundidade(((pontoCorrelação.Pv.Valor - TopoCorrelação) * (BaseTrabalho - TopoTrabalho)) / (BaseCorrelação - TopoCorrelação) + TopoTrabalho);

                    if (!pontosPvTrechoTrabalho.ContainsKey(profPontoTrabalho) && CriarNovoPontoNoPoçoEmPv(PoçoTrabalho,
                            profPontoTrabalho, pontoCorrelação.Valor,
                            out Ponto pontoTrabalho) && !pontosPvTrechoTrabalho.ContainsKey(pontoTrabalho.Pv))
                    {
                        pontosPvTrechoTrabalho.Add(pontoTrabalho.Pv, pontoTrabalho);

                        if (RemoverTendência)
                        {
                            perfilTrabalho.AddPontoEmPv(PoçoTrabalho.Trajetória, pontoTrabalho.Pv, newValues[pointIndex], TipoProfundidade.PV, OrigemPonto.Montado);
                        }
                        else
                        {
                            perfilTrabalho.AddPontoEmPv(PoçoTrabalho.Trajetória, pontoTrabalho.Pv, pontoTrabalho.Valor, TipoProfundidade.PV, OrigemPonto.Montado);
                        }
                        pvAnterior = pontoTrabalho.Pv;
                    }
                }

                if (novosPerfis.Count(p => p.Mnemonico == perfilTrabalho.Mnemonico) == 0)
                    novosPerfis.Add(perfilTrabalho);
            }

            return novosPerfis;
        }

        private static bool CriarÚltimoPontoNoTrechoDoPerfilDeTrabalho(Poço poçoTrabalho, PerfilBase perfilTrabalho, Profundidade profTopoTrabalho, Profundidade profBaseTrabalho, Profundidade profTopoCorrelação, Profundidade profBaseCorrelação, IList<Ponto> postosCorrelação)
        {
            Profundidade profTrabalho;

            if (profBaseCorrelação.Valor > postosCorrelação[postosCorrelação.Count - 1].Pv.Valor)
            {
                var trechoCorrelaçãoSize = profBaseCorrelação.Valor - profTopoCorrelação.Valor;
                var trechoTrabalhoSize = profBaseTrabalho.Valor - profTopoTrabalho.Valor;

                var deltaCorrelação = (decimal)postosCorrelação[postosCorrelação.Count - 1].Pv.Valor - (decimal)profTopoCorrelação.Valor;
                var deltaTrabalho = deltaCorrelação * (decimal)trechoTrabalhoSize / (decimal)trechoCorrelaçãoSize;
                var pvTrabalho = (decimal)profTopoTrabalho.Valor + deltaTrabalho;
                profTrabalho = new Profundidade((double)pvTrabalho);
            }
            else
                profTrabalho = profBaseTrabalho;

            perfilTrabalho.AddPontoEmPv(poçoTrabalho.Trajetória, profTrabalho, postosCorrelação[postosCorrelação.Count - 1].Valor, TipoProfundidade.PV, OrigemPonto.Montado);

            return true;
        }

        private static bool CriarPrimeiroPontoNoTrechoDoPerfilTrabalho(Poço poçoTrabalho, PerfilBase perfilTrabalho, Profundidade profTopoTrabalho, Profundidade profBaseTrabalho, Profundidade profTopoCorrelação, Profundidade profBaseCorrelação, IList<Ponto> postosCorrelação, out Ponto pontoTopoTrabalho)
        {
            Profundidade profTrabalho;

            if (profTopoCorrelação.Valor < postosCorrelação[0].Pv.Valor)
            {
                var trechoCorrelaçãoSize = profBaseCorrelação.Valor - profTopoCorrelação.Valor;
                var trechoTrabalhoSize = profBaseTrabalho.Valor - profTopoTrabalho.Valor;

                var deltaCorrelação = (decimal)postosCorrelação[0].Pv.Valor - (decimal)profTopoCorrelação.Valor;
                var deltaTrabalho = deltaCorrelação * (decimal)trechoTrabalhoSize / (decimal)trechoCorrelaçãoSize;
                var pvTrabalho = (decimal)profTopoTrabalho.Valor + deltaTrabalho;
                profTrabalho = new Profundidade((double)pvTrabalho);
            }
            else
                profTrabalho = profTopoTrabalho;

            perfilTrabalho.AddPontoEmPv(poçoTrabalho.Trajetória, profTrabalho, postosCorrelação[0].Valor, TipoProfundidade.PV, OrigemPonto.Montado);

            var pt = new Ponto(profTrabalho, profTrabalho, postosCorrelação[0].Valor, TipoProfundidade.PV, OrigemPonto.Montado, poçoTrabalho.Trajetória, poçoTrabalho.ObterLitologiaPadrão());

            pontoTopoTrabalho = pt;

            return true;
        }


        public static void AdicionarLimítrofesDosTH(Trajetória TrajetóriaCorrelação, Trajetória trajetóriaTrabalho, Profundidade profTopoTrabalho,
            Profundidade profBaseTrabalho, Profundidade profTopoCorrelação, Profundidade profBaseCorrelação, PerfilBase perfilCorrelacao, PerfilBase perfilTrabalho)
        {
            var trechoCorrelaçãoSize = profBaseCorrelação.Valor - profTopoCorrelação.Valor;
            var trechoTrabalhoSize = profBaseTrabalho.Valor - profTopoTrabalho.Valor;
            var pvsDeTrechosHorizontaisTrabalho =
                trajetóriaTrabalho.GetPvsDeTrechosHorizontais(profTopoTrabalho, profBaseTrabalho);

            var pontosTH = trajetóriaTrabalho.GetPvsDeTrechosHorizontaisPontos(profTopoTrabalho, profBaseTrabalho);

            for (int pvIndex = 0; pvIndex < pontosTH.Values.Count; pvIndex++)
            {
                var pv = pontosTH.Values[pvIndex];
                var pontoTrajetóriaInicioTrecho = pv.First();
                var pontoTrajetóriaFinalTrecho = pv.Last();

                var profCorrelaçãoIni = new Profundidade((((pontoTrajetóriaInicioTrecho.Pv.Valor - profTopoTrabalho.Valor) / profBaseCorrelação.Valor - profTopoCorrelação.Valor) / (profBaseTrabalho.Valor - profTopoTrabalho.Valor)) + profTopoCorrelação.Valor);
                var gotTvdPoint = perfilCorrelacao.TryGetPontoEmPv(TrajetóriaCorrelação, perfilCorrelacao, profCorrelaçãoIni, out Ponto pontoCorrelaçãoIni);

                var valorCorr = pontoCorrelaçãoIni.Valor;

                if (perfilTrabalho.TryGetPontoEmPv(trajetóriaTrabalho, perfilTrabalho, pontoTrajetóriaInicioTrecho.Pv, out Ponto pontoTrab))
                    valorCorr = pontoTrab.Valor;

                perfilTrabalho.AddPontoEmPm(trajetóriaTrabalho, pontoTrajetóriaInicioTrecho.Pm, valorCorr, TipoProfundidade.PM, OrigemPonto.Montado);
                perfilTrabalho.AddPontoEmPm(trajetóriaTrabalho, pontoTrajetóriaFinalTrecho.Pm, valorCorr, TipoProfundidade.PM, OrigemPonto.Montado);
            
            }
        }

        private bool CriarNovoPontoNoPoçoEmPv(Poço poço, Profundidade profundidade, double valor, out Ponto ponto)
        {
            if (!poço.Trajetória.TryGetMDFromTVD(profundidade.Valor, out var pm))
            {
                ponto = null;
                return false;
            }


            var pmProf = new Profundidade(pm);
            ponto = new Ponto(pmProf, pmProf, valor, TipoProfundidade.PM, OrigemPonto.Montado, poço.Trajetória, poço.ObterLitologiaPadrão());

            ponto.AtualizarPV();
            ponto.TrocarProfundidadeReferenciaParaPV();

            return true;
        }

    }
}
