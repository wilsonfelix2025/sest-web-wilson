using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçãoPeloNome
{
    internal class ObterCorrelaçãoPeloNomeUseCase : IObterCorrelaçãoPeloNomeUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterCorrelaçãoPeloNomeUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterCorrelaçãoPeloNomeOutput> Execute(string idPoço, string name)
        {
            try
            {
                var correlação = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(name);

                if (correlação == null)
                {
                    var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                    if (!existePoço)
                    {
                        return ObterCorrelaçãoPeloNomeOutput.PoçoNãoEncontrado(idPoço);
                    }

                    var correlaçãoPoço = _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçãoPoçoPeloNome(idPoço,name);
                    
                    if(correlaçãoPoço == null)
                        return ObterCorrelaçãoPeloNomeOutput.CorrelaçãoNãoEncontrada(name);

                    correlação = correlaçãoPoço.Result;
                }

                return ObterCorrelaçãoPeloNomeOutput.CorrelaçãoObtida(correlação);
            }
            catch (Exception e)
            {
                return ObterCorrelaçãoPeloNomeOutput.CorrelaçãoNãoObtida(e.Message);
            }
        }
    }
}
