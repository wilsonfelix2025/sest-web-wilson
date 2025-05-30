using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.RegistrosEventos;

namespace SestWeb.Domain.Exportadores.Base
{
    public class ExportadorBase: IExportadorBase
    {
        public Poço Poço { get; }
        public List<PerfilBase> Perfis { get; }
        public TipoExportação TipoExportação { get; }
        public ConfiguraçõesExportador Configurações { get; }
        public string CaminhoArquivo { get; protected set; }
        public List<RegistroEvento> Registros { get; protected set; }


        public ExportadorBase(Poço poço, List<PerfilBase> perfis, ConfiguraçõesExportador configurações, TipoExportação tipoExportação)
        {
            Poço = poço;
            Perfis = perfis;
            Configurações = configurações;
            TipoExportação = tipoExportação;
        }

        public ExportadorBase(Poço poço, List<RegistroEvento> registros)
        {
            Poço = poço;
            Registros = registros;
        }
        public virtual byte[] Exportar()
        {
            throw new NotImplementedException("");
        }

        public virtual byte[] ExportarRegistros()
        {
            throw new NotImplementedException("");
        }

        protected double ObterProfundidadePrimeiroPontoComInclinação()
        {
            var pontos = Poço.Trajetória.GetPontos();
            foreach (var ponto in pontos)
            {
                if (ponto.Inclinação != 0 || ponto.Azimute != 0)
                {
                    return ponto.Pm.Valor;
                }                
            }

            return pontos[pontos.Count - 1].Pm.Valor;
        }
    }
}
