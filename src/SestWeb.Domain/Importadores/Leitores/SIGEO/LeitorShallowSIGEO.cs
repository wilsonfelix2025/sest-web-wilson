using System.Collections.Generic;
using System.IO;
using System.Linq;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Importadores.Shallow;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorShallowSIGEO
    {
        public string[] Linhas { get; set; }

        public LeitorShallowSIGEO(string caminhoDoArquivo)
        {
            Linhas = CarregarArquivo(caminhoDoArquivo).Where(linha => !string.IsNullOrEmpty(linha) && !string.IsNullOrWhiteSpace(linha)).ToArray();
        }

        public RetornoDTO LerDadosSIGEO()
        {
            var temSapatas = new LeitorSapataSIGEO(Linhas, true).Sapatas.Count > 0;
            var temLitologia = new LeitorLitologiaInterpretadaSIGEO(Linhas, null, true).Litologia?.Pontos.Count > 0;
            var temEstratigrafia = new LeitorEstratigrafiaSIGEO(Linhas, true).Estratigrafia?.Itens.Count > 0;
            var perfil = new LeitorPerfisSIGEO(Linhas, true, null, 0.0).Perfil;
            var trajetória = new LeitorTrajetoriaSIGEO(Linhas).Trajetória;
            var listaPerfil = new List<RetornoPerfis>();
            var listaLitologia = new List<string>();
            var temRegistro = new LeitorRegistrosDePressãoSIGEO(Linhas, true).RegistroDePerfuração?.Pontos.Count > 0;


            if (perfil != null && perfil.PontosDTO.Count > 0)
                listaPerfil.Add(new RetornoPerfis(perfil.Nome, TiposPerfil.GeTipoPerfil(perfil.Mnemonico).Mnemônico, GrupoUnidades.GetUnidadePadrão(perfil.Mnemonico).Símbolo));

            if (temLitologia)
                listaLitologia.Add("Litologia");

            var retorno = new DadosLidos
            {
                
                TemSapatas = temSapatas,
                TemObjetivos = false,
                TemLitologia = temLitologia,
                TemEstratigrafia = temEstratigrafia,
                TemTrajetória = trajetória.Pontos.Count > 0,
                Perfis = listaPerfil,
                Litologias = listaLitologia,
                TemDadosGerais = true,
                TemRegistros = temRegistro,
                TemEventos = false
            };

            return retorno;

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
