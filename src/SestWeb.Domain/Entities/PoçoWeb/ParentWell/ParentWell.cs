using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Domain.Entities.PoçoWeb.ParentWell
{
    public class ParentWell : IPoçoWebResponseItem
    {
        public ParentWell(string name)
        {
            Name = name;
        }

        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string OilField { get; set; }
        public List<File.File> Files { get; set; } = new List<File.File>();
    }
}
