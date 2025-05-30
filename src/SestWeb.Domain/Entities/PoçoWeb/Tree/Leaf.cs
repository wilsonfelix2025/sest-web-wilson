using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Domain.Entities.PoçoWeb.Tree
{
    public class Leaf : IPoçoWebResponseItem
    {
        public Leaf(string id, string name, string url, string type)
        {
            Id = id;
            Name = name;
            Url = url;
            Type = type;
        }

        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
    }
}
