using System.Collections.Generic;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada
{
    public interface IPerfisEntrada
    {
        List<string> IdPerfis { get; }
        List<PerfilBase> Perfis { get; }
        bool ContémPerfilPorId(string perfil);
        bool ContémPerfil(string mnemônico);
        bool PossuemPontos();
        
    }
}
