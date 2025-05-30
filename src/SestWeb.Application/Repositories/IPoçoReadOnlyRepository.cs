using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.Repositories
{
    public interface IPoçoReadOnlyRepository
    {
        /// <summary>
        /// Verifica se existe um poço com o id informado.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>True se existe um poço; caso contrário, false.</returns>
        Task<bool> ExistePoço(string id);

        /// <summary>
        /// Obtém todos os poços do banco de forma assíncrona.
        /// </summary>
        /// <returns>Coleção contendo os poços existentes.</returns>
        Task<List<Poço>> ObterPoços();

        /// <summary>
        /// Obtém um poço do banco de forma assíncrona.
        /// </summary>
        /// <param name="id">Id do poço</param>
        /// <param name="buscaPerfis">Se é necessário ir na coleção de perfis e buscar todos os perfis do poço.</param>
        /// <returns>O poço com o id informado se ele existe; caso contrário, null.</returns>
        /// <exception cref="Exception">Lançado quando não consegue converter <paramref name="id"/> em <see cref="ObjectId"/>.</exception>
        Task<Poço> ObterPoço(string id);

        /// <summary>
        /// Obtém um poço do banco de forma assíncrona, sem a lista de perfis.
        /// </summary>
        /// <param name="id">Id do poço</param>
        /// <param name="buscaPerfis">Se é necessário ir na coleção de perfis e buscar todos os perfis do poço.</param>
        /// <returns>O poço com o id informado se ele existe; caso contrário, null.</returns>
        /// <exception cref="Exception">Lançado quando não consegue converter <paramref name="id"/> em <see cref="ObjectId"/>.</exception>
        Task<Poço> ObterPoçoSemPerfis(string id);

        /// <summary>
        /// Verifica se existe algum poço com o nome especificado de forma assíncrona.
        /// </summary>
        /// <param name="nome">Nome a ser verificado.</param>
        /// <returns>True se existe algum poço com o nome informado; caso contrário, false.</returns>
        Task<bool> ExistePoçoComMesmoNome(string nome);

        /// <summary>
        /// Obtém um poço do banco de forma assíncrona pelo seu nome.
        /// </summary>
        /// <param name="nome">Nome a ser verificado.</param>
        /// <returns></returns>
        Task<Poço> GetPoçoByName(string nome);

        /// <summary>
        /// Obtém os dados gerais de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Os dados gerais do poço.</returns>
        Task<DadosGerais> ObterDadosGerais(string id);

        /// <summary>
        /// Obtém a trajetória de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>A trajetória do poço.</returns>
        Task<Trajetória> ObterTrajetória(string id);

        /// <summary>
        /// Obtém a estratigrafia de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>A estratigrafia do poço.</returns>
        Task<Estratigrafia> ObterEstratigrafia(string id);

        /// <summary>
        /// Verifica se existe alguma sapata no poço com a profundidade medida especificada de forma assíncrona.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="profundidadeMedida">Profundidade medida.</param>
        /// <returns>True se existe alguma sapata na profundidade indicada; caso contrário, false.</returns>
        Task<bool> ExisteSapataNaProfundidade(string id, double profundidadeMedida);

        /// <summary>
        /// Verifica se existe algum objetivo no poço com a profundidade medida especificado de forma assíncrona.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="profundidadeMedida">Profundidade medida.</param>
        /// <returns>True se existe algum objetivo na profundidade indicada; caso contrário, false.</returns>
        Task<bool> ExisteObjetivoNaProfundidade(string id, double profundidadeMedida);

        /// <summary>
        /// Obtém as sapatas de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Coleção contendo as sapatas do poço.</returns>
        Task<IReadOnlyCollection<Sapata>> ObterSapatas(string id);

        /// <summary>
        /// Obtém os objetivos de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Coleção contendo os objetivos do poço.</returns>
        Task<IReadOnlyCollection<Objetivo>> ObterObjetivos(string id);

        /// <summary>
        /// Obtém as litologias de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Coleção contendo as litologias do poço.</returns>
        Task<IReadOnlyCollection<Litologia>> ObterLitologias(string id);

        /// <summary>
        /// Obtém uma litologia de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="idLitologia">Id da litologia.</param>
        /// <returns>Coleção contendo as litologias do poço.</returns>
        /// <exception cref="SestException">Lançado quando não consegue converter <paramref name="id"/> em <see cref="ObjectId"/>.</exception>
        Task<Litologia> ObterLitologia(string id, string idLitologia);

        Task<Cálculo> ObterCálculo(string poçoId, string cálculoId);

        /// <summary>
        /// Verifica se há cálculos no banco com o nome passado
        /// </summary>
        /// <param name="nome">nome do cálculo para ser verificado</param>
        /// <param name="idPoço">id do poço que tem os cálculos</param>
        /// <returns>True se existe algum cálculo com mesmo nome; caso contrário, false.</returns>
        Task<bool> ExisteCálculoComMesmoNome(string nome, string idPoço);
    }
}