using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.TensãoVertical.EmCriação;
using SestWeb.Domain.Entities.Cálculos.TensãoVertical.Validator;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.TensãoVertical.Factory
{
    public class CálculoTensãoVerticalFactory : ICálculoTensãoVerticalFactory
    {
        public CálculoTensãoVerticalFactory(ICálculoTensãoVerticalValidator cálculoTensãoVerticalValidator, ICálculoEmCriaçãoValidator<CálculoTensãoVerticalEmCriação> cálculoEmCriaçãoValidator)
        {
            _cálculoTensãoVerticalValidator = cálculoTensãoVerticalValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
        }

        #region Fields

        private readonly ICálculoTensãoVerticalValidator _cálculoTensãoVerticalValidator;
        private readonly ICálculoEmCriaçãoValidator<CálculoTensãoVerticalEmCriação> _cálculoEmCriaçãoValidator;

        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, Geometria, DadosGerais, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoTensãoVerticalCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, Geometria, DadosGerais, CálculoTensãoVertical> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoTensãoVertical(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, Geometria geometria, DadosGerais dadosGerais, out ICálculoTensãoVertical cálculo)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, geometria, dadosGerais);
            if (!result.IsValid)
            {
                cálculo = default;
                return result;
            }

            // Monta as dependências
            GrupoCálculo grupoDeCálculo = (GrupoCálculo)Enum.Parse(typeof(GrupoCálculo), grupoCálculo, true);
            IPerfisEntrada perfisDeEntrada = PerfisEntrada.GetFactory().CreatePerfisEntrada(perfisEntrada);
            IPerfisSaída perfisDeSaída = PerfisSaída.GetFactory().CreatePerfisSaída(perfisSaída);

            // Solicita o registro do construtor
            CálculoTensãoVertical.RegisterCálculoTensõesCtor();

            // Chama o construtor
            cálculo = (CálculoTensãoVertical)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, geometria, dadosGerais);

            // Valida a entidade 
            return _cálculoTensãoVerticalValidator.Validate((CálculoTensãoVertical)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, Geometria geometria, DadosGerais dadosGerais)
        {
            CálculoTensãoVerticalEmCriação cálculoEmCriação =
                new CálculoTensãoVerticalEmCriação(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, geometria, dadosGerais);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
