using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada
{
    public class PerfisEntrada : IPerfisEntrada
    {
        private PerfisEntrada(IList<PerfilBase> perfis)
        {
            Perfis = new List<PerfilBase>(perfis);
            IdPerfis = new List<string>();
            IdPerfis.AddRange(Perfis.Select(s => s.Id.ToString()));
        }

        private PerfisEntrada(List<string> ids)
        {
            IdPerfis = new List<string>();
            IdPerfis = ids;
        }

        #region Properties

        public List<PerfilBase> Perfis { get; }

        public List<string> IdPerfis { get; }
        #endregion

        #region Methods

        public bool ContémPerfilPorId(string idPerfil)
        {
            return IdPerfis.Find(perfilEntrada => perfilEntrada == idPerfil) != null;
        }

        public bool ContémPerfil(string mnemônico)
        {
            return Perfis.Find(perfilEntrada => perfilEntrada?.Mnemonico == mnemônico) != null;
        }

        public bool PossuemPontos()
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

            return possuiPontos;
        }

        public static IPerfisEntradaFactory GetFactory()
        {
            return new PerfisEntradaFactory(perfis => new PerfisEntrada(perfis));
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PerfisEntrada)))
                return;


            BsonClassMap.RegisterClassMap<PerfisEntrada>(pe =>
            {
                pe.AutoMap();
                pe.MapCreator(p => new PerfisEntrada(p.IdPerfis));
                pe.UnmapMember(p => p.Perfis);
                pe.MapMember(p => p.IdPerfis);
                pe.SetDiscriminator("PerfisEntrada");
            });
        }

        #endregion
    }
}
