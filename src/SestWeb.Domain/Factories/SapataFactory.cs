using System;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Factories
{
    public sealed class SapataFactory
    {
        private readonly IConversorProfundidade _conversor;

        public SapataFactory(IConversorProfundidade conversor)
        {
            _conversor = conversor;
        }

        public Sapata CriarSapata(double pm, string diâmetro)
        {
            if (!_conversor.TryGetTVDFromMD(pm, out var pv))
            {
                throw new InvalidOperationException("Pm " + pm + " da Sapata fora da trajetória");
            }

            return new Sapata(pm, pv, diâmetro);
        }
    }
}
