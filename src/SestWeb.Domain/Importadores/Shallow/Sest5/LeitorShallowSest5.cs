using System;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public class LeitorShallowSest5
    {
        private readonly XDocument _xDoc;

        public LeitorShallowSest5(string caminho)
        {
            try
            {
                _xDoc = XDocument.Load(caminho);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Não foi possível ler o arquivo {caminho}");
            }
        }

        public DadosLidos LerDadosSest5()
        {
            try
            {
                var temDadosGerais = true; // SEST 5 sempre tem dados gerais
                var temTrajetória = true; // SEST 5 sempre tem trajetória
                var temSapatas = LeitorShallowSapatas.LerSapatas(_xDoc);
                var temObjetivos = LeitorShallowObjetivos.LerObjetivos(_xDoc);
                var litologias = LeitorShallowLitologia.LerLitologia(_xDoc);
                var temEstratigrafia = LeitorShallowEstratigrafia.LerEstratigrafia(_xDoc);
                var mnemônicos = LeitorShallowPerfis.LerPerfis(_xDoc);
                var temRegistros = LeitorShallowRegistrosSest5.LerRegistros(_xDoc);
                var temEventos = LeitorShallowRegistrosSest5.LerEventos(_xDoc);


                var perfis = mnemônicos.Select(mnemônico => new RetornoPerfis(mnemônico, TiposPerfil.GeTipoPerfil(mnemônico).Mnemônico, GrupoUnidades.GetUnidadePadrão(TiposPerfil.GeTipoPerfil(mnemônico).Mnemônico).Símbolo)).ToList();

                var retorno = new DadosLidos
                {
                    TemDadosGerais = temDadosGerais,
                    TemTrajetória = temTrajetória,
                    TemSapatas = temSapatas,
                    TemObjetivos = temObjetivos,
                    Litologias = litologias,
                    TemEstratigrafia = temEstratigrafia,
                    Perfis = perfis,
                    TemEventos = temEventos,
                    TemRegistros = temRegistros
                };

                return retorno;
            }
            catch (Exception e)
            {
                throw new Exception($"LeitorShallowSest5 - Não foi possível ler dados - {e.Message}");
            }
        }
    }
}