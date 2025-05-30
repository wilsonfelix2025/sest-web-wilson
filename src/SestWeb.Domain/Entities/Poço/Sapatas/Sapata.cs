using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Poço.Sapatas
{
    public sealed class Sapata
    {
        public double Pm { get; private set; }

        public double Pv { get; private set; }

        public string Diâmetro { get; }

        [BsonConstructor]
        public Sapata(double pm, double pv, string diâmetro)
        {
            Pm = pm;
            Pv = pv;
            Diâmetro = diâmetro;
        }

        public void Shift(double delta, Trajetória trajetoria)
        {
            Pm += delta;
            trajetoria.TryGetTVDFromMD(Pm, out double pv);
            Pv = pv;
        }

        public void AtualizarPv(Trajetória trajetoria)
        {
            trajetoria.TryGetTVDFromMD(Pm, out double pv);
            Pv = pv;
        }
    }
}