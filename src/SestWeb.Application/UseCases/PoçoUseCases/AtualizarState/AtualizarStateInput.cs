using System.Collections.Generic;
using MongoDB.Bson;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarState
{
    public class AtualizarStateInput
    {
        public List<TreeInput> Tree { get; set; } = new List<TreeInput>();
        public List<AbaInput> Tabs { get; set; } = new List<AbaInput>();
        public string IdAbaAtual { get; set; }
        public TipoProfundidade ProfundidadeExibição { get; set; }
        public float PosicaoDivisaoAreaGrafica { get; set; }
    }

    public class AbaInput
    {
        public string Id { get; set; }
        public bool Fixa { get; set; }
        public string Name { get; set; }
        public List<GráficoInput> Data { get; set; } = new List<GráficoInput>();
    }

    public class GráficoInput
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public float Largura { get; set; }
        public double MaxX { get; set; }
        public double MinX { get; set; }
        public double Intervalo { get; set; }
        public List<ItemInput> Items { get; set; } = new List<ItemInput>();
    }

    public class ItemInput
    {
        public string Id { get; set; }
        public string IdPoço { get; set; }
        public bool AdicionadoTrend { get; set; }
    }

    public class TreeInput
    {
        public string Id { get; set; }
        public bool Fixa { get; set; }
        public string Name { get; set; }
        public string Tipo { get; set; }
        public List<TreeInput> Data { get; set; } = new List<TreeInput>();
    }
}
