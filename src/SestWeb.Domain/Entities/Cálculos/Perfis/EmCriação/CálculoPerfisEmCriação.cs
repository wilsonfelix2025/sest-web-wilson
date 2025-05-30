using System.Collections.Generic;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Perfis.EmCriação
{
    public class CálculoPerfisEmCriação : CálculoEmCriação
    {
        public IList<ICorrelação> ListaCorrelação { get; }
        public IList<VariávelDTO> Variáveis { get; }
        public IList<TrechoCálculo> Trechos { get; }
        public Geometria Geometria { get; }
        public DadosGerais DadosGerais { get; }

        public CálculoPerfisEmCriação(string nome, IList<ICorrelação> listaCorrelação, string grupoCálculo, IList<PerfilBase> perfisEntrada
            , IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, IList<VariávelDTO> variáveis
            , IList<TrechoCálculo> trechos, Geometria geometria, DadosGerais dadosGerais) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            ListaCorrelação = listaCorrelação;
            Variáveis = variáveis;
            Trechos = trechos;
            Geometria = geometria;
            DadosGerais = dadosGerais;
        }
    }
}
