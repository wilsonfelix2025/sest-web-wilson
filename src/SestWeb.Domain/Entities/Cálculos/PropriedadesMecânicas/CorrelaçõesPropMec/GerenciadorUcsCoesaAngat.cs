using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.EditingRelacionamentoPropMec;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.CorrelaçõesFixas;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public static class GerenciadorUcsCoesaAngat
    {
        private static List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> _relacionamentosUcsCoesaAngat;

        private static IRelacionamentoPropMecFactory _relacionamentoPropMecFactory;

        static GerenciadorUcsCoesaAngat()
        {
            _relacionamentosUcsCoesaAngat = new List<RelacionamentoUcsCoesaAngatPorGrupoLitológico>();
            _relacionamentoPropMecFactory = new RelacionamentoPropMecFactory(new RelacionamentoPropMecEmCriaçãoValidator(), new RelacionamentoPropMecEmEdiçãoValidator(), new RelacionamentoPropMecValidator(new AutorValidator()));
        }

        public static void AdicionarRelacionamento(GrupoLitologico grupoLitologico, string origem, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr)
        {
            if (JáCadastrado(grupoLitologico, corrUcs, corrCoesa, corrAngat, corrRestr))
                return;

            ChecarCorrelaçõesEstãoCadastradasNaOrdemCerta(corrUcs, corrCoesa, corrAngat, corrRestr);
            _relacionamentoPropMecFactory.CreateRelacionamentoPropMec(grupoLitologico.Nome, origem, nomeAutor, chaveAutor,
                corrUcs, corrCoesa, corrAngat, corrRestr, out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamento);
            _relacionamentosUcsCoesaAngat.Add(relacionamento);
        }

        public static void AdicionarRelacionamento(RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoUcsCoesaAngatPorGrupoLitológico)
        {
            if (JáCadastrado(relacionamentoUcsCoesaAngatPorGrupoLitológico.GrupoLitológico, relacionamentoUcsCoesaAngatPorGrupoLitológico.Ucs, relacionamentoUcsCoesaAngatPorGrupoLitológico.Coesa,
                relacionamentoUcsCoesaAngatPorGrupoLitológico.Angat, relacionamentoUcsCoesaAngatPorGrupoLitológico.Restr))
                return;

            _relacionamentosUcsCoesaAngat.Add(relacionamentoUcsCoesaAngatPorGrupoLitológico);
        }

        public static void RemoverRelacionamento(RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoUcsCoesaAngatPorGrupoLitológico)
        {
            if (!JáCadastrado(relacionamentoUcsCoesaAngatPorGrupoLitológico.GrupoLitológico, relacionamentoUcsCoesaAngatPorGrupoLitológico.Ucs, relacionamentoUcsCoesaAngatPorGrupoLitológico.Coesa,
                relacionamentoUcsCoesaAngatPorGrupoLitológico.Angat, relacionamentoUcsCoesaAngatPorGrupoLitológico.Restr))
                return;

            _relacionamentosUcsCoesaAngat.Remove(relacionamentoUcsCoesaAngatPorGrupoLitológico);
        }

        public static void RemoverRelacionamento(GrupoLitologico grupoLitologico, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr)
        {
            ChecarCorrelaçõesEstãoCadastradasNaOrdemCerta(corrUcs, corrCoesa, corrAngat, corrRestr);

            for (var index = 0; index < _relacionamentosUcsCoesaAngat.Count; index++)
            {
                var rel = _relacionamentosUcsCoesaAngat[index];
                if (rel.Ucs.Nome.Equals(corrUcs.Nome) ||
                    rel.Coesa.Nome.Equals(corrCoesa.Nome) ||
                    rel.Angat.Nome.Equals(corrAngat.Nome) ||
                    rel.Ucs.Nome.Equals(corrRestr.Nome))
                {
                    _relacionamentosUcsCoesaAngat.RemoveAt(index);
                    return;
                }
            }
        }

        public static bool IsCorrsLoaded()
        {
            return _relacionamentosUcsCoesaAngat.Count > 0;
        }

        public static List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> GetRelacionamentos()
        {
            return _relacionamentosUcsCoesaAngat;
        }

        public static List<Correlação> ObterCorrelaçõesPossíveisPorGrupoLitologicoETipo(GrupoLitologico grupoLitologico, string mnemônico)
        {
            List<Correlação> correlações = new List<Correlação>();
            for (int index = 0; index < _relacionamentosUcsCoesaAngat.Count; index++)
            {
                var relacionamento = _relacionamentosUcsCoesaAngat[index];
                if (relacionamento.GrupoLitológico != grupoLitologico) 
                    continue;

                switch (mnemônico)
                {
                    case "UCS":
                        if (correlações.Find(corr => corr.Nome == relacionamento.Ucs.Nome) == null)
                            correlações.Add(relacionamento.Ucs);
                        break;
                    case "ANGAT":
                        if (correlações.Find(corr => corr.Nome == relacionamento.Angat.Nome) == null)
                            correlações.Add(relacionamento.Ucs);
                        break;
                    case "COESA":
                        if (correlações.Find(corr => corr.Nome == relacionamento.Coesa.Nome) == null)
                            correlações.Add(relacionamento.Ucs);
                        break;
                    case "RESTR":
                        if (correlações.Find(corr => corr.Nome == relacionamento.Restr.Nome) == null)
                            correlações.Add(relacionamento.Ucs);
                        break;
                }
            }
            return correlações;
        }

        public static List<GrupoLitologico> ObterGruposLitológicosPorCorrelação(string nomeCorrelação)
        {
            List<GrupoLitologico> gruposLitológicos = new List<GrupoLitologico>();

            for (int index = 0; index < _relacionamentosUcsCoesaAngat.Count; index++)
            {
                var relacionamento = _relacionamentosUcsCoesaAngat[index];
                if (relacionamento.Ucs.Nome.Equals(nomeCorrelação) ||
                    relacionamento.Coesa.Nome.Equals(nomeCorrelação) ||
                    relacionamento.Angat.Nome.Equals(nomeCorrelação) ||
                    relacionamento.Ucs.Nome.Equals(nomeCorrelação))
                {
                    if (!gruposLitológicos.Contains(relacionamento.GrupoLitológico))
                    {
                        gruposLitológicos.Add(relacionamento.GrupoLitológico);
                    }
                }
            }
            return gruposLitológicos;
        }

        public static List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> ObterRelacionamentosDaCorrelação(string nomeCorrelação)
        {
            List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> relacionamentos = new List<RelacionamentoUcsCoesaAngatPorGrupoLitológico>();

            for (int index = 0; index < _relacionamentosUcsCoesaAngat.Count; index++)
            {
                var relacionamento = _relacionamentosUcsCoesaAngat[index];
                if (relacionamento.Ucs.Nome.Equals(nomeCorrelação) ||
                    relacionamento.Coesa.Nome.Equals(nomeCorrelação) ||
                    relacionamento.Angat.Nome.Equals(nomeCorrelação) ||
                    relacionamento.Ucs.Nome.Equals(nomeCorrelação))
                {
                    if (!relacionamentos.Contains(relacionamento))
                    {
                        relacionamentos.Add(relacionamento);
                    }
                }
            }
            return relacionamentos;
        }

        public static void RemoverRelacionamentosDaCorrelação(string nomeCorrelação)
        {
            for (var index = 0; index < _relacionamentosUcsCoesaAngat.Count; index++)
            {
                var rel = _relacionamentosUcsCoesaAngat[index];
                if (rel.Ucs.Nome.Equals(nomeCorrelação) ||
                    rel.Coesa.Nome.Equals(nomeCorrelação) ||
                    rel.Angat.Nome.Equals(nomeCorrelação) ||
                    rel.Ucs.Nome.Equals(nomeCorrelação))
                {
                    if (_relacionamentosUcsCoesaAngat.Contains(rel))
                    {
                        _relacionamentosUcsCoesaAngat.Remove(rel);
                    }
                }
            }
        }

        public static void Reset(List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> relacionamentos)
        {
            _relacionamentosUcsCoesaAngat.Clear();

            foreach (var relacionamento in relacionamentos)
            {
                AdicionarRelacionamento(relacionamento);
            }
        }

        public static AllPropMecPossibleCorrs GetAllPropMecPossibleCorrs(Poço.Poço poço)
        {
            AllPropMecPossibleCorrs allPropMecPossibleCorrs = new AllPropMecPossibleCorrs();

            var litoGroups = poço.ObterLitologiaPadrão().GetGrupoLitologicos();

            for (int index = 0; index < litoGroups.Count; index++)
            {
                if (litoGroups[index].Nome.Equals("Evaporitos"))
                    continue;

                PropMecPossibleCorrsPerLitoGroup corrsPerLitoGroup = GetPropMecPossibleCorrsPerLitoGroup(litoGroups[index]);
                allPropMecPossibleCorrs.AddPropMecPossibleCorrs(corrsPerLitoGroup);
            }

            return allPropMecPossibleCorrs;
        }


        private static PropMecPossibleCorrsPerLitoGroup GetPropMecPossibleCorrsPerLitoGroup(GrupoLitologico grupoLitologico)
        {
            List<Correlação> correlaçõesUcsPossíveis = new List<Correlação>();
            List<Correlação> correlaçõesCoesaPossíveis = new List<Correlação>();
            List<Correlação> correlaçõesAngatPossíveis = new List<Correlação>();
            List<Correlação> correlaçõesrestrPossíveis = new List<Correlação>();

            foreach (var rel in _relacionamentosUcsCoesaAngat)
            {
                if (rel.GrupoLitológico != grupoLitologico) 
                    continue;

                if (correlaçõesUcsPossíveis.Find(c=>c.Nome.Equals(rel.Ucs.Nome)) == null)
                    correlaçõesUcsPossíveis.Add(rel.Ucs);

                if (correlaçõesCoesaPossíveis.Find(c => c.Nome.Equals(rel.Coesa.Nome)) == null)
                    correlaçõesCoesaPossíveis.Add(rel.Coesa);

                if (correlaçõesAngatPossíveis.Find(c => c.Nome.Equals(rel.Angat.Nome)) == null)
                    correlaçõesAngatPossíveis.Add(rel.Angat);

                if (correlaçõesrestrPossíveis.Find(c => c.Nome.Equals(rel.Restr.Nome)) == null)
                    correlaçõesrestrPossíveis.Add(rel.Restr);
            }

            return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico, correlaçõesUcsPossíveis, correlaçõesCoesaPossíveis, correlaçõesAngatPossíveis, correlaçõesrestrPossíveis);
        }

        public static PropMecPossibleCorrsPerLitoGroup ObterCorrelaçõesPossíveisPorGrupoLitologico(GrupoLitologico grupoLitologico, Correlação correlaçãoUcsSelecionada,
            Correlação correlaçãoCoesaSelecionada, Correlação correlaçãoAngatSelecionada, Correlação correlaçãoRestrSelecionada)
        {
            var todasCorrelaçõesNãoSelecionadas = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada == null &&
                                               correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada == null;

            var todasCorrelaçõesSelecionadas = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada != null &&
                                                  correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada != null;

            if (todasCorrelaçõesSelecionadas || todasCorrelaçõesNãoSelecionadas)
            {
                var correlaçõesUcsPossíveis = _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico).GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
                var correlaçõesCoesaPossíveis = _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico).GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
                var correlaçõesAngatPossíveis = _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico).GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
                var correlaçõesRestrPossíveis = _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico).GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico, correlaçõesUcsPossíveis, correlaçõesCoesaPossíveis, correlaçõesAngatPossíveis, correlaçõesRestrPossíveis);
            }

            var somenteUcsSelecionada = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada == null && correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada == null;
            var somenteCoesaSelecionada = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada != null && correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada == null;
            var somenteAngatSelecionada = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada == null && correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada == null;
            var somenteRestrSelecionada = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada == null && correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada != null;

            var ucsCoesaSelecionadas = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada != null && correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada == null;
            var ucsAngatSelecionadas = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada == null && correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada == null;
            var ucsRestrSelecionadas = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada == null && correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada != null;
            var coesaAngatSelecionadas = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada != null && correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada == null;
            var coesaRestrSelecionadas = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada != null && correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada != null;
            var angatRestrSelecionadas = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada == null && correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada != null;

            var ucsCoesaAngatSelecionadas = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada != null && correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada == null;
            var ucsCoesaRestrSelecionadas = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada != null && correlaçãoAngatSelecionada == null && correlaçãoRestrSelecionada != null;
            var ucsAngatRestrSelecionadas = correlaçãoUcsSelecionada != null && correlaçãoCoesaSelecionada == null && correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada != null;
            var coesaAngatRestrSelecionadas = correlaçãoUcsSelecionada == null && correlaçãoCoesaSelecionada != null && correlaçãoAngatSelecionada != null && correlaçãoRestrSelecionada != null;

            // Somente uma correlação selecionada

            if (somenteUcsSelecionada)
            {
                var correlaçõesCoesaPossíveis = ObterCorrelaçõesCoesaPossíveis(grupoLitologico, correlaçãoUcsSelecionada);
                var correlaçõesAngatPossíveis = ObterCorrelaçõesAngatPossíveis(grupoLitologico, correlaçãoUcsSelecionada);
                var correlaçõesRestrPossíveis = ObterCorrelaçõesRestrPossíveis(grupoLitologico, correlaçãoUcsSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico, new List<Correlação> { correlaçãoUcsSelecionada }, correlaçõesCoesaPossíveis, correlaçõesAngatPossíveis, correlaçõesRestrPossíveis);
            }
            if (somenteCoesaSelecionada)
            {
                var correlaçõesUcsPossíveis = ObterCorrelaçõesUcsPossíveis(grupoLitologico, correlaçãoCoesaSelecionada);
                var correlaçõesAngatPossíveis = ObterCorrelaçõesAngatPossíveis(grupoLitologico, correlaçãoCoesaSelecionada);
                var correlaçõesRestrPossíveis = ObterCorrelaçõesRestrPossíveis(grupoLitologico, correlaçãoCoesaSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico, correlaçõesUcsPossíveis, new List<Correlação> { correlaçãoCoesaSelecionada }, correlaçõesAngatPossíveis, correlaçõesRestrPossíveis);
            }
            if (somenteAngatSelecionada)
            {
                var correlaçõesUcsPossíveis = ObterCorrelaçõesUcsPossíveis(grupoLitologico, correlaçãoAngatSelecionada);
                var correlaçõesCoesaPossíveis = ObterCorrelaçõesCoesaPossíveis(grupoLitologico, correlaçãoAngatSelecionada);
                var correlaçõesRestrPossíveis = ObterCorrelaçõesRestrPossíveis(grupoLitologico, correlaçãoAngatSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico, correlaçõesUcsPossíveis, correlaçõesCoesaPossíveis, new List<Correlação> { correlaçãoAngatSelecionada }, correlaçõesRestrPossíveis);
            }
            if (somenteRestrSelecionada)
            {
                var correlaçõesUcsPossíveis = ObterCorrelaçõesUcsPossíveis(grupoLitologico, correlaçãoRestrSelecionada);
                var correlaçõesCoesaPossíveis = ObterCorrelaçõesCoesaPossíveis(grupoLitologico, correlaçãoRestrSelecionada);
                var correlaçõesAngatPossíveis = ObterCorrelaçõesAngatPossíveis(grupoLitologico, correlaçãoRestrSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico, correlaçõesUcsPossíveis, correlaçõesCoesaPossíveis, correlaçõesAngatPossíveis, new List<Correlação> { correlaçãoRestrSelecionada });
            }

            // Duas correlações selecionadas

            if (ucsCoesaSelecionadas)
            {
                var correlaçõesAngatPossíveis = ObterCorrelaçõesAngatPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoCoesaSelecionada);
                var correlaçõesRestrPossíveis = ObterCorrelaçõesRestrPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoCoesaSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    new List<Correlação> { correlaçãoUcsSelecionada },
                    new List<Correlação> { correlaçãoCoesaSelecionada },
                    correlaçõesAngatPossíveis,
                    correlaçõesRestrPossíveis);
            }
            if (ucsAngatSelecionadas)
            {
                var correlaçõesCoesaPossíveis = ObterCorrelaçõesCoesaPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoAngatSelecionada);
                var correlaçõesRestrPossíveis = ObterCorrelaçõesRestrPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoAngatSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    new List<Correlação> { correlaçãoUcsSelecionada },
                    correlaçõesCoesaPossíveis,
                    new List<Correlação> { correlaçãoAngatSelecionada },
                    correlaçõesRestrPossíveis);
            }
            if (coesaAngatSelecionadas)
            {
                var correlaçõesUcsPossíveis = ObterCorrelaçõesUcsPossíveis(grupoLitologico, correlaçãoCoesaSelecionada, correlaçãoAngatSelecionada);
                var correlaçõesRestrPossíveis = ObterCorrelaçõesRestrPossíveis(grupoLitologico, correlaçãoCoesaSelecionada, correlaçãoAngatSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    correlaçõesUcsPossíveis,
                    new List<Correlação> { correlaçãoCoesaSelecionada },
                    new List<Correlação> { correlaçãoAngatSelecionada },
                    correlaçõesRestrPossíveis);
            }
            if (ucsRestrSelecionadas)
            {
                var correlaçõesCoesaPossíveis = ObterCorrelaçõesCoesaPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoRestrSelecionada);
                var correlaçõesAngatPossíveis = ObterCorrelaçõesAngatPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoRestrSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    new List<Correlação> { correlaçãoUcsSelecionada },
                    correlaçõesCoesaPossíveis,
                    correlaçõesAngatPossíveis,
                    new List<Correlação> { correlaçãoRestrSelecionada });
            }
            if (coesaRestrSelecionadas)
            {
                var correlaçõesUcsPossíveis = ObterCorrelaçõesUcsPossíveis(grupoLitologico, correlaçãoCoesaSelecionada, correlaçãoRestrSelecionada);
                var correlaçõesAngatPossíveis = ObterCorrelaçõesAngatPossíveis(grupoLitologico, correlaçãoCoesaSelecionada, correlaçãoRestrSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    correlaçõesUcsPossíveis,
                    new List<Correlação> { correlaçãoCoesaSelecionada },
                    correlaçõesAngatPossíveis,
                    new List<Correlação> { correlaçãoRestrSelecionada });
            }
            if (angatRestrSelecionadas)
            {
                var correlaçõesUcsPossíveis = ObterCorrelaçõesUcsPossíveis(grupoLitologico, correlaçãoAngatSelecionada, correlaçãoRestrSelecionada);
                var correlaçõesCoesaPossíveis = ObterCorrelaçõesCoesaPossíveis(grupoLitologico, correlaçãoAngatSelecionada, correlaçãoRestrSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    correlaçõesUcsPossíveis,
                    correlaçõesCoesaPossíveis,
                    new List<Correlação> { correlaçãoAngatSelecionada },
                    new List<Correlação> { correlaçãoRestrSelecionada });
            }

            // Três correlações selecionadas

            if (ucsCoesaAngatSelecionadas)
            {
                var correlaçõesRestrPossíveis = ObterCorrelaçõesRestrPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoCoesaSelecionada, correlaçãoAngatSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    new List<Correlação> { correlaçãoUcsSelecionada },
                    new List<Correlação> { correlaçãoCoesaSelecionada },
                    new List<Correlação> { correlaçãoAngatSelecionada },
                    correlaçõesRestrPossíveis);
            }
            if (ucsCoesaRestrSelecionadas)
            {
                var correlaçõesAngatPossíveis = ObterCorrelaçõesAngatPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoCoesaSelecionada, correlaçãoRestrSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    new List<Correlação> { correlaçãoUcsSelecionada },
                    new List<Correlação> { correlaçãoCoesaSelecionada },
                    correlaçõesAngatPossíveis,
                    new List<Correlação> { correlaçãoRestrSelecionada });
            }
            if (ucsAngatRestrSelecionadas)
            {
                var correlaçõesCoesaPossíveis = ObterCorrelaçõesCoesaPossíveis(grupoLitologico, correlaçãoUcsSelecionada, correlaçãoAngatSelecionada, correlaçãoRestrSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    new List<Correlação> { correlaçãoUcsSelecionada },
                    correlaçõesCoesaPossíveis,
                    new List<Correlação> { correlaçãoAngatSelecionada },
                    new List<Correlação> { correlaçãoRestrSelecionada });
            }
            if (coesaAngatRestrSelecionadas)
            {
                var correlaçõesUcsPossíveis = ObterCorrelaçõesUcsPossíveis(grupoLitologico, correlaçãoCoesaSelecionada, correlaçãoAngatSelecionada, correlaçãoRestrSelecionada);
                return new PropMecPossibleCorrsPerLitoGroup(grupoLitologico,
                    correlaçõesUcsPossíveis,
                    new List<Correlação> { correlaçãoCoesaSelecionada },
                    new List<Correlação> { correlaçãoAngatSelecionada },
                    new List<Correlação> { correlaçãoRestrSelecionada });
            }
            return null;
        }

        private static bool JáCadastrado(GrupoLitologico grupoLitologico, Correlação corrUcs, Correlação corrCoesa,
            Correlação corrAngat, Correlação corrRestr)
        {
            var relname = grupoLitologico.Nome + "_" + corrUcs.Nome + "_" + corrCoesa.Nome + "_" + corrAngat.Nome + "_" +
                          corrRestr.Nome;
            return _relacionamentosUcsCoesaAngat.Find(r => r.Nome.Equals(relname)) != null;
        }

        private static void ChecarCorrelaçõesEstãoCadastradasNaOrdemCerta(Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr)
        {
            if (corrUcs == null || corrCoesa == null || corrAngat == null || corrRestr == null)
                throw new ArgumentException($"Uma das correlações é null!");

            if (!corrUcs.PerfisSaída.Tipos.Contains("UCS"))
            {
                throw new ArgumentException($"Correlação {corrUcs.Nome} deve ter como saída {"UCS"}");
            }

            if (!corrCoesa.PerfisSaída.Tipos.Contains("COESA"))
            {
                throw new ArgumentException($"Correlação {corrCoesa.Nome} deve ter como saída {"COESA"}");
            }

            if (!corrAngat.PerfisSaída.Tipos.Contains("ANGAT"))
            {
                throw new ArgumentException($"Correlação {corrAngat.Nome} deve ter como saída {"ANGAT"}");
            }

            if (!corrRestr.PerfisSaída.Tipos.Contains("RESTR"))
            {
                throw new ArgumentException($"Correlação {corrRestr.Nome} deve ter como saída {"RESTR"}");
            }
        }

        #region Dependente do Grupo Litológico

        #region Uma Correlação Selecionada

        private static List<Correlação> ObterCorrelaçõesAngatPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoSelecionada)
        {
            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "ANGAT":
                    return new List<Correlação> { correlaçãoSelecionada };
                case "UCS":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Ucs.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
                case "COESA":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Coesa.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
                case "RESTR":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Restr.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
                default: return null;
            }
        }

        private static List<Correlação> ObterCorrelaçõesCoesaPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoSelecionada)
        {
            if (correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_REIS.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_REIS1.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_HORSUD.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_REIS2.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_CHANG_ET_AL.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.ANGAT_22_1.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.ANGAT_50.ToString())
            {
                var COESA_MOHR_COULOMB = _relacionamentosUcsCoesaAngat
                    .Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Coesa.Nome == MnemônicoDeCorrelaçãoFixa.COESA_CALCULADO.ToString())
                    .Select(r => r.Coesa).FirstOrDefault();
                return new List<Correlação> { COESA_MOHR_COULOMB };
            }

            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "COESA":
                    return new List<Correlação> { correlaçãoSelecionada };
                case "UCS":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Ucs.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
                case "ANGAT":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Angat.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
                case "RESTR":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Restr.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
                default: return null;
            }
        }

        private static List<Correlação> ObterCorrelaçõesUcsPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoSelecionada)
        {
            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "UCS":
                    return new List<Correlação> { correlaçãoSelecionada };
                case "ANGAT":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Angat.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
                case "COESA":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Coesa.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
                case "RESTR":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Restr.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
                default: return null;
            }
        }

        private static List<Correlação> ObterCorrelaçõesRestrPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoSelecionada)
        {
            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "ANGAT":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Angat.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
                case "UCS":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Ucs.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
                case "COESA":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.GrupoLitológico == grupoLitologico && rel.Coesa.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
                case "RESTR":
                    return new List<Correlação> { correlaçãoSelecionada };
                default: return null;
            }
        }

        #endregion

        #region Duas correlações Selecionadas

        private static List<Correlação> ObterCorrelaçõesUcsPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser UCS");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser UCS");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Coesa e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
            }

            // selecionadas Coesa e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
            }

            // selecionadas Angat e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoAngatSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
                {
                    correlaçãoAngatSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoAngatSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Angat.Nome == correlaçãoAngatSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Erro ao Obter Correlações Ucs Possíveis com duas correlações selecionadas.");
        }

        private static List<Correlação> ObterCorrelaçõesCoesaPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser Coesão");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser Coesão");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Ucs e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
            }

            // selecionadas Ucs e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
            }

            // selecionadas Angat e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoAngatSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
                {
                    correlaçãoAngatSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoAngatSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Angat.Nome == correlaçãoAngatSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Erro ao Obter Correlações Coesa Possíveis com duas correlações selecionadas.");
        }

        private static List<Correlação> ObterCorrelaçõesAngatPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser Angat");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser Angat");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Ucs e Coesa
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoCoesaSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoCoesaSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoCoesaSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome).
                    GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
            }

            // selecionadas Ucs e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
            }

            // selecionadas Coesa e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Erro ao Obter Correlações Angat Possíveis com duas correlações selecionadas.");
        }

        private static List<Correlação> ObterCorrelaçõesRestrPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser Restr");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser Restr");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Ucs e Coesa
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoCoesaSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoCoesaSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoCoesaSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome).
                    GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
            }

            // selecionadas Ucs e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
            }

            // selecionadas Coesa e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.GrupoLitológico == grupoLitologico && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Erro ao Obter Correlações Restr Possíveis com duas correlações selecionadas.");
        }

        #endregion

        #region Três correlações selecionadas

        private static List<Correlação> ObterCorrelaçõesUcsPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoCoesa, Correlação correlaçãoAngat, Correlação correlaçãoRestr)
        {
            if (correlaçãoCoesa == null || correlaçãoAngat == null || correlaçãoRestr == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoCoesa.PerfisSaída.Tipos[0].Equals("COESA"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoCoesa.PerfisSaída.Tipos[0]} deve ser Coesa");
            if (!correlaçãoAngat.PerfisSaída.Tipos[0].Equals("ANGAT"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoAngat.PerfisSaída.Tipos[0]} deve ser Angat");
            if (!correlaçãoRestr.PerfisSaída.Tipos[0].Equals("RESTR"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoRestr.PerfisSaída.Tipos[0]} deve ser Restr");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.GrupoLitológico == grupoLitologico && r.Coesa.Nome == correlaçãoCoesa.Nome &&
                            r.Angat.Nome == correlaçãoAngat.Nome && r.Restr.Nome == correlaçãoRestr.Nome)
                .GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
        }

        private static List<Correlação> ObterCorrelaçõesCoesaPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoUcs, Correlação correlaçãoAngat, Correlação correlaçãoRestr)
        {
            if (correlaçãoUcs == null || correlaçãoAngat == null || correlaçãoRestr == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoUcs.PerfisSaída.Tipos[0].Equals("UCS"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoUcs.PerfisSaída.Tipos[0]} deve ser Ucs");
            if (!correlaçãoAngat.PerfisSaída.Tipos[0].Equals("ANGAT"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoAngat.PerfisSaída.Tipos[0]} deve ser Angat");
            if (!correlaçãoRestr.PerfisSaída.Tipos[0].Equals("RESTR"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoRestr.PerfisSaída.Tipos[0]} deve ser Restr");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcs.Nome &&
                            r.Angat.Nome == correlaçãoAngat.Nome && r.Restr.Nome == correlaçãoRestr.Nome)
                .GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
        }

        private static List<Correlação> ObterCorrelaçõesAngatPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoUcs, Correlação correlaçãoCoesa, Correlação correlaçãoRestr)
        {
            if (correlaçãoUcs == null || correlaçãoCoesa == null || correlaçãoRestr == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoUcs.PerfisSaída.Tipos[0].Equals("UCS"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoUcs.PerfisSaída.Tipos[0]} deve ser Ucs");
            if (!correlaçãoCoesa.PerfisSaída.Tipos[0].Equals("COESA"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoCoesa.PerfisSaída.Tipos[0]} deve ser Coesa");
            if (!correlaçãoRestr.PerfisSaída.Tipos[0].Equals("RESTR"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoRestr.PerfisSaída.Tipos[0]} deve ser Restr");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcs.Nome &&
                            r.Coesa.Nome == correlaçãoCoesa.Nome && r.Restr.Nome == correlaçãoRestr.Nome)
                .GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
        }

        private static List<Correlação> ObterCorrelaçõesRestrPossíveis(GrupoLitologico grupoLitologico, Correlação correlaçãoUcs, Correlação correlaçãoCoesa, Correlação correlaçãoAngat)
        {
            if (correlaçãoUcs == null || correlaçãoCoesa == null || correlaçãoAngat == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoUcs.PerfisSaída.Tipos[0].Equals("UCS"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoUcs.PerfisSaída.Tipos[0]} deve ser Ucs");
            if (!correlaçãoCoesa.PerfisSaída.Tipos[0].Equals("COESA"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoCoesa.PerfisSaída.Tipos[0]} deve ser Coesa");
            if (!correlaçãoAngat.PerfisSaída.Tipos[0].Equals("ANGAT"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoAngat.PerfisSaída.Tipos[0]} deve ser Angat");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.GrupoLitológico == grupoLitologico && r.Ucs.Nome == correlaçãoUcs.Nome &&
                            r.Coesa.Nome == correlaçãoCoesa.Nome && r.Angat.Nome == correlaçãoAngat.Nome)
                .GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
        }

        #endregion

        #endregion

        #region Independente do Grupo Litológico

        #region Uma Correlação Selecionada

        private static List<Correlação> ObterCorrelaçõesAngatPossíveis(Correlação correlaçãoSelecionada)
        {
            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "ANGAT":
                    return new List<Correlação> { correlaçãoSelecionada };
                case "UCS":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Ucs.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
                case "COESA":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Coesa.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
                case "RESTR":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Restr.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
                default: return null;
            }
        }

        private static List<Correlação> ObterCorrelaçõesCoesaPossíveis(Correlação correlaçãoSelecionada)
        {
            if (correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_REIS.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_REIS1.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_HORSUD.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_REIS2.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.UCS_CHANG_ET_AL.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.ANGAT_22_1.ToString() ||
                correlaçãoSelecionada.Nome == MnemônicoDeCorrelaçãoFixa.ANGAT_50.ToString())
            {
                var COESA_MOHR_COULOMB = _relacionamentosUcsCoesaAngat
                    .Where(rel => rel.Coesa.Nome == MnemônicoDeCorrelaçãoFixa.COESA_CALCULADO.ToString())
                    .Select(r => r.Coesa).FirstOrDefault();
                return new List<Correlação> { COESA_MOHR_COULOMB };
            }

            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "COESA":
                    return new List<Correlação> { correlaçãoSelecionada };
                case "UCS":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Ucs.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
                case "ANGAT":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Angat.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
                case "RESTR":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Restr.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
                default: return null;
            }
        }

        private static List<Correlação> ObterCorrelaçõesUcsPossíveis(Correlação correlaçãoSelecionada)
        {
            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "UCS":
                    return new List<Correlação> { correlaçãoSelecionada };
                case "ANGAT":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Angat.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
                case "COESA":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Coesa.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
                case "RESTR":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Restr.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
                default: return null;
            }
        }

        private static List<Correlação> ObterCorrelaçõesRestrPossíveis(Correlação correlaçãoSelecionada)
        {
            switch (correlaçãoSelecionada.PerfisSaída.Tipos[0])
            {
                case "ANGAT":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Angat.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
                case "UCS":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Ucs.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
                case "COESA":
                    return _relacionamentosUcsCoesaAngat.Where(rel => rel.Coesa.Nome == correlaçãoSelecionada.Nome).GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
                case "RESTR":
                    return new List<Correlação> { correlaçãoSelecionada };
                default: return null;
            }
        }

        #endregion

        #region Duas correlações Selecionadas

        private static List<Correlação> ObterCorrelaçõesUcsPossíveis(Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser UCS");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser UCS");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Coesa e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
            }

            // selecionadas Coesa e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
            }

            // selecionadas Angat e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoAngatSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
                {
                    correlaçãoAngatSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoAngatSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Angat.Nome == correlaçãoAngatSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Erro ao Obter Correlações Ucs Possíveis com duas correlações selecionadas.");
        }

        private static List<Correlação> ObterCorrelaçõesCoesaPossíveis(Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser Coesão");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser Coesão");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Ucs e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
            }

            // selecionadas Ucs e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
            }

            // selecionadas Angat e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoAngatSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
                {
                    correlaçãoAngatSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoAngatSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Angat.Nome == correlaçãoAngatSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Erro ao Obter Correlações Coesa Possíveis com duas correlações selecionadas.");
        }

        private static List<Correlação> ObterCorrelaçõesAngatPossíveis(Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser Angat");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser Angat");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Ucs e Coesa
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoCoesaSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoCoesaSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoCoesaSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome).
                    GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
            }

            // selecionadas Ucs e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
            }

            // selecionadas Coesa e Restr
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoRestrSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoRestrSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoRestrSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Restr.Nome == correlaçãoRestrSelecionada.Nome).
                    GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Erro ao Obter Correlações Angat Possíveis com duas correlações selecionadas.");
        }

        private static List<Correlação> ObterCorrelaçõesRestrPossíveis(Correlação correlaçãoA, Correlação correlaçãoB)
        {
            if (correlaçãoA == null || correlaçãoB == null)
            {
                throw new ArgumentException("GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação não foi passada corretamente.");
            }
            if (correlaçãoA.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não pode ser Restr");
            }
            if (correlaçãoB.PerfisSaída.Tipos[0].Equals("RESTR"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não pode ser Restr");
            }
            if (!correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoA.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }
            if (!correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA") && !correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT"))
            {
                throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Correlação selecionada {correlaçãoB.PerfisSaída.Tipos[0]} não é de propriedades mecânicas");
            }

            // selecionadas Ucs e Coesa
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoCoesaSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoCoesaSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoCoesaSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome).
                    GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
            }

            // selecionadas Ucs e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("UCS")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoUcsSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("UCS"))
                {
                    correlaçãoUcsSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoUcsSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Ucs.Nome == correlaçãoUcsSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
            }

            // selecionadas Coesa e Angat
            if ((correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("COESA")) &&
                (correlaçãoA.PerfisSaída.Tipos[0].Equals("ANGAT") ||
                 correlaçãoB.PerfisSaída.Tipos[0].Equals("ANGAT")))
            {
                Correlação correlaçãoCoesaSelecionada = null;
                Correlação correlaçãoAngatSelecionada = null;

                if (correlaçãoA.PerfisSaída.Tipos[0].Equals("COESA"))
                {
                    correlaçãoCoesaSelecionada = correlaçãoA;
                    correlaçãoAngatSelecionada = correlaçãoB;
                }
                else
                {
                    correlaçãoCoesaSelecionada = correlaçãoB;
                    correlaçãoAngatSelecionada = correlaçãoA;
                }

                return _relacionamentosUcsCoesaAngat.
                    Where(r => r.Coesa.Nome == correlaçãoCoesaSelecionada.Nome && r.Angat.Nome == correlaçãoAngatSelecionada.Nome).
                    GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
            }
            throw new ArgumentException($"GerenciadorUcsCoesaAngat/ObterCorrelaçõesRestrPossíveis: Erro ao Obter Correlações Restr Possíveis com duas correlações selecionadas.");
        }

        #endregion

        #region Três correlações selecionadas

        private static List<Correlação> ObterCorrelaçõesUcsPossíveis(Correlação correlaçãoCoesa, Correlação correlaçãoAngat, Correlação correlaçãoRestr)
        {
            if (correlaçãoCoesa == null || correlaçãoAngat == null || correlaçãoRestr == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoCoesa.PerfisSaída.Tipos[0].Equals("COESA"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoCoesa.PerfisSaída.Tipos[0]} deve ser Coesa");
            if (!correlaçãoAngat.PerfisSaída.Tipos[0].Equals("ANGAT"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoAngat.PerfisSaída.Tipos[0]} deve ser Angat");
            if (!correlaçãoRestr.PerfisSaída.Tipos[0].Equals("RESTR"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesUcsPossíveis: Correlação selecionada {correlaçãoRestr.PerfisSaída.Tipos[0]} deve ser Restr");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.Coesa.Nome == correlaçãoCoesa.Nome &&
                            r.Angat.Nome == correlaçãoAngat.Nome && r.Restr.Nome == correlaçãoRestr.Nome)
                .GroupBy(r => r.Ucs.Nome).Select(g => g.First().Ucs).ToList();
        }

        private static List<Correlação> ObterCorrelaçõesCoesaPossíveis(Correlação correlaçãoUcs, Correlação correlaçãoAngat, Correlação correlaçãoRestr)
        {
            if (correlaçãoUcs == null || correlaçãoAngat == null || correlaçãoRestr == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoUcs.PerfisSaída.Tipos[0].Equals("UCS"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoUcs.PerfisSaída.Tipos[0]} deve ser Ucs");
            if (!correlaçãoAngat.PerfisSaída.Tipos[0].Equals("ANGAT"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoAngat.PerfisSaída.Tipos[0]} deve ser Angat");
            if (!correlaçãoRestr.PerfisSaída.Tipos[0].Equals("RESTR"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesCoesaPossíveis: Correlação selecionada {correlaçãoRestr.PerfisSaída.Tipos[0]} deve ser Restr");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.Ucs.Nome == correlaçãoUcs.Nome &&
                            r.Angat.Nome == correlaçãoAngat.Nome && r.Restr.Nome == correlaçãoRestr.Nome)
                .GroupBy(r => r.Coesa.Nome).Select(g => g.First().Coesa).ToList();
        }

        private static List<Correlação> ObterCorrelaçõesAngatPossíveis(Correlação correlaçãoUcs, Correlação correlaçãoCoesa, Correlação correlaçãoRestr)
        {
            if (correlaçãoUcs == null || correlaçãoCoesa == null || correlaçãoRestr == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoUcs.PerfisSaída.Tipos[0].Equals("UCS"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoUcs.PerfisSaída.Tipos[0]} deve ser Ucs");
            if (!correlaçãoCoesa.PerfisSaída.Tipos[0].Equals("COESA"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoCoesa.PerfisSaída.Tipos[0]} deve ser Coesa");
            if (!correlaçãoRestr.PerfisSaída.Tipos[0].Equals("RESTR"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoRestr.PerfisSaída.Tipos[0]} deve ser Restr");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.Ucs.Nome == correlaçãoUcs.Nome &&
                            r.Coesa.Nome == correlaçãoCoesa.Nome && r.Restr.Nome == correlaçãoRestr.Nome)
                .GroupBy(r => r.Angat.Nome).Select(g => g.First().Angat).ToList();
        }

        private static List<Correlação> ObterCorrelaçõesRestrPossíveis(Correlação correlaçãoUcs, Correlação correlaçãoCoesa, Correlação correlaçãoAngat)
        {
            if (correlaçãoUcs == null || correlaçãoCoesa == null || correlaçãoAngat == null)
                throw new ArgumentException(
                    "GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação não foi passada corretamente.");
            if (!correlaçãoUcs.PerfisSaída.Tipos[0].Equals("UCS"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoUcs.PerfisSaída.Tipos[0]} deve ser Ucs");
            if (!correlaçãoCoesa.PerfisSaída.Tipos[0].Equals("COESA"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoCoesa.PerfisSaída.Tipos[0]} deve ser Coesa");
            if (!correlaçãoAngat.PerfisSaída.Tipos[0].Equals("ANGAT"))
                throw new ArgumentException(
                    $"GerenciadorUcsCoesaAngat/ObterCorrelaçõesAngatPossíveis: Correlação selecionada {correlaçãoAngat.PerfisSaída.Tipos[0]} deve ser Angat");

            return _relacionamentosUcsCoesaAngat
                .Where(r => r.Ucs.Nome == correlaçãoUcs.Nome &&
                            r.Coesa.Nome == correlaçãoCoesa.Nome && r.Angat.Nome == correlaçãoAngat.Nome)
                .GroupBy(r => r.Restr.Nome).Select(g => g.First().Restr).ToList();
        }

        #endregion

        #endregion
    }
}
