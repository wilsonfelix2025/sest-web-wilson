using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Dto;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public class PropMecPossibleCorrsPerLitoGroup
    {
        public GrupoLitologico GrupoLitologico { get; }

        public List<CorrelaçãoDto> UcsPossibleCorrelations { get; }
        
        public List<CorrelaçãoDto> CoesaPossibleCorrelations { get; }
        
        public List<CorrelaçãoDto> AngatPossibleCorrelations { get; }

        public List<CorrelaçãoDto> RestrPossibleCorrelations { get; }

        public List<CorrelaçãoDto> BiotPossibleCorrelations { get; private set; }

        public PropMecPossibleCorrsPerLitoGroup(GrupoLitologico grupoLitologico, List<Correlação> ucsPossibleCorrelations, List<Correlação> coesaPossibleCorrelations, List<Correlação> angatPossibleCorrelations, List<Correlação> restrPossibleCorrelations)
        {
            GrupoLitologico = grupoLitologico;
            UcsPossibleCorrelations = GetPossibleCorrelationsDto(ucsPossibleCorrelations);
            CoesaPossibleCorrelations = GetPossibleCorrelationsDto(coesaPossibleCorrelations);
            AngatPossibleCorrelations = GetPossibleCorrelationsDto(angatPossibleCorrelations);
            RestrPossibleCorrelations = GetPossibleCorrelationsDto(restrPossibleCorrelations);
        }

        public void SetBiotPossibleCorrelations(List<Correlação> biotPossibleCorrelations)
        {
            BiotPossibleCorrelations = GetPossibleCorrelationsDto(biotPossibleCorrelations);
        }

        private List<CorrelaçãoDto> GetPossibleCorrelationsDto(List<Correlação> possibleCorrelations)
        {
            List<CorrelaçãoDto> possibleCorrelationsDto = new List<CorrelaçãoDto>();

            foreach (var corr in possibleCorrelations)
            {
                if(possibleCorrelationsDto.Find(c=>c.Nome.Equals(corr.Nome)) == null)
                    possibleCorrelationsDto.Add(new CorrelaçãoDto(corr.Nome, corr.PerfisSaída.Tipos[0], corr.PerfisEntrada.Tipos, corr.Autor.Chave, corr.Origem.ToString()));
            }

            return possibleCorrelationsDto;
        }
    }
}
