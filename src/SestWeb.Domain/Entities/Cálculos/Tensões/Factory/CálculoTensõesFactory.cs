using System;
using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.Tensões.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Tensões.Validator;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Entities.Cálculos.Tensões.Factory
{
    public class CálculoTensõesFactory : ICálculoTensõesFactory
    {
        public CálculoTensõesFactory(ICálculoTensõesValidator cálculoTensõesValidator, ICálculoEmCriaçãoValidator<CálculoTensõesEmCriação> cálculoEmCriaçãoValidator)
        {
            _cálculoTensõesValidator = cálculoTensõesValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
        }

        #region Fields

        private readonly IPerfisEntradaFactory _perfisEntradaFactory = PerfisEntrada.GetFactory();
        private readonly IPerfisSaídaFactory _perfisSaídaFactory = PerfisSaída.GetFactory();
        private readonly ICálculoTensõesValidator _cálculoTensõesValidator;
        private readonly ICálculoEmCriaçãoValidator<CálculoTensõesEmCriação> _cálculoEmCriaçãoValidator;

        private static Func<string, MetodologiaCálculoTensãoHorizontalMenorEnum, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, List<ParâmetrosLotDTO>, DepleçãoDTO, double?, BreakoutDTO, RelaçãoTensãoDTO, FraturasTrechosVerticaisDTO, Geometria, DadosGerais, MetodologiaCálculoTensãoHorizontalMaiorEnum, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoTensõesCtor(Func<string, MetodologiaCálculoTensãoHorizontalMenorEnum, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, List<ParâmetrosLotDTO>, DepleçãoDTO, double?, BreakoutDTO, RelaçãoTensãoDTO, FraturasTrechosVerticaisDTO, Geometria, DadosGerais, MetodologiaCálculoTensãoHorizontalMaiorEnum, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoTensões(string nome, MetodologiaCálculoTensãoHorizontalMenorEnum metodologiaCálculoTHORmin, string grupoCálculo,
            IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, List<ParâmetrosLotDTO> parâmetrosLOT
            , DepleçãoDTO depleção, double? coeficiente, BreakoutDTO breakout, RelaçãoTensãoDTO relaçãoTensão, FraturasTrechosVerticaisDTO fraturasTrechosVerticais
            , Geometria geometria, DadosGerais dadosGerais, MetodologiaCálculoTensãoHorizontalMaiorEnum metodologiaCálculoTHORmax, out ICálculoTensões cálculo)
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
            CálculoTensões.RegisterCálculoTensõesCtor();

            // Chama o construtor
            cálculo = (CálculoTensões)_defaultCtorCaller(nome, metodologiaCálculoTHORmin, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, parâmetrosLOT, depleção, coeficiente, breakout, relaçãoTensão, fraturasTrechosVerticais, geometria, dadosGerais, metodologiaCálculoTHORmax);

            // Valida a entidade 
            return _cálculoTensõesValidator.Validate((CálculoTensões)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia)
        {
            CálculoTensõesEmCriação cálculoEmCriação =
                new CálculoTensõesEmCriação(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
