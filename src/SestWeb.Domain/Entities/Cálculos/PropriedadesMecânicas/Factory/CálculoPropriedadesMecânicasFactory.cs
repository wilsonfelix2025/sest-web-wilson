using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.EmCriação;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Validator;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Factory
{
    public class CálculoPropriedadesMecânicasFactory : ICálculoPropriedadesMecânicasFactory
    {
        public CálculoPropriedadesMecânicasFactory(ICálculoPropriedadesMecânicasValidator cálculoValidator
            , ICálculoPropriedadesMecânicasEmCriaçãoValidator cálculoEmCriaçãoValidator
            , ICorrelaçãoFactory correlaçãoFactory)
        {
            _cálculoValidator = cálculoValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
            _correlaçãoFactory = correlaçãoFactory;
        }

        #region Fields

        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private readonly ICálculoPropriedadesMecânicasValidator _cálculoValidator;
        private readonly ICálculoPropriedadesMecânicasEmCriaçãoValidator _cálculoEmCriaçãoValidator;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private static Func<string, IList<ICorrelação>, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico>, ICorrelaçãoFactory, Geometria, DadosGerais, List<TrechoDTO>, string, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas>, List<RegiãoDTO>, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoPropriedadesMecânicasCtor(Func<string, IList<ICorrelação>, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico>, ICorrelaçãoFactory, Geometria, DadosGerais, List<TrechoDTO>, string, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas>, List<RegiãoDTO>, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoPropriedadesMecânicas(string nome, IList<ICorrelação> listaCorrelação, string grupoCálculo,
            IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória
            , ILitologia litologia, IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos, Geometria geometria
            , DadosGerais dadosGerais, List<TrechoDTO> trechosFront, string correlaçãoDoCálculo, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões
            , List<RegiãoDTO> regiõesFront, out ICálculoPropriedadesMecânicas cálculo)
        {
            var result = GarantirCriaçãoEntidade(nome, listaCorrelação, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, trechos, geometria, dadosGerais, regiões, correlaçãoDoCálculo);
            if (!result.IsValid)
            {
                cálculo = default;
                return result;
            }

            // Monta as dependências
            GrupoCálculo grupoDeCálculo = (GrupoCálculo)Enum.Parse(typeof(GrupoCálculo), grupoCálculo, true);
            IPerfisEntrada perfisDeEntrada = _perfisEntradaFactory.CreatePerfisEntrada(perfisEntrada);
            IPerfisSaída perfisDeSaída = _perfisSaídaFactory.CreatePerfisSaída(perfisSaída);

            // Solicita o registro do construtor
            CálculoPropriedadesMecânicas.RegisterCálculoPerfisCtor();

            // Chama o construtor
            cálculo = (CálculoPropriedadesMecânicas)_defaultCtorCaller(nome, listaCorrelação, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, trechos, _correlaçãoFactory, geometria, dadosGerais, trechosFront, correlaçãoDoCálculo, regiões, regiõesFront);

            // Valida a entidade 
            return _cálculoValidator.Validate((CálculoPropriedadesMecânicas)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, IList<ICorrelação> listaCorrelação, string grupoCálculoString, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos, Geometria geometria
            , DadosGerais dadosGerais, IList<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões, string correlaçãoDoCálculo)
        {
            CálculoPropriedadesMecânicasEmCriação cálculoEmCriação =
                new CálculoPropriedadesMecânicasEmCriação(nome, listaCorrelação, grupoCálculoString, perfisEntrada, perfisSaída, trajetória, litologia, trechos, geometria, dadosGerais, regiões, correlaçãoDoCálculo);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
