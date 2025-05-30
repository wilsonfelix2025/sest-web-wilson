namespace SestWeb.Api.UseCases.PoçoWeb.CreateFile
{
    ///<inheritdoc />
    public class FileRequest
    {
        ///<inheritdoc />
        public string Name { get; set; }
        ///<inheritdoc />
        public string WellId { get; set; }
        ///<inheritdoc />
        public string Description { get; set; }
        ///<inheritdoc />
        public string Schema { get; set; }
        ///<inheritdoc />
        public string FileType { get; set; }
    }
}
