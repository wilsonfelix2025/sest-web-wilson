using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetTree;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.GetTree
{
    [Route("api/pocoweb/tree")]
    [ApiController]
    public class TreeController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IGetTreeUseCase _useCase;

        public TreeController(IGetTreeUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Obtem a árvore.
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetTree")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTree()
        {
            var output = await _useCase.Execute();

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
