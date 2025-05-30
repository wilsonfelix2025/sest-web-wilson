using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Composto;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Factory;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas
{
    public class CálculoPropriedadesMecânicas : Cálculo, ICálculoPropriedadesMecânicas
    {
        #region Constructor

        private CálculoPropriedadesMecânicas(string nome, IList<ICorrelação> listaCorrelação, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos, ICorrelaçãoFactory correlaçãoFactory, Geometria geometria, DadosGerais dadosGerais, List<TrechoDTO> trechosFront, string correlaçãoDoCálculo, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões, List<RegiãoDTO> regiõesFront) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            CorrelaçãoDoCálculo = correlaçãoDoCálculo;
            ListaCorrelação = listaCorrelação;
            Trechos = trechos;
            TrechosFront = trechosFront;
            CorrelaçãoFactory = correlaçãoFactory;
            Geometria = geometria;
            DadosGerais = dadosGerais;
            ListaNomesCorrelação = new List<string>();
            if (ListaCorrelação != null)
                ListaNomesCorrelação.AddRange(ListaCorrelação.Select(s => s.Nome));
            Regiões = regiões;
            RegiõesFront = regiõesFront;
            ZerarPerfisSaída();
        }

        public static void RegisterCálculoPerfisCtor()
        {
            CálculoPropriedadesMecânicasFactory.RegisterCálculoPropriedadesMecânicasCtor((nome, correlação, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, trechos, correlaçãoFactory, geometria, dadosGerais, trechosFront, correlaçãoDoCálculo, regiões, regiõesFront) => new CálculoPropriedadesMecânicas(nome, correlação, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia,  trechos, correlaçãoFactory, geometria, dadosGerais, trechosFront, correlaçãoDoCálculo, regiões, regiõesFront));
        }

        #endregion

        #region Properties

        private DadosGerais DadosGerais { get; }
        private Geometria Geometria { get; }
        public IList<ICorrelação> ListaCorrelação { get; }
        public List<string> ListaNomesCorrelação { get; private set; }
        public IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> Trechos { get; private set; }
        public List<TrechoDTO> TrechosFront { get; private set; }
        private ICorrelaçãoFactory CorrelaçãoFactory { get; }
        public string CorrelaçãoDoCálculo { get; private set; }
        public List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> Regiões { get; private set; }
        public List<RegiãoDTO> RegiõesFront { get; private set; }

        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            var variáveis = ObterVariáveisDoCálculo(chamadaPelaPipeline);
            var cálculoComposto = new CálculoComposto(Nome, ListaCorrelação, PerfisEntrada, variáveis, Litologia, GrupoCálculo, ConversorProfundidade, CorrelaçãoFactory, PerfisSaída, Geometria, DadosGerais, CorrelaçãoDoCálculo, Regiões, Trechos, true);
            CorrelaçãoDoCálculo = cálculoComposto.Correlação.Expressão.Bruta;
            cálculoComposto.Execute(chamadaPelaPipeline);
        }

        private Dictionary<string, double> ObterVariáveisDoCálculo(ICorrelação corr)
        {
            var variáveis = new Dictionary<string, double>();

                foreach (var varName in corr.Expressão.Variáveis.Names)
                {
                    if (!variáveis.ContainsKey(varName))
                    {
                        if (corr.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                        {
                            variáveis.Add(varName, varValue);
                        }
                    }
                }
                     
            return variáveis;
        }
       
        private Dictionary<string, double> ObterVariáveisDoCálculo(bool chamadaPelaPipeline)
        {
            if (chamadaPelaPipeline)
                return null;

            var variáveis = new Dictionary<string, double>();

            foreach (var corr in ListaCorrelação)
            {
                foreach (var varName in corr.Expressão.Variáveis.Names)
                {
                    if (!variáveis.ContainsKey(varName))
                    {
                        if (corr.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                        {
                            variáveis.Add(varName, varValue);
                        }
                    }
                }
            }

            foreach (var corr in Regiões)
            {
                foreach (var varName in corr.Angat.Expressão.Variáveis.Names)
                {
                    if (!variáveis.ContainsKey(varName))
                    {
                        if (corr.Angat.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                        {
                            variáveis.Add(varName, varValue);
                        }
                    }
                }
                foreach (var varName in corr.Coesa.Expressão.Variáveis.Names)
                {
                    if (!variáveis.ContainsKey(varName))
                    {
                        if (corr.Coesa.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                        {
                            variáveis.Add(varName, varValue);
                        }
                    }
                }
                foreach (var varName in corr.Restr.Expressão.Variáveis.Names)
                {
                    if (!variáveis.ContainsKey(varName))
                    {
                        if (corr.Restr.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                        {
                            variáveis.Add(varName, varValue);
                        }
                    }
                }
                foreach (var varName in corr.Ucs.Expressão.Variáveis.Names)
                {
                    if (!variáveis.ContainsKey(varName))
                    {
                        if (corr.Ucs.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                        {
                            variáveis.Add(varName, varValue);
                        }
                    }
                }
            }

            if (Trechos != null)
            {
                foreach (var trecho in Trechos)
                {
                    foreach (var varName in trecho.Biot.Expressão.Variáveis.Names)
                    {
                        if (!variáveis.ContainsKey(varName))
                        {
                            if (trecho.Biot.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                            {
                                variáveis.Add(varName, varValue);
                            }
                        }
                    }

                    foreach (var varName in trecho.Ucs.Expressão.Variáveis.Names)
                    {
                        if (!variáveis.ContainsKey(varName))
                        {
                            if (trecho.Ucs.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                            {
                                variáveis.Add(varName, varValue);
                            }
                        }
                    }

                    foreach (var varName in trecho.Coesa.Expressão.Variáveis.Names)
                    {
                        if (!variáveis.ContainsKey(varName))
                        {
                            if (trecho.Coesa.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                            {
                                variáveis.Add(varName, varValue);
                            }
                        }
                    }

                    foreach (var varName in trecho.Angat.Expressão.Variáveis.Names)
                    {
                        if (!variáveis.ContainsKey(varName))
                        {
                            if (trecho.Angat.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                            {
                                variáveis.Add(varName, varValue);
                            }
                        }
                    }

                    foreach (var varName in trecho.Restr.Expressão.Variáveis.Names)
                    {
                        if (!variáveis.ContainsKey(varName))
                        {
                            if (trecho.Restr.Expressão.Variáveis.TryGetValue(varName, out double varValue))
                            {
                                variáveis.Add(varName, varValue);
                            }
                        }
                    }
                }
            }

            return variáveis;
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Map
        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<CálculoPropriedadesMecânicas>(calc =>
            {
                calc.AutoMap();
                calc.UnmapMember(p => p.ListaCorrelação);
                calc.MapMember(p => p.Trechos);
                calc.UnmapMember(p => p.Regiões);
                calc.MapMember(p => p.ListaNomesCorrelação);
                calc.MapMember(p => p.CorrelaçãoDoCálculo);
                calc.MapMember(p => p.TrechosFront);
                calc.MapMember(p => p.RegiõesFront);
            });

        }
        #endregion
    }
}
