using System;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Tests.ProfundidadeTests
{
    [TestFixture]
    public class ProfundidadeTests
    {
        [TestCase(5)]
        [TestCase(9.6)]
        [TestCase(50.11)]
        [TestCase(70.185)]
        public void DeveTruncarCasasAoCriar(double pm)
        {
            var profundidade = new Profundidade(pm);

            Check.That(profundidade.Valor).IsEqualTo((double)Math.Truncate((decimal)pm * 10000000000) / 10000000000);
            //Check.That(profundidade.Valor).IsEqualTo(pm);
        }

        [TestCase(3.0, 3.011)]
        [TestCase(4.01, 4)]
        [TestCase(5.1, 5.112)]
        [TestCase(9.66, 9.6)]
        [TestCase(50.9, 50.999)]
        public void DeveCompararProfundidadesIguais(double pm1, double pm2)
        {
            var profundidade1 = new Profundidade(pm1);
            var profundidade2 = new Profundidade(pm2);

            Check.That(profundidade1).IsEqualTo(profundidade2);
        }

        [TestCase(5, 6)]
        [TestCase(9.6, 10)]
        [TestCase(30, 50.11)]
        [TestCase(70.18, 70.185)]
        public void DeveCompararQueProfundidade1MenorQueProfundidade2(double pm1, double pm2)
        {
            var profundidade1 = new Profundidade(pm1);
            var profundidade2 = new Profundidade(pm2);

            Check.That(profundidade1 < profundidade2);
        }

        [TestCase(6, 5)]
        [TestCase(10, 9.6)]
        [TestCase(50.11, 30)]
        [TestCase(70.185, 70.18)]
        public void DeveCompararQueProfundidade1MaiorQueProfundidade2(double pm1, double pm2)
        {
            var profundidade1 = new Profundidade(pm1);
            var profundidade2 = new Profundidade(pm2);

            Check.That(profundidade1 > profundidade2);
        }
    }
}