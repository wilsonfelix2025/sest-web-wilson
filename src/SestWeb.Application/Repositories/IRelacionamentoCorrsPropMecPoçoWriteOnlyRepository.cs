using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec;

namespace SestWeb.Application.Repositories
{
    public interface IRelacionamentoCorrsPropMecPoçoWriteOnlyRepository
    {
        Task CriarRelacionamentoCorrsPropMecPoço(RelacionamentoPropMecPoço relacionamentoPropMecPoço);
        Task<bool> RemoverRelacionamentoCorrsPropMecPoço(string nome);
        Task<bool> UpdateRelacionamentoCorrsPropMecPoço(RelacionamentoPropMecPoço relacionamentoPropMecPoço);
    }
}
