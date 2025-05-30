using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Usuario;
using SestWeb.Infra.Exceptions;
using SestWeb.Infra.MongoDataAccess.Entities;
using SestWeb.Infra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.MongoDataAccess.Repositories
{
    internal class UsuarioRepository : IUsuarioReadOnlyRepository, IUsuarioWriteOnlyRepository
    {
        private readonly Context _context;
        private readonly EmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsuarioRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, EmailService emailService, Context context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _context = context;
        }

        private readonly Dictionary<string, string> _erros = new Dictionary<string, string>
        {
            ["PasswordMismatch"] = "Senha incorreta.",
            ["InvalidToken"] = "Token inválido.",
            ["LoginAlreadyAssociated"] = "Já existe um usuário com este login.",
            ["DuplicateUserName"] = "Este login já está sendo utilizado."
        };

        private async Task ValidarEmailUnico(string email)
        {
            var emailUser = await _userManager.FindByNameAsync(email);
            if (emailUser != null)
            {
                throw new InfrastructureException("Este e-mail já está sendo utilizado.");
            }
        }

        private Usuario GetUsuario(ApplicationUser user)
        {
            var isAdmin = user.Roles.Any(x => x == ApplicationRole.ADMIN_ROLE);
            var dataCriação = new ObjectId(user.Id).CreationTime;
            return new Usuario(user.Id, user.PrimeiroNome, user.SegundoNome, user.Email, user.EmailConfirmed, isAdmin,
                dataCriação, user.Roles);
        }

        public async Task<Usuario> AddUser(string email, string primeiroNome, string segundoNome, string senha)
        {
            primeiroNome = primeiroNome.Trim();
            segundoNome = segundoNome.Trim();
            email = email.Trim();

            var user = new ApplicationUser
            {
                Email = email,
                PrimeiroNome = primeiroNome,
                SegundoNome = segundoNome,
                UserName = email
            };

            await ValidarEmailUnico(email);

            var result = await _userManager.CreateAsync(user, senha);

            if (!result.Succeeded)
            {
                var erro = result.Errors.First();
                if (_erros.TryGetValue(erro.Code, out var mensagem))
                {
                    throw new InfrastructureException(mensagem);
                }

                throw new InfrastructureException("Não foi possível registrar o usuário");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailService.EnviarEmailConfirmação(user.Id, user.Email, code);
            var usuario = GetUsuario(user);
            usuario.CódigoConfirmaçãoEmail = code;
            return usuario;
        }

        public async Task<bool> ConfirmarEmail(string idUsuario, string codigo)
        {
            var user = await _userManager.FindByIdAsync(idUsuario);

            if (user == null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, codigo);

            if (result.Succeeded)
            {
                return true;
            }

            return false;
        }

        public async Task<string> EnviarEmailDeRecuperaçãoSenha(string email)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    return null;
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _emailService.EnviarEsqueceuSenha(email, code);
                return code;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<string>> GetAdminEmails()
        {
            try
            {
                return await _context.Usuarios.Find(x => x.Roles.Any(r => r == ApplicationRole.ADMIN_ROLE)).Project(x => x.Email).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Usuario>> GetAll()
        {
            try
            {
                var users = await _context.Usuarios.Find(_ => true).ToListAsync();
                var usuarios = new List<Usuario>();
                foreach (var applicationUser in users)
                {
                    usuarios.Add(GetUsuario(applicationUser));
                }

                return usuarios;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Usuario> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
            {
                return null;
            }

            return GetUsuario(user);
        }

        public async Task<Usuario> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new UsuarioNaoExisteOuNaoConfirmadoException();
            }

            return GetUsuario(user);
        }

        public async Task<Usuario> GetUserByPassword(string email, string senha)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
            {
                throw new FalhaAutenticacaoUsuarioInvalido();
            }

            var isSenhaValida = await _userManager.CheckPasswordAsync(user, senha);

            if (!isSenhaValida)
            {
                throw new FalhaAutenticacaoSenhaInvalidaException();
            }

            return GetUsuario(user);
        }

        public async Task<bool> ReenviarEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user == null || await _userManager.IsEmailConfirmedAsync(user))
                {
                    return false;
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _emailService.EnviarEmailConfirmação(user.Id, user.Email, code);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<bool> RemoveUser(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ResetarSenha(string email, string novaSenha, string codigo)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    return false;
                }

                var result = await _userManager.ResetPasswordAsync(user, codigo, novaSenha);
                return result.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<bool> TornarAdmin(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TrocarSenha(string idUsuario, string senhaAntiga, string novaSenha)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(idUsuario);

                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    return false;
                }

                var resultadoTrocaSenha = await _userManager.ChangePasswordAsync(user, senhaAntiga, novaSenha);
                return resultadoTrocaSenha.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
