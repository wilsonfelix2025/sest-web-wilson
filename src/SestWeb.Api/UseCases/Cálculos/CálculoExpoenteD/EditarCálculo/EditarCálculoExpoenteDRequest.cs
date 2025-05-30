using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoExpoenteD.EditarCálculo
{
    public class EditarCálculoExpoenteDRequest
    {
        /// <summary>
        /// ExpoenteD ou ExpoenteDCorrigido
        /// </summary>
        public string Correlação { get; set; }

        /// <summary>
        /// Id do perfil de ROP ou IROP.
        /// </summary>
        public string PerfilROPId { get; set; }

        /// <summary>
        /// Id do perfil de RPM.
        /// </summary>
        public string PerfilRPMId { get; set; }

        /// <summary>
        /// Id do perfil de WOB.
        /// </summary>
        public string PerfilWOBId { get; set; }

        /// <summary>
        /// Id do perfil de DIAM_BROCA.
        /// </summary>
        public string PerfilDIAM_BROCA { get; set; }

        /// <summary>
        /// Id do perfil de ECD (somente para o cálculo de ExpoenteDCorrigido).
        /// </summary>
        public string PerfilECDId { get; set; }

        /// <summary>
        /// Nome do Cálculo
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Id do poço.
        /// </summary>
        public string IdPoço { get; set; }

        /// <summary>
        /// Id do cálculo.
        /// </summary>
        public string IdCálculo { get; set; }
    }
}
