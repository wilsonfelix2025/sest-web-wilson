using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída
{
    public class PerfisSaída : IPerfisSaída
    {
        private PerfisSaída(IList<PerfilBase> perfis)
        {
            Perfis = new List<PerfilBase>(perfis);
            IdPerfis = new List<string>();
            IdPerfis.AddRange(Perfis.Select(s => s.Id.ToString()));
        }

        private PerfisSaída(List<string> ids)
        {
            IdPerfis = new List<string>();
            IdPerfis = ids;
        }
        #region Properties

        public List<string> IdPerfis { get; set; }
        public List<PerfilBase> Perfis { get; }

        #endregion

        #region Methods

        public void AtualizarPvs(IConversorProfundidade conversor)
        {
            Parallel.ForEach(Perfis, perfil => { perfil.AtualizarPvs(conversor); });
        }

        public void ApagarPontos()
        {
            Parallel.ForEach(Perfis, perfil => { perfil.Clear(); });
        }

        public void Renomear(string nomeCálculoNovo, string nomeCálculoAntigo)
        {
            Parallel.ForEach(Perfis, perfil =>
            {
                var novoNome = perfil.Nome.Replace(nomeCálculoAntigo, nomeCálculoNovo);
                perfil.EditarNome(novoNome);
            });
        }

        public bool TemSaidaZerada()
        {
            bool possuiPontos = true;

            if (Perfis != null)
            {
                Parallel.ForEach(Perfis, (perfilEntrada, state) =>
                {
                    if (!perfilEntrada.ContémPontos())
                    {
                        possuiPontos = false;
                        state.Stop();
                    }
                });
            }

            return !possuiPontos;
        }

        public static IPerfisSaídaFactory GetFactory()
        {
            return new PerfisSaídaFactory(perfis => new PerfisSaída(perfis));
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PerfisSaída)))
                return;


            BsonClassMap.RegisterClassMap<PerfisSaída>(ps =>
            {
                ps.AutoMap();
                ps.MapCreator(p => new PerfisSaída(p.IdPerfis));
                ps.UnmapMember(p => p.Perfis);
                ps.MapMember(p => p.IdPerfis);
                ps.SetDiscriminator("PerfisSaída");
            });
        }

        #endregion
    }
}
