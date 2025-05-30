using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Domain.Entities.PoçoWeb.Well
{
    public class Well : IPoçoWebResponseItem
    {
        public Well(string url, string name, string oilFieldId)
        {
            Name = name;
            Url = url;

            string[] aux = FormatUrl.FormatUrl.Execute(url);
            Id = aux[0];

            OilFieldId = oilFieldId;
        }

        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string OilFieldId { get; set; }
        public List<string> Files { get; set; } = new List<string>();
    }
}
