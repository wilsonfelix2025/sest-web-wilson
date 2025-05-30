using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.Cálculos.Tensões.Factory;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public class CálculoTensões : Cálculo, ICálculoTensões
    {
        #region Constructor
        private CálculoTensões(string nome, MetodologiaCálculoTensãoHorizontalMenorEnum metodologiaCálculoTensãoHorizontalMenor, GrupoCálculo grupoCálculo
            , IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, List<ParâmetrosLotDTO> parâmetrosLotDTO
            , DepleçãoDTO depleção, double? coeficiente, BreakoutDTO breakout, RelaçãoTensãoDTO relaçãoTensão, FraturasTrechosVerticaisDTO fraturasTrechosVerticais
            , Geometria geometria, DadosGerais dadosGerais, MetodologiaCálculoTensãoHorizontalMaiorEnum metodologiaTHORmax) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            MetodologiaTHORmin = metodologiaCálculoTensãoHorizontalMenor;
            ParâmetrosLotDTO = parâmetrosLotDTO;
            Depleção = depleção;
            Coeficiente = coeficiente;
            Breakout = breakout;
            RelaçãoTensão = relaçãoTensão;
            FraturasTrechosVerticais = fraturasTrechosVerticais;
            Geometria = geometria;
            DadosGerais = dadosGerais;
            MetodologiaTHORmax = metodologiaTHORmax;
            TensãoMenor = PerfisSaída.Perfis.Single(p => p.Mnemonico == "THORmin");

        }

        public static void RegisterCálculoTensõesCtor()
        {
            CálculoTensõesFactory.RegisterCálculoTensõesCtor((nome, metodologiaCálculoTensãoHorizontalMenor, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, parâmetrosLotDTO, depleção, coeficiente, breakout, relaçãoTensão, fraturasTrechosVerticais, geometria, dadosGerais, metodologiaCálculoTensãoHorizontalMaior) => new CálculoTensões(nome, metodologiaCálculoTensãoHorizontalMenor, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, parâmetrosLotDTO, depleção, coeficiente, breakout, relaçãoTensão, fraturasTrechosVerticais, geometria, dadosGerais, metodologiaCálculoTensãoHorizontalMaior));
        }

        #endregion

        #region Properties
        public FraturasTrechosVerticaisDTO FraturasTrechosVerticais { get; private set; }
        public RelaçãoTensãoDTO RelaçãoTensão { get; private set; }
        public BreakoutDTO Breakout { get; private set; }
        public MetodologiaCálculoTensãoHorizontalMenorEnum MetodologiaTHORmin { get; private set; }
        public MetodologiaCálculoTensãoHorizontalMaiorEnum MetodologiaTHORmax { get; private set; }
        public List<ParâmetrosLotDTO> ParâmetrosLotDTO { get; private set; }
        public DepleçãoDTO Depleção { get; private set; }
        public double? Coeficiente { get; private set; }
        public Geometria Geometria { get; private set; }
        public DadosGerais DadosGerais { get; private set; }
        public PerfilBase TensãoMenor { get; private set; }

        [MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<double, double> ProfundidadeEValorThorMinSemDepleção { get; private set; }
        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            var perfilEntradaThormin = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "THORmin");
            var profundidades = ObterProfundidades();
            var thorMin = PerfisSaída.Perfis.Single(p => p.Mnemonico == "THORmin");
            var index = PerfisSaída.Perfis.IndexOf(thorMin);
            var gfrat = PerfisSaída.Perfis.Single(p => p.Mnemonico == "GFRAT_σh");
            gfrat.Clear();
            var nomePerfil = thorMin.Nome;
            var idPerfil = thorMin.Id;

            if (perfilEntradaThormin == null)
            {
                switch (MetodologiaTHORmin)
                {
                    case MetodologiaCálculoTensãoHorizontalMenorEnum.ModeloElástico:
                        thorMin = CalcularModeloElástico(profundidades, thorMin.Nome);
                        break;
                    case MetodologiaCálculoTensãoHorizontalMenorEnum.NormalizaçãoLDA:
                        thorMin = CalcularNormalizaçãoLDA(profundidades, thorMin.Nome);
                        break;
                    case MetodologiaCálculoTensãoHorizontalMenorEnum.NormalizaçãoPP:
                        thorMin = CalcularNormalizaçãoPressãoDePoros(profundidades, thorMin.Nome);
                        break;
                    case MetodologiaCálculoTensãoHorizontalMenorEnum.K0Acompanhamento:
                    case MetodologiaCálculoTensãoHorizontalMenorEnum.K0:
                        thorMin = CalcularModeloK0(thorMin.Nome);
                        break;
                }

                if (Depleção != null)
                    thorMin = CalcularDepleção(thorMin, thorMin, thorMin.Nome);
            }
            else
            {
                if (Depleção != null)
                    thorMin = CalcularDepleção(perfilEntradaThormin, thorMin, thorMin.Nome);
                else
                {
                    thorMin = perfilEntradaThormin;
                    thorMin.EditarNome(nomePerfil);
                    thorMin.SetNewId();
                }
            }

            if (idPerfil != ObjectId.Empty)
                thorMin.EditarId(idPerfil);

            CalcularGradiente(thorMin, gfrat);

            CalcularTensãoMaior(thorMin);
            PerfisSaída.Perfis.RemoveAt(index);
            PerfisSaída.Perfis.Add(thorMin);

        }

        private void CalcularTensãoMaior(PerfilBase thorMinOriginal)
        {
            var depletado = Depleção != null;
            var perfisEntrada = new List<PerfilBase>(PerfisEntrada.Perfis) { thorMinOriginal };
            var calculoTensaoMaior = new CálculoTensãoHorizontalMaior(perfisEntrada, PerfisSaída.Perfis, RelaçãoTensão, Breakout, ConversorProfundidade, Geometria, Litologia, FraturasTrechosVerticais, MetodologiaTHORmax);
            calculoTensaoMaior.Calcular(thorMinOriginal, depletado, Depleção, ProfundidadeEValorThorMinSemDepleção);
        }

        private void CalcularGradiente(PerfilBase thorMin, PerfilBase gfrat)
        {
            var pontosDeThorMin = thorMin.GetPontos().ToArray();

            //TODO Parallel para performance
            //Parallel.For(0, pontosDeThorMin.Length, index =>
            for (int index = 0; index < pontosDeThorMin.Length; index++)
            {
                var ponto = pontosDeThorMin[index];
                gfrat.AddPonto(ConversorProfundidade, ponto.Pm, ponto.Pv, OperaçõesDeConversão.ObterGradiente(ponto.Pv.Valor, ponto.Valor), TipoProfundidade.PM, OrigemPonto.Calculado);
            }
        }

        private PerfilBase CalcularDepleção(PerfilBase thorMin, PerfilBase thorMinSaída, string nome)
        {
            if (Depleção == null)
                return null;

            var depleção = new CálculoDepleção(thorMin, Depleção, ConversorProfundidade, Geometria, Litologia, PerfisEntrada.Perfis);
            var perfil = depleção.Calcular(nome);

            if (perfil == null || perfil.ContémPontos() == false)
                return null;

            thorMinSaída = perfil;

            ProfundidadeEValorThorMinSemDepleção = depleção.profundidadeValorThorMinSemDepleção;

            return thorMinSaída;
        }

        private PerfilBase CalcularModeloK0(string nome)
        {
            CálculoK0 calculoK0;
            switch (MetodologiaTHORmin)
            {
                case MetodologiaCálculoTensãoHorizontalMenorEnum.K0Acompanhamento:
                    calculoK0 = new CálculoK0(PerfisEntrada.Perfis, ParâmetrosLotDTO, ConversorProfundidade, Geometria, Litologia, true);
                    break;

                case MetodologiaCálculoTensãoHorizontalMenorEnum.K0:
                    calculoK0 = new CálculoK0(PerfisEntrada.Perfis, ParâmetrosLotDTO, ConversorProfundidade, Geometria, Litologia, false);
                    break;

                default:
                    return null;
            }

            //calcular perfil K0
            var resultadoK0 = calculoK0.Calcular();

            var k0Perfil = PerfisSaída.Perfis.First(p => p.Mnemonico == "K0");

            foreach (var item in resultadoK0)
            {
                k0Perfil.AddPontoEmPm(ConversorProfundidade, item.Item1, item.Item2, TipoProfundidade.PM, OrigemPonto.Calculado);
            }

            PerfisEntrada.Perfis.Add(k0Perfil);
            
            //calcular tensão horizontal menor, modelo K0
            var calculo = new CálculoTensãoHorizontalMenorK0(PerfisEntrada.Perfis, ConversorProfundidade, Geometria, Litologia);
            var resultado = calculo.Calcular();

            var perfil = PerfisFactory.Create("THORmin", nome, ConversorProfundidade, Litologia);

            foreach (var item in resultado)
            {
                perfil.AddPontoEmPm(ConversorProfundidade, item.Item1, item.Item2, TipoProfundidade.PM, OrigemPonto.Calculado);
            }

           return perfil;
        }

        private PerfilBase CalcularNormalizaçãoPressãoDePoros(double[] profundidades, string nome)
        {
            var calculo = new CálculoTensãoHorizontalMenorNormalizaçãoPressãoPoros(PerfisEntrada.Perfis, ParâmetrosLotDTO, ConversorProfundidade, Geometria, Litologia, Coeficiente);
            var perfil = calculo.Calcular(nome);

            return perfil;
        }

        private PerfilBase CalcularNormalizaçãoLDA(double[] profundidades, string nome)
        {
            var tvert = PerfisEntrada.Perfis.Single(p => p.Mnemonico == "TVERT");

            var normalizaçãoLda = new CálculoTensãoHorizontalMenorNormalizaçãoLDA(tvert, ParâmetrosLotDTO, ConversorProfundidade, Geometria, DadosGerais, Litologia, Coeficiente);
            var perfil = normalizaçãoLda.Calcular(profundidades, nome);
            
            return perfil;
        }

        private PerfilBase CalcularModeloElástico(double[] profundidades, string nome)
        {
            var modeloElástico = new CálculoTensãoHorizontalMenorModeloElástico(PerfisEntrada.Perfis, ConversorProfundidade, Litologia, Geometria);
            var perfil = modeloElástico.Calcular(profundidades, nome);
            return perfil;
        }

        private double[] ObterProfundidades()
        {
            var sincronizadorProfundidades = new SincronizadorProfundidades(PerfisEntrada.Perfis, ConversorProfundidade, Litologia, GrupoCálculo.Tensões);
            return sincronizadorProfundidades.GetProfundidadeDeReferência();
        }
        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        public void InvalidatePerfilDepleção(ObjectId idPerfil)
        {
            if (Depleção != null)
            {
                Depleção.Invalidate(idPerfil);
            }
        }

        #endregion

        #region Map
        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<CálculoTensões>(calc =>
            {
                calc.AutoMap();
                calc.UnmapMember(p => p.DadosGerais);
                calc.UnmapMember(p => p.Geometria);
                calc.UnmapMember(p => p.TensãoMenor);
                calc.MapMember(p => p.MetodologiaTHORmax).SetSerializer(new EnumSerializer<MetodologiaCálculoTensãoHorizontalMaiorEnum>(BsonType.String)); 
                calc.MapMember(p => p.MetodologiaTHORmin).SetSerializer(new EnumSerializer<MetodologiaCálculoTensãoHorizontalMenorEnum>(BsonType.String)); 
                calc.MapMember(p => p.ParâmetrosLotDTO);
                calc.MapMember(p => p.Breakout);
                calc.MapMember(p => p.RelaçãoTensão);
                calc.MapMember(p => p.FraturasTrechosVerticais);
            });

        }
        #endregion
    }
}
