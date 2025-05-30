using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.NormalizadorCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;
using SestWeb.Domain.Tests.AutofacTests.Common;
using SestWeb.Domain.Tests.AutofacTests.Common.Types;

namespace SestWeb.Domain.Tests.AutofacTests
{
    /// <summary>
    /// Classe de exemplo de teste do Módulo de Domínio.
    /// Herde de DomainModuleArrange
    /// Cadastre os tipos que devem ser registrados e os que devem ser resolvidos.
    /// Override ou não os 3 primeiros testes
    /// Implemente os seus testes
    /// </summary>
    [TestFixture]
    public class Exemplo : DomainModuleArrange
    {
        // Cadastre aqui os tipos que devem ser resolvidos no Modulo do Domínio
        protected override TypesExpectedToBeResolved ExpectedToBeResolved => new TypesExpectedToBeResolved(new List<Type> { typeof(INormalizador) });

        // Cadastre aqui os tipos que devem ser registrados no Modulo do Domínio
        protected override TypesExpectedToBeRegistered ExpectedToBeRegistered => new TypesExpectedToBeRegistered(new List<Type>{ typeof(ICorrelação) });

        #region Overrided Tests

        [Test]
        public override void DeveDispararExcessãoCasoAlgumTipoRegistradoNãoSejaResolvido()
        {
            Assert.Throws<Autofac.Core.DependencyResolutionException>(() => Scope.TryResolveAll(ExpectedToBeIgnored));
        }

        [Test]
        public override void TodosTiposASeremRegistradosSãoRegistrados()
        {
            foreach (var expecterRegisteredType in ExpectedToBeRegistered)
            {
                var type = (Type)expecterRegisteredType.First();
                Check.That(Scope.IsRegistered(type)).IsTrue();
            }
        }

        [Test]
        public override void NãoDeveDispararExcessãoCasoOsTiposASeremResolvidosSãoResolvidos()
        {
            Assert.DoesNotThrow(() => Scope.TryResolve(ExpectedToBeResolved));
        }

        #endregion

        [Test]
        public void DeveDispararExcessãoCasoOTipoNãoSejaResolvido()
        {
            Assert.Throws<Autofac.Core.DependencyResolutionException>(() => Scope.TryResolve(new TypesExpectedToBeResolved(new List<Type> { typeof(ICorrelação) })));
        }

        [Test]
        public void TipoNãoRegistradosNãoDevemSerReconhecidos()
        {
            var type = typeof(TipoNãoRegistrado);
            Check.That(Scope.IsRegistered(type)).IsFalse();
        }

        public class TipoNãoRegistrado { }
    }
}
