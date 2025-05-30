using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Importadores.Deep.Sest5.Leitores;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    [Obsolete("Lógica foi migrada para a classe LeitorDeepSest5.")]
    public class LeituraArquivoSest5
    {
        public string Path { get; }
        private readonly XDocument _xDoc;

        public LeituraArquivoSest5(string path)
        {
            Path = path;

            try
            {
                _xDoc = XDocument.Load(Path);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Não foi possivel ler o arquivo: {path}");
            }
        }

        public bool TryObterPerfis(out List<PerfilBase> perfis, IConversorProfundidade conversorProfundidade, ILitologia litologia)
        {
            try
            {
                var leitorPerfis = new LeitorPerfis(_xDoc, conversorProfundidade, litologia);
                perfis = leitorPerfis.ObterTodosPerfis();
                return true;
            }
            catch (Exception)
            {
                perfis = null;
                return false;
            }
        }

        public bool TryObterDadosGerais(out DadosGerais dadosGerais)
        {
            try
            {
                dadosGerais = LeitorDadosPoço.ObterDadosPoço(_xDoc);
                return true;
            }
            catch (Exception)
            {
                dadosGerais = null;
                return false;
            }
        }

        public bool TryObterSapatas(out List<SapataDTO> sapatas)
        {
            try
            {
                sapatas = LeitorSapatas.ObterSapatas(_xDoc);
                return true;
            }
            catch (Exception)
            {
                sapatas = null;
                return false;
            }
        }

        public bool TryObterObjetivos(out List<ObjetivoDTO> objetivos)
        {
            try
            {
                objetivos = LeitorObjetivos.ObterObjetivos(_xDoc);
                return true;
            }
            catch (Exception)
            {
                objetivos = null;
                return false;
            }
        }

        public bool TryObterTrajetória(out Trajetória trajetória)
        {
            try
            {
                trajetória = LeitorTrajetória.ObterTrajetória(_xDoc);
                return true;
            }
            catch (Exception)
            {
                trajetória = null;
                return false;
            }
        }

        public bool TryObterEstratigrafia(Trajetória trajetória ,out Estratigrafia estratigrafia)
        {
            try
            {
                estratigrafia = LeitorEstratigrafia.ObterEstratigrafia(trajetória, _xDoc);
                return true;
            }
            catch (Exception)
            {
                estratigrafia = null;
                return false;
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
    }
}