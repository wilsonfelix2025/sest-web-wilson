using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Entities.Perfis.Base
{

    [Obsolete("Classe Aperfomática. Não pode ser assim..")]
    public abstract class PerfilOld
    {
        private static readonly List<TipoPerfil> TodosTipos;
        private static Dictionary<string, TipoPerfil> _mnemônicos;
        private Pontos<Ponto> _pontos;

        #region Constructor

        static PerfilOld()
        {
            var pai = typeof(PerfilOld);
            var assembly = Assembly.GetExecutingAssembly();
            var tipos = assembly.GetTypes();

            var tiposSubclasses = tipos.Where(t => t.IsSubclassOf(pai));

            var classMap = new BsonClassMap(typeof(PerfilOld));
            classMap.AutoMap();
            //classMap.SetIsRootClass(true);

            TodosTipos = new List<TipoPerfil>();
            foreach (var item in tiposSubclasses)
            {
                //var mnemonico = TipoPerfil.Create(item);
                //TodosTipos.Add(mnemonico);

                classMap.AddKnownType(item);
            }

            BsonClassMap.RegisterClassMap(classMap);
        }

        #endregion
    }
}