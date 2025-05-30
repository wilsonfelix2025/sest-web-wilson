using SestWeb.Domain.Entities.Perfis.Pontos;
using System.Collections.Generic;
using SestWeb.Domain.DTOs;

namespace SestWeb.Api.UseCases.Perfil.EditarPerfil
{
    public class EditarPerfilRequest
    {
        public string IdPerfil { get; set; }

        public string Nome { get; set; }

        public string Descrição { get; set; }

        public PontoDTO[] PontosDePerfil { get; set; }
        //TODO(Gabriel Pinheiro): por favor, altera no front esse request?
        ///public PontoOld[] PontosDePerfil { get; set; }

        public Dictionary<string, string> EstiloVisual { get; set; }

        public bool EmPv { get; set; } = false;
    }
}