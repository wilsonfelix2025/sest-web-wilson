using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class PoçoReadOnlyRepository : IPoçoReadOnlyRepository
    {
        private readonly Context _context;

        public PoçoReadOnlyRepository(Context context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<bool> ExistePoço(string id)
        {
            return await _context.Poços.Find(poço => poço.Id == id).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<List<Poço>> ObterPoços()
        {
            return await _context.Poços.Find(FilterDefinition<Poço>.Empty).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Poço> ObterPoço(string id)
        {
            var poço = await _context.Poços.Find(p => p.Id == id).Limit(1).FirstOrDefaultAsync();

            if (poço != null)
            {
                poço.Perfis = await _context.Perfis.Find(perfil => perfil.IdPoço == id).ToListAsync();
                poço.RegistrosEventos = await _context.RegistrosEventos.Find(regEv => regEv.IdPoço == id).ToListAsync();

                foreach (var perfil in poço.Perfis)
                {
                    if (perfil.ConversorProfundidade == null)
                    {
                        perfil.ConversorProfundidade = poço.Trajetória;
                    }
                }
            }

            return poço;
        }

        /// <inheritdoc />
        public async Task<Poço> ObterPoçoSemPerfis(string id)
        {
            var poço = await _context.Poços.Find(p => p.Id == id).Limit(1).FirstOrDefaultAsync();

            return poço;
        }

        /// <inheritdoc />
        public async Task<bool> ExistePoçoComMesmoNome(string nome)
        {
            return await _context.Poços.Find(poço => poço.DadosGerais.Identificação.Nome == nome).AnyAsync();
        }

        public async Task<Poço> GetPoçoByName(string nome)
        {
            return await _context.Poços.Find(poço => poço.DadosGerais.Identificação.Nome == nome).Limit(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<DadosGerais> ObterDadosGerais(string id)
        {
            return await _context.Poços.Find(poço => poço.Id == id).Project(poço => poço.DadosGerais).Limit(1).FirstOrDefaultAsync();

        }

        /// <inheritdoc />
        public async Task<Trajetória> ObterTrajetória(string id)
        {
            return await _context.Poços.Find(poço => poço.Id == id).Project(poço => poço.Trajetória).Limit(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<Estratigrafia> ObterEstratigrafia(string id)
        {
            return await _context.Poços.Find(poço => poço.Id == id).Project(poço => poço.Estratigrafia).Limit(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExisteSapataNaProfundidade(string id, double profundidadeMedida)
        {
            return _context.Poços.Find(poço => poço.Id == id && poço.Sapatas.Any(s => s.Pm == profundidadeMedida)).AnyAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExisteObjetivoNaProfundidade(string id, double profundidadeMedida)
        {
            return _context.Poços.Find(poço => poço.Id == id && poço.Objetivos.Any(s => s.Pm == profundidadeMedida)).AnyAsync();

        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<Sapata>> ObterSapatas(string id)
        {
            return await _context.Poços.Find(poço => poço.Id == id).Project(poço => poço.Sapatas).Limit(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<Objetivo>> ObterObjetivos(string id)
        {
            return await _context.Poços.Find(poço => poço.Id == id).Project(poço => poço.Objetivos).Limit(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<Litologia>> ObterLitologias(string id)
        {
            return await _context.Poços.Find(poço => poço.Id == id).Project(poço => poço.Litologias).Limit(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<Litologia> ObterLitologia(string id, string idLitologia)
        {
            if (!ObjectId.TryParse(idLitologia, out var mongoIdLitologia))
                throw new InfrastructureException($"O idLitologia {idLitologia} não está em um formato válido.");

            return await _context.Poços.Find(poço => poço.Id == id).Limit(1).Project(poço => poço.Litologias.Find(lito => lito.Id == mongoIdLitologia)).Limit(1).FirstOrDefaultAsync();
        }

        public async Task<Cálculo> ObterCálculo(string poçoId, string cálculoId)
        {
            var poço = await _context.Poços.Find(p => p.Id == poçoId).Limit(1).FirstOrDefaultAsync();
            var calc = poço.Cálculos.First(c => c.Id.ToString() == cálculoId);
            return calc;
        }

        public async Task<bool> ExisteCálculoComMesmoNome(string nome, string idPoço)
        {
            return await _context.Poços.Find(poço => poço.Id == idPoço && poço.Cálculos.Any(s => s.Nome == nome)).AnyAsync();
        }
    }
}