using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.Corte.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Filtros.Corte.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Corte.Factory
{
    public class FiltroCorteFactory : IFiltroCorteFactory
    {
        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, Cálculo> _defaultCtorCaller;
        private IFiltroCorteValidator _filtroCorteValidator;
        private IFiltroCorteEmCriaçãoValidator _filtroCorteEmCriaçãoValidator;

        public FiltroCorteFactory(IFiltroCorteValidator filtroCorteValidator, IFiltroCorteEmCriaçãoValidator filtroCorteEmCriaçãoValidator)
        {
            _filtroCorteValidator = filtroCorteValidator;
            _filtroCorteEmCriaçãoValidator = filtroCorteEmCriaçãoValidator;
        }

        public static void RegisterFiltroCorteCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateFiltroCorte(string nome, string grupoCálculo, PerfilBase perfilEntrada, PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, out IFiltro filtro)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfilEntrada, perfilSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte);

            if (!result.IsValid)
            {
                filtro = default;
                return result;
            }
            // Monta as dependências
            GrupoCálculo grupoDeCálculo = (GrupoCálculo)Enum.Parse(typeof(GrupoCálculo), grupoCálculo, true);
            IPerfisEntrada perfisDeEntrada = _perfisEntradaFactory.CreatePerfisEntrada(new List<PerfilBase> { perfilEntrada });
            IPerfisSaída perfisDeSaída = _perfisSaídaFactory.CreatePerfisSaída(new List<PerfilBase> { perfilSaída });

            // Solicita o registro do construtor
            FiltroCorte.RegisterFiltroCorteCtor();

            // Chama o construtor
            filtro = (FiltroCorte)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte);
            filtro.Execute();

            // Valida a entidade 
            return _filtroCorteValidator.Validate((FiltroCorte) filtro);
        }

        /// <summary>
        /// Verifica se a filtro de corte pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculoString, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte)
        {
            var filtroCorteEmCriação =
                new FiltroCorteEmCriação(nome, grupoCálculoString, new List<PerfilBase> { perfilEntrada }, new List<PerfilBase> { perfilSaída }, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte);

            return _filtroCorteEmCriaçãoValidator.Validate(filtroCorteEmCriação);
        }
    }

}
