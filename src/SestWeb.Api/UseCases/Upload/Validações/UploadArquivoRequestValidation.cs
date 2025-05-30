using System.IO;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace SestWeb.Api.UseCases.Upload.Validações
{
    public class UploadArquivoRequestValidation : AbstractValidator<UploadArquivoRequest>
    {
        public UploadArquivoRequestValidation(IConfiguration config)
        {
            RuleFor(r => r.Arquivo).NotEmpty();

            // Check the file length. This check doesn't catch files that only have 
            // a BOM as their content.
            RuleFor(r => r.Arquivo.Length).NotEmpty();

            // Valida extensão
            RuleFor(r => Path.GetExtension(r.Arquivo.FileName).ToLowerInvariant()).Must(ExtensõesPermitidas).WithName("Erro")
                .WithMessage("Extensão: '{PropertyValue}' não é válida. Apenas '.txt', '.las', '.xml', '.xsrt', '.xsrt2' são permitidas.");

            var limiteTamanhoArquivo = config.GetValue<long>("LimiteTamanhoArquivo");
            RuleFor(r => r.Arquivo.Length).LessThan(limiteTamanhoArquivo);
        }

        private static bool ExtensõesPermitidas(string extensão)
        {
            string[] extensõesPermitidas = { ".txt", ".las", ".xml", ".xsrt", ".xsrt2" };

            return extensão != null && extensõesPermitidas.Any(ext => extensão == ext);
        }
    }
}
