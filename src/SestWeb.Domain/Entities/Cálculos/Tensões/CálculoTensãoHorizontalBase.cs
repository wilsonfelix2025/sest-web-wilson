using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public abstract class CálculoTensãoHorizontalBase
    {
        protected List<PerfilBase> Entradas { get; }
        protected Geometria Geometria { get; }
        protected ILitologia Litologia { get; }
        protected IConversorProfundidade Trajetória { get; }

        public CálculoTensãoHorizontalBase(IList<PerfilBase> entradas, IConversorProfundidade trajetória, Geometria geometria, ILitologia litologia)
        {
            Entradas = entradas.ToList();
            Geometria = geometria;
            Litologia = litologia;
            Trajetória = trajetória;
        }

        protected double[] ObterProfundidades()
        {
            var sincronizadorProfundidades = new SincronizadorProfundidades(Entradas, Trajetória, Litologia, GrupoCálculo.Tensões);
            return sincronizadorProfundidades.GetProfundidadeDeReferência();
        }
    }
}
