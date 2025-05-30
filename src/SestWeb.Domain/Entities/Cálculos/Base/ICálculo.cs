using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Base
{
    public interface ICálculo : ISupportInitialize
    {
        ObjectId Id { get; }

        string Nome { get; }

        GrupoCálculo GrupoCálculo { get; }

        IPerfisEntrada PerfisEntrada { get; }

        IPerfisSaída PerfisSaída { get; }

        IConversorProfundidade ConversorProfundidade { get; } 

        ILitologia Litologia { get; }

        bool PerfisEntradaPossuemPontos { get; }

        bool TemSaídaZerada { get; }

        void Execute(bool chamadaPelaPipeline = false);

        void AtualizarPvsSaídas(IConversorProfundidade conversor);

        void ZerarPerfisSaídaAfetados(PerfilBase perfil);// TODO(RCM): discutir passar id ou nome

        bool ContémPerfilEntradaPorId(string perfil);// TODO(RCM): discutir passar id ou nome

        List<string> GetTiposPerfisEntradaFaltantes();

        List<PerfilBase> GetPerfisEntradaSemPontos();
    }
}
