using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateOpUnit
{
    internal class CreateOpUnitUseCase : ICreateOpUnitUseCase
    {
        private readonly IOpUnitWriteOnlyRepository _repository;

        public CreateOpUnitUseCase(IOpUnitWriteOnlyRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateOpUnitOutput> Execute(string name, string url)
        {
            try
            {
                OpUnit opUnit = new OpUnit(url, name);
                await _repository.CreateOpUnit(opUnit);
                return CreateOpUnitOutput.OpUnitCreatedSuccesfully(opUnit);
            }
            catch (Exception ex)
            {
                return CreateOpUnitOutput.OpUnitNotCreated(ex.Message);
            }
        }
    }
}
