using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Domain.Entities.PoçoWeb.OilField
{
    public class OilField : IPoçoWebResponseItem
    {
        public OilField(string url, string name, string opUnitId)
        {
            Name = name;
            Url = url;

            string[] aux = FormatUrl.FormatUrl.Execute(url);
            Id = aux[0];

            OpUnitId = opUnitId;
        }

        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string OpUnitId { get; set; }
        public List<string> Wells { get; set; } = new List<string>();
    }
}
