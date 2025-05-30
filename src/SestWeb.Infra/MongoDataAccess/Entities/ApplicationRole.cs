using AspNetCore.Identity.Mongo.Model;

namespace SestWeb.Infra.MongoDataAccess.Entities
{
    public class ApplicationRole : MongoRole
    {
        public const string ADMIN_ROLE = "ADMIN";

        public const string NORMAL_ROLE = "NORMAL";

        public ApplicationRole(string role) : base(role)
        {

        }
    }
}
