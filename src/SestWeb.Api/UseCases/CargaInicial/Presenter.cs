using Microsoft.AspNetCore.Mvc;

namespace SestWeb.Api.UseCases.CargaInicial
{
    /// <summary>
    /// Presenter do caso de uso Alimentar banco.
    /// </summary>
    public class Presenter
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel.
        /// </summary>
        /// <param name="isOK">Status da operação.</param>
        public void Populate(bool isOK)
        {
            ViewModel = isOK
                ? new OkObjectResult(new
                    {Status = "OK", Mensagem = "Banco de dados carregado com sucesso"})
                : new OkObjectResult(new {Status = "Not OK", Mensagem = "Operação não realizada"});
        }
    }
}
