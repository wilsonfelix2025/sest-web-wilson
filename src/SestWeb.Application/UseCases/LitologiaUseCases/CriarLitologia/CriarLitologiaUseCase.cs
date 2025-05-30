using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.LitologiaUseCases.CriarLitologia
{
    internal class CriarLitologiaUseCase : ICriarLitologiaUseCase
    {
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public CriarLitologiaUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<CriarLitologiaOutput> Execute(string idPoço, string tipoLitologia)
        {
            try
            {
                var tipo = TipoLitologia.FromNome(tipoLitologia);
                if (tipo == null)
                {
                    return CriarLitologiaOutput.LitologiaNãoCriada("Tipo de litologia inválido.");
                }


                var litologia = new Litologia(tipo, null);

                var result = await _poçoWriteOnlyRepository.CriarLitologia(idPoço, litologia);

                if (result)
                {
                    return CriarLitologiaOutput.LitologiaCriada();
                }

                return CriarLitologiaOutput.LitologiaNãoCriada();
            }
            catch (Exception e)
            {
                return CriarLitologiaOutput.LitologiaNãoCriada(e.Message);
            }
        }
    }
}
