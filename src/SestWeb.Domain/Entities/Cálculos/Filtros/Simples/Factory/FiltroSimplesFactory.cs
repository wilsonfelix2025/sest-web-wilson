using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Factory
{
    public class FiltroSimplesFactory : IFiltroSimplesFactory
    {
        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, double, Cálculo> _defaultCtorCaller;
        private readonly IFiltroSimplesValidator _filtroSimplesValidator;
        private readonly IFiltroSimplesEmCriaçãoValidator _filtroSimplesEmCriaçãoValidator;

        public FiltroSimplesFactory(IFiltroSimplesValidator filtroSimplesValidator, IFiltroSimplesEmCriaçãoValidator filtroSimplesEmCriaçãoValidator)
        {
            _filtroSimplesValidator = filtroSimplesValidator;
            _filtroSimplesEmCriaçãoValidator = filtroSimplesEmCriaçãoValidator;
        }

        public static void RegisterFiltroSimplesCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, double, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateFiltroSimples(string nome, string grupoCálculo, PerfilBase perfilEntrada, PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, double desvioMáximo, out IFiltro filtro)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfilEntrada, perfilSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, desvioMáximo);
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
            FiltroSimples.RegisterFiltroSimplesCtor();

            // Chama o construtor
            filtro = (FiltroSimples)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, desvioMáximo);
            filtro.Execute();

            // Valida a entidade 
            return _filtroSimplesValidator.Validate((FiltroSimples)filtro);
        }

        /// <summary>
        /// Verifica se a filtro simples pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculoString, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, double? desvioMáximo)
        {
            var filtroSimplesEmCriação =
                new FiltroSimplesEmCriação(nome, grupoCálculoString, new List<PerfilBase> { perfilEntrada }, new List<PerfilBase> { perfilSaída }, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, desvioMáximo);

            return _filtroSimplesEmCriaçãoValidator.Validate(filtroSimplesEmCriação);
        }

    }

}
