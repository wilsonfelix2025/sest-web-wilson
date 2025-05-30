using SestWeb.Domain.DTOs;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorDadosGeraisTR2
    {
        public DadosGeraisDTO DadosGerais { get; private set; }
        private string _nomePoço { get; set; }

        public LeitorDadosGeraisTR2(string nomePoço) 
        {
            _nomePoço = nomePoço;
            Reset();
        }

        public void ProcessaLinha(string linha)
        {
            linha = linha.Trim();

            if (linha.Contains("<d5p1:Bacia"))
                DadosGerais.Identificação.Bacia = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:Campo"))
                DadosGerais.Identificação.Campo = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:AlturaDeAntePoço"))
                DadosGerais.Geometria.OnShore.AlturaDeAntePoço = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:Elevação"))
                DadosGerais.Geometria.OnShore.Elevação = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:LençolFreático"))
                DadosGerais.Geometria.OnShore.LençolFreático = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:LaminaDagua"))
                DadosGerais.Geometria.OffShore.LaminaDagua = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:MesaRotativa"))
                DadosGerais.Geometria.MesaRotativa = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:DensidadeSuperfície"))
                DadosGerais.Area.DensidadeSuperficie = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:DensidadeÁguaDoMar"))
                DadosGerais.Area.DensidadeAguaMar = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d5p1:SônicoSuperfície"))
                DadosGerais.Area.SonicoSuperficie = LeitorHelperTR2.ObterValorTag(linha);
        }

        public void Reset()
        {
            IdentificaçãoDTO identificação = new IdentificaçãoDTO
            {
                NomePoço = _nomePoço,
                Bacia = "",
                Campo = "",
                NomePoçoLocalImportação = "SEST"
            };

            OnShoreDTO onShore = new OnShoreDTO
            {
                AlturaDeAntePoço = "",
                Elevação = "",
                LençolFreático = ""
            };

            OffShoreDTO offShore = new OffShoreDTO
            {
                LaminaDagua = ""
            };

            GeometriaDTO geometria = new GeometriaDTO
            {
                OnShore = onShore,
                OffShore = offShore,
                MesaRotativa = "",
            };

            AreaDTO area = new AreaDTO
            {
                DensidadeAguaMar = "",
                DensidadeSuperficie = "",
                SonicoSuperficie = ""
            };

            DadosGerais = new DadosGeraisDTO
            {
                Area = area,
                Geometria = geometria,
                Identificação = identificação
            };
        }
    }
}
