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
using SestWeb.Domain.Entities.Cálculos.Perfis.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Perfis.Validator;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Perfis.Factory
{
    public class CálculoPerfisFactory : ICálculoPerfisFactory
    {
        public CálculoPerfisFactory(ICálculoPerfisValidator cálculoPerfisValidator
            , ICálculoPerfisEmCriaçãoValidator cálculoEmCriaçãoValidator
            , ICorrelaçãoFactory correlaçãoFactory)
        {
            _cálculoPerfisValidator = cálculoPerfisValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
            _correlaçãoFactory = correlaçãoFactory;
        }

        #region Fields

        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private readonly ICálculoPerfisValidator _cálculoPerfisValidator;
        private readonly ICálculoPerfisEmCriaçãoValidator _cálculoEmCriaçãoValidator;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private static Func<string, IList<ICorrelação>, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, IList<VariávelDTO>, IList<TrechoCálculo>, ICorrelaçãoFactory, Geometria, DadosGerais, List<TrechoDTO>, string, Cálculo> _defaultCtorCaller;
        
        #endregion

        #region Methods

        public static void RegisterCálculoPerfisCtor(Func<string, IList<ICorrelação>, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, IList<VariávelDTO>, IList<TrechoCálculo>, ICorrelaçãoFactory, Geometria, DadosGerais, List<TrechoDTO>, string, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoPerfis(string nome, IList<ICorrelação> listaCorrelação, string grupoCálculo,
            IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória
            , ILitologia litologia, IList<VariávelDTO> variáveis, IList<TrechoCálculo> trechos, Geometria geometria
            , DadosGerais dadosGerais, List<TrechoDTO> trechosFront, string correlaçãoDoCálculo, out ICálculoPerfis cálculo)
        {
            var result = GarantirCriaçãoEntidade(nome, listaCorrelação, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, variáveis, trechos, geometria, dadosGerais);
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
            CálculoPerfis.RegisterCálculoPerfisCtor();

            // Chama o construtor
            cálculo = (CálculoPerfis)_defaultCtorCaller(nome, listaCorrelação, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, variáveis, trechos, _correlaçãoFactory, geometria, dadosGerais, trechosFront, correlaçãoDoCálculo);

            // Valida a entidade 
            return _cálculoPerfisValidator.Validate((CálculoPerfis)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, IList<ICorrelação> listaCorrelação, string grupoCálculoString, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, IList<VariávelDTO> variáveis, IList<TrechoCálculo> trechos, Geometria geometria
            , DadosGerais dadosGerais)
        {
            CálculoPerfisEmCriação cálculoEmCriação =
                new CálculoPerfisEmCriação(nome, listaCorrelação, grupoCálculoString, perfisEntrada, perfisSaída, trajetória, litologia, variáveis, trechos, geometria, dadosGerais);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
