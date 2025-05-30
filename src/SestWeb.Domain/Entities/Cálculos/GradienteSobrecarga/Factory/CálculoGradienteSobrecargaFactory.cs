using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.Validator;
using SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.EmCriação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.Factory
{
    public class CálculoGradienteSobrecargaFactory : ICálculoGradienteSobrecargaFactory
    {
        public CálculoGradienteSobrecargaFactory(ICálculoGradienteSobrecargaValidator cálculoGradienteSobrecargaValidator, ICálculoEmCriaçãoValidator<CálculoGradienteSobrecargaEmCriação> cálculoEmCriaçãoValidator)
        {
            _cálculoGradienteSobrecargaValidator = cálculoGradienteSobrecargaValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
        }

        #region 
        
        private readonly ICálculoGradienteSobrecargaValidator _cálculoGradienteSobrecargaValidator;
        private readonly ICálculoEmCriaçãoValidator<CálculoGradienteSobrecargaEmCriação> _cálculoEmCriaçãoValidator;

        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoGradienteSobrecargaCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, CálculoGradienteSobrecarga> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoGradienteSobrecarga(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, out ICálculoGradienteSobrecarga cálculo)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia);
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
            CálculoGradienteSobrecarga.RegisterCálculoGradienteSobrecargaCtor();

            // Chama o construtor
            cálculo = (CálculoGradienteSobrecarga)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia);

            // Valida a entidade 
            return _cálculoGradienteSobrecargaValidator.Validate((CálculoGradienteSobrecarga)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia)
        {
            CálculoGradienteSobrecargaEmCriação cálculoEmCriação =
                new CálculoGradienteSobrecargaEmCriação(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
