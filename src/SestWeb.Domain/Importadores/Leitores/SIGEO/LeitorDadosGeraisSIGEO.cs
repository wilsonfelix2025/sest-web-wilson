using System.Collections.Generic;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorDadosGeraisSIGEO
    {
        public DadosGeraisDTO DadosGerais { get; private set; }
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;
        private string categoriaDoPoço;

        public LeitorDadosGeraisSIGEO(IReadOnlyList<string> linhas)
        {
            _linhas = linhas;
            DadosGerais = new DadosGeraisDTO();
            _leitorGeral = new LeitorHelperSIGEO();
            GetDadosGerais();
            GetGeometria();
        }

        private void GetDadosGerais()
        {
            var index = _leitorGeral.EncontrarIdentificadorDeSeção("DADOS GERAIS DE POCOS", _linhas);
            _leitorGeral.CarregarDados(index, _linhas);

            if (index > 0)
            {
                DadosGerais.Identificação.NomePoço = _leitorGeral.ObterDado("PREFIXO");
                DadosGerais.Identificação.NomePoçoLocalImportação = "SIGEO";
                categoriaDoPoço = _leitorGeral.ObterDado("AMBIENTE");
            }

            index = _leitorGeral.EncontrarIdentificadorDeSeção("DADOS DE LOCALIZACAO GEOLOGIC", _linhas);
            _leitorGeral.CarregarDados(index, _linhas);
            if (index > 0)
            {
                DadosGerais.Identificação.Campo = _leitorGeral.ObterDado("CAMPO");
                DadosGerais.Identificação.Bacia = _leitorGeral.ObterDado("BACIA");
            }
        }

        private void GetGeometria()
        {
            switch (categoriaDoPoço)
            {
                case "MAR":
                    {
                        DadosGerais.Geometria.OffShore.LaminaDagua = _leitorGeral.ObterDadoDireto("LAMINA D'AGUA",_linhas);
                        break;
                    }
                case "TERRA":
                    {
                        DadosGerais.Geometria.OnShore.Elevação = _leitorGeral.ObterDadoDireto("COTA ALTIMETRICA", _linhas);
                        DadosGerais.Geometria.OnShore.AlturaDeAntePoço = "0";
                        DadosGerais.Geometria.OnShore.LençolFreático = "0";
                        break;
                    }
            }

            DadosGerais.Geometria.Coordenadas.UtMx = _leitorGeral.ObterDadoDireto(@"E Base", _linhas);
            DadosGerais.Geometria.Coordenadas.UtMy = _leitorGeral.ObterDadoDireto(@"N Base", _linhas);

            DadosGerais.Geometria.MesaRotativa = _leitorGeral.ObterDadoDireto(@"ALTITUDE MESA ROT\(SONDA REF\.PROF\.\)", _linhas, true);

        }


    }
}
