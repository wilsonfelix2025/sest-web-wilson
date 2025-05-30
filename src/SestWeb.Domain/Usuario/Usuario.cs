using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Usuario
{
    public class Usuario : IEquatable<Usuario>
    {
        public Usuario(string id, string primeiroNome, string segundoNome, string email, bool emailConfirmado, bool isAdmin, DateTime dataCriação, List<string> roles)
        {
            Id = id;
            PrimeiroNome = primeiroNome;
            SegundoNome = segundoNome;
            Email = email;
            EmailConfirmado = emailConfirmado;
            IsAdmin = isAdmin;
            DataCriação = dataCriação;
            Roles = roles;
        }

        public Usuario(string email, string primeiroNome, string segundoNome, string senha)
        {
            Email = email;
            PrimeiroNome = primeiroNome;
            SegundoNome = segundoNome;
        }

        public string Id { get; set; }

        public string PrimeiroNome { get; set; }

        public string SegundoNome { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmado { get; set; }

        public DateTime DataCriação { get; set; }

        public List<string> Roles { get; set; }

        public string CódigoConfirmaçãoEmail { get; set; }

        public string UserName => $"{PrimeiroNome} {SegundoNome}";

        public bool IsAdmin { get; private set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Usuario);
        }

        public bool Equals(Usuario other)
        {
            return other != null &&
                Id == other.Id &&
                Email == other.Email &&
                PrimeiroNome == other.PrimeiroNome &&
                SegundoNome == other.SegundoNome &&
                UserName == other.UserName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Email, PrimeiroNome, SegundoNome, UserName);
        }

        public static bool operator ==(Usuario usuario1, Usuario usuario2)
        {
            return EqualityComparer<Usuario>.Default.Equals(usuario1, usuario2);
        }

        public static bool operator !=(Usuario usuario1, Usuario usuario2)
        {
            return !(usuario1 == usuario2);
        }
    }
}
