using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.EmCriação
{
    public class CálculoPropriedadesMecânicasEmCriação : CálculoEmCriação
    {
        public IList<ICorrelação> ListaCorrelação { get; }
        public IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> Trechos { get; }
        public Geometria Geometria { get; }
        public DadosGerais DadosGerais { get; }
        public IList<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> Regiões { get; }
        public string CorrelaçãoDoCálculo { get; }


        public CálculoPropriedadesMecânicasEmCriação(string nome, IList<ICorrelação> listaCorrelação, string grupoCálculo, IList<PerfilBase> perfisEntrada
            , IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia
            , IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos, Geometria geometria, DadosGerais dadosGerais
            , IList<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões, string correlaçãoDoCálculo) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            ListaCorrelação = listaCorrelação;
            Trechos = trechos;
            Geometria = geometria;
            DadosGerais = dadosGerais;
            Regiões = regiões;
            CorrelaçãoDoCálculo = correlaçãoDoCálculo;
        }
    }
}
