using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.Factory
{
    public class FiltroMédiaMóvelFactory : IFiltroMédiaMóvelFactory
    {
        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, int, Cálculo> _defaultCtorCaller;
        private readonly IFiltroMédiaMóvelValidator _filtroMédiaMóvelValidator;
        private readonly IFiltroMédiaMóvelEmCriaçãoValidator _filtroMédiaMóvelEmCriaçãoValidator;

        public FiltroMédiaMóvelFactory(IFiltroMédiaMóvelValidator filtroMédiaMóvelValidator, IFiltroMédiaMóvelEmCriaçãoValidator filtroMédiaMóvelEmCriaçãoValidator)
        {
            _filtroMédiaMóvelValidator = filtroMédiaMóvelValidator;
            _filtroMédiaMóvelEmCriaçãoValidator = filtroMédiaMóvelEmCriaçãoValidator;
        }

        public static void RegisterFiltroMédiaMóvelCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, int, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateFiltroMédiaMóvel(string nome, string grupoCálculo, PerfilBase perfilEntrada, PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, int númeroPontos, out IFiltro filtro)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfilEntrada, perfilSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, númeroPontos);
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
            FiltroMédiaMóvel.RegisterFiltroMédiaMóvelCtor();

            // Chama o construtor
            filtro = (FiltroMédiaMóvel)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, númeroPontos);
            filtro.Execute();

            // Valida a entidade 
            return _filtroMédiaMóvelValidator.Validate((FiltroMédiaMóvel)filtro);
        }

        /// <summary>
        /// Verifica se a filtro média móvel pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculoString, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, int númeroPontos)
        {
            var filtroSimplesEmCriação =
                new FiltroMédiaMóvelEmCriação(nome, grupoCálculoString, new List<PerfilBase> { perfilEntrada }, new List<PerfilBase> { perfilSaída }, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, númeroPontos);

            return _filtroMédiaMóvelEmCriaçãoValidator.Validate(filtroSimplesEmCriação);
        }
    }
}
