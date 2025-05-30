using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.Upload
{
    public class UploadArquivoRequest
    {
        [Required]
        public IFormFile Arquivo { get; set; }
    }
}
