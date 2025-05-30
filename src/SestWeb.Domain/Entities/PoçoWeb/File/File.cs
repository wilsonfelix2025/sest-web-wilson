using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Domain.Entities.PoçoWeb.File
{
    public class File : IPoçoWebResponseItem
    {
        public File(string url, string name, string wellId, string description, string createdAt, string lastUpdatedAt, string fileType, string schema, string comment)
        {
            Name = name;
            Url = url;

            string[] aux = FormatUrl.FormatUrl.Execute(url);
            Id = aux[0];
            if (aux.Length > 1)
            {
                Rev = aux[1];
            }

            WellId = wellId;

            Description = description;
            CreatedAt = createdAt;
            LastUpdatedAt = lastUpdatedAt;
            FileType = fileType;
            Schema = schema;
            Comment = comment;
        }
        public File(string url, string name, string wellUrl, string fileType)
        {
            Name = name;
            Url = url;

            string[] aux = FormatUrl.FormatUrl.Execute(url);
            Id = aux[0];
            if (aux.Length > 1)
            {
                Rev = aux[1];
            }

            aux = FormatUrl.FormatUrl.Execute(wellUrl);
            WellId = aux[0];

            FileType = fileType;
        }

        [BsonId]
        public string Id { get; set; }
        public string Rev { get; set; }
        public string Name { get; set; }
        public string WellId { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string Schema { get; set; }
        public string Comment { get; set; }
        public string Url { get; set; }
        public string CreatedAt { get; set; }
        public string LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
