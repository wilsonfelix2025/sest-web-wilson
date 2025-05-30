using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SestWeb.Application.UseCases.UsuárioUseCases.AutenticarUsuario;
using SestWeb.Domain.Usuario;
using SestWeb.Infra;

namespace SestWeb.Api.UseCases.Usuário.AutenticarUsuario
{
    public class Presenter
    {
        private readonly IConfiguration _configuration;

        public Presenter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult ViewModel { get; private set; }

        public void Populate(AutenticarUsuarioOutput output)
        {
            switch (output.Status)
            {
                case AutenticarUsuarioStatus.UsuarioAutenticado:
                    var token = GetToken(output.Usuario);
                    ViewModel = new OkObjectResult(new { email = output.Usuario.Email, username = output.Usuario.UserName, isAdmin = output.Usuario.IsAdmin, emailConfirmado = output.Usuario.EmailConfirmado, token = token });
                    return;
                case AutenticarUsuarioStatus.UsuarioNaoAutenticado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    return;
                default:
                    ViewModel = new BadRequestObjectResult(new { mensagem = "Erro inesperado no servidor!" });
                    return;
            }
        }


        private string GetToken(Usuario usuario)
        {
            var section = _configuration.GetSection("AppSettings");
            var appsettions = section.Get<AppSettings>();
            var secret = appsettions.Secret;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
           
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Id),
                    new Claim(ClaimTypes.Email, usuario.Email),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            tokenDescriptor.Subject.AddClaims(usuario.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
