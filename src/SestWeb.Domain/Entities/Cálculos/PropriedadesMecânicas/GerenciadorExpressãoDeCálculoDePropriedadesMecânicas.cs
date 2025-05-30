using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Composto.Correlação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas
{
    public class GerenciadorExpressãoDeCálculoDePropriedadesMecânicas : IGerenciadorExpressãoDeCálculo
    {
        private readonly IList<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> _default;
        private readonly IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> _trechos;
        private readonly IList<ICorrelação> _correlaçõesBase;
        private readonly string _separador = ",";
        

        public GerenciadorExpressãoDeCálculoDePropriedadesMecânicas(IList<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> correlaçõesDefaultdefault,
            IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos,
            IList<ICorrelação> correlaçõesBase)
        {
            _default = correlaçõesDefaultdefault;
            _trechos = trechos;
            _correlaçõesBase = correlaçõesBase;
        }

        public string Extrair()
        {
            string expressão = string.Empty;

            if (_correlaçõesBase != null)
                expressão = ObterExpressãoDasCorrelaçõesBase(_correlaçõesBase.ToList());

            var gruposLitológicos = ObterGruposLitológicos();
            if (gruposLitológicos == null || !gruposLitológicos.Any()) return expressão;

            if (expressão.Length > 0)
                expressão += _separador + Environment.NewLine;

            foreach (GrupoLitologico grupoLitológico in gruposLitológicos)
            {
                var corrDefault = ObterDefaultDoGrupoLitológico(grupoLitológico);
                var trechosDoGrupo = ObterTrechosDoGrupoLitológico(grupoLitológico);
                var expressãoComTrechos = ObterExpressãoComTrechos(grupoLitológico, corrDefault.Ucs,
                    corrDefault.Coesa, corrDefault.Angat, corrDefault.Restr, trechosDoGrupo);
                expressão += expressãoComTrechos;

                if (grupoLitológico != gruposLitológicos.Last())
                {
                    var trimedExpression = expressão.TrimEnd();
                    var last = trimedExpression.Substring(trimedExpression.Length - 1, 1);

                    if (last != _separador)
                        expressão += _separador + Environment.NewLine;
                }
            }

            var trimedExpr = expressão.TrimEnd();
            var lastChar = trimedExpr.Substring(trimedExpr.Length - 1, 1);
            if (lastChar == _separador)
            {
                expressão = trimedExpr.Remove(trimedExpr.Length - 1);
            }
            return expressão;
        }

        private string ObterExpressãoDasCorrelaçõesBase(List<ICorrelação> correlações)
        {
            OrdenadorDeCorrelações ordenadorDeCorrelações = new OrdenadorDeCorrelações(correlações, _separador);
            var correlaçõesOrdenadas = ordenadorDeCorrelações.OrdenarCorrelações();
            MontadorDeExpressões montadorDeExpressões = new MontadorDeExpressões(correlaçõesOrdenadas, _separador);
            return montadorDeExpressões.MontarExpressão();
        }

        private CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas ObterDefaultDoGrupoLitológico(GrupoLitologico grupoLitológico)
        {
            return _default.First(r => r.GrupoLitológico == grupoLitológico);
        }

        private IOrderedEnumerable<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> ObterTrechosDoGrupoLitológico(GrupoLitologico grupoLitológico)
        {
            return _trechos?.Where(t => t.GrupoLitológico == grupoLitológico).OrderBy(t => t.Topo);
        }

        private List<GrupoLitologico> ObterGruposLitológicos()
        {
            return _default.Select(r => r.GrupoLitológico).ToList();
        }

        private string ObterExpressãoComTrechos(GrupoLitologico grupoLitológico, Correlação ucsBase, Correlação coesaBase, Correlação angatBase,
            Correlação restrBase, IEnumerable<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechosDoGrupoLitológico)
        {
            string expressão = string.Empty;

            expressão += MontaExpressãoDoGrupoLitológico(grupoLitológico, ucsBase, coesaBase, angatBase, restrBase);

            if (trechosDoGrupoLitológico == null || !trechosDoGrupoLitológico.Any()) return expressão;

            expressão += _separador;
            expressão += ObterExpressõesDosTrechos(trechosDoGrupoLitológico);
            return expressão;
        }

        private string ObterExpressõesDosTrechos(IEnumerable<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechosDoGrupoLitológico)
        {
            bool sol01 = false;
            string expressão = string.Empty;
            var trechos = trechosDoGrupoLitológico.OrderBy(t => t.Topo);

            foreach (var trecho in trechos)
            {
                // pula uma linha
                expressão += Environment.NewLine;
                expressão += sol01 ? ObterExpressãoTrechoSolução01(trecho) : ObterExpressãoTrechoSolução02(trecho);

                if (trecho != trechosDoGrupoLitológico.Last())
                {
                    expressão += _separador;
                }
            }
            return expressão;
        }

        private string ObterExpressãoTrechoSolução01(TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico trecho)
        {
            const string FAZ_NADA = "0";
            var expressão = string.Empty;
            string profundidade = Tokens.TokenDeCálculoPorTrecho;
            var identificadorGrupo = Tokens.TokenDeGrupoLitológico;
            var grupoLitológico = trecho.GrupoLitológico;

            var expressãoUcs =
                $@"({identificadorGrupo} == {grupoLitológico} && {profundidade} >= {trecho.Topo} && {profundidade} < {trecho.Base}) ? {trecho.Ucs.Expressão} : {FAZ_NADA}";

            var expressãoCoesa =
                $@"({identificadorGrupo} == {grupoLitológico} && {profundidade} >= {trecho.Topo} && {profundidade} < {trecho.Base}) ? {trecho.Coesa.Expressão} : {FAZ_NADA}";

            var expressãoAngat =
                $@"({identificadorGrupo} == {grupoLitológico} && {profundidade} >= {trecho.Topo} && {profundidade} < {trecho.Base}) ? {trecho.Angat.Expressão} : {FAZ_NADA}";

            var expressãoRestr =
                $@"({identificadorGrupo} == {grupoLitológico} && {profundidade} >= {trecho.Topo} && {profundidade} < {trecho.Base}) ? {trecho.Restr.Expressão} : {FAZ_NADA}";

            expressão += expressãoUcs;
            expressão += _separador;
            expressão += Environment.NewLine;

            expressão += expressãoCoesa;
            expressão += _separador;
            expressão += Environment.NewLine;

            expressão += expressãoAngat;
            expressão += _separador;
            expressão += Environment.NewLine;

            expressão += expressãoRestr;
            return expressão;
        }

        private string ObterExpressãoTrechoSolução02(TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico trecho)
        {
            const string FAZ_NADA = "0";
            var expressão = string.Empty + Environment.NewLine;
            string profundidade = Tokens.TokenDeCálculoPorTrecho;
            var identificadorGrupo = Tokens.TokenDeGrupoLitológico;
            var grupoLitológico = trecho.GrupoLitológico.Nome;

            var exprCorrelações = MontarExpressão(trecho.Ucs, trecho.Coesa, trecho.Angat, trecho.Restr, trecho.Biot);

            expressão += $@"({identificadorGrupo} == {grupoLitológico} && {profundidade} >= {trecho.Topo} && {profundidade} < {trecho.Base}) ? {exprCorrelações} : {FAZ_NADA}";
            return expressão;
        }

        private string MontaExpressãoDoGrupoLitológico(GrupoLitologico grupoLitológico, Correlação ucsBase, Correlação coesaBase, Correlação angatBase, Correlação restrBase)
        {
            const string FAZ_NADA = "0";
            string expressão = string.Empty;

            if (ucsBase == null || coesaBase == null || angatBase == null || restrBase == null) return expressão;
            var identificadorGrupo = Tokens.TokenDeGrupoLitológico;

            var exprCorrelações = MontarExpressão(ucsBase, coesaBase, angatBase, restrBase);

            expressão += Environment.NewLine;
            expressão += $@"({identificadorGrupo} == {grupoLitológico.Nome}) ? {exprCorrelações} : {FAZ_NADA}";
            return expressão;
        }

        private string MontarExpressão(Correlação ucsBase, Correlação coesaBase, Correlação angatBase, Correlação restrBase, Correlação biot = null)
        {
            List<Correlação> correlações = new List<Correlação>();
            if(ucsBase!=null)
                correlações.Add(ucsBase);
            if (coesaBase != null)
                correlações.Add(coesaBase);
            if (angatBase != null)
                correlações.Add(angatBase);
            if (restrBase != null)
                correlações.Add(restrBase);
            if (biot != null)
                correlações.Add(biot);

            OrdenadorDeCorrelações ordenadorDeCorrelações = new OrdenadorDeCorrelações(correlações, _separador);
            var correlaçõesOrdenadas = ordenadorDeCorrelações.OrdenarCorrelações();
            MontadorDeExpressões montadorDeExpressões = new MontadorDeExpressões(correlaçõesOrdenadas, _separador);
            return montadorDeExpressões.MontarExpressão();
        }
    }
}
