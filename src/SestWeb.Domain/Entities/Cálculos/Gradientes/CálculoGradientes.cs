using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.DTOs.Cálculo.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Factory;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes
{
    public class CálculoGradientes : Cálculo, ICálculoGradientes
    {
        public DadosMalha Dadosmalha { get; private set; }
        public EntradasColapsosDTO EntradaColapsos { get; private set; }
        public List<InformaçãoDTO> Informações { get; private set; } = new List<InformaçãoDTO>();
        #region Constructor

        private CálculoGradientes(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, DadosMalha dadosMalha, EntradasColapsosDTO entradasColapsos) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            Dadosmalha = dadosMalha;
            EntradaColapsos = entradasColapsos;
        }

        public static void RegisterCálculoGradientesCtor()
        {
            CálculoGradientesFactory.RegisterCálculoGradientesCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, dadosMalha, entradasColapsos) => new CálculoGradientes(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, dadosMalha, entradasColapsos));
        }

        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            var sincronizadorDeProfundidades = new SincronizadorProfundidades(PerfisEntrada.Perfis, ConversorProfundidade, Litologia, GrupoCálculo.Gradientes);
            var profundidades = sincronizadorDeProfundidades.GetProfundidadeDeReferência();

            var GQuebra = PerfisSaída.Perfis.Single(p => p.Mnemonico == "GQUEBRA");
            var GCOLI = PerfisSaída.Perfis.Single(p => p.Mnemonico == "GCOLI");
            var GCOLS = PerfisSaída.Perfis.Single(p => p.Mnemonico == "GCOLS");
            var angat = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "ANGAT");
            var biot = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "BIOT");
            var ucs = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "UCS");
            var poisson = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "POISS");
            var tensãoMaior = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "THORmax");
            var tensãoMenor = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "THORmin");
            var azimuteMenor = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "AZTHmin");
            var resistênciaTração = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "RESTR");
            var gPoro = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "GPORO" || p.Mnemonico == "GPPI");
            var gSobrecarga = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "GSOBR");
            var diamBroca = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "DIAM_BROCA");
            var coesao = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "COESA");
            var ks = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "KS");
            var poro = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "PORO");
            var perm = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "PERM");
            var young = PerfisEntrada.Perfis.Find(p => p.Mnemonico == "YOUNG");
            var pontosTrajetória = ConversorProfundidade.GetPontos();

            Parallel.For(0, profundidades.Length, i =>
            //for (int i = 0; i < profundidades.Length; i++)
            {
                try
                {

                    PontoDeTrajetória pontoTraj;
                    var pontoTrajetória = ConversorProfundidade.TryGetPonto(new Profundidade(profundidades[i]), out pontoTraj);

                    PontoLitologia pontoLito;
                    var encontrarPontoLito = Litologia.TryGetLitoPontoEmPm(ConversorProfundidade, pontoTraj.Pm, out pontoLito);

                    if (encontrarPontoLito)
                    {                       
                        //Se for do grupo evaporitos todos os gradientes são 0
                        if (pontoLito.TipoRocha.Grupo == GrupoLitologico.Evaporitos)
                        {
                            GQuebra.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, 0, TipoProfundidade.PV, OrigemPonto.Calculado);
                            GCOLI.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, 0, TipoProfundidade.PV, OrigemPonto.Calculado);
                            GCOLS.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, 0, TipoProfundidade.PV, OrigemPonto.Calculado);
                            return;
                        }
                    }

                    var entradas = new EntradasColapsos
                    {
                        AnguloAtrito = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, angat),
                        Biot = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, biot),
                        AreaPlastificada = EntradaColapsos.AreaPlastificada,
                        Ucs = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, ucs),
                        Poisson = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, poisson),
                        TensaoMaior = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, tensãoMaior),
                        TensaoMenor = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, tensãoMenor),
                        AzimuteMenor = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, azimuteMenor),
                        ResistenciaTracao = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, resistênciaTração),
                        PressaoPoros = ConverterGradienteParaTensão(ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, gPoro), pontoTraj.Pv.Valor),
                        TensaoVertical = ConverterGradienteParaTensão(ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, gSobrecarga), pontoTraj.Pv.Valor),
                        Azimute = pontoTraj.Azimute,
                        DiametroPoco = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, diamBroca),
                        EhFluidoPenetrante = EntradaColapsos.FluidoPenetrante.Value,
                        Inclinacao = pontoTraj.Inclinação,
                        Pv = pontoTraj.Pv.Valor,
                        Pw = 0,
                        Coesao = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, coesao)
                    };

                    if (EntradaColapsos.PoroElastico != null)
                    {
                        entradas.DadosEntradaModeloPoroElastico = PreencherDadosPoroElastico(EntradaColapsos.PoroElastico, entradas, ks, poro, perm, young, pontoTraj);
                    }


                    var result = CálculoPressões.Calcular(Dadosmalha, entradas, EntradaColapsos.TipoCritérioRuptura);

                    if (EntradaColapsos.CalcularFraturasColapsosSeparadamente.Value)
                    {
                        GQuebra.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Fraturas.FS), TipoProfundidade.PV, OrigemPonto.Calculado);
                        GCOLI.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Colapsos.CI), TipoProfundidade.PV, OrigemPonto.Calculado);
                        GCOLS.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Colapsos.CS), TipoProfundidade.PV, OrigemPonto.Calculado);
                    }
                    else
                    {
                        GQuebra.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Fraturas.FS), TipoProfundidade.PV, OrigemPonto.Calculado);

                        if (result.Colapsos.CI < result.Fraturas.FI)
                            GCOLI.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Fraturas.FI), TipoProfundidade.PV, OrigemPonto.Calculado);
                        else
                            GCOLI.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Colapsos.CI), TipoProfundidade.PV, OrigemPonto.Calculado);

                        if (result.Colapsos.CS > result.Fraturas.FS)
                            GCOLS.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Fraturas.FS), TipoProfundidade.PV, OrigemPonto.Calculado);
                        else
                            GCOLS.AddPontoEmPv(ConversorProfundidade, pontoTraj.Pv, ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Colapsos.CS), TipoProfundidade.PV, OrigemPonto.Calculado);
                    }

                    if (result.Colapsos.NãoConvergiuCS || result.Colapsos.NãoConvergiuCI)
                    {
                        var info = new InformaçãoDTO
                        {
                            Pm = pontoTraj.Pm.Valor,
                            Pv = pontoTraj.Pv.Valor,
                            GCOLI = ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Colapsos.CI),
                            GCOLS = ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Colapsos.CS),
                            GQUEBRA = ConverterTensãoParaGradientes(pontoTraj.Pv.Valor, result.Fraturas.FS),
                            Obs = PreencherObservação(result.Colapsos.FPCI, result.Colapsos.FPCS, result.Colapsos.APCI, result.Colapsos.APCS, this.Dadosmalha.PorcentagemMalha, result.Colapsos.NãoConvergiuCI, result.Colapsos.NãoConvergiuCS)
                        };

                        Informações.Add(info);
                    }
                }
                catch (CálculoGradientesException)
                {
                    return;
                }
            //}
            });

            //Após finalizar o calculo, ordena a lista de erros, se houver itens
            if (Informações.Any())
                Informações = Informações.OrderBy(i => i.Pm).ToList();

            if (EntradaColapsos.HabilitarFiltroAutomatico == true)
            {
                Parallel.ForEach(PerfisSaída.Perfis, AplicarDesvioMáximo);
            }
        }

        private string PreencherObservação(double fPCI, double fPCS, double aPCI, double aPCS, double porcentagemMalha, bool naoConvergiuCI, bool naoConvergiuCS)
        {
            string obs = string.Empty;

            if (naoConvergiuCI)
            {
                if (fPCI <= 1)
                    obs += "Área plastificada para GCOLI = 0 \n";

                if (aPCI > 0 && aPCI <= 100)
                    obs += "Área plastificada para GCOLI = " + aPCI.ToString() + " \n";

                if (aPCI > 100 && aPCI < porcentagemMalha)
                    obs += "Área plastificada para GCOLI > 100% \n";

                if (aPCI > porcentagemMalha)
                    obs += "Área plastificada para GCOLI ultrapassa o diâmetro externo da malha \n";
            }

            if (naoConvergiuCS)
            {
                if (fPCS <= 1)
                    obs += "Área plastificada para GCOLS = 0 \n";

                if (aPCS > 0 && aPCS <= 100)
                    obs += "Área plastificada para GCOLS = " + aPCS.ToString() + " \n";

                if (aPCS > 100 && aPCS < porcentagemMalha)
                    obs += "Área plastificada para GCOLS > 100% \n";

                if (aPCS > porcentagemMalha)
                    obs += "Área plastificada para GCOLS ultrapassa o diâmetro externo da malha \n";
            }

            return obs;
        }

        private DadosEntradaModeloPoroElastico PreencherDadosPoroElastico(PoroelasticoDTO poroElastico, EntradasColapsos entradas, PerfilBase ks, PerfilBase poro, PerfilBase perm, PerfilBase young, PontoDeTrajetória pontoTraj)
        {
            var dados = new DadosEntradaModeloPoroElastico
            {
                CoefDifusaoSoluto = poroElastico.CoeficienteDifusãoSoluto,
                Biot = entradas.Biot,
                CoefInchamento = poroElastico.CoeficienteInchamento,
                CoefReflexao = poroElastico.CoeficienteReflexão,
                Kf = poroElastico.Kf,
                ConcentracaoSolutoFluidoPerfuracao = poroElastico.ConcentraçãoSolFluidoPerfuração,
                ConcentracaoSolutoFluidoRocha = poroElastico.ConcentraçãoSolutoRocha,
                DensidadeFluidoFormacao = poroElastico.DensidadeFluidoFormação,
                DiametroPoco = entradas.DiametroPoco,
                DifusividadeTermica = poroElastico.DifusidadeTérmica,
                EfeitoFisicoQuimico = EntradaColapsos.IncluirEfeitosFísicosQuímicos.Value,
                EfeitoTermico = EntradaColapsos.IncluirEfeitosTérmicos.Value,
                ExpansaoTermicaFluidoPoros = poroElastico.ExpansãoTérmicaVolumeFluidoPoros,
                ExpansaoTermicaRocha = poroElastico.ExpansãoTérmicaRocha,
                MatrizTensorDeTensoes = new MatrizTensorDeTensoes(entradas.Inclinacao, entradas.Azimute, entradas.TensaoMenor, entradas.TensaoMaior, entradas.TensaoVertical, entradas.AzimuteMenor),
                Poisson = entradas.Poisson,
                PressaoPoros = entradas.PressaoPoros,
                Pv = entradas.Pv,
                Pw = 0,
                TemperaturaFormacao = poroElastico.TemperaturaFormação,
                TemperaturaPoco = poroElastico.TemperaturaPoço,
                Tempo = EntradaColapsos.Tempo.Value,
                TensaoMaior = entradas.TensaoMaior,
                TensaoMenor = entradas.TensaoMenor,
                TensaoVertical = entradas.TensaoVertical,
                TipoSal = PreencherTipoSal(poroElastico.TipoSal),
                Viscosidade = poroElastico.Viscosidade,
                Young = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, young),
                Ks = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, ks),
                TemperaturaFormacao_FisicoQuimico = poroElastico.TemperaturaFormaçãoFisicoQuimica,
                Porosidade = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, poro),
                Permeabilidade = ObtemValorPerfilEntrada(pontoTraj.Pm, pontoTraj.Pv.Valor, perm)

            };
            return dados;
        }

        private TipoSal PreencherTipoSal(string tipoSal)
        {
            switch (tipoSal)
            {
                case "NaCl":
                    return TipoSal.NaCl;
                case "CaCl2":
                    return TipoSal.CaCL2;
                case "KCl":
                    return TipoSal.KCL;
                default:
                    return TipoSal.NaCl;
            }
        }

        private double ObtemValorPerfilEntrada(Profundidade pm, double pv, PerfilBase perfil)
        {
            Ponto ponto;

            if (perfil.Mnemonico == "DIAM_BROCA")
            {
                var pontos = perfil.GetPontos();


                if (perfil.ContémPontoNoPm(pm) && pm.Valor >= pontos.First().Pm.Valor)
                {
                    perfil.TryGetPontoEmPm(ConversorProfundidade, pm, out ponto, GrupoCálculo.Gradientes);

                    if (ponto != null)
                        return ponto.Valor;
                }
                else
                {
                    //var profundidade = pontos.Where(p => p.Pv != null).Last().Pv;
                    var lastPontoComPv = pontos.Last(p => p.Pv != null);
                    var lastPv = lastPontoComPv.Pv;

                    if (lastPv != null && pv > lastPv.Valor)
                    {
                        return pontos.Last().Valor; // Usar o last ou o lastPontoComPv?                            
                    }

                    var pontoAnterior = pontos[0];
                    foreach (var p in pontos.Skip(1))
                    {
                        if (pv > pontoAnterior.Pv.Valor && pv < p.Pv.Valor)
                        {
                            return pontoAnterior.Valor;
                        }

                        pontoAnterior = p;
                    }
                }
            }

            if (!perfil.TryGetPontoEmPm(ConversorProfundidade, pm, out ponto))
                throw new CálculoGradientesException();
            else
                return ponto.Valor;

        }

        private double ConverterTensãoParaGradientes(double pv, double fS)
        {
            var fatorConversão = 5.8674;
            var gradiente = (fS * fatorConversão) / pv;
            return gradiente;
        }

        private double ConverterGradienteParaTensão(double gradiente, double pv)
        {
            var fatorConversão = 5.8674;
            var tensão = (gradiente * pv) / fatorConversão;
            return tensão;
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        private void AplicarDesvioMáximo(PerfilBase perfil)
        {
            const double DESVIO_MAXIMO = 0.1;

            var valorAnterior = 0.0;
            var pontos = perfil.GetPontos();
            var novosPontosPM = new List<Profundidade>();
            var novosPontosValor = new List<double>();

            if (pontos.Count <= 1) return;

            for (var index = 0; index < pontos.Count; index++)
            {
                if (index == 0)
                {
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(pontos[index].Valor);
                    valorAnterior = pontos[index].Valor;
                    continue;
                }

                //se for sal, acrescento o valor zero e segue em frente
                if (pontos[index].TipoRocha == null || pontos[index].TipoRocha.Grupo == GrupoLitologico.Evaporitos)
                {
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(pontos[index].Valor);
                    continue;
                }

                //seguindo, se o ponto anterior for sal, acrescento o valor como está e sigo em frente
                if (pontos[index - 1].TipoRocha == null || pontos[index - 1].TipoRocha.Grupo == GrupoLitologico.Evaporitos)
                {
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(pontos[index].Valor);
                    valorAnterior = pontos[index].Valor;
                    continue;
                }

                var diferença = pontos[index].Valor - valorAnterior;
                var diferençaAbsoluta = Math.Abs(diferença);

                if (!(diferençaAbsoluta > DESVIO_MAXIMO))
                {
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(pontos[index].Valor);
                    valorAnterior = pontos[index].Valor;
                    continue;
                }

                if (diferença > 0)
                {
                    var valor = Math.Round(valorAnterior + DESVIO_MAXIMO, 2);
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(valor);
                    valorAnterior = valor;
                }
                else if (diferença < 0)
                {
                    var valor = Math.Round(valorAnterior - DESVIO_MAXIMO, 2);
                    novosPontosPM.Add(pontos[index].Pm);
                    novosPontosValor.Add(valor);
                    valorAnterior = valor;
                }
            }

            perfil.Clear();
            perfil.AddPontosEmPm(ConversorProfundidade, novosPontosPM, novosPontosValor, TipoProfundidade.PM, OrigemPonto.Calculado, Litologia);
        }


        #endregion


        #region Map
        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<CálculoGradientes>(calc =>
            {
                calc.AutoMap();
                calc.MapMember(p => p.EntradaColapsos);
                calc.MapMember(p => p.Dadosmalha);
                calc.UnmapMember(p => p.Informações);
            });

        }
        #endregion
    }


}
