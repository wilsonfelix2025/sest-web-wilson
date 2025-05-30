using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.Base.EmCriação
{
    public abstract class CálculoEmCriação
    {
        public string Nome { get; }
        public string GrupoCálculo { get; }
        public IList<PerfilBase> PerfisEntrada { get; }
        public IList<PerfilBase> PerfisSaída { get; }
        public Trajetória Trajetória { get; }
        public ILitologia Litologia { get; }

        public CálculoEmCriação(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia)
        {
            Nome = nome;
            GrupoCálculo = grupoCálculo;
            PerfisEntrada = perfisEntrada;
            PerfisSaída = perfisSaída;
            Trajetória = trajetória;
            Litologia = litologia;
        }
    }
}
