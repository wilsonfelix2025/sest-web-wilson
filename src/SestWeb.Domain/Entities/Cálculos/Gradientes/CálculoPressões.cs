using System.Collections.Generic;
using SestWeb.Domain.DTOs.Cálculo.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes
{
    public class CálculoPressões
    {
        public static Pressoes Calcular(DadosMalha dadosMalha, EntradasColapsos entrada, CritérioRupturaGradientesEnum tipoCritérioRuptura)
        {
            var fraturas = new Fraturas(dadosMalha, entrada);
            fraturas.Calcular(entrada.EhFluidoPenetrante, entrada.EhPoroelastico);  

            var colapsos = new Colapsos.Colapsos(dadosMalha, entrada, tipoCritérioRuptura);
            colapsos.Calcular(entrada.EhFluidoPenetrante, entrada.EhPoroelastico, fraturas.FI, fraturas.FS);
            
            return new Pressoes
            {
                Colapsos = colapsos,
                Fraturas = fraturas,
            };
        }

        public static List<CampoResult> CalcularPorCampo(DadosMalha dadosMalha, EntradasColapsos entrada, double pw)
        {
            var calcularPressoesPorCampo = new CálculoPressõesPorCampo(dadosMalha, entrada);
            return calcularPressoesPorCampo.CalcularCampo(pw);
        }
    }

    public class Pressoes
    {
        public Colapsos.Colapsos Colapsos { get; set; }
        public Fraturas Fraturas { get; set; }
    }
}
