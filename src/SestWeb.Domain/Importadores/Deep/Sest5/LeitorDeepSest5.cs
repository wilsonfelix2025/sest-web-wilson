using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.Deep.Sest5.Leitores;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public class LeitorDeepSest5 : LeitorDeepArquivo
    {
        private XDocument _xDoc;

        public LeitorDeepSest5(string caminhoDoArquivo, List<DadosSelecionadosEnum> dadosSelecionados,
            List<PerfilParaImportarDTO> perfisSelecionados, List<LitologiaParaImportarDTO> litologiasSelecionadas,
            Dictionary<string, object> dadosExtras)
            : base(caminhoDoArquivo, dadosSelecionados, perfisSelecionados, litologiasSelecionadas, dadosExtras)
        {
            try
            {
                _xDoc = XDocument.Load(caminhoDoArquivo);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Não foi possivel ler o arquivo: {caminhoDoArquivo}");
            }
        }

        public bool TryObterLitologias(out List<Litologia> litologias)
        {
            try
            {
                var adaptada = LeitorLitologiaAdaptada.ObterLitologiaAdaptada(_xDoc);
                var prevista = LeitorLitologiaPrevista.ObterLitologiaPrevista(_xDoc);

                litologias = new List<Litologia> { adaptada, prevista };
                litologias.RemoveAll(x => x == null);

                if (litologias.Any()) return true;

                litologias = null;

                return true;
            }
            catch (Exception)
            {
                litologias = null;
                return false;
            }
        }

        public override bool TryGetPerfis(out List<PerfilDTO> perfis)
        {
            try
            {
                var leitor = new LeitorPerfisSest5(_xDoc, PerfisSelecionados);
                perfis = leitor.ObterTodosPerfis();
                return true;
            }
            catch (Exception)
            {
                perfis = null;
                return false;
            }
        }

        public override bool TryGetLitologias(out List<LitologiaDTO> litologias)
        {
            try
            {
                LitologiaDTO litologiaAdaptada = null;
                LitologiaDTO litologiaPrevista = null;

                if (LitologiasSelecionadas.Any(lito => lito.Nome == "Litologia"))
                {
                    litologiaAdaptada = LeitorLitologiaSest5.ObterLitologiaAdaptada(_xDoc);
                }

                if (LitologiasSelecionadas.Any(lito => lito.Nome == "Prevista"))
                {
                    litologiaPrevista = LeitorLitologiaPrevistaSest5.ObterLitologiaPrevista(_xDoc);
                }

                litologias = new List<LitologiaDTO> { litologiaAdaptada, litologiaPrevista };
                litologias.RemoveAll(x => x == null);

                return litologias.Any();
            }
            catch (Exception)
            {
                litologias = null;
                return false;
            }
        }

        public override bool TryGetTrajetória(out TrajetóriaDTO trajetória)
        {
            try
            {
                trajetória = null;

                if (DadosSelecionados.Contains(DadosSelecionadosEnum.Trajetória))
                {
                    trajetória = LeitorTrajetóriaSest5.ObterTrajetória(_xDoc);
                }

                return true;
            }
            catch (Exception)
            {
                trajetória = null;
                return false;
            }
        }

        public override bool TryGetDadosGerais(out DadosGeraisDTO dadosGerais)
        {
            try
            {
                dadosGerais = null;

                if(DadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
                {
                    dadosGerais = LeitorDadosGeraisSest5.ObterDadosGerais(_xDoc);
                }

                return true;
            }
            catch (Exception)
            {
                dadosGerais = null;
                return false;
            }
        }

        public override bool TryGetSapatas(out List<SapataDTO> sapatas)
        {
            try
            {
                sapatas = null;

                if (DadosSelecionados.Contains(DadosSelecionadosEnum.Sapatas))
                {
                    sapatas = LeitorSapataSest5.ObterSapatas(_xDoc);
                }

                return true;
            }
            catch (Exception)
            {
                sapatas = null;
                return false;
            }
        }

        public override bool TryGetObjetivos(out List<ObjetivoDTO> objetivos)
        {
            try
            {
                objetivos = null;

                if (DadosSelecionados.Contains(DadosSelecionadosEnum.Objetivos))
                {
                    objetivos = LeitorObjetivosSest5.ObterObjetivos(_xDoc);
                }

                return true;
            }
            catch (Exception)
            {
                objetivos = null;
                return false;
            }
        }

        public override bool TryGetEstratigrafia(out EstratigrafiaDTO estratigrafia)
        {
            try
            {
                estratigrafia = null;

                if (DadosSelecionados.Contains(DadosSelecionadosEnum.Estratigrafia))
                {
                    estratigrafia = LeitorEstratigrafiaSest5.ObterEstratigrafia(_xDoc);
                }

                return true;
            }
            catch (Exception)
            {
                estratigrafia = null;
                return false;
            }
        }

        public override void FazerAçõesIniciais()
        {
            try
            {
                _xDoc = XDocument.Load(CaminhoOuDadoDoArquivo);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Não foi possivel ler o arquivo: {CaminhoOuDadoDoArquivo} - {e.Message}");
            }
        }

        public override bool TryGetRegistros(out List<RegistroDTO> registros)
        {
            try
            {
                registros = new List<RegistroDTO>();

                if (DadosSelecionados.Contains(DadosSelecionadosEnum.Registros))
                {
                    registros = LeitorRegistroEventosSest5.ObterRegistros(_xDoc);
                }

                return true;
            }
            catch (Exception)
            {
                registros = null;
                return false;
            }
        }

        public override bool TryGetEventos(out List<RegistroDTO> eventos)
        {
            try
            {
                eventos = new List<RegistroDTO>();

                if (DadosSelecionados.Contains(DadosSelecionadosEnum.Eventos))
                {
                    eventos = LeitorRegistroEventosSest5.ObterEventos(_xDoc);
                }

                return true;
            }
            catch (Exception)
            {
                eventos = null;
                return false;
            }
        }
    }
}