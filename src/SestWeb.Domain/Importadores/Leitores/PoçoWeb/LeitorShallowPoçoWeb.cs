using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Importadores.Shallow;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorShallowPoçoWeb
    {
        private PoçoWebDto PoçoWebDto = new PoçoWebDto();

        public LeitorShallowPoçoWeb(PoçoWebDto poçoWeb)
        {
            PoçoWebDto = poçoWeb;
        }

        public RetornoDTO LerDadosPoçoWeb()
        {
            var temSapatas = new LeitorSapatasPoçoWeb(PoçoWebDto, true).Sapatas.Count > 0;
            var temLitologia = new LeitorLitologiaPoçoWeb(PoçoWebDto, true, null, 0.0).Litologia?.Pontos.Count > 0;
            var temEstratigrafia = new LeitorEstratigrafiaPoçoWeb(PoçoWebDto, true, null, 0.0).Estratigrafia?.Itens.Count > 0;
            var perfis = new LeitorPerfisPoçoWeb(PoçoWebDto, true, null, null).ListaPerfil;
            var trajetória = new LeitorTrajetoriaPoçoWeb(PoçoWebDto, true).Trajetória;
            var listaLitologia = new List<string>();
            var temObjetivos = new LeitorObjetivosPoçoWeb(PoçoWebDto, true, null).Objetivos.Count > 0;

            var listaPerfil = perfis.Select(perfil => new RetornoPerfis(perfil.Nome, TiposPerfil.GeTipoPerfil(perfil.Mnemonico).Mnemônico, GrupoUnidades.GetUnidadePadrão(perfil.Mnemonico).Símbolo)).ToList();

            if (temLitologia)
                listaLitologia.Add("Litologia");

            var retorno = new DadosLidos
            {

                TemSapatas = temSapatas,
                TemObjetivos = temObjetivos,
                TemLitologia = temLitologia,
                TemEstratigrafia = temEstratigrafia,
                TemTrajetória = trajetória.Pontos.Count > 0,
                Perfis = listaPerfil,
                Litologias = listaLitologia,
                TemDadosGerais = true
            };

            return retorno;

        }
    }
}
