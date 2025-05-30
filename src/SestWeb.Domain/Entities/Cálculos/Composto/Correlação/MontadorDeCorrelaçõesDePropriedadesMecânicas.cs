using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Domain.Entities.Cálculos.Composto.Correlação
{
    public class MontadorDeCorrelaçõesDePropriedadesMecânicas
    {
        private readonly string _nome;
        private readonly List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> _regiões;
        private readonly IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> _trechos;
        private readonly Dictionary<string, double> _variáveis;
        private readonly IList<ICorrelação> _correlaçõesBase;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;

        public MontadorDeCorrelaçõesDePropriedadesMecânicas(string nome, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões,
            IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos, Dictionary<string, double> variáveis
            , IList<ICorrelação> correlaçõesBase, ICorrelaçãoFactory correlaçãoFactory)
        {
            _nome = nome;
            _regiões = regiões;
            _trechos = trechos;
            _variáveis = variáveis;
            _correlaçõesBase = correlaçõesBase;
            _correlaçãoFactory = correlaçãoFactory;
        }

        public ICorrelação Montar()
        {
            var gerenciador = new GerenciadorExpressãoDeCálculoDePropriedadesMecânicas(_regiões, _trechos, _correlaçõesBase);
            var expressão = gerenciador.Extrair();
            var corr = _correlaçãoFactory.CreateCorrelação($"{_nome}_Correlação", "Cálculo", "Cálculo", "Cálculo de PropriedadesMecânicas", Origem.CálculoDePropriedadesMecânicas.ToString(), expressão, out ICorrelação correlaçãoCriada);
            return correlaçãoCriada;
        }
    }
}
