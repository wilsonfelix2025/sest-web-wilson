using System.Collections.Generic;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorTrajetoriaSIGEO
    {
        public TrajetóriaDTO Trajetória { get; private set; }
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;

        public LeitorTrajetoriaSIGEO(IReadOnlyList<string> linhas)
        {
            _linhas = linhas;
            Trajetória = new TrajetóriaDTO();
            _leitorGeral = new LeitorHelperSIGEO();
            GetTrajetória();
        }

        private void GetTrajetória()
        {
            var tipoTrajetória = _leitorGeral.ObterDadoDireto("GEOMETRIA DO POCO", _linhas);

            if (tipoTrajetória == "VERTICAL")
            {
                Trajetória.Pontos.Add(new PontoTrajetóriaDTO
                {
                    Pm = _leitorGeral.ObterDadoDireto("MAIOR PROFUNDIDADE ALCANCADA", _linhas),
                    Azimute = "0",
                    Inclinação = "0"
                });
            }
        }


    }
}
