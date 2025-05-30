namespace SestWeb.Domain.Entities.Trajetoria
{
    public class SessãoTrajetória
    {
        public PontoDeTrajetória PontoInicial { get; }
        public PontoDeTrajetória PontoFinal { get; }

        public SessãoTrajetória(PontoDeTrajetória pontoInicial, PontoDeTrajetória pontoFinal)
        {
            PontoInicial = pontoInicial;
            PontoFinal = pontoFinal;
        }

        protected bool Equals(SessãoTrajetória other)
        {
            return Equals(PontoInicial, other.PontoInicial) && Equals(PontoFinal, other.PontoFinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SessãoTrajetória)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((PontoInicial != null ? PontoInicial.GetHashCode() : 0) * 397) ^ (PontoFinal != null ? PontoFinal.GetHashCode() : 0);
            }
        }
    }
}
