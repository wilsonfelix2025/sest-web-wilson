using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.AtualizarDadosGerais
{
    [TestFixture]
    public class AtualizarDadosGeraisTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveAtualizarDadosGeraisComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string jsonString = @"
            {
                'geometria': {
                    'onShore': {
                        'lençolFreático': 0,
                        'elevação': 0,
                        'alturaDeAntePoço': 0
                    },
                    'offShore': {
                        'laminaDagua': 650
                    },
                    'coordenadas': {
                        'utMx': 386663.6,
                        'utMy': 7524326.9
                    },
                    'categoriaPoço': 1,
                    'mesaRotativa': 25
                },
                'identificação': {
                    'sonda': 'SS-66,SS-63',
                    'campo': 'MARLIM',
                    'companhia': '',
                    'bacia': 'CAMPOS'
                },
                'area': {
                    'densidadeAguaMar': 1.04,
                    'densidadeSuperficie': 1.8,
                    'sonicoSuperficie': 180
                }
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"/api/pocos/{idPoço}/atualizar-dados-gerais", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Dados gerais atualizados com sucesso.");
        }

        [Test]
        public async Task IntegrationTest_DeveRetornarNotFoundCasoNãoEncontrePoço()
        {
            // Arrange
            const string jsonString = @"
            {
                'geometria': {
                    'onShore': {
                        'lençolFreático': 0,
                        'elevação': 0,
                        'alturaDeAntePoço': 0
                    },
                    'offShore': {
                        'laminaDagua': 650
                    },
                    'coordenadas': {
                        'utMx': 386663.6,
                        'utMy': 7524326.9
                    },
                    'categoriaPoço': 1,
                    'mesaRotativa': 25
                },
                'identificação': {
                    'sonda': 'SS-66,SS-63',
                    'campo': 'MARLIM',
                    'companhia': '',
                    'bacia': 'CAMPOS'
                },
                'area': {
                    'densidadeAguaMar': 1.04,
                    'densidadeSuperficie': 1.8,
                    'sonicoSuperficie': 180
                }
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            var outroId = ObjectId.GenerateNewId().ToString();

            // Act
            var response = await Client.PutAsync($"/api/pocos/{outroId}/atualizar-dados-gerais", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível encontrar poço com id {outroId}.");
        }

        [Test, Ignore("Verificar como testar falha.")]
        public async Task IntegrationTest_DeveRetornarBadRequestCasoOcorraErro()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string jsonString = @"
            {
                'geometria': {
                    'onShore': {
                        'lençolFreático': 0,
                        'elevação': 0,
                        'alturaDeAntePoço': 0
                    },
                    'offShore': {
                        'laminaDagua': 650
                    },
                    'coordenadas': {
                        'utMx': 386663.6,
                        'utMy': 7524326.9
                    },
                    'categoriaPoço': 1,
                    'mesaRotativa': 25
                },
                'identificação': {
                    'sonda': 'SS-66,SS-63',
                    'campo': 'MARLIM',
                    'companhia': '',
                    'bacia': 'CAMPOS'
                },
                'area': {
                    'densidadeAguaMar': 1.04,
                    'densidadeSuperficie': 1.8,
                    'sonicoSuperficie': 180
                }
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            const string mensagemDeErro = "Mensagem de erro.";

            // Act
            var response = await Client.PutAsync($"/api/pocos/{idPoço}/atualizar-dados-gerais", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível atualizar dados gerais. {mensagemDeErro}");
        }
    }
}