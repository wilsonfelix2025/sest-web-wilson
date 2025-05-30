using AspNetCore.Identity.Mongo.Model;

namespace SestWeb.Infra.MongoDataAccess.Entities
{
    public class ApplicationUser : MongoUser
    {
        public const string EmailMainUser = "portalsest@puc-rio.br";

        public string PrimeiroNome { get; set; }
        public string SegundoNome { get; set; }
    }
}
