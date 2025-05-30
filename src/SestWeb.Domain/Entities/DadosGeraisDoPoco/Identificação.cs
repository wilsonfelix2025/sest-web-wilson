
using MongoDB.Bson.Serialization.Attributes;

namespace SestWeb.Domain.Entities.DadosGeraisDoPoco
{
    public class Identificação
    {
        public string Nome { get; set; }
        public string NomePoço { get; set; }
        public string NomePoçoWeb { get; set; }
        public string Sonda { get; set; } = string.Empty;
        public string Campo { get; set; } = string.Empty;
        public string Companhia { get; set; } = string.Empty;
        public string Bacia { get; set; } = string.Empty;
        public TipoPoço TipoPoço { get; set; }
        public string Finalidade { get; set; } = string.Empty;
        public string Analista { get; set; } = string.Empty;
        public string NívelProteção { get; set; } = string.Empty;
        public string ClassificaçãoPoço { get; set; } = string.Empty;
        public string TipoCompletação { get; set; } = string.Empty;
        public double ComplexidadePoço { get; set; } = 0;
        public double VidaÚtilPrevista { get; set; } = 0;
        public string PoçoWebUrl { get; set; } = string.Empty;
        public string PoçoWebRevisãoUrl { get; set; } = string.Empty;
        public string PoçoWebDtÚltimaAtualização { get; set; } = string.Empty;
        public string PoçoWebUrlRevisões { get; set; } = string.Empty;
        [BsonIgnore] public bool PoçoWebAtualizado { get; set; } = true;
        [BsonIgnore] public string PoçoWebUrlÚltimaRevisão { get; set; } = string.Empty;
        public bool? CriticidadePoço { get; set; } = false;
        public bool? IntervençãoWorkover { get; set; } = false;
        public string NomePoçoLocalImportação { get; set; } = string.Empty;
    }
}