using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Application.Repositories
{
    public interface IPoçoWriteOnlyRepository
    {
        /// <summary>
        /// Insere um poço no banco de forma assíncrona.
        /// </summary>
        /// <param name="poço">Poço a ser inserido.</param>
        Task CriarPoço(Poço poço);

        /// <summary>
        /// Insere um poço no banco de forma assíncrona, como os dados de outro poço, com novos ids.
        /// </summary>
        /// <param name="poço">Poço a ser inserido.</param>
        Task DuplicarPoço(Poço poço);
        /// <summary>
        /// Remove um poço do banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço</param>
        /// <returns>True se conseguiu remover o poço; caso contrário, false.</returns>
        /// <exception cref="Exception">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        Task<bool> RemoverPoço(string idPoço);

        /// <summary>
        /// Atualiza um poço no banco de forma assíncrona.
        /// </summary>
        /// <param name="poço">Poço atualizado.</param>
        /// <returns>True se conseguiu fazer a atualização; caso contrário, false.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando <paramref name="poço"/> é nulo.</exception>
        Task<bool> AtualizarPoço(Poço poço);

        /// <summary>
        /// Atualiza os dados gerais de um poço no banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="entity">A entidade poço </param>
        /// <param name="atualizaTrajetória">True/False para atualizar a trajetória</param>
        /// <returns>True se conseguiu fazer a atualização; caso contrário, false.</returns>
        /// <exception cref="Exception">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        Task<bool> AtualizarDadosGerais(string idPoço, Poço entity, bool atualizaTrajetória);

        /// <summary>
        /// Atualiza os dados de um poço no banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="entity">A entidade poço </param>
        /// <param name="atualizaTrajetória">True/False para atualizar a trajetória</param>
        /// <returns>True se conseguiu fazer a atualização; caso contrário, false.</returns>
        /// <exception cref="Exception">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        Task<bool> AtualizarDados(string idPoço, Poço entity, List<DadosSelecionadosEnum> dadosSelecionados);
        /// <summary>
        /// Insere uma sapata no banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="sapata">Sapata a ser inserida.</param>
        /// <returns>True se conseguiu inserir a sapata; caso contrário, false.</returns>
        /// <exception cref="Exception">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        Task<bool> CriarSapata(string idPoço, Sapata sapata);

        /// <summary>
        /// Remove uma sapata do banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="profundidadeMedida">Profundidade medida da sapata.</param>
        /// <returns>True se conseguiu remover a sapata; caso contrário, false.</returns>
        Task<bool> RemoverSapata(string idPoço, double profundidadeMedida);

        /// <summary>
        /// Insere um objetivo no banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="objetivo">Objetivo a ser inserido.</param>
        /// <returns>True se conseguiu inserir o objetivo; caso contrário, false.</returns>
        /// <exception cref="Exception">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        Task<bool> CriarObjetivo(string idPoço, Objetivo objetivo);

        /// <summary>
        /// Remove um objetivo do banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="profundidadeMedida">Profundidade medida do objetivo.</param>
        /// <returns>True se conseguiu remover o objetivo; caso contrário, false.</returns>
        Task<bool> RemoverObjetivo(string idPoço, double profundidadeMedida);

        /// <summary>
        /// Insere uma litologia no banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="litologia">Litologia a ser inserida.</param>
        /// <returns>True se conseguiu inserir a litologia; caso contrário, false.</returns>
        /// <exception cref="Exception">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        Task<bool> CriarLitologia(string idPoço, Litologia litologia);

        /// <summary>
        /// Insere uma litologia do banco de forma assíncrona.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="idLitologia">Id da litologia.</param>
        /// <returns>True se conseguiu remover a litologia; caso contrário, false.</returns>
        /// <exception cref="Exception">Lançado quando <paramref name="idPoço"/> ou <paramref name="idLitologia"/> não está em um formato válido.</exception>
        Task<bool> RemoverLitologia(string idPoço, string idLitologia);

        /// <summary>
        /// Atualiza a estratigrafia de um poço.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="estratigrafia">Estratigrafia</param>
        /// <returns><c>true</c> se conseguiu fazer a atualização; caso contrário, <c>false</c>.</returns>
        /// <exception cref="InfrastructureException">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        /// <exception cref="ArgumentNullException">Lançado se <paramref name="estratigrafia"/> é nulo.</exception>
        Task<bool> AtualizarEstratigrafia(string idPoço, Estratigrafia estratigrafia);

        /// <summary>
        /// Atualiza a estratigrafia de um poço.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="sapatas">Lista de sapatas</param>
        /// <returns><c>true</c> se conseguiu fazer a atualização; caso contrário, <c>false</c>.</returns>
        /// <exception cref="InfrastructureException">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        /// <exception cref="ArgumentNullException">Lançado se <paramref name="sapatas"/> é nulo.</exception>
        Task<bool> AtualizarSapatas(string idPoço, List<Sapata> sapatas);

        /// <summary>
        /// Atualiza a estratigrafia de um poço.
        /// </summary>
        /// <param name="idPoço">Id do poço.</param>
        /// <param name="objetivos">Lista de objetivos</param>
        /// <returns><c>true</c> se conseguiu fazer a atualização; caso contrário, <c>false</c>.</returns>
        /// <exception cref="InfrastructureException">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        /// <exception cref="ArgumentNullException">Lançado se <paramref name="objetivos"/> é nulo.</exception>
        Task<bool> AtualizarObjetivos(string idPoço, List<Objetivo> objetivos);

        /// <summary>
        /// Atualiza a trajetória de um poço
        /// </summary>
        /// <param name="idPoço">Id do poço</param>
        /// <param name="trajetória">Trajetória</param>
        /// <returns><c>true</c> se conseguiu fazer a atualização; caso contrário, <c>false</c>.</returns>
        /// <exception cref="InfrastructureException">Lançado quando <paramref name="idPoço"/> não está em um formato válido.</exception>
        Task<bool> AtualizarTrajetória(string idPoço, Trajetória trajetória);

        Task<bool> AtualizarLitologias(string idPoço, Poço poçoAtualizado);
        Task<bool> CriarCálculo(Poço poço, Cálculo cálculo, string grupoCálculo, bool perfuracao = false);
        Task<bool> RemoverCálculo(string idPoço, string idCálculo);
        Task<bool> EditarCálculo(Poço poço, Cálculo cálculo, string idCálculoAntigo, string grupoCálculoState);
        Task<bool> AtualizarListaPoçoApoio(string idPoço, List<string> lista);

    }
}