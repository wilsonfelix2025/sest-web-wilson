using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Métodos
{
    public class CálculoPressãoPorosEatonDTCFiltrado: CálculoPressãoPoros, ICálculoPressãoPoros
    {
        public DadosReservatório DadosReservatório { get; set; }
        public bool ComDepleção { get; set; } = false;
        public CálculoPressãoPorosEatonDTCFiltrado(string nome, GrupoCálculo grupoCálculo, CorrelaçãoPressãoPoros métodoCálculo, IPerfisEntrada perfisEntrada,
            IPerfisSaída perfisSaída, IList<ParâmetroCorrelação> parâmetros, Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais, DadosReservatório dadosReservatório) : base(nome, grupoCálculo, métodoCálculo, perfisEntrada, perfisSaída, parâmetros, trajetória, litologia, dadosGerais)
        {
            DadosReservatório = dadosReservatório;
            if (dadosReservatório != null)
            {
                ComDepleção = true;
            }
        }

        public override void Execute(bool chamadaPelaPipeline)
        {
            var pontos = Calcular();
            var perfilSaída = PerfisSaída.Perfis.First(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<GPORO>().Mnemônico);
            var perfilSaídaEmPressão = PerfisSaída.Perfis.First(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<PPORO>().Mnemônico);
            perfilSaída.Clear();
            perfilSaídaEmPressão.Clear();

            if (DadosReservatório != null)
            {
                var cotaEmPv = Geometria.CategoriaPoço == CategoriaPoço.OffShore ? (-1) * DadosReservatório.Referencia.Cota + Geometria.MesaRotativa : (-1) * DadosReservatório.Referencia.Cota + Geometria.OnShore.Elevação + Geometria.MesaRotativa;
                var dadosDepleção = new DadosDepleção
                {
                    NomePoço = DadosReservatório.Referencia.Poco,
                    PressãoCorrelação = DadosReservatório.Referencia.Pp,
                    PvCorrelação = new Profundidade(CotaToPv(DadosReservatório.Referencia.Cota)),
                    Reservatório = new Depleção.Reservatório
                    {
                        Nome = DadosReservatório.Nome,
                        ContatoGásÓleo = new Profundidade(CotaToPv(DadosReservatório.Referencia.Contatos.GasOleo)),
                        ContatoÓleoÁgua = new Profundidade(CotaToPv(DadosReservatório.Referencia.Contatos.OleoAgua)),
                        ProfundidadeBase = new Profundidade(CotaToPv(DadosReservatório.Interesse.Base)),
                        ProfundidadeTopo = new Profundidade(CotaToPv(DadosReservatório.Interesse.Topo)),
                        ValorGásGás = DadosReservatório.Referencia.Gradiente.Gas,
                        ValorGásÁgua = DadosReservatório.Referencia.Gradiente.Agua,
                        ValorGásÓleo = DadosReservatório.Referencia.Gradiente.Oleo
                    }
                };

                var calculador = new CalculadorDepleção(new List<DadosDepleção>() { dadosDepleção });
                var pontosDepletados = calculador.ObterPontosGporoDepletada(pontos);

                foreach (var pontoDepletado in pontosDepletados)
                {
                    perfilSaída.AddPonto(ConversorProfundidade, pontoDepletado.Pm, pontoDepletado.Pv, pontoDepletado.Valor, pontoDepletado.TipoProfundidade, pontoDepletado.Origem);
                    perfilSaídaEmPressão.AddPonto(ConversorProfundidade, pontoDepletado.Pm, pontoDepletado.Pv, pontoDepletado.Valor * pontoDepletado.Pv.Valor * FatorConversão, pontoDepletado.TipoProfundidade, pontoDepletado.Origem);
                }
            } 
            else
            {
                foreach (var ponto in pontos)
                {
                    perfilSaída.AddPonto(ConversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor, ponto.TipoProfundidade, ponto.Origem);
                    perfilSaídaEmPressão.AddPonto(ConversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor * ponto.Pv.Valor * FatorConversão, ponto.TipoProfundidade, ponto.Origem);
                }
            }
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override PerfilBase ObterPerfilComTrend()
        {
            return PerfisEntrada.Perfis.Single(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<DTC>().Mnemônico && x.TemTrendCompactação);
        }

        public override PerfilBase ObterPerfilObservado()
        {
            return PerfisEntrada.Perfis.Single(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<DTC>().Mnemônico && x.TemTrendCompactação);
        }

        protected override double CalcularGporo(double profundidadeMedida, double profundidadeVertical, double sobrecarga,
            double trend, double dtc, double exp, Func<double, double> getGppn, GrupoLitologico grupoLitológico,
            ConcurrentBag<Tuple<double, double>> profundidadesForaLitologia)
        {
            double gporo;

            if (grupoLitológico == GrupoLitologico.Evaporitos)
            {
                gporo = 0.0;
            }
            else
            {
                var gppn = getGppn(profundidadeVertical);
                gporo = sobrecarga - ((sobrecarga - gppn) * Math.Pow(trend / dtc, exp));
                if (gporo < gppn)
                    gporo = gppn;
            }

            return gporo;
        }

        private double CotaToPv(double cota)
        {
            return -cota + Geometria.MesaRotativa;
        }

        public new static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(CálculoPressãoPorosEatonDTCFiltrado)))
                return;

            BsonClassMap.RegisterClassMap<CálculoPressãoPorosEatonDTCFiltrado>(calcPPoros =>
            {
                calcPPoros.SetDiscriminator(nameof(CálculoPressãoPorosEatonDTCFiltrado));
                calcPPoros.AutoMap();
                calcPPoros.MapMember(c => c.ComDepleção);
                calcPPoros.MapMember(c => c.DadosReservatório);
            });
        }
    }
}
