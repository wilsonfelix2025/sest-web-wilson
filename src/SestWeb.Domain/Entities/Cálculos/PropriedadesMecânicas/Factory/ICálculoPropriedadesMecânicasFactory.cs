using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Factory
{
    public interface ICálculoPropriedadesMecânicasFactory
    {
        ValidationResult CreateCálculoPropriedadesMecânicas(string nome, IList<ICorrelação> listaCorrelação, string grupoCálculo,
            IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória
            , ILitologia litologia, IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos
            , Geometria geometria, DadosGerais dadosGerais, List<TrechoDTO> trechosFront, string correlaçãoDoCálculo
            , List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões, List<RegiãoDTO> regiõesFront,  out ICálculoPropriedadesMecânicas cálculo);
    }
}
