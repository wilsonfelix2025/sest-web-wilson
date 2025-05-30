using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Factory
{
    public class FiltroLitologiaFactory : IFiltroLitologiaFactory
    {
        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, List<string>, Cálculo> _defaultCtorCaller;
        private readonly IFiltroLitologiaValidator _filtroValidator;
        private readonly IFiltroLitologiaEmCriaçãoValidator _filtroLitologiaEmCriaçãoValidator;

        public FiltroLitologiaFactory(IFiltroLitologiaValidator filtroLitologiaValidator, IFiltroLitologiaEmCriaçãoValidator filtroLitologiaEmCriaçãoValidator)
        {
            _filtroValidator = filtroLitologiaValidator;
            _filtroLitologiaEmCriaçãoValidator = filtroLitologiaEmCriaçãoValidator;
        }

        public static void RegisterFiltroLitologiaCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, List<string>, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateFiltroLitologia(string nome, string grupoCálculo, PerfilBase perfilEntrada, PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, List<string> litologias, out IFiltro filtro)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfilEntrada, perfilSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, litologias);
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
            FiltroLitologia.RegisterFiltroLitologiaCtor();

            // Chama o construtor
            filtro = (FiltroLitologia)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, litologias);

            // Valida a entidade 
            return _filtroValidator.Validate((FiltroLitologia)filtro);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculoString, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, List<string> litologias)  
        {
            var filtroLitologia =
                new FiltroLitologiaEmCriação(nome, grupoCálculoString, new List<PerfilBase> { perfilEntrada }, new List<PerfilBase> { perfilSaída }, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, litologias);

            return _filtroLitologiaEmCriaçãoValidator.Validate(filtroLitologia);
        }
    }
}
