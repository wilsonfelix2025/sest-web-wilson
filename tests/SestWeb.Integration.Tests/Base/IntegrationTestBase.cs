using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SestWeb.Api;

namespace SestWeb.Integration.Tests.Base
{
    [TestFixture]
    public class IntegrationTestBase
    {
        protected HttpClient Client;
        private string _connectionString;
        private string _databaseName;
        private MongoClient _mongoClient;
        private TestServer _server;
        protected bool AllowDatabaseCleanUpOnTearDown { get; set; } = true;

        [OneTimeSetUp]
        public void ClassInit()
        {
            FillDatabaseProperties();
            _mongoClient = new MongoClient(_connectionString);
        }

        [SetUp]
        public virtual async Task Setup()
        {
            await _mongoClient.DropDatabaseAsync(_databaseName);

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IHostingEnvironment env = builderContext.HostingEnvironment;
                    config.AddJsonFile("autofac.json")
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables();
                })
                .ConfigureServices(services => services.AddAutofac());

            _server = new TestServer(webHostBuilder);
            Client = _server.CreateClient();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (AllowDatabaseCleanUpOnTearDown)
            {
                _mongoClient.DropDatabase(_databaseName);
            }
        }

        private void FillDatabaseProperties()
        {
            var autofac = JsonConvert.DeserializeObject<JObject>(string.Join("", File.ReadAllLines("autofac.json")));

            var modules = autofac["modules"].Values();
            _databaseName = string.Empty;
            _connectionString = string.Empty;

            for (int i = 0; i < autofac["modules"].Count(); i++)
            {
                var properties = autofac["modules"][i]["properties"];

                if (properties != null)
                {
                    _connectionString = properties["ConnectionString"].Value<string>();
                    _databaseName = properties["DatabaseName"].Value<string>();
                    break;
                }
            }

            if (string.IsNullOrEmpty(_databaseName) || string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentException("Informações de conexão com o banco de dados não foram encontradas.");
            }
        }

        protected async Task<string> CriarPoçoDebug()
        {
            var response = await Client.PostAsync("/api/debug/alimentar-database", null);

            response.EnsureSuccessStatusCode();

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["idPoço"].Value<string>();
        }
    }
}