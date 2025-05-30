using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação;

namespace SestWeb.Domain.Entities.Correlações.VariablesCorrelação
{
    public abstract class Variables : IVariables
    {
        #region Fields

        public Dictionary<string, double> _variables;

        #endregion

        public Variables(string expressão)
        {
            _variables = new Dictionary<string, double>();

            // RCM: Virtual member call ok
            Identify(expressão);
        }

        #region Properties

        public List<string> Names => GetNames();

        public List<double> Values => GetValues();

        #endregion

        #region Methods

        public bool Exists(string variable)
        {
            return _variables.ContainsKey(variable);
        }

        public bool Any() => _variables.Any();

        public bool TryGetValue(string name, out double valor)
        {
            if (!_variables.ContainsKey(name))
            {
                valor = default;
                return false;
            }

            valor = _variables[name];
            return true;
        }

        private protected List<string> GetNames()
        {
            return _variables.Keys.ToList();
        }

        private protected List<double> GetValues()
        {
            return _variables.Values.ToList();
        }

        private protected abstract void Identify(string expression);

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Variables)))
                return;

            BsonClassMap.RegisterClassMap<Variables>(var => {
                var.AutoMap();
                var.MapMember(c => c._variables);
                var.UnmapMember(c => c.Names);
                var.UnmapMember(c => c.Values);
                var.SetIsRootClass(true);
                var.SetIgnoreExtraElements(true);
                var.SetDiscriminator("Variables");
            });
            BsonClassMap.RegisterClassMap<Variáveis>(var =>
            {
                var.SetDiscriminator("Variáveis");
            });
            BsonClassMap.RegisterClassMap<Constantes>(cons =>
            {
                cons.SetDiscriminator("Constantes");
            });
        }

        #endregion
    }
}
