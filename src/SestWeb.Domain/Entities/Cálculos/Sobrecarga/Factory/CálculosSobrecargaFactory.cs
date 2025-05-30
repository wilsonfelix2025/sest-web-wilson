using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga;
using SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.Factory;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga.Validator;
using SestWeb.Domain.Entities.Cálculos.TensãoVertical;
using SestWeb.Domain.Entities.Cálculos.TensãoVertical.Factory;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.Factory
{
    public class CálculoSobrecargaFactory : ICálculoSobrecargaFactory
    {
        public CálculoSobrecargaFactory(ICálculoSobrecargaValidator cálculoSobrecargaValidator, ICálculoEmCriaçãoValidator<CálculoSobrecargaEmCriação> cálculoEmCriaçãoValidator,
            ICálculoTensãoVerticalFactory tensãoVerticalFactory, ICálculoGradienteSobrecargaFactory gradienteSobrecargaFactory)
        {
            _cálculoSobrecargaValidator = cálculoSobrecargaValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
            _tensãoVerticalFactory = tensãoVerticalFactory;
            _gradienteSobrecargaFactory = gradienteSobrecargaFactory;
        }

        #region Fields

        private readonly ICálculoSobrecargaValidator _cálculoSobrecargaValidator;
        private readonly ICálculoEmCriaçãoValidator<CálculoSobrecargaEmCriação> _cálculoEmCriaçãoValidator;
        private readonly ICálculoTensãoVerticalFactory _tensãoVerticalFactory;
        private readonly ICálculoGradienteSobrecargaFactory _gradienteSobrecargaFactory;

        private static Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia, ICálculoTensãoVertical, ICálculoGradienteSobrecarga, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoSobrecargaCtor(Func<string, GrupoCálculo, IPerfisEntrada, IPerfisSaída, Trajetória, ILitologia,
            ICálculoTensãoVertical, ICálculoGradienteSobrecarga, CálculoSobrecarga> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoSobrecarga(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais, out ICálculoSobrecarga cálculo)
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

            // Solicita o registro do construtor
            CálculoSobrecarga.RegisterCálculoSobrecargaCtor();

            var perfilSaidaTvert = perfisSaída.FirstOrDefault(perfil => perfil.Mnemonico == TiposPerfil.GeTipoPerfil<TVERT>().Mnemônico);
            if (perfilSaidaTvert == null)
            {
                perfilSaidaTvert = PerfisFactory.Create("TVERT", $"TVERT_{nome}", trajetória, litologia);
            }

            var perfilSaídaGsobr = perfisSaída.FirstOrDefault(perfil => perfil.Mnemonico == TiposPerfil.GeTipoPerfil<GSOBR>().Mnemônico);
            if (perfilSaídaGsobr == null)
            {
                perfilSaídaGsobr = PerfisFactory.Create("GSOBR", $"GSOBR_{nome}", trajetória, litologia);
            }

            IPerfisSaída perfisDeSaída = PerfisSaída.GetFactory().CreatePerfisSaída(new List<PerfilBase>() { perfilSaidaTvert, perfilSaídaGsobr });

            _tensãoVerticalFactory.CreateCálculoTensãoVertical($"TVERT_{nome}", "TensãoVertical", perfisEntrada, new List<PerfilBase>() { perfilSaidaTvert },
                trajetória, litologia, dadosGerais.Geometria, dadosGerais, out var tvert);

            _gradienteSobrecargaFactory.CreateCálculoGradienteSobrecarga($"GSOBR_{nome}", "GradienteSobrecarga", tvert.PerfisSaída.Perfis,
                new List<PerfilBase>() { perfilSaídaGsobr }, trajetória, litologia, out var gSobr);

            // Chama o construtor
            cálculo = (CálculoSobrecarga)_defaultCtorCaller(nome, grupoDeCálculo, perfisDeEntrada, perfisDeSaída, trajetória, litologia, tvert, gSobr);

            // Valida a entidade 
            return _cálculoSobrecargaValidator.Validate((CálculoSobrecarga)cálculo);
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia)
        {
            CálculoSobrecargaEmCriação cálculoEmCriação =
                new CálculoSobrecargaEmCriação(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
