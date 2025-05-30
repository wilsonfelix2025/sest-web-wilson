using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões
{
    public class CriarPerfilRelaçãoInput
    {
        public List<ValoresInput> Valores { get; set; }
        public string TipoRelação { get; set; }
        public string IdPoço { get; set; }
        public string NomePerfil { get; set; }
    }
}
