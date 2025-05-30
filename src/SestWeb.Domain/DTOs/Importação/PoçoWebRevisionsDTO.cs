using System;

namespace SestWeb.Domain.DTOs.Importação
{
    public class PoçoWebRevisionsDTO
    {
       
            public Uri Url { get; set; }
            public string Description { get; set; }
            public Details Author { get; set; }
            public Details Source { get; set; }

    }

    public class Details
    {
        public Uri Url { get; set; }
        public string Name { get; set; }
    }
}
