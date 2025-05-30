using System.ComponentModel;

namespace SestWeb.Domain.Entities.PoçoWeb.File
{
    public enum FileType
    {
        [DisplayName("sesttr.project")]
        SestTR_Project = 0,
        [DisplayName("sesttr.monitoring")]
        SestTR_Monitoring = 1,
        [DisplayName("sesttr.retroanalysis")]
        SestTR_Retroanalysis = 2
    }
}
