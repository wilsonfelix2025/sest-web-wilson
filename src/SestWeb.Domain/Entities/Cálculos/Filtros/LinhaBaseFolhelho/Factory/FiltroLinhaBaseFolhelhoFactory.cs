using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.Factory
{
    public class FiltroLinhaBaseFolhelhoFactory : IFiltroLinhaBaseFolhelhoFactory
    {
        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, PerfilBase, Cálculo> _defaultCtorCaller;
        private readonly IFiltroLinhaBaseFolhelhoValidator _filtroValidator;
        private readonly IFiltroLBFEmCriaçãoValidator _filtroLbfEmCriaçãoValidator;

        public FiltroLinhaBaseFolhelhoFactory(IFiltroLinhaBaseFolhelhoValidator filtroLinhaBaseFolhelhoValidator, IFiltroLBFEmCriaçãoValidator filtroLbfEmCriaçãoValidator)
        {
            _filtroValidator = filtroLinhaBaseFolhelhoValidator;
            _filtroLbfEmCriaçãoValidator = filtroLbfEmCriaçãoValidator;
        }

        public static void RegisterFiltroLinhaBaseFolhelhoCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, double?, double?, TipoCorteEnum?, PerfilBase, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateFiltroLinhaBaseFolhelho(string nome, string grupoCálculo, PerfilBase perfilEntrada, PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, PerfilBase perfilLBF, out IFiltro filtro)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfilEntrada, perfilSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, perfilLBF);
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
            FiltroLinhaBaseFolhelho.RegisterFiltroLinhaBaseFolhelhoCtor();

            // Chama o construtor
            filtro = (FiltroLinhaBaseFolhelho)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, perfilLBF);
            filtro.Execute();

            // Valida a entidade 
            return _filtroValidator.Validate((FiltroLinhaBaseFolhelho)filtro);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculoString, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, PerfilBase perfilLBF)
        {
            var filtroLBF =
                new FiltroLBFEmCriação(nome, grupoCálculoString, new List<PerfilBase> { perfilEntrada }, new List<PerfilBase> { perfilSaída }, trajetória, litologia, limiteInferior, limiteSuperior, tipoCorte, perfilLBF);

            return _filtroLbfEmCriaçãoValidator.Validate(filtroLBF);
        }
    }
}
