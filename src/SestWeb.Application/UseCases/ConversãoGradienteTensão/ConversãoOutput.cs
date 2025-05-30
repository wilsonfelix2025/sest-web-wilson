using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.ConversãoGradienteTensão
{
    public class ConversãoOutput : UseCaseOutput<ConversãoStatus>
    {
        public ConversãoOutput()
        {

        }

        public PerfilBase Perfil { get; set; }

        public static ConversãoOutput Convertido(PerfilBase perfil)
        {
            return new ConversãoOutput
            {
                Status = ConversãoStatus.Convertido,
                Mensagem = "Conversão feita com sucesso.",
                Perfil = perfil
            };
        }

        public static ConversãoOutput NãoConvertido(string msg)
        {
            return new ConversãoOutput
            {
                Status = ConversãoStatus.NãoConvertido,
                Mensagem = msg
            };
        }
    }
}
