using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarPerfilUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    [Route("api/importar-dados/perfil")]
    [ApiController]
    public class ImportarDadosPerfilController : ControllerBase
    {
        private readonly IImportarDadosPerfilUseCase _importarDadosUseCase;
        private readonly ImportarDadosPresenter _presenter;

        public ImportarDadosPerfilController(IImportarDadosPerfilUseCase importarDadosUseCase, ImportarDadosPresenter presenter)
        {
            _importarDadosUseCase = importarDadosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Faz a importação de dados de perfil da planilha
        /// </summary>
        /// <param name="request">Informações necessárias para a importação dos dados de perfil da planilha.</param>
        /// <returns>O resultado da importação.</returns>
        /// <response code="200">Arquivo importado com sucesso.</response>
        /// <response code="400">O arquivo não foi importado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportarDadosPerfil(ImportarDadosPerfilRequest request)
        {
            Log.Information("Recebendo importação de dados de trajetória: " + request);

            var listaPerfil = PreencherInput(request.Perfis);

            var output = await _importarDadosUseCase.Execute(request.PoçoId, listaPerfil);
            Log.Information("Finalizando importação de dados de trajetória: " + request + " Output:" + output);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }

        private List<PerfilInput> PreencherInput(List<PerfilRequest> listaPerfilRequest)
        {
            var listaInput = new List<PerfilInput>();

            foreach (var perfil in listaPerfilRequest)
            {
                var perfilInput = new PerfilInput
                {
                    Ação = perfil.Ação,
                    Nome = perfil.Nome == string.Empty ? perfil.Tipo : perfil.Nome,
                    NovoNome = perfil.NovoNome,
                    TipoPerfil = PreencherTipoPerfil(perfil.Tipo),
                    TipoProfundidade = perfil.TipoProfundidade,
                    Unidade = perfil.Unidade,
                    ValorBase = perfil.ValorBase,
                    ValorTopo = perfil.ValorTopo
                };

                foreach (var ponto in perfil.PontosPerfil)
                {
                    if (string.IsNullOrWhiteSpace(ponto.Pm) && string.IsNullOrWhiteSpace(ponto.Valor))
                        break;

                    var pontoInput = new PontoPerfilInput
                    {
                        PM = ponto.Pm,
                        Valor = ponto.Valor
                    };

                    perfilInput.PontosPerfil.Add(pontoInput);
                }

                listaInput.Add(perfilInput);
            }

            return listaInput;
        }

        private string PreencherTipoPerfil(string tipo)
        {
            var tipoPerfil = tipo;

            switch (tipo)
            {
                case "BITSIZE":
                    tipoPerfil = "DIAM_BROCA";
                    break;
                case "AZTHm":
                    tipoPerfil = "AZTHmin";
                    break;
                default:                    
                    break;
            }

            return tipoPerfil;

        }
    }
}
