using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public class LoaderRelacionamentosPropMec : ILoaderRelacionamentosPropMec
    {
        private const string NomeAutor = "Puc-Rio";
        private const string ChaveAutor = "GTEP";
        private static readonly string Origem = Correlações.OrigemCorrelação.Origem.Fixa.ToString();


        public List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> Load(List<Correlação> correlações)
        {
            var corrs = correlações.ToDictionary(c => c.Nome, c => c);

            #region Relacionamentos de Correlações de Propriedades Mecânicas

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Arenitos, Origem, NomeAutor, ChaveAutor,corrs["UCS_MECPRO"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor,corrs["UCS_MECPRO"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_CALCULADO"], corrs["COESA_LAL"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Arenitos, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_SANTOS_ET_AL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Arenitos, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG2"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_GOLUBEV_RABINOVICH"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_LAL"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_LAL"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_SANTOS_ET_AL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_SANTOS_ET_AL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_1"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Carbonatos, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_2"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Ígneas, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Ígneas, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU_1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Arenitos, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_HORSUD"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]);

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Arenitos, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS2"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_50"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Argilosas, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG_ET_AL"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_50"], corrs["RESTR_MECPRO"]);

            #endregion

            #region Relacionamentos de Correlações de Propriedades Mecânicas para o Grupo Litológico Outros

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MECPRO"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CALCULADO"], corrs["COESA_MECPRO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CALCULADO"], corrs["COESA_MECPRO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CALCULADO"], corrs["COESA_MECPRO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CALCULADO"], corrs["COESA_LAL"], corrs["ANGAT_30"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CALCULADO"], corrs["COESA_LAL"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CALCULADO"], corrs["COESA_LAL"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG1"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG1"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG2"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG2"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_GOLUBEV_RABINOVICH"],
                corrs["COESA_MECPRO"], corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_GOLUBEV_RABINOVICH"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_LAL"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_LAL"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_1"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_1"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_2"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_2"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS"], corrs["COESA_MECPRO"], corrs["ANGAT_22_1"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS"], corrs["COESA_LAL"], corrs["ANGAT_22_1"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS1"], corrs["COESA_MECPRO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS1"], corrs["COESA_LAL"], corrs["ANGAT_22_1"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_HORSUD"], corrs["COESA_MECPRO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_HORSUD"], corrs["COESA_LAL"], corrs["ANGAT_22_1"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS2"], corrs["COESA_MECPRO"], corrs["ANGAT_50"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS2"], corrs["COESA_LAL"], corrs["ANGAT_50"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG_ET_AL"], corrs["COESA_MECPRO"],
                corrs["ANGAT_50"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG_ET_AL"], corrs["COESA_LAL"],
                corrs["ANGAT_50"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU_1"], corrs["COESA_MECPRO"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU_1"], corrs["COESA_LAL"],
                corrs["ANGAT_CALCULADO"], corrs["RESTR_MECPRO"]);

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_BREHM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_SANTOS_ET_AL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG2"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG2"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG2"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_GOLUBEV_RABINOVICH"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_GOLUBEV_RABINOVICH"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_GOLUBEV_RABINOVICH"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_LAL"], corrs["COESA_CALCULADO"], corrs["ANGAT_30"],
                corrs["RESTR_MECPRO"]); 
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_LAL"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_LAL"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_CALCULADO"], corrs["ANGAT_30"],
                corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_SANTOS_ET_AL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CPM"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_SANTOS_ET_AL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_MILITZER_STOLL_ADAP"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_2"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_2"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_PRASAD_ET_AL_2"],
                corrs["COESA_CALCULADO"], corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU_1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_30"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU_1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_LAL"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_TEIKOKU_1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_PLUMB"], corrs["RESTR_MECPRO"]);

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS1"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_HORSUD"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_22_1"], corrs["RESTR_MECPRO"]);

            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_REIS2"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_50"], corrs["RESTR_MECPRO"]);
            GerenciadorUcsCoesaAngat.AdicionarRelacionamento(GrupoLitologico.Outros, Origem, NomeAutor, ChaveAutor, corrs["UCS_CHANG_ET_AL"], corrs["COESA_CALCULADO"],
                corrs["ANGAT_50"], corrs["RESTR_MECPRO"]);

            #endregion

            return GetRelacionamentos();
        }

        public bool IsCorrsLoaded()
        {
            return GerenciadorUcsCoesaAngat.IsCorrsLoaded();
        }

        public List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> GetRelacionamentos()
        {
            return GerenciadorUcsCoesaAngat.GetRelacionamentos();
        }
    }
}
