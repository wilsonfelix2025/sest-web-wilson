using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.DTOs.Cálculo.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Gradientes.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Factory
{
    public class CálculoGradientesFactory : ICálculoGradientesFactory
    {
        public CálculoGradientesFactory(ICálculoGradientesValidator cálculoGradientesValidator, ICálculoEmCriaçãoValidator<CálculoGradientesEmCriação> cálculoEmCriaçãoValidator)
        {
            _cálculoGradientesValidator = cálculoGradientesValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
        }

        #region Fields

        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private readonly ICálculoGradientesValidator _cálculoGradientesValidator;
        private readonly ICálculoEmCriaçãoValidator<CálculoGradientesEmCriação> _cálculoEmCriaçãoValidator;

        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, DadosMalha, EntradasColapsosDTO, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoGradientesCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, DadosMalha, EntradasColapsosDTO, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoGradientes(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, DadosMalha dadosMalha, EntradasColapsosDTO entradasColapsos
            , out ICálculoGradientes cálculo)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia);
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
            CálculoGradientes.RegisterCálculoGradientesCtor();

            // Chama o construtor
            cálculo = (CálculoGradientes)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, dadosMalha, entradasColapsos);

            // Valida a entidade 
            return _cálculoGradientesValidator.Validate((CálculoGradientes)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia)
        {
            CálculoGradientesEmCriação cálculoEmCriação =
                new CálculoGradientesEmCriação(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
