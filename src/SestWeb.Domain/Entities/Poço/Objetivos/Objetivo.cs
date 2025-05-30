using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Poço.Objetivos
{
    public sealed class Objetivo
    {
        public double Pm { get; private set; }
        public double Pv { get; private set; }
        public TipoObjetivo TipoObjetivo { get; }

        [BsonConstructor]
        public Objetivo(double pm, double pv, TipoObjetivo tipoObjetivo)
        {
            Pm = pm;
            Pv = pv;
            TipoObjetivo = tipoObjetivo;
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