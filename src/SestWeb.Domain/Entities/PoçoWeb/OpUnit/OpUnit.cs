using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Domain.Entities.PoçoWeb.OpUnit
{
    public class OpUnit : IPoçoWebResponseItem
    {
        public OpUnit(string url, string name)
        {
            Name = name;
            Url = url;

            string[] aux = FormatUrl.FormatUrl.Execute(url);
            Id = aux[0];
        }

        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> OilFields { get; set; } = new List<string>();
    }
}
