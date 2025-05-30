using System;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Importadores.Leitores.PoçoWeb;
using SestWeb.Domain.Importadores.Leitores.SIGEO;
using SestWeb.Domain.Importadores.Shallow.Las;
using SestWeb.Domain.Importadores.Shallow.Sest5;
using SestWeb.Domain.Importadores.SestTR1.Shallow;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Shallow
{
    public class LeitorShallowArquivos : ILeitorShallowArquivos
    {
        public RetornoDTO LerArquivo(TipoArquivo tipoArquivo, string caminho, object obj = null)
        {
            switch (tipoArquivo)
            {
                case TipoArquivo.Sest5:
                    var leitor = new LeitorShallowSest5(caminho);
                    return leitor.LerDadosSest5();
                case TipoArquivo.SestTr1:
                    var leitorSestTr1 = new LeitorShallowSestTR1(caminho);
                    return leitorSestTr1.LerDados();
                case TipoArquivo.SestTr2:
                    var leitorSestTr2 = new LeitorShallowSestTR2(caminho);
                    return leitorSestTr2.LerDados();
                case TipoArquivo.Las:
                    var leitorLas = new LeitorShallowLas(caminho);
                    return leitorLas.LerDados();
                case TipoArquivo.Sigeo:
                    var leitorSIGEO = new LeitorShallowSIGEO(caminho);
                    return leitorSIGEO.LerDadosSIGEO();
                case TipoArquivo.PoçoWeb:
                    var leitorPoçoWeb = new LeitorShallowPoçoWeb(obj as PoçoWebDto);
                    return leitorPoçoWeb.LerDadosPoçoWeb();
                default:
                    throw new ArgumentOutOfRangeException(nameof(tipoArquivo), tipoArquivo, null);
            }
        }
    }
}
