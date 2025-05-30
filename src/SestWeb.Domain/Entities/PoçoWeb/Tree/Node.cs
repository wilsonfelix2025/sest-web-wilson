using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Domain.Entities.PoçoWeb.Tree
{
    public class Node : IPoçoWebResponseItem
    {
        public Node(string id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url;
        }

        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int FilesCount { get; set; }
        public List<IPoçoWebResponseItem> Children { get; set; } = new List<IPoçoWebResponseItem>();
    }
}
