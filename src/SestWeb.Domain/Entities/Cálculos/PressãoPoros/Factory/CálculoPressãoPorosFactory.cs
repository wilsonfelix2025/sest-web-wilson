using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.EmCriação;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Métodos;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Validator;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Factory
{
    public class CálculoPressãoPorosFactory : ICálculoPressãoPorosFactory
    {
        public CálculoPressãoPorosFactory(ICálculoPressãoPorosValidator cálculoPressãoPorosValidator, ICálculoEmCriaçãoValidator<CálculoPressãoPorosEmCriação> cálculoEmCriaçãoValidator)
        {
            _perfisEntradaFactory = PerfisEntrada.GetFactory();
            _perfisSaídaFactory = PerfisSaída.GetFactory();
            _cálculoPressãoPorosValidator = cálculoPressãoPorosValidator;
            _cálculoEmCriaçãoValidator = cálculoEmCriaçãoValidator;
        }

        #region Fields

        private readonly IPerfisEntradaFactory _perfisEntradaFactory;
        private readonly IPerfisSaídaFactory _perfisSaídaFactory;
        private readonly ICálculoPressãoPorosValidator _cálculoPressãoPorosValidator;
        private readonly ICálculoEmCriaçãoValidator<CálculoPressãoPorosEmCriação> _cálculoEmCriaçãoValidator;

        private static Func<string, GrupoCálculo, CorrelaçãoPressãoPoros, IPerfisEntrada, IPerfisSaída, IList<ParâmetroCorrelação>, Trajetória, ILitologia, DadosGerais, Cálculo> _defaultCtorCaller;

        #endregion

        #region Methods

        public static void RegisterCálculoPressãoPorosCtor(Func<string, GrupoCálculo, CorrelaçãoPressãoPoros, IPerfisEntrada, IPerfisSaída, IList<ParâmetroCorrelação>, Trajetória, ILitologia, DadosGerais, Cálculo> ctorCaller)
        {
            _defaultCtorCaller = ctorCaller;
        }

        public ValidationResult CreateCálculoPressãoPoros(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, CorrelaçãoPressãoPoros métodoCálculo, IList<ParâmetroCorrelação> parâmetrosCorrelação,
            Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais, DadosReservatório reservatório, out ICálculoPressãoPoros cálculo)
        {
            var result = GarantirCriaçãoEntidade(nome, grupoCálculo, perfisEntrada, perfisSaída, métodoCálculo, parâmetrosCorrelação, trajetória, litologia, dadosGerais, reservatório);
            if (!result.IsValid)
            {
                cálculo = default;
                return result;
            }

            // Monta as dependências
            GrupoCálculo grupoDeCálculo = (GrupoCálculo)Enum.Parse(typeof(GrupoCálculo), grupoCálculo, true);
            IPerfisEntrada perfisDeEntrada = _perfisEntradaFactory.CreatePerfisEntrada(perfisEntrada);
            IPerfisSaída perfisDeSaída = _perfisSaídaFactory.CreatePerfisSaída(new List<PerfilBase>());

            // Solicita o registro do construtor
            CálculoPressãoPoros.RegisterCálculoPressãoPorosCtor();

            var prefixoPressaoSaida = "";

            if (métodoCálculo == CorrelaçãoPressãoPoros.EatonDTC || métodoCálculo == CorrelaçãoPressãoPoros.EatonExpoenteD || métodoCálculo == CorrelaçãoPressãoPoros.EatonResistividade)
            {
                var perfilGporo = perfisSaída.SingleOrDefault(perfil => perfil.Mnemonico == TiposPerfil.GeTipoPerfil<GPORO>().Mnemônico);

                if (perfilGporo == null)
                {
                    prefixoPressaoSaida = "PPORO_";
                    var prefixoGradienteSaida = "GPORO_";
                    var perfilSaída = PerfisFactory.Create(TiposPerfil.GeTipoPerfil<GPORO>().Mnemônico, prefixoGradienteSaida + nome, trajetória, litologia);
                    perfisDeSaída.Perfis.Add(perfilSaída);
                } 
                else
                {
                    perfisDeSaída.Perfis.Add(perfilGporo);
                }
            }

            if (métodoCálculo != CorrelaçãoPressãoPoros.Gradiente)
            {
                var perfilPporo = perfisSaída.SingleOrDefault(perfil => perfil.Mnemonico == TiposPerfil.GeTipoPerfil<PPORO>().Mnemônico);

                if (perfilPporo == null)
                {
                    var perfilSaídaEmPressão = PerfisFactory.Create(TiposPerfil.GeTipoPerfil<PPORO>().Mnemônico, prefixoPressaoSaida + nome, trajetória, litologia);
                    perfisDeSaída.Perfis.Add(perfilSaídaEmPressão);
                }
                else
                {
                    perfisDeSaída.Perfis.Add(perfilPporo);
                }
            }
            else
            {
                var perfilGradInterp = perfisSaída.SingleOrDefault(perfil => perfil.Mnemonico == TiposPerfil.GeTipoPerfil<GPPI>().Mnemônico);

                if (perfilGradInterp == null)
                {
                    var perfilSaída = PerfisFactory.Create(TiposPerfil.GeTipoPerfil<GPPI>().Mnemônico, nome, trajetória, litologia);
                    perfisDeSaída.Perfis.Add(perfilSaída);
                }
                else
                {
                    perfisDeSaída.Perfis.Add(perfilGradInterp);
                }
            }

            // Chama o construtor
            if (métodoCálculo == CorrelaçãoPressãoPoros.EatonDTC)
            {
                cálculo = new CálculoPressãoPorosEatonDTCFiltrado(nome, grupoDeCálculo, métodoCálculo, perfisDeEntrada, perfisDeSaída, parâmetrosCorrelação, trajetória, litologia, dadosGerais, reservatório);
            }
            else if (métodoCálculo == CorrelaçãoPressãoPoros.EatonExpoenteD)
            {
                cálculo = new CálculoPressãoPorosEatonExpoenteDFiltrado(nome, grupoDeCálculo, métodoCálculo, perfisDeEntrada, perfisDeSaída, parâmetrosCorrelação, trajetória, litologia, dadosGerais, reservatório);
            }
            else if (métodoCálculo == CorrelaçãoPressãoPoros.EatonResistividade)
            {
                cálculo = new CálculoPressãoPorosEatonResistividadeFiltrado(nome, grupoDeCálculo, métodoCálculo, perfisDeEntrada, perfisDeSaída, parâmetrosCorrelação, trajetória, litologia, dadosGerais, reservatório);
            }
            else if (métodoCálculo == CorrelaçãoPressãoPoros.Hidrostática)
            {
                cálculo = new CálculoPressãoPorosHidrostática(nome, grupoDeCálculo, métodoCálculo, perfisDeEntrada, perfisDeSaída, parâmetrosCorrelação, trajetória, litologia, dadosGerais);
            }
            else if (métodoCálculo == CorrelaçãoPressãoPoros.Gradiente)
            {
                cálculo = new CálculoPressãoPorosGradienteInterpretado(nome, grupoDeCálculo, métodoCálculo, perfisDeEntrada, perfisDeSaída, parâmetrosCorrelação, trajetória, litologia, dadosGerais);
            }
            else
            {
                throw new InvalidOperationException("tipo inválido de cálculo recebido.");
            }

            // Valida a entidade 
            var newresult = _cálculoPressãoPorosValidator.Validate((CálculoPressãoPoros)cálculo);
            return newresult;
        }

        /// <summary>
        /// Verifica se a cálculo de perfis pode ser montado em segurança, mesmo que com dados inválidos.
        /// </summary>
        private ValidationResult GarantirCriaçãoEntidade(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, CorrelaçãoPressãoPoros métodoCálculo, IList<ParâmetroCorrelação> parâmetrosCorrelação,
            Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais, DadosReservatório reservatório)
        {
            CálculoPressãoPorosEmCriação cálculoEmCriação =
                new CálculoPressãoPorosEmCriação(nome, grupoCálculo, perfisEntrada, perfisSaída, métodoCálculo, parâmetrosCorrelação, trajetória, litologia, dadosGerais);

            return _cálculoEmCriaçãoValidator.Validate(cálculoEmCriação);
        }

        #endregion
    }
}
