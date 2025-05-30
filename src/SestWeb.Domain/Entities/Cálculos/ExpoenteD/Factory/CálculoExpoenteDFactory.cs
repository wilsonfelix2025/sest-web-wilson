using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Correlação;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.EmCriação;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.Factory
{
    public class CálculoExpoenteDFactory : ICálculoExpoenteDFactory
    {
        public CálculoExpoenteDFactory(ICálculoExpoenteDValidator cálculoExpoenteDValidator
            , ICálculoExpoenteDEmCriaçãoValidator cálculoEmCriaçãoValidator)
        {
            _cálculoExpoenteDValidator = cálculoExpoenteDValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
        }

        #region Fields

        private readonly IPerfisEntradaFactory _expoenteDEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _expoenteDSaídaFactory = PerfisSaída.GetFactory();
        private readonly ICálculoExpoenteDValidator _cálculoExpoenteDValidator;
        private readonly ICálculoExpoenteDEmCriaçãoValidator _cálculoEmCriaçãoValidator;
        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, CorrelaçãoExpoenteD, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoExpoenteDCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, CorrelaçãoExpoenteD, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoExpoenteD(string nome, string grupoCálculo,
            IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória
            , ILitologia litologia, string correlação, out ICálculoExpoenteD cálculo)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, correlação);
            if (!result.IsValid)
            {
                cálculo = default;
                return result;
            }

            // Monta as dependências
            GrupoCálculo grupoDeCálculo = (GrupoCálculo)Enum.Parse(typeof(GrupoCálculo), grupoCálculo, true);
            IPerfisEntrada entradas = _expoenteDEntradaFactory.CreatePerfisEntrada(perfisEntrada);
            IPerfisSaída expoenteDDeSaída = _expoenteDSaídaFactory.CreatePerfisSaída(perfisSaída);
            CorrelaçãoExpoenteD corrExpD = (CorrelaçãoExpoenteD)Enum.Parse(typeof(CorrelaçãoExpoenteD), correlação, true);

            // Solicita o registro do construtor
            CálculoExpoenteD.RegisterCálculoExpoenteDCtor();

            // Chama o construtor
            cálculo = (CálculoExpoenteD)_defaultCtorCaller(nome, grupoDeCálculo, entradas, expoenteDDeSaída, trajetória, litologia, corrExpD);

            // Valida a entidade 
            return _cálculoExpoenteDValidator.Validate((CálculoExpoenteD)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de ExpoenteD pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculoString, IList<PerfilBase> entradas,
            IList<PerfilBase> saída, Trajetória trajetória, ILitologia litologia, string correlação)
        {
            CálculoExpoenteDEmCriação cálculoEmCriação =
                new CálculoExpoenteDEmCriação(nome, grupoCálculoString, entradas, saída, trajetória, litologia, correlação);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
