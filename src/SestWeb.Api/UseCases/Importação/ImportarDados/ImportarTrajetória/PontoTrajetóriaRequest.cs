
using System;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class PontoTrajetóriaRequest : IEquatable<PontoTrajetóriaRequest>
    {
        public string PM;
        public string Azimute;
        public string Inclinacao;

        public bool Equals(PontoTrajetóriaRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PM == other.PM && Azimute == other.Azimute && Inclinacao == other.Inclinacao;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PontoTrajetóriaRequest) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PM, Azimute, Inclinacao);
        }
    }
}
