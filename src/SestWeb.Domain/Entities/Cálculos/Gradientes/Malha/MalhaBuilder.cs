using System;
using System.Collections.Immutable;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    //Malha de 180
    public class MalhaBuilder
    {
        //Angulo total padrão

        private ImmutableDictionary<double, Malha> _cache = ImmutableDictionary.Create<double, Malha>();
        public DadosMalha DadosMalha { get; }

        public MatrizTensorDeTensoes MatrizTensorDeTensoes { get; }

        private readonly MatrizTensorDeTensoes _matrizTensorDeTensoes;

        public MalhaBuilder(double inclinacao, double azimute, double tensaoMenor, double tensaoMaior, double tensaoVertical, double azimuteMenor)
        {
            DadosMalha = new DadosMalha();
            MatrizTensorDeTensoes = new MatrizTensorDeTensoes(inclinacao, azimute, tensaoMenor, tensaoMaior, tensaoVertical, azimuteMenor);
        }

        public MalhaBuilder(DadosMalha dadosMalha,
            double inclinacao, double azimute, double tensaoMenor, double tensaoMaior, double tensaoVertical, double azimuteMenor)
        {
            DadosMalha = dadosMalha;
            MatrizTensorDeTensoes = new MatrizTensorDeTensoes(inclinacao, azimute, tensaoMenor, tensaoMaior, tensaoVertical, azimuteMenor);
        }

        public Malha Build(double diametro, double poisson, bool isPoroElastico)
        {
            if (_cache.TryGetValue(Math.Round(diametro), out var m))
            {
                return m;
            }

            var malha = new Malha(this.DadosMalha, diametro, MatrizTensorDeTensoes, poisson, isPoroElastico);

            _cache = _cache.Add(Math.Round(diametro, 2), malha);

            return malha;
        }
    }
}
