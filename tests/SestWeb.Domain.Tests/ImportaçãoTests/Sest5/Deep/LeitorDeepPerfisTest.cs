using NFluent;
using NUnit.Framework;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Helpers;
using SestWeb.Domain.Importadores.Deep.Sest5;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SestWeb.Domain.Tests.ImportaçãoTests.Sest5.Deep
{
    public class LeitorDeepPerfisTest
    {
        [Test]
        public void DeveLerSeExistemPerfis_Sucesso()
        {
            const int valorTopo = 0;
            const int valorBase = 3000;

            var arquivo = XDocument.Load("ArquivosEntradaParaTestes/Jo_Editado.xml");
            var perfis = new List<PerfilParaImportarDTO>
            {
                new PerfilParaImportarDTO { Nome = "DTC", NovoNome = "DTC_novo", Ação = Enums.TipoDeAção.Novo, Tipo = "DTC", Unidade = "us/ft", ValorTopo = valorTopo, ValorBase = valorBase },
                new PerfilParaImportarDTO { Nome = "DTS", NovoNome = "DTS_novo", Ação = Enums.TipoDeAção.Novo, Tipo = "DTS", Unidade = "us/ft", ValorTopo = valorTopo, ValorBase = valorBase },
                new PerfilParaImportarDTO { Nome = "YOUNG", NovoNome = "YOUNG_novo", Ação = Enums.TipoDeAção.Novo, Tipo = "YOUNG", Unidade = "psi", ValorTopo = valorTopo, ValorBase = valorBase },
                new PerfilParaImportarDTO { Nome = "GSOBR", NovoNome = "GSOBR_novo", Ação = Enums.TipoDeAção.Novo, Tipo = "GSOBR", Unidade = "lb/gal", ValorTopo = valorTopo, ValorBase = valorBase }
            };

            var leitor = new LeitorPerfisSest5(arquivo, perfis);
            var result = leitor.ObterTodosPerfis();

            Check.That(result).IsNotNull();
            Check.That(result).HasSize(4);
            Check.That(result).ContainsOnlyElementsThatMatch(p => p.PontosDTO.All(pp => pp.Pm.ToDouble() >= valorTopo && pp.Pm.ToDouble() < valorBase));
        }
    }
}
