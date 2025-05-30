using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterAutoresCorrelações
{
    internal class ObterAutoresCorrelaçõesUseCase : IObterAutoresCorrelaçõesUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterAutoresCorrelaçõesUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterAutoresCorrelaçõesOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterAutoresCorrelaçõesOutput.PoçoNãoEncontrado(idPoço);
                }

                var autores = await GetAllAuthors(idPoço);

                if (autores.Count == 0)
                {
                    return ObterAutoresCorrelaçõesOutput.AutoresNãoEncontrados();
                }

                return ObterAutoresCorrelaçõesOutput.AutoresObtidos(autores);
            }
            catch (Exception e)
            {
                return ObterAutoresCorrelaçõesOutput.AutoresNãoObtidos(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<Autor>> GetAllAuthors(string idPoço)
        {
            List<Autor> authors = new List<Autor>();

            var autores = await _correlaçãoReadOnlyRepository.ObterAutoresCorrelações();
            if(autores?.Count> 0)
                authors.AddRange(autores);

            var autoresCorrelaçõesPoço = await _correlaçãoPoçoReadOnlyRepository.ObterAutoresCorrelaçõesPoço(idPoço);
            if(autoresCorrelaçõesPoço?.Count > 0)
                authors.AddRange(autoresCorrelaçõesPoço);

            return authors.Distinct(Autor.AutorComparer).ToList();
        }
    }
}
