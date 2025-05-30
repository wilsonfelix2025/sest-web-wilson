using System.Collections.Generic;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Trajetoria
{
    public interface IConversorProfundidade
    {
        PontoDeTrajetória PrimeiroPonto { get; }

        PontoDeTrajetória ÚltimoPonto { get; }

        Profundidade PmFinal { get; }

        Profundidade PvFinal { get; }

        Profundidade PmInicial { get; }

        Profundidade PvInicial { get; }

        int Count { get;  }

        bool TryGetMDFromTVD(double tvd, out double md);

        bool TryGetTVDFromMD(double md, out double tvd);

        bool ContémDados();

        bool EstáEmTrechoHorizontal(Profundidade pm, Profundidade pv);

        bool TryGetLastPmHorizontal(Profundidade pv, out Profundidade pm);

        bool TryGetUniquePms(Profundidade pv, out IList<Profundidade> pms);
        bool TryGetPonto(Profundidade pm, out PontoDeTrajetória pontoDeTrajetória);
        IReadOnlyList<PontoDeTrajetória> GetPontos();

    }
}