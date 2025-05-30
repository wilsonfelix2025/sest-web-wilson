using System.Collections.Generic;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.LitologiaDoPoco
{
    public interface ILitologia
    {
        bool ObterGrupoLitológicoNessaProfundidade(double profundidade, out int valorGrupoLitológico);
        PerfilBase GetDTMC(double[] profundidadesReferência, TipoProfundidade tipoProfundidade, IConversorProfundidade trajetória, string nomeCálculo);
        PerfilBase GetRHOG(double[] profundidadeDeReferência, TipoProfundidade tipoProfundidade, IConversorProfundidade trajetória, string nomeCálculo);
        ObjectId Id { get; }
        TipoLitologia Classificação { get; }
        IReadOnlyList<PontoLitologia> Pontos { get; }
        //Pontos<PontoLitologia> PontosDeLitologia { get; }
        bool ContémPontos();
        Profundidade PmTopo { get; }
        Profundidade PmBase { get; }
        bool TryGetPontoEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia ponto);
        IReadOnlyList<PontoLitologia> GetPontos();
        bool TryGetLitoPontoEmPm(IConversorProfundidade conversorProfundidade, Profundidade pm, out PontoLitologia ponto);
        PontoLitologia UltimoPonto { get; }
        bool ObterTipoRochaNaProfundidade(double pm, out TipoRocha grupo);
    }
}