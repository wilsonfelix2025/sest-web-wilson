using System;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Factories
{
    public sealed class ObjetivoFactory
    {
        private readonly IConversorProfundidade _conversor;

        public ObjetivoFactory(IConversorProfundidade conversor)
        {
            _conversor = conversor;
        }

        public Objetivo CriarObjetivo(double pm, TipoObjetivo tipoObjetivo)
        {
            if (!_conversor.TryGetTVDFromMD(pm, out var pv))
            {
                throw new InvalidOperationException("Pm " + pm + " do objetivo fora da trajetória.");
            }

            return new Objetivo(pm, pv, tipoObjetivo);
        }
    }
}
