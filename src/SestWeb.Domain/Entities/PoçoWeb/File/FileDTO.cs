using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SestWeb.Domain.Entities.PoçoWeb.File
{
    public class FileDTO
    {
        public FileDTO(string name, string url)
        {
            Name = name;
            Url = url;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}
