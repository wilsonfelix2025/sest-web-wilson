using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Entities.RegistrosDePerfuração;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Factories;

namespace SestWeb.Domain.Entities.Poço
{
    public class Poço : ISupportInitialize
    {
        private Poço(string id, string nome, TipoPoço tipoPoço)
        {
            Id = id;
            DadosGerais.Identificação.Nome = nome;
            DadosGerais.Identificação.NomePoço = nome;
            DadosGerais.Identificação.TipoPoço = tipoPoço;
        }

        #region Properties

        [BsonId]
        public string Id { get; set; }

        public string Nome => DadosGerais.Identificação.Nome;

        public TipoPoço TipoPoço => DadosGerais.Identificação.TipoPoço;

        public DadosGerais DadosGerais { get; set; } = new DadosGerais();

        [BsonIgnore]
        public List<PerfilBase> Perfis { get; set; } = new List<PerfilBase>();

        public Trajetória Trajetória { get; set; } = new Trajetória(MétodoDeCálculoDaTrajetória.RaioCurvatura);

        public List<Litologia> Litologias { get; set; } = new List<Litologia>();

        public List<Sapata> Sapatas { get; set; } = new List<Sapata>();

        public List<Objetivo> Objetivos { get; set; } = new List<Objetivo>();

        [BsonIgnore]
        public List<RegistroEvento> RegistrosEventos { get; set; } = new List<RegistroEvento>();

        public Estratigrafia Estratigrafia { get; set; } = new Estratigrafia();

        public State State { get; set; } = new State();

        public IReadOnlyList<RegistroDePerfuração> RegistrosDePerfuração = new List<RegistroDePerfuração>();

        public IReadOnlyList<Cálculo> Cálculos = new List<Cálculo>();

        public List<string> IdsPoçosApoio = new List<string>();

        #endregion

        #region Methods

        public SapataFactory ObterSapataFactory()
        {
            return new SapataFactory(Trajetória);
        }

        public ObjetivoFactory ObterObjetivoFactory()
        {
            return new ObjetivoFactory(Trajetória);
        }

        public double ObterBaseDeSedimentos()
        {
            var soma = 0.0;

            if (DadosGerais.Geometria.CategoriaPoço == CategoriaPoço.OffShore)
                soma = DadosGerais.Geometria.OffShore.LaminaDagua + DadosGerais.Geometria.MesaRotativa;
            else
                soma = DadosGerais.Geometria.OnShore.AlturaDeAntePoço + DadosGerais.Geometria.MesaRotativa;

            return soma;

        }

        public Litologia ObterLitologiaPadrão()
        {
            var litologia = TipoPoço == TipoPoço.Projeto ?
                Litologias.Find(l => l.Classificação == TipoLitologia.Adaptada) :
                Litologias.Find(l => l.Classificação == TipoLitologia.Interpretada);

            return litologia;
        }

        public void EditarTrajetória(IList<PontoDeTrajetória> novosPontos)
        {
            Trajetória novaTrajetória = new Trajetória(Trajetória.MétodoDeCálculoDaTrajetória);
            novaTrajetória.Reset(novosPontos);

            Parallel.ForEach(Perfis, perfil =>
            {
                var pontos = perfil.Pontos;
                perfil.Clear();

                foreach (var ponto in pontos)
                {
                    novaTrajetória.TryGetMDFromTVD(ponto.Pv.Valor, out var novoPm);
                    perfil.AddPonto(novaTrajetória, novoPm, ponto.Pv.Valor, ponto.Valor, ponto.TipoProfundidade, ponto.Origem);
                }
            });

            Parallel.ForEach(Litologias, litologia =>
            {
                var pontos = litologia.Pontos;
                litologia.Clear();

                foreach (var ponto in pontos)
                {
                    novaTrajetória.TryGetMDFromTVD(ponto.Pv.Valor, out var novoPm);
                    litologia.AddPonto(novaTrajetória, novoPm, ponto.Pv.Valor, ponto.TipoRocha.Mnemonico, ponto.TipoProfundidade, ponto.Origem);
                }
            });
        }

        public void AdicionarPoçoApoio(string idPoçoApoio)
        {
            IdsPoçosApoio.Add(idPoçoApoio);
        }

        public void RemoverPoçoApoio(string idPoçoApoio)
        {
            IdsPoçosApoio.Remove(idPoçoApoio);
        }

        public void ConverterParaPm()
        {
            Trajetória?.ExibirEmPM();

            Parallel.ForEach(Litologias, litogogia => { litogogia.ConverterParaPm(); });

            //Estratigrafia.ConverterParaPm(Trajetória);

            Parallel.ForEach(Perfis, perfil => { perfil.ConverterParaPm(); });

            Parallel.ForEach(RegistrosDePerfuração, registro => { registro.ConverterParaPm(Trajetória); });
        }

        public void ConverterParaPv()
        {
            Trajetória?.ExibirEmPV();

            Parallel.ForEach(Litologias, litogogia => { litogogia.ConverterParaPv(); });

            //Estratigrafia.ConverterParaPv(Trajetória);

            Parallel.ForEach(Perfis, perfil => { perfil.ConverterParaPv(); });

            Parallel.ForEach(RegistrosDePerfuração, registro => { registro.ConverterParaPv(Trajetória); });
        }

        public void AtualizarPvs()
        {
            // atualiza pvs dos perfis
            Parallel.ForEach(Perfis, perfil => { perfil.AtualizarPvs(Trajetória); });

            // atualiza pvs das litologias
            Parallel.ForEach(Litologias, litologia => { litologia.AtualizarPvs(Trajetória); });

            // atualiza pvs das de sapatas
            Parallel.ForEach(Sapatas, sapata => { sapata.AtualizarPv(Trajetória); });

            // atualiza pvs dos objetivos
            Parallel.ForEach(Objetivos, objetivo => { objetivo.AtualizarPv(Trajetória); });

            // atualiza pvs da estratigrafia
            Estratigrafia.AtualizarPvs(Trajetória);

            // atualiza pvs dos registros
            Parallel.ForEach(RegistrosEventos, registro => { registro.AtualizarPvs(Trajetória); });
        }

        public void Shift(double delta)
        {
            // shiftar trajetória
            if (Trajetória != null && Trajetória.ContémDados())
            {
                Trajetória.Shift(delta);
            }

            // shift litologias
            Parallel.ForEach(Litologias, litologia => { litologia.Shift(delta); });

            // shift de estratigrafias
            Estratigrafia.Shift(delta);

            // shift de sapatas
            Parallel.ForEach(Sapatas, sapata => { sapata.Shift(delta, Trajetória); });

            // shift de objetivos
            Parallel.ForEach(Objetivos, objetivo => { objetivo.Shift(delta, Trajetória); });

            // shift perfis
            Parallel.ForEach(Perfis, perfil => { perfil.Shift(delta); });

            // shift registros
            Parallel.ForEach(RegistrosDePerfuração, registro => { registro.Shift(delta); });
        }

        #endregion

        #region Map

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            int a = 2;
        }

        #endregion
    }
}