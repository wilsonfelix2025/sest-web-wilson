using SestWeb.Domain.SistemasUnidades.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.SistemasUnidades.Grupos.Base
{
    public abstract class GrupoUnidades
    {
        private static readonly Dictionary<Type,GrupoUnidades> TodosGrupos;
        private readonly Dictionary<UnidadeMedida, double> _unidadesPorFator;
        private static readonly Dictionary<string, GrupoUnidades> GruposPorTipoPerfil;

        static GrupoUnidades()
        {
            var pai = typeof(GrupoUnidades);
            var assembly = Assembly.GetExecutingAssembly();
            var tipos = assembly.GetTypes();

            var tiposSubclasses = tipos.Where(t => t.IsSubclassOf(pai));

            TodosGrupos = new Dictionary<Type, GrupoUnidades>();
            GruposPorTipoPerfil = new Dictionary<string, GrupoUnidades>();

            foreach (var item in tiposSubclasses)
            {
                var grupo = (GrupoUnidades)Activator.CreateInstance(item);
                TodosGrupos.Add(item,grupo);
            }
        }

        protected GrupoUnidades(string nome, UnidadeMedida unidadePadrão)
        {
            Nome = nome;
            UnidadePadrão = unidadePadrão;
            _unidadesPorFator = new Dictionary<UnidadeMedida, double>();
        }

        public string Nome { get; private set; }

        public UnidadeMedida UnidadePadrão { get; private set; }

        public static GrupoUnidades GetGrupoUnidades<T>() where T : GrupoUnidades
        {
            return TodosGrupos[typeof(T)];
        }

        public static GrupoUnidades GetGrupoUnidades(string mnemônico) 
        {
            return GruposPorTipoPerfil[mnemônico];
        }

        public static UnidadeMedida GetUnidadePadrão(string menmônico)
        {
            return GetGrupoUnidades(menmônico).UnidadePadrão;
        }

        public static void RegisterTipoPerfil<T>(TipoPerfil tipoPerfil) where T : GrupoUnidades
        {
            if(!GruposPorTipoPerfil.ContainsKey(tipoPerfil.Mnemônico))
                GruposPorTipoPerfil.Add(tipoPerfil.Mnemônico, GetGrupoUnidades<T>());
        }

        public double ConvertToUnidadePadrão(UnidadeMedida unidade, double valor)
        {
            if (unidade == UnidadePadrão || this.Nome == "Grupo de unidades sem dimensão") return Math.Round(valor,2,MidpointRounding.AwayFromZero);

            if (_unidadesPorFator.TryGetValue(unidade, out var fator))
            {
                var valorConvertido = Convert(valor, fator, unidade);
                return Math.Round(valorConvertido,2,MidpointRounding.AwayFromZero);
            }

            throw new ArgumentException($"Unidade {unidade} não encontrada no grupo de unidade!");
        }

        public override string ToString()
        {
            return $"{Nome} ({UnidadePadrão.Símbolo})";
        }

        protected virtual double Convert(double valor, double fator, UnidadeMedida unidadeOrigem)
        {
            return valor * fator;
        }

        protected void RegistrarUnidade(UnidadeMedida unidade, double fator)
        {
            _unidadesPorFator.Add(unidade, fator);
        }
    }
}