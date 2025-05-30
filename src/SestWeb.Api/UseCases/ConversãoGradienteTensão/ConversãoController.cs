using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ConversãoGradienteTensão;
using SestWeb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.ConversãoGradienteTensão
{
    [Route("api/converter-grad-tensao")]
    [ApiController]
    public class ConversãoController : ControllerBase
    {
        private readonly IConversãoGradienteTensãoUseCase _useCase;
        private readonly Presenter _presenter;

        public ConversãoController(IConversãoGradienteTensãoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Converter(ConversãoRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private ConversãoInput PreencherInput(ConversãoRequest request)
        {
            var input = new ConversãoInput
            {
                IdPerfil = request.IdPerfil,
                TipoConversão = PreencherCritério(request.TipoConversão),             
            };

            return input;
        }

        private TipoConversãoEnum PreencherCritério(string tipo)
        {

            switch (tipo)
            {
                case "pressãoAbsoluta":
                    return TipoConversãoEnum.PressãoAbsoluta;
                case "pressãoManométrica":
                    return TipoConversãoEnum.PressãoManométrica;
                case "Gradiente":
                    return TipoConversãoEnum.Gradiente;
                default:
                    return TipoConversãoEnum.NãoIdentificado;
            }
        }

    }
}
