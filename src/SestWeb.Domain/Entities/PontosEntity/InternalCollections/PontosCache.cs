namespace SestWeb.Domain.Entities.PontosEntity.InternalCollections
{
    public class PontosCache<TKey, T> : LurchTable<TKey, T>
    {
        private static int _pointsLimit = 20000;

        public PontosCache() : base(_pointsLimit){ }
    }
}
