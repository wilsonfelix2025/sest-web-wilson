
using System;
using System.Collections.Generic;
using System.IO;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;
using SestWeb.Domain.Importadores.Deep;
using SestWeb.Domain.Importadores.Deep.Las;
using SestWeb.Domain.Importadores.Deep.Sest5;
using SestWeb.Domain.Importadores.Deep.SestTR2;
using SestWeb.Domain.Importadores.Leitores.PoçoWeb;
using SestWeb.Domain.Importadores.Leitores.SIGEO;
using SestWeb.Domain.Validadores;
using SestWeb.Domain.Importadores.SestTR1.Deep;
using SestWeb.Domain.Entities.RegistrosEventos;

namespace SestWeb.Domain.Importadores
{
    public class ImportadorArquivo
    {
        private readonly List<DadosSelecionadosEnum> _dadosSelecionados;
        private readonly string _caminhoOuDadoDoArquivo;
        private readonly List<PerfilParaImportarDTO> _perfisSelecionados;
        private readonly List<LitologiaParaImportarDTO> _litologiasSelecionadas;
        private PoçoDTO _poçoArquivo = new PoçoDTO();
        private Result _resultadoImportação = new Result();
        private Poço _poçoAtual { get; }
        private Dictionary<string, object> _dadosExtras;
        private IReadOnlyCollection<RegistroEvento> _registrosDoPoço;

        public ImportadorArquivo(List<DadosSelecionadosEnum> dadosSelecionados, string caminhoOuDadoDoArquivo, List<PerfilParaImportarDTO> perfisSelecionados, List<LitologiaParaImportarDTO> litologiasSelecionadas, Poço poçoAtual, Dictionary<string, object> dadosExtras, IReadOnlyCollection<RegistroEvento> registrosDoPoço)
        {
            _dadosSelecionados = dadosSelecionados;
            _caminhoOuDadoDoArquivo = caminhoOuDadoDoArquivo;
            _perfisSelecionados = perfisSelecionados;
            _litologiasSelecionadas = litologiasSelecionadas;
            _poçoAtual = poçoAtual;
            _dadosExtras = dadosExtras;
            _registrosDoPoço = registrosDoPoço;
        }

        public Result ImportarArquivo()
        {
            LerDadosDoArquivo();
            _resultadoImportação = ValidaECriaEntidade();

            return _resultadoImportação;

        }

        private void LerDadosDoArquivo()
        {
            string extensãoDoArquivo = string.Empty;
            LeitorDeepArquivo leitor;

            if (_dadosExtras.ContainsKey("poçoWeb"))
            {
                extensãoDoArquivo = ".poçoWeb";
            }
            else
            {
                extensãoDoArquivo = GetExtensãoDoArquivo(_caminhoOuDadoDoArquivo);
            }

            switch (extensãoDoArquivo)
            {
                case ".txt":
                     leitor = new LeitorDeepSIGEO(_caminhoOuDadoDoArquivo, _dadosSelecionados, _perfisSelecionados, _litologiasSelecionadas, _dadosExtras);
                    _poçoArquivo = leitor.GetPoço();
                    break;
                case ".las":
                    leitor = new LeitorDeepLas(_caminhoOuDadoDoArquivo, _dadosSelecionados, _perfisSelecionados, _litologiasSelecionadas, _dadosExtras);
                    _poçoArquivo = leitor.GetPoço();
                    break;
                case ".xml":
                    leitor = new LeitorDeepSest5(_caminhoOuDadoDoArquivo, _dadosSelecionados, _perfisSelecionados, _litologiasSelecionadas, _dadosExtras);
                    _poçoArquivo = leitor.GetPoço();
                    break;
                case ".xsrt":
                    leitor = new LeitorDeepSestTR1(_caminhoOuDadoDoArquivo, _dadosSelecionados, _perfisSelecionados, _litologiasSelecionadas, _dadosExtras);
                    _poçoArquivo = leitor.GetPoço();
                    break;
                case ".xsrt2":
                    leitor = new LeitorDeepSestTR2(_caminhoOuDadoDoArquivo, _dadosSelecionados, _perfisSelecionados, _litologiasSelecionadas, _dadosExtras);
                    _poçoArquivo = leitor.GetPoço();
                    break;
                case ".poçoWeb":
                    leitor = new LeitorDeepPoçoWeb(_caminhoOuDadoDoArquivo, _dadosSelecionados, _perfisSelecionados, _litologiasSelecionadas, _dadosExtras);
                    _poçoArquivo = leitor.GetPoço();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private Result ValidaECriaEntidade()
        {
            return PoçoFactory.EditarPoço(_poçoArquivo, _poçoAtual, _perfisSelecionados, _litologiasSelecionadas, _dadosSelecionados, _registrosDoPoço);
        }
        private static string GetExtensãoDoArquivo(string caminhoDoArquivo)
        {
            if (string.IsNullOrWhiteSpace(caminhoDoArquivo))
                throw new Exception("Arquivo não encontrado");

            return Path.GetExtension(caminhoDoArquivo);
        }
    }
}
