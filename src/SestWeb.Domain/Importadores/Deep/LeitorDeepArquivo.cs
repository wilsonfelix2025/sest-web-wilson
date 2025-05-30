using System.Collections.Generic;
using System.IO;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;

namespace SestWeb.Domain.Importadores.Deep
{
    public abstract class LeitorDeepArquivo
    {
        private PoçoDTO _novoPoço;

        protected LeitorDeepArquivo(string caminhoOuDadoDoArquivo, List<DadosSelecionadosEnum> dadosSelecionados,
            List<PerfilParaImportarDTO> perfisSelecionados, List<LitologiaParaImportarDTO> litologiasSelecionadas,
            Dictionary<string, object> dadosExtras)
        {
            CaminhoOuDadoDoArquivo = caminhoOuDadoDoArquivo;
            DadosSelecionados = dadosSelecionados;
            PerfisSelecionados = perfisSelecionados;
            LitologiasSelecionadas = litologiasSelecionadas;
            DadosExtras = dadosExtras;
            _novoPoço = LerDadosSelecionados();
        }

        protected string CaminhoOuDadoDoArquivo { get; }
        protected List<DadosSelecionadosEnum> DadosSelecionados { get; }
        protected List<LitologiaParaImportarDTO> LitologiasSelecionadas { get; }
        protected List<PerfilParaImportarDTO> PerfisSelecionados { get; }
        protected Dictionary<string, object> DadosExtras { get; }

        public abstract bool TryGetPerfis(out List<PerfilDTO> perfis);
        public abstract bool TryGetLitologias(out List<LitologiaDTO> litologias);
        public abstract bool TryGetTrajetória(out TrajetóriaDTO trajetória);
        public abstract bool TryGetDadosGerais(out DadosGeraisDTO dadosGerais);
        public abstract bool TryGetSapatas(out List<SapataDTO> sapatas);
        public abstract bool TryGetObjetivos(out List<ObjetivoDTO> objetivos);
        public abstract bool TryGetEstratigrafia(out EstratigrafiaDTO estratigrafia);
        public abstract void FazerAçõesIniciais();
        public abstract bool TryGetRegistros(out List<RegistroDTO> registros);
        public abstract bool TryGetEventos(out List<RegistroDTO> eventos);


        public PoçoDTO GetPoço()
        {
            return _novoPoço;
        }

        private PoçoDTO LerDadosSelecionados()
        {
            FazerAçõesIniciais();
            TryGetDadosGerais(out var dadosGerais);
            TryGetPerfis(out var perfis);
            TryGetTrajetória(out var trajetória);
            TryGetLitologias(out var litologias);
            TryGetSapatas(out var sapatas);
            TryGetObjetivos(out var objetivos);
            TryGetEstratigrafia(out var estratigrafia);
            TryGetRegistros(out var registros);
            TryGetEventos(out var eventos);
            _novoPoço = CriarPoço(trajetória, litologias, dadosGerais, perfis, sapatas, objetivos, estratigrafia, registros, eventos);

            return _novoPoço;
        }

        private static PoçoDTO CriarPoço(TrajetóriaDTO trajetória, List<LitologiaDTO> litologias,
            DadosGeraisDTO dadosGerais, List<PerfilDTO> perfis, List<SapataDTO> sapatas, List<ObjetivoDTO> objetivos,
            EstratigrafiaDTO estratigrafia, List<RegistroDTO> registros, List<RegistroDTO> eventos)
        {
            var poço = PoçoDTOFactory.CriarPoçoDTO(trajetória, litologias, dadosGerais, perfis, sapatas, objetivos,
                estratigrafia, registros, eventos);
            return poço;
        }

        protected string[] CarregarArquivo(string caminhoDoArquivo)
        {
            try
            {
                return File.ReadAllLines(caminhoDoArquivo);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Arquivo não encontrado: {caminhoDoArquivo}");
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }
        }
    }
}