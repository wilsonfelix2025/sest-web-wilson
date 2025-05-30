namespace SestWeb.Domain.Entities.PoçoWeb.File
{
    public class FileRequest
    {
        public FileRequest(string Name, string Description, string FileType, string WellUrl)
        {
            name = Name;
            description = Description;
            file_type = FileType;
            well = WellUrl;
        }

        public string name { get; set; }
        public string description { get; set; }
        public string file_type { get; set; }
        public string well { get; set; }
    }
}
