using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Poço
{
    public class State
    {
        public List<Tree> Tree { get; set; } = new List<Tree>();

        public List<Aba> Tabs { get; set; } = new List<Aba>();

        public ObjectId IdAbaAtual { get; set; }

        public TipoProfundidade ProfundidadeExibição { get; set; }

        public float PosicaoDivisaoAreaGrafica { get; set; }

        public State()
        {
            ProfundidadeExibição = TipoProfundidade.PM;
            PosicaoDivisaoAreaGrafica = 0;
            InicializaTree();
            InicializaTabs();
        }

        public void RemoveFromTree(string id)
        {
            Tree.ForEach(tree =>
            {
                Remove(tree, id);
            });
        }

        private void Remove(Tree tree, string id)
        {
            int i = tree.Data.FindIndex(t => t.Id == id);
            if (i >= 0)
            {
                tree.Data.RemoveAt(i);
            }
            else if (tree.Data.Count > 0)
            {
                tree.Data.ForEach(t =>
                {
                    Remove(t, id);
                });
            }
        }

        private void InicializaTree()
        {
            Tree.Add(new Tree { Name = "Trajetória", Fixa = true, Tipo = "3" });
            Tree.Add(new Tree { Name = "Litologia", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Perfis", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Propriedades Mecânicas", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Tensões/Pressões", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Gradientes", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Outros", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Filtros", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Cálculos", Fixa = true, Tipo = "1" });
            Tree.Add(new Tree { Name = "Perfuração", Fixa = true, Tipo = "1" });
        }

        private void InicializaTabs()
        {
            var perfis = new Aba("Perfis", true);
            perfis.CriarGráficos(12);
            Tabs.Add(perfis);

            var sobrecarga = new Aba("Sobrecarga", true);
            sobrecarga.CriarGráficos(3);
            Tabs.Add(sobrecarga);

            var pressãoPoros = new Aba("Pressão de poros", true);
            pressãoPoros.CriarGráficos(5);
            Tabs.Add(pressãoPoros);

            var propriedadeMecânicas = new Aba("Propriedades mecânicas", true);
            propriedadeMecânicas.CriarGráficos(9);
            Tabs.Add(propriedadeMecânicas);

            var tensões = new Aba("Tensões in situ", true);
            tensões.CriarGráficos(6);
            Tabs.Add(tensões);

            var janelaOperacional = new Aba("Janela operacional", true);
            janelaOperacional.CriarGráficos(1, 3);
            Tabs.Add(janelaOperacional);

            IdAbaAtual = perfis.Id;
        }
    }

    public class Aba
    {
        public Aba(string nome, bool fixa = false)
        {
            if (Id == ObjectId.Empty)
            {
                Id = ObjectId.GenerateNewId();
            }
            Name = nome;
            Fixa = fixa;
        }
        public Aba(string id, string nome, bool fixa = false)
        {
            Id = ObjectId.Parse(id);
            Name = nome;
            Fixa = fixa;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public bool Fixa { get; set; }
        public string Name { get; set; }
        public List<Gráfico> Data { get; set; } = new List<Gráfico>();

        public void CriarGráficos(int numGraficos, float largura = 1)
        {
            for (int i = 1; i <= numGraficos; i++)
            {
                Data.Add(new Gráfico("", largura));
            }
        }
    }

    public class Gráfico
    {
        public Gráfico(string nome, float largura = 1)
        {
            Name = nome;
            Largura = largura;
            if (Id == ObjectId.Empty)
            {
                Id = ObjectId.GenerateNewId();
            }
        }
        public Gráfico(string id, string nome, float largura = 1)
        {
            Name = nome;
            Largura = largura;
            Id = ObjectId.Parse(id);
        }
        [BsonId]
        public ObjectId Id { get; private set; }
        public string Name { get; set; }
        public float Largura { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public double MaxX { get; set; }
        public double MinX { get; set; }
        public double Intervalo { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string IdPoço { get; set; }
        public bool AdicionadoTrend { get; set; } = false;
    }

    public class Tree
    {
        public string Id { get; set; }
        public bool Fixa { get; set; }
        public string Name { get; set; }
        public string Tipo { get; set; }
        public List<Tree> Data { get; set; } = new List<Tree>();
    }
}