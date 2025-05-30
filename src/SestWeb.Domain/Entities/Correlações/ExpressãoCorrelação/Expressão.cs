using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnaliticsCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Factory;
using SestWeb.Domain.Entities.Correlações.NormalizadorCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;
using SestWeb.Domain.Entities.Correlações.VariablesCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação
{
    public class Expressão : IExpressão
    {
        private Expressão(string expressão, IAnalitics analitics, IVariáveis variáveis, IConstantes constantes, IPerfisSaída perfisSaída, IPerfisEntrada perfisEntrada)
        {
            Bruta = expressão;
            Normalizada = Normalizador.NormalizarExpressão(expressão);
            Analitics = analitics;
            Variáveis = variáveis;
            Constantes = constantes;
            PerfisSaída = perfisSaída;
            PerfisEntrada = perfisEntrada;
        }

        #region Base Properties

        public string Bruta { get; }

        public string Normalizada { get;  }

        public IVariáveis Variáveis { get;  }

        public IConstantes Constantes { get;  }

        public IPerfisEntrada PerfisEntrada { get;  }

        public IPerfisSaída PerfisSaída { get;  }

        public IAnalitics Analitics { get; }

        #endregion

        #region Methods

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Expressão)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Variables)))
                Variables.Map();

            BsonClassMap.RegisterClassMap<Expressão>(exp =>
            {
                exp.AutoMap();
                exp.MapCreator(e =>
                    new Expressão(e.Bruta, e.Analitics, e.Variáveis, e.Constantes, e.PerfisSaída, e.PerfisEntrada));
                exp.MapMember(e => e.Bruta);
                exp.MapMember(e => e.Analitics).SetSerializer(new ImpliedImplementationInterfaceSerializer<IAnalitics, Analitics>()); 
                exp.MapMember(e => e.Variáveis).SetSerializer(new ImpliedImplementationInterfaceSerializer<IVariáveis, Variáveis>()); 
                exp.MapMember(e => e.Constantes).SetSerializer(new ImpliedImplementationInterfaceSerializer<IConstantes, Constantes>());
                exp.MapMember(e => e.PerfisSaída).SetSerializer(new ImpliedImplementationInterfaceSerializer<IPerfisSaída, PerfisSaída>());
                exp.MapMember(e => e.PerfisEntrada).SetSerializer(new ImpliedImplementationInterfaceSerializer<IPerfisEntrada, PerfisEntrada>());
                exp.SetIgnoreExtraElements(true);
                exp.SetDiscriminator("Expressão");
            });
        }

        public static void RegisterCorrelaçãoCtor()
        {
            ExpressãoFactory.RegisterExpressãoCtor((expressão, analitics, variáveis, constantes, perfisSaída, perfisEntrada) => new Expressão(expressão, analitics, variáveis, constantes, perfisSaída, perfisEntrada));
        }

        public override string ToString()
        {
            return Bruta;
        }

        #endregion
    }
}
