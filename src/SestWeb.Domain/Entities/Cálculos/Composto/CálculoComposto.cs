using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Composto.Correlação;
using SestWeb.Domain.Entities.Cálculos.Perfis;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Correlações.NormalizadorCorrelação;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Composto
{
    public class CálculoComposto : ICálculoComposto
    {

        public CálculoComposto(string nome, IList<ICorrelação> correlações, IPerfisEntrada perfisDeEntrada, Dictionary<string, double> parâmetros,
            IList<TrechoCálculo> trechosDeCálculos, ILitologia litologia, GrupoCálculo grupoDeCálculo, IConversorProfundidade trajetória
            , ICorrelaçãoFactory correlaçãoFactory, IPerfisSaída perfisDeSaída, Geometria geometria, DadosGerais dadosGerais
            , string correlaçãoDoCálculo, bool ignorarValoresNegativos)
        {
            Nome = nome;
            GrupoCálculo = grupoDeCálculo;
            PerfisSaída = perfisDeSaída;
            PerfisEntrada = perfisDeEntrada;
            Geometria = geometria;
            DadosGerais = dadosGerais;
            ListaCorrelação = correlações;
            TrechosDeCálculos = trechosDeCálculos;
            CorrelaçãoFactory = correlaçãoFactory;
            Litologia = litologia;
            ConversorProfundidade = trajetória;
            var variáveis = ObterVariáveisDoCálculo(nome, parâmetros, trechosDeCálculos);
            PerfisNãoRestritivosÀSincroniaDeProfundidades = new List<string> { "RHOG" };
            PerfisOpcionais = new List<string> { "RHOG", "DTS", "DTC", "VP" };
            Correlação = correlaçãoDoCálculo != string.Empty ? CriarCorrelaçãoAPartirDaExpressãoPronta(correlaçãoDoCálculo) : ExtrairCorrelação(nome, correlações, trechosDeCálculos, grupoDeCálculo, variáveis);
            Variáveis = variáveis ?? new Dictionary<string, double>();
            IgnorarValoresNegativos = ignorarValoresNegativos;
        }

        public CálculoComposto(string nome, IList<ICorrelação> correlações, IPerfisEntrada perfisDeEntrada, Dictionary<string, double> parâmetros
            , ILitologia litologia, GrupoCálculo grupoDeCálculo, IConversorProfundidade trajetória
            , ICorrelaçãoFactory correlaçãoFactory, IPerfisSaída perfisDeSaída, Geometria geometria, DadosGerais dadosGerais
            , string correlaçãoDoCálculo, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões
            , IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos, bool ignorarValoresNegativos)
        {
            Nome = nome;
            GrupoCálculo = grupoDeCálculo;
            PerfisSaída = perfisDeSaída;
            PerfisEntrada = perfisDeEntrada;
            Geometria = geometria;
            DadosGerais = dadosGerais;
            ListaCorrelação = correlações;
            TrechosDeCálculoPropMec = trechos;
            CorrelaçãoFactory = correlaçãoFactory;
            Litologia = litologia;
            ConversorProfundidade = trajetória;
            PerfisNãoRestritivosÀSincroniaDeProfundidades = new List<string> { };
            PerfisOpcionais = new List<string> { };
            Correlação = correlaçãoDoCálculo != string.Empty ? CriarCorrelaçãoAPartirDaExpressãoPronta(correlaçãoDoCálculo) : MontarCorrelação(nome, regiões, trechos, parâmetros, correlações, correlaçãoFactory);
            Variáveis = parâmetros ?? ObterVariáveisDoCálculo(Correlação);
            IgnorarValoresNegativos = ignorarValoresNegativos;
        }

        private Dictionary<string, double> ObterVariáveisDoCálculo(ICorrelação corr)
        {
            var variáveis = new Dictionary<string, double>();

            if (corr == null)
                return variáveis;

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
        #region Properties
        public DadosGerais DadosGerais { get; private set; }
        public Geometria Geometria { get; private set; }
        private bool _primeiraExecução = false;
        private List<string> PerfisNãoRestritivosÀSincroniaDeProfundidades { get; }
        private List<string> PerfisOpcionais { get; }
        private ICorrelaçãoFactory CorrelaçãoFactory { get; }
        public IList<TrechoCálculo> TrechosDeCálculos { get; set; }
        public IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> TrechosDeCálculoPropMec { get; set; }
        public IList<ICorrelação> ListaCorrelação { get; }
        public ObjectId Id { get; }
        public string Nome { get; }
        public GrupoCálculo GrupoCálculo { get; }
        public IPerfisEntrada PerfisEntrada { get; }
        public IPerfisSaída PerfisSaída { get; }
        public IConversorProfundidade ConversorProfundidade { get; }
        public ILitologia Litologia { get; }
        public ICorrelação Correlação { get; }
        public double[] ProfundidadeDeReferência { get; private set; }
        public int QtdPontos => ProfundidadeDeReferência?.Length ?? 0;
        public Dictionary<string, PerfilVetorial> PerfisDeSaídaVetorial { get; private set; }
        public Dictionary<string, PerfilVetorial> PerfisDeEntradaVetorial { get; private set; }
        public Dictionary<string, double> Variáveis { get; set; }
        public bool PerfisEntradaPossuemPontos => throw new NotImplementedException();
        public bool TemSaídaZerada => throw new NotImplementedException();
        public bool IgnorarValoresNegativos { get; set; } = false;
        #endregion

        public void AtualizarPvsSaídas(IConversorProfundidade conversor)
        {
            throw new NotImplementedException();
        }

        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public bool ContémPerfilEntradaPorId(string perfilOld)
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }

        public void Execute(bool chamadaPelaPipeline)
        {
            try
            {
                using (var parserWrapper = new MuParserNetWrapper($"Parser_Cálculo_{Nome}"))
                {
                    var parser = parserWrapper.Parser;
                    ProfundidadeDeReferência = ObterProfundidadeDeRefência(PerfisEntrada.Perfis, GrupoCálculo);
                    RemoverPerfisDoDomínioDasEntradas(); //perfis dominio
                    AtualizarPerfisDoCálculo();
                    InicializarParserDeCálculo(parser);
                    Calcular(parser);
                    CarregarValoresCalculados(_primeiraExecução);

                    if (this.GrupoCálculo == GrupoCálculo.Perfis)
                        CarregarPerfisLitológicosComoPerfisSaida(PerfisEntrada);

                    if (_primeiraExecução == false)
                    {
                        _primeiraExecução = true;
                    }
                }
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        private void CarregarPerfisLitológicosComoPerfisSaida(IPerfisEntrada perfisEntrada)
        {
            var rhog = perfisEntrada.Perfis.Find(f => f.Mnemonico == "RHOG");
            var dtmc = perfisEntrada.Perfis.Find(f => f.Mnemonico == "DTMC");

            if (rhog != null)
                PerfisSaída.Perfis.Add(rhog);

            if (dtmc != null)
                PerfisSaída.Perfis.Add(dtmc);
        }

        public List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        public List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public void ZerarPerfisSaídaAfetados(PerfilBase perfilOld)
        {
            throw new NotImplementedException();
        }

        private ICorrelação ExtrairCorrelação(string nome, IList<ICorrelação> correlações, IList<TrechoCálculo> trechosDeCálculos, GrupoCálculo grupoDeCálculo, Dictionary<string, double> variáveis)
        {
            var expressão = ObterExpressãoDoCálculo(correlações, trechosDeCálculos);
            var origemDaCorrelação = ObterOrigemDaCorrelação(grupoDeCálculo);
            var validationCorrelação = CorrelaçãoFactory.CreateCorrelação($"{nome}_Correlação", "Cálculo", "Cálculo", "CálculoPerfis", origemDaCorrelação.ToString(), expressão, out ICorrelação correlaçãoCriada);
            return correlaçãoCriada;
        }

        private ICorrelação CriarCorrelaçãoAPartirDaExpressãoPronta(string correlaçãoDoCálculo)
        {
            var origemDaCorrelação = ObterOrigemDaCorrelação(GrupoCálculo);
            CorrelaçãoFactory.CreateCorrelação($"{Nome}_Correlação", "Cálculo", "Cálculo", "CálculoPerfis", origemDaCorrelação.ToString(), correlaçãoDoCálculo, out ICorrelação correlaçãoCriada);
            return correlaçãoCriada;
        }

        private Dictionary<string, double> ObterVariáveisDoCálculo(string nome, Dictionary<string, double> parâmetros, IList<TrechoCálculo> trechosDeCálculos = null)
        {
            var variáveis = new Dictionary<string, double>();
            if (parâmetros != null && parâmetros.Count > 0)
            {
                foreach (var parâmetro in parâmetros)
                {
                    if (!variáveis.ContainsKey(parâmetro.Key))
                    {
                        // TODO (RCM) Identificar variáveis com nome idêntico
                        variáveis.Add(parâmetro.Key, parâmetro.Value);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"O Cálculo {nome} possui duas variáveis com o mesmo nome {parâmetro.Key}.");
                    }
                }
            }

            if (trechosDeCálculos == null) return variáveis;

            foreach (var trecho in trechosDeCálculos)
            {
                if (trecho.Parâmetros == null) continue;
                foreach (var parâmetroDeTrecho in trecho.Parâmetros)
                {
                    if (!variáveis.ContainsKey(parâmetroDeTrecho.Key))
                    {
                        // TODO (RCM) Identificar variáveis com nome idêntico
                        variáveis.Add(parâmetroDeTrecho.Key, parâmetroDeTrecho.Value);
                    }
                }
            }

            return variáveis;
        }

        private string ObterExpressãoDoCálculo(IList<ICorrelação> correlações, IList<TrechoCálculo> trechosDeCálculos)
        {
            if (PerfisEntrada.ContémPerfil("VP") == false && correlações.Any(c => c.PerfisEntrada.Tipos.Contains("VP")))
            {
                foreach (var corr in correlações.ToList())
                {
                    if (corr.PerfisEntrada.Tipos.Contains("VP"))
                    {
                        correlações.Remove(corr);
                        PerfisSaída.Perfis.RemoveAll(c => corr.PerfisSaída.Tipos.Contains(c.Mnemonico));
                    }
                }
            }

            return new GerenciadorDeExpressõesDoCálculoDePerfis(correlações, trechosDeCálculos).Extrair();
        }

        private void RemoverPerfisDoDomínioDasEntradas()
        {
            var perfisObtidosNoDomínio = ObterPerfisDoDomínio();

            if (perfisObtidosNoDomínio?.Count < PerfisEntrada?.Perfis.Count)
            {
                foreach (var perfil in perfisObtidosNoDomínio)
                {
                    var existeComoEntrada =
                        PerfisEntrada.Perfis.Find(p => p.Mnemonico == perfil.Mnemonico);

                    if (existeComoEntrada != null)
                    {
                        PerfisEntrada.Perfis.Remove(existeComoEntrada);
                    }
                }
            }
        }

        private List<PerfilBase> ObterPerfisDoDomínio()
        {
            var perfisObtidosNoDomínio = new List<PerfilBase>();
            var perfisDoDomínio = Correlação.Expressão.Analitics.PerfisQuePodemSerAdquiridosNoDomínioPresentesNaCorrelação;

            if (perfisDoDomínio == null)
                return perfisObtidosNoDomínio;

            foreach (var perfil in perfisDoDomínio)
            {
                if (perfil == "DTMC")
                {
                    var dtmc = Litologia.GetDTMC(ProfundidadeDeReferência, TipoProfundidade.PM, ConversorProfundidade, Nome);
                    perfisObtidosNoDomínio.Add(dtmc);
                }
                else if (perfil == "RHOG")
                {
                    var rhog = Litologia.GetRHOG(ProfundidadeDeReferência, TipoProfundidade.PM, ConversorProfundidade, Nome);
                    perfisObtidosNoDomínio.Add(rhog);
                }
                else if (perfil == "TVERT")
                {
                    //TODO (Vanessa Chalub) Verificar com o Roberto esse item
                    // TODO (RCM): Verificar se ficará armazenado no domínio
                }
            }

            return perfisObtidosNoDomínio;
        }

        private List<PerfilBase> ObterPerfisDoDomínio(List<PerfilBase> perfisDeEntrada)
        {
            List<PerfilBase> perfisDomínio = new List<PerfilBase>();

            var perfisObtidosNoDomínio = ObterPerfisDoDomínio();
            foreach (var perfil in perfisObtidosNoDomínio)
            {
                var nãoExisteComoEntrada =
                    perfisDeEntrada.All(p => p.Mnemonico != perfil.Mnemonico);

                if (nãoExisteComoEntrada)
                {
                    perfisDomínio.Add(perfil);
                }
            }
            return perfisDomínio;
        }

        private double[] ObterProfundidadeDeRefência(IList<PerfilBase> perfisDeEntrada, GrupoCálculo grupoDeCálculo)
        {
            var perfisASeremSincronizados = new List<PerfilBase>();

            foreach (var perfil in perfisDeEntrada)
            {
                if (PerfilNãoÉRestriçãoÀSincronia(perfil.Mnemonico)) continue;
                if (ÉPerfilDeSaídaEDeEntradaAoMesmoTempo(perfil.Mnemonico)) continue;
                perfisASeremSincronizados.Add(perfil);
            }
            var sincronizadorDeProfundidades = new SincronizadorProfundidades(perfisASeremSincronizados, ConversorProfundidade, Litologia, grupoDeCálculo);
            return sincronizadorDeProfundidades.GetProfundidadeDeReferência();
        }

        private bool PerfilNãoÉRestriçãoÀSincronia(string mnemônico)
        {
            return PerfisNãoRestritivosÀSincroniaDeProfundidades.Contains(mnemônico);
        }

        private bool ÉPerfilDeSaídaEDeEntradaAoMesmoTempo(string mnemônico)
        {
            var saída = PerfisSaída.Perfis.Find(p => p.Mnemonico == mnemônico);
            var entrada = PerfisEntrada.Perfis.Find(p => p.Mnemonico == mnemônico);
            return saída != null && entrada != null;
        }

        private void AtualizarPerfisDoCálculo()
        {
            CriarPerfisDoCálculo(PerfisEntrada.Perfis);
            AtualizarPerfisDeEntrada();
            AtualizarPerfisDeSaída();
        }

        private void CriarPerfisDoCálculo(List<PerfilBase> perfisDeEntrada)
        {
            CriarPerfisDeEntrada(perfisDeEntrada);
            CriarPerfisVetoriaisDeSaída();
        }

        private void AtualizarPerfisDeEntrada()
        {
            foreach (var perfil in PerfisDeEntradaVetorial)
            {
                var perfilVetorial = perfil.Value;
                AtualizarPerfilDeEntrada(perfilVetorial);
            }
        }

        private void AtualizarPerfilDeEntrada(PerfilVetorial perfilVetorial)
        {
            perfilVetorial.CarregarValoresDosPontos(ProfundidadeDeReferência);
        }

        private void AtualizarPerfisDeSaída()
        {
            foreach (var perfilDeSaída in PerfisDeSaídaVetorial)
            {
                AtualizarPerfiDeSaída(perfilDeSaída);
            }
        }

        private void AtualizarPerfiDeSaída(KeyValuePair<string, PerfilVetorial> perfilDeSaída)
        {
            var perfilDeSaídaVetorial = perfilDeSaída.Value;

            if (!ÉPerfilDeSaídaEDeEntradaAoMesmoTempo(perfilDeSaída.Key))
                perfilDeSaídaVetorial.CarregarValoresDosPontos(ProfundidadeDeReferência);
        }

        private void CriarPerfisDeEntrada(List<PerfilBase> perfisDeEntrada)
        {

            if (Correlação.Expressão.Analitics.TemPerfisQuePossamSerAdquiridosNoDomínio)
            {
                var perfisDomínio = ObterPerfisDoDomínio(perfisDeEntrada);

                if (perfisDomínio.Any())
                {
                    foreach (var perfil in perfisDomínio)
                    {
                        perfisDeEntrada.Add(perfil);
                    }
                }
            }

            if (Correlação.Expressão.Analitics.TemPerfilDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário)
            {
                var perfisAvaliadosEmOperadorTernário = ObterPerfisAvaliadosEmOperadorTernário(perfisDeEntrada);

                if (perfisAvaliadosEmOperadorTernário.Any())
                {
                    foreach (var perfil in perfisAvaliadosEmOperadorTernário)
                    {
                        perfisDeEntrada.Add(perfil);
                    }
                }
            }


            VerificarSeTodosPerfisDeEntradaUtilizadosNoCálculoEstãoPresentes(perfisDeEntrada);
            PerfisDeEntradaVetorial = new Dictionary<string, PerfilVetorial>();

            int index;
            for (index = 0; index < perfisDeEntrada.Count; index++)
            {
                var perfil = perfisDeEntrada[index];
                CriarEntradaDoCálculo(perfil);
            }
        }

        private List<PerfilBase> ObterPerfisAvaliadosEmOperadorTernário(List<PerfilBase> perfisDeEntrada)
        {
            List<PerfilBase> PerfisAvaliadosEmOperadorTernário = new List<PerfilBase>();

            foreach (var perfilIdentificado in Correlação.Expressão.Analitics.PerfisDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário)
            {
                if (perfisDeEntrada.Any(p => p.TipoPerfil.Mnemônico.ToString().Equals(perfilIdentificado)))
                    continue;

                var mnemônico = perfilIdentificado;
                var nomePerfilSaída = mnemônico + "_" + Nome;
                var perfil = PerfisFactory.Create(mnemônico, nomePerfilSaída, ConversorProfundidade, Litologia);
                PerfisAvaliadosEmOperadorTernário.Add(perfil);
            }
            return PerfisAvaliadosEmOperadorTernário;
        }

        private void VerificarSeTodosPerfisDeEntradaUtilizadosNoCálculoEstãoPresentes(List<PerfilBase> perfisDeEntrada)
        {
            foreach (var perfil in Correlação.PerfisEntrada.Tipos)
            {
                //// Todo(RCM): Testar se o perfil de entrada é necessário somente num dos lados de uma atribuição através de um operador ternario. Exemplo: (RHOB > 0) PORO = RHOB + 1 : PORO = DTS
                //if (_grupoDeCálculo == GrupoDeCálculo.Perfis && perfil.Equals(Mnemônico.RHOB.ToString()))
                //    continue;

                var perfilEntradaEncontrado = perfisDeEntrada.Find(p => p.Mnemonico == perfil);

                // Implementar a Verificação de Perfil de Saída de um Cálculo Composto
                if (perfilEntradaEncontrado == null &&
                    !PerfisOpcionais.Contains(perfil))
                {
                    throw new ArgumentException($"Perfil {perfil} não presente no cálculo.");
                }

                if (perfilEntradaEncontrado != null && perfilEntradaEncontrado.Count == 0)
                    throw new ArgumentException($"Perfil {perfil} não tem pontos.");
            }
        }

        private void CriarEntradaDoCálculo(PerfilBase perfil)
        {
            var vpc = new PerfilVetorial(ConversorProfundidade, perfil, ProfundidadeDeReferência, GrupoCálculo);

            if (perfil.Mnemonico == "RHOB")
            {
                PerfisDeEntradaVetorial.Add("RHOBUSER", vpc);
            }

            PerfisDeEntradaVetorial.Add(perfil.Mnemonico, vpc);
        }

        private void CriarPerfisVetoriaisDeSaída()
        {
            if (PerfisDeEntradaVetorial == null || PerfisDeEntradaVetorial.Count == 0)
            {
                throw new ArgumentException(
                    $"Cálculo {Nome} realizou tentativa de inicializar perfis de saída antes da inicialização dos perfis de entrada.");
            }
            var sincronia = ProfundidadeDeReferência != null;

            if (!sincronia)
            {
                throw new InvalidOperationException(
                    $"Cálculo {Nome} realizou tentativa de inicializar perfis de saída antes de sincronizar os perfis de entrada.");
            }

            PerfisDeSaídaVetorial = new Dictionary<string, PerfilVetorial>();

            foreach (var perfil in PerfisSaída.Perfis)
            {
                perfil.Clear();
                var mnemônico = perfil.Mnemonico;
                if (!PerfilDeSaídaÉCalculado(mnemônico))
                {
                    throw new ArgumentException(
                        $"Cálculo {Nome} realizou tentativa de inicializar o perfil de saída \"{mnemônico.ToString()}\" que não é calculado na Expressão.");
                }

                var perfilDeSaídaVetorial = CriarPerfilDeSaídaVetorial(perfil);

                if (PerfisDeSaídaVetorial.ContainsKey(mnemônico))
                {
                    PerfisDeSaídaVetorial.Remove(mnemônico);
                }
                PerfisDeSaídaVetorial.Add(mnemônico, perfilDeSaídaVetorial);
            }
        }

        private PerfilVetorial CriarPerfilDeSaídaVetorial(PerfilBase perfil)
        {
            var mnemônico = perfil.Mnemonico;

            // testa se o perfil de saída também é perfil de entrada. Perfil de entrada de ser inicializado à priori
            if (ÉPerfilDeSaídaEDeEntradaAoMesmoTempo(mnemônico))
            {
                return PerfisDeEntradaVetorial[mnemônico];
            }

            return new PerfilVetorial(ConversorProfundidade, perfil, ProfundidadeDeReferência, GrupoCálculo);
        }

        private bool PerfilDeSaídaÉCalculado(string perfilDeSaída)
        {
            return new Regex($@"\b{perfilDeSaída}\b\s*=\s*").IsMatch(Correlação.Expressão.Normalizada);
        }

        private void InicializarParserDeCálculo(Parser parser)
        {
            SetFábricaDeVariáveisDoCálculo(parser);
            SetExpressãoDoCálculoNoParser(parser);
            SetConstantesNoParser(parser);
            SetVariáveisNoParser(parser);
            SetIdentificadorDeGrupoLitológicoNoParser(parser);
            SetIdentificadorDeCálculoPorTrechoNoParser(parser);
            SetIdentificadorDeProfundidadeInicialNoParser(parser);
            SetIdentificadoresDeDadosGeraisEGeometriaNoParser(parser);
            SetStepFixoNoParser(parser);
            SetPerfisDoCálculoNoParser(parser);
        }

        private void Calcular(Parser parser)
        {
            parser.EvalBulk(ProfundidadeDeReferência.Length);
        }

        private void CarregarValoresCalculados(bool recalculo)
        {
            try
            {
                foreach (var tupla in PerfisDeSaídaVetorial)
                {
                    var perfilDeSaídaVetorial = tupla.Value;
                    if (!ÉPerfilDeSaídaEDeEntradaAoMesmoTempo(tupla.Key))
                        perfilDeSaídaVetorial.CriarPerfilComPontosCalculados(recalculo, IgnorarValoresNegativos);
                    else
                    {
                        var perfil = PerfisSaída.Perfis.Single(p => p.Mnemonico == tupla.Key);
                        perfil.Clear();

                        for (var index = 0; index < perfilDeSaídaVetorial.ValoresDosPontos.Length; index++)
                        {
                            if (IgnorarValoresNegativos && perfilDeSaídaVetorial.ValoresDosPontos[index] < 0) continue;

                            perfil.AddPontoEmPm(ConversorProfundidade, ProfundidadeDeReferência[index], perfilDeSaídaVetorial.ValoresDosPontos[index], TipoProfundidade.PM, OrigemPonto.Calculado);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Carregar pontos calculados : " + exception.Message, exception);
                // TODO: (RCM) Investigar possíveis casos
            }
        }

        #region Métodos Inicializar Parser

        private void SetPerfisDoCálculoNoParser(Parser parser)
        {
            SetPerfisDeEntradaDoCálculo(parser);

            SetPerfisDeSaídaDoCálculo(parser);
        }

        private void SetPerfisDeSaídaDoCálculo(Parser parser)
        {
            if (PerfisDeSaídaVetorial == null || PerfisDeSaídaVetorial.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cálculo {Nome} realizou tentativa de inicializar perfis de saída no parser antes de inicializa-los.");
            }

            foreach (var perfilDeSaída in PerfisDeSaídaVetorial)
            {
                SetPerfilDoCálculo(perfilDeSaída, parser);
            }
        }

        private void SetPerfisDeEntradaDoCálculo(Parser parser)
        {
            if (PerfisDeEntradaVetorial == null || PerfisDeEntradaVetorial.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cálculo {Nome} realizou tentativa de inicializar perfis de entrada no parser antes de inicializa-los.");
            }

            foreach (var perfilDeEntrada in PerfisDeEntradaVetorial)
            {
                SetPerfilDoCálculo(perfilDeEntrada, parser);
            }
        }

        private void SetPerfilDoCálculo(KeyValuePair<string, PerfilVetorial> perfilVetorial, Parser parser)
        {
            var mnemônico = perfilVetorial.Key;

            if (!PerfilExisteNoCálculo(mnemônico))
            {
                throw new ArgumentException(
                    $"Cálculo {Nome} executou uma tentativa de inicializar um perfil de entrada {mnemônico} que não está no cálculo.");
            }
            if (VariávelJáEstáDefinida(mnemônico.ToString(), parser))
            {
                return;
            }

            DefinirVariávelNoParser(mnemônico.ToString(), perfilVetorial.Value.ValoresDosPontos, parser);
        }

        private bool PerfilExisteNoCálculo(string mnemônico)
        {
            return Regex.IsMatch(Correlação.Expressão.Normalizada, mnemônico);
        }

        private void SetStepFixoNoParser(Parser parser)
        {
            if (OcorreCálculoComStepFixo())
            {
                if (GrupoCálculo != GrupoCálculo.Sobrecarga)
                {
                    throw new InvalidOperationException(
                        $"Cálculo {Nome} realizou tentativa de inicializar step fixo. Token ({Tokens.TokenDeCálculoComStepFixo}) somente permitido para o cálculo de {GrupoCálculo.Sobrecarga}.");
                }

                // step é usado no cálculo de TVERT
                // Assim, caso tamanho da ProfundidadeDeReferência seja < 2, ele não é utilizado. 
                // Caso contrário, é a diferença entre duas profundidades subsequentes.
                var step = ProfundidadeDeReferência.Length < 2
                    ? 0
                    : ProfundidadeDeReferência[1] - ProfundidadeDeReferência[0];
                DefinirStepFixoNoParser(step, parser);
            }
        }

        private void DefinirStepFixoNoParser(double valor, Parser parser)
        {
            var step = InicializarVariávelVetorial(valor);

            // Criação de variável para STEP
            parser.DefineVar(Tokens.TokenDeCálculoComStepFixo, step);
        }

        private bool OcorreCálculoComStepFixo()
        {
            return Correlação.Expressão.Analitics.TemCálculoComStepFixo;
        }

        private void SetIdentificadoresDeDadosGeraisEGeometriaNoParser(Parser parser)
        {
            SetIdentificadorDeDensidadeÁguaMarNoParser(parser);
            SetIdentificadorDeLâminaDÁguaNoParser(parser);
            SetIdentificadorDeMesaRotativaNoParser(parser);
            SetIdentificadorDeAlturaDeAntePoçoNoParser(parser);
            SetIdentificadorDeCategoriaDoPocoNoParser(parser);
            SetIdentificadorDePoçoOffShoreNoParser(parser);
            SetIdentificadorDePoçoOnShoreNoParser(parser);

        }

        private void SetIdentificadorDePoçoOnShoreNoParser(Parser parser)
        {
            if (OcorreCálculoUsandoPoçoOnShore())
            {
                double onShore = (double)CategoriaPoço.OnShore;
                DefinirPoçoOnShoreNoParser(onShore, parser);
            }
        }

        private void DefinirPoçoOnShoreNoParser(double valor, Parser parser)
        {
            var onShore = InicializarVariávelVetorial(valor);

            // Criação de variável para ONSHORE
            parser.DefineVar(Tokens.TokenDePoçoOnShore, onShore);
        }

        private bool OcorreCálculoUsandoPoçoOnShore()
        {
            return Correlação.Expressão.Analitics.TemCálculoComPoçoOnShore;
        }

        private void SetIdentificadorDePoçoOffShoreNoParser(Parser parser)
        {
            if (OcorreCálculoUsandoPoçoOffShore())
            {
                double offShore = (double)CategoriaPoço.OffShore;
                DefinirPoçoOffShoreNoParser(offShore, parser);
            }
        }

        private void DefinirPoçoOffShoreNoParser(double valor, Parser parser)
        {
            var offshore = InicializarVariávelVetorial(valor);

            // Criação de variável para OFFSHORE
            parser.DefineVar(Tokens.TokenDePoçoOffShore, offshore);
        }

        private bool OcorreCálculoUsandoPoçoOffShore()
        {
            return Correlação.Expressão.Analitics.TemCálculoComPoçoOffShore;
        }

        private void SetIdentificadorDeCategoriaDoPocoNoParser(Parser parser)
        {
            if (OcorreCálculoUsandoCategoriaDoPoco())
            {
                double categoriaDoPoço = (double)Geometria.CategoriaPoço;
                DefinirCategoriaDoPoçoNoParser(categoriaDoPoço, parser);
            }
        }

        private void DefinirCategoriaDoPoçoNoParser(double valor, Parser parser)
        {
            var categoriaDoPoço = InicializarVariávelVetorial(valor);

            // Criação de variável para CATEGORIA_POÇO
            parser.DefineVar(Tokens.TokenDeCategoriaDoPoco, categoriaDoPoço);
        }

        private bool OcorreCálculoUsandoCategoriaDoPoco()
        {
            return Correlação.Expressão.Analitics.TemCálculoComCategoriaDoPoco;
        }

        private void SetIdentificadorDeAlturaDeAntePoçoNoParser(Parser parser)
        {
            if (OcorreCálculoUsandoAlturaDeAntepoço())
            {
                if (Geometria.OnShore == null)
                {
                    throw new InvalidOperationException(
                        $"DadosOnShore não foi passado para o Cálculo {Nome}");
                }

                double valorAlturaAntepoço = Geometria.OnShore.AlturaDeAntePoço;
                DefinirAlturaDeAntepoçoNoParser(valorAlturaAntepoço, parser);
            }
        }

        private void DefinirAlturaDeAntepoçoNoParser(double valor, Parser parser)
        {
            var alturaDeAntepoço = InicializarVariávelVetorial(valor);

            // Criação de variável para ALTURA_ANTEPOÇO
            parser.DefineVar(Tokens.TokenDeAlturaDeAntepoço, alturaDeAntepoço);
        }

        private bool OcorreCálculoUsandoAlturaDeAntepoço()
        {
            return Correlação.Expressão.Analitics.TemCálculoComAlturaDeAntepoço;
        }

        private void SetIdentificadorDeMesaRotativaNoParser(Parser parser)
        {
            if (OcorreCálculoUsandoMesaRotativa())
            {
                double valorMesaRotativa = Geometria.MesaRotativa;
                DefinirMesaRotativaNoParser(valorMesaRotativa, parser);
            }
        }

        private void DefinirMesaRotativaNoParser(double valor, Parser parser)
        {
            var mesaRotativa = InicializarVariávelVetorial(valor);

            // Criação de variável para MESA_ROTATIVA
            parser.DefineVar(Tokens.TokenDeMesaRotativa, mesaRotativa);
        }

        private bool OcorreCálculoUsandoMesaRotativa()
        {
            return Correlação.Expressão.Analitics.TemCálculoComMesaRotativa;
        }

        private void SetIdentificadorDeLâminaDÁguaNoParser(Parser parser)
        {
            if (OcorreCálculoUsandoLâminaDAgua())
            {
                if (Geometria.OffShore == null)
                {
                    throw new InvalidOperationException(
                        $"DadosOffShore não foi passad0 para o Cálculo {Nome}");
                }

                double valorLâminaDágua = Geometria.OffShore.LaminaDagua;
                DefinirLâminaDáguaNoParser(valorLâminaDágua, parser);
            }
        }

        private void DefinirLâminaDáguaNoParser(double valor, Parser parser)
        {
            var lâminaDagua = InicializarVariávelVetorial(valor);

            // Criação de variável para LAMINA_DAGUA
            parser.DefineVar(Tokens.TokenDeLâminaDAgua, lâminaDagua);
        }

        private bool OcorreCálculoUsandoLâminaDAgua()
        {
            return Correlação.Expressão.Analitics.TemCálculoComLâminaDAgua;
        }

        private void SetIdentificadorDeDensidadeÁguaMarNoParser(Parser parser)
        {
            if (OcorreCálculoUsandoDensidadeAguaDoMar())
            {
                double valorDensidadeDÁgua = DadosGerais.Area.DensidadeAguaMar;
                DefinirDensidadeDÁguaNoParser(valorDensidadeDÁgua, parser);
            }
        }

        private void DefinirDensidadeDÁguaNoParser(double valor, Parser parser)
        {
            var dendidadeAguaMar = InicializarVariávelVetorial(valor);

            // Criação de variável para DENSIDADE_AGUA_MAR
            parser.DefineVar(Tokens.TokenDeDensidadeÁguaMar, dendidadeAguaMar);
        }

        private bool OcorreCálculoUsandoDensidadeAguaDoMar()
        {
            return Correlação.Expressão.Analitics.TemCálculoComDensidadeAguaDoMar;
        }

        private void SetIdentificadorDeProfundidadeInicialNoParser(Parser parser)
        {
            if (OcorreCálculoApartirDeProfundidadeInicial() && ProfundidadeDeReferência.Length > 0)
            {
                DefinirProfundidadeInicialNoParser(ProfundidadeDeReferência.First(), parser);
            }
        }

        private bool OcorreCálculoApartirDeProfundidadeInicial()
        {
            return Correlação.Expressão.Analitics.TemCálculoApartirDeProfundidadeInicial;
        }

        private void DefinirProfundidadeInicialNoParser(double profundidadeInicial, Parser parser)
        {
            var profInicial = InicializarVariávelVetorial(profundidadeInicial);

            // Criação de variável para PROFUNDIDADE_INICIAL
            parser.DefineVar(Tokens.TokenDeProfundidadeInicial, profInicial);
        }

        private void SetIdentificadorDeCálculoPorTrechoNoParser(Parser parser)
        {
            if (OcorreCálculoPorTrecho())
            {
                DefinirProfundidadeNoParser(ProfundidadeDeReferência, parser);
            }
        }

        private bool OcorreCálculoPorTrecho()
        {
            return Correlação.Expressão.Analitics.TemCálculoPorTrecho;
        }

        private void DefinirProfundidadeNoParser(double[] valorProfundidade, Parser parser)
        {
            // Criação de variável para PROFUNDIDADE
            parser.DefineVar(Tokens.TokenDeCálculoPorTrecho, valorProfundidade);
        }

        private void SetFábricaDeVariáveisDoCálculo(Parser parser)
        {
            if (ProfundidadeDeReferência == null || ProfundidadeDeReferência.Length == 0)
            {
                throw new InvalidOperationException("Tentativa de inicialização do parser sem a prévia sincronia de profundidades.");
            }

            // adiciona a função de factory
            parser.SetVarFactory((name, userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);
        }

        private void SetExpressãoDoCálculoNoParser(Parser parser)
        {
            parser.Expr = InicializarVariáveisNaExpressão();
        }

        private string InicializarVariáveisNaExpressão()
        {
            var expressão = Correlação.Expressão.Bruta;
            var pattern = @"\bvar\b\s*(?<Nome>\b[a-zA-Z]\w*)\s*=\s*(?<Valor>\d+\.?\d*)";
            var regex = new Regex(pattern);
            expressão = regex.Replace(expressão, match => ReplaceValorDaVariável("Valor", match));
            return Normalizador.NormalizarExpressão(expressão);
        }

        private string ReplaceValorDaVariável(string groupName, Match match)
        {
            var variável = match.Groups["Nome"].Value;
            if (!Variáveis.ContainsKey(variável))
            {
                throw new InvalidOperationException($"Cálculo {Nome} teve suas variáveis carregadas incorretamente.");
            }

            var novoValor = Variáveis[variável].ToString(CultureInfo.InvariantCulture);

            var capturado = match.Value;
            capturado = capturado.Remove(match.Groups[groupName].Index - match.Index, match.Groups[groupName].Length);
            capturado = capturado.Insert(match.Groups[groupName].Index - match.Index, novoValor);
            return capturado;
        }

        private void SetConstantesNoParser(Parser parser)
        {
            parser.Consts.Clear();

            foreach (var constante in Correlação.Expressão.Constantes.Names)
            {
                if (!ConstanteExisteNoCálculo(constante))
                {
                    throw new InvalidOperationException(
                        $"Cálculo {Nome} executou uma tentativa de instanciar a constante {constante}, que não faz parte do cálculo.");
                }

                var valorConstanteExiste = Correlação.Expressão.Constantes.TryGetValue(constante, out double valorConstante);
                var valor = InicializarVariávelVetorial(valorConstante);
                DefinirConstanteNoParser(constante, valor, parser);
            }
        }

        private bool ConstanteExisteNoCálculo(string constante)
        {
            return Regex.IsMatch(Correlação.Expressão.Normalizada, constante);
        }

        private void DefinirConstanteNoParser(string constante, double[] valor, Parser parser)
        {
            parser.DefineVar(constante, valor);
        }

        private void SetVariáveisNoParser(Parser parser)
        {
            if (Correlação.Expressão.Variáveis.Names.Count != Variáveis.Count)
            {
                throw new InvalidOperationException($"Cálculo {Nome} teve suas variáveis carregadas incorretamente.");
            }

            if (Variáveis == null) return;

            foreach (var variável in Variáveis)
            {
                if (!VariávelExisteNoCálculo(variável))
                {
                    throw new InvalidOperationException(
                        $"Cálculo {Nome} executou uma tentativa de instanciar a variável {variável}, que não faz parte do cálculo.");
                }

                if (VariávelJáEstáDefinida(variável.Key, parser))
                {
                    return;
                }
                var valor = InicializarVariávelVetorial(variável.Value);
                DefinirVariávelNoParser(variável.Key, valor, parser);
            }
        }

        private bool VariávelExisteNoCálculo(KeyValuePair<string, double> variável)
        {
            return Regex.IsMatch(Correlação.Expressão.Normalizada, variável.Key);
        }

        private bool VariávelJáEstáDefinida(string variável, Parser parser)
        {
            return parser.Vars.ContainsKey(variável);
        }

        private void DefinirVariávelNoParser(string variável, double[] valor, Parser parser)
        {
            parser.DefineVar(variável, valor);
        }

        private void SetIdentificadorDeGrupoLitológicoNoParser(Parser parser)
        {
            if (OcorreCálculoPorGrupoLitológico())
            {
                var valorGrupoLitológico = InicializaGrupoLitológicoVetorial();
                DefinirGrupoLitológicoNoParser(valorGrupoLitológico, parser);
            }
        }

        private bool OcorreCálculoPorGrupoLitológico()
        {
            return Correlação.Expressão.Analitics.TemCálculoPorGrupoLitológico;
        }

        private double[] InicializaGrupoLitológicoVetorial()
        {
            // nesse ponto a sincronia dos perfis já deverá ter sido feita
            var valorGrupoLitológico = new double[QtdPontos];

            for (var index = 0; index < QtdPontos; index++)
            {
                var profundidade = ProfundidadeDeReferência[index];
                valorGrupoLitológico[index] = ObterGrupoLitológicoNessaProfundidade(profundidade);
            }

            //Setando o grupo litológico da última profundidade
            var ultimaProfundidade = ProfundidadeDeReferência[QtdPontos - 1];
            var pontos = Litologia.GetPontos();

            for (int i = pontos.Count - 1; i >= 0; i--)
            {
                var ponto = pontos[i];
                if (ponto.Pm.Valor <= ultimaProfundidade && ponto.Pm.Valor >= ultimaProfundidade)
                {
                    valorGrupoLitológico[QtdPontos - 1] = ponto.TipoRocha.Grupo.Valor;
                    break;
                }
            }

            return valorGrupoLitológico;
        }

        // TODO (RCM): usar algoritmo de busca binária a ser implementado.
        private double ObterGrupoLitológicoNessaProfundidade(double profundidade)
        {
            // TODO: Verificar qual procedimento nessa situação.
            if (!Litologia.ContémPontos())
            {
                return GrupoLitologico.NãoIdentificado.Valor;
            }

            Litologia.ObterGrupoLitológicoNessaProfundidade(profundidade, out int valorGrupoLitológico);

            return valorGrupoLitológico;
        }

        private void DefinirGrupoLitológicoNoParser(double[] valorGrupoLitológico, Parser parser)
        {
            // Criação de variável para o GRUPO_LITOLOGICO
            parser.DefineVar(Tokens.TokenDeGrupoLitológico, valorGrupoLitológico);
        }
        #endregion

        private double[] InicializarVariávelVetorial(double valor)
        {
            var variável = new double[QtdPontos];

            for (var index = 0; index < QtdPontos; index++)
            {
                variável[index] = valor;
            }

            return variável;
        }
        private Origem ObterOrigemDaCorrelação(GrupoCálculo grupoDeCálculo)
        {
            switch (grupoDeCálculo)
            {
                case GrupoCálculo.Perfis:
                    return Origem.CálculoDePerfis;
                case GrupoCálculo.PropriedadesMecânicas:
                    return Origem.CálculoDePropriedadesMecânicas;
                case GrupoCálculo.Sobrecarga:
                    return Origem.CálculoDeSobrecarga;
                case GrupoCálculo.PressãoPoros:
                    return Origem.CálculoDePressãoDePoros;
                case GrupoCálculo.Gradientes:
                    return Origem.CálculoDeGradientes;
                case GrupoCálculo.Tensões:
                    return Origem.Tensões;
                default:
                    return Origem.Usuário;
            }
        }

        private ICorrelação MontarCorrelação(string nome, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões,
            IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> trechos, Dictionary<string, double> variáveis
            , IList<ICorrelação> correlaçõesBase, ICorrelaçãoFactory correlaçãoFactory)
        {
            var montador = new MontadorDeCorrelaçõesDePropriedadesMecânicas(nome, regiões, trechos, variáveis, correlaçõesBase, correlaçãoFactory);
            return montador.Montar();
        }
    }
}
