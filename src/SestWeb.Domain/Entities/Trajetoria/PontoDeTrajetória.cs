using System;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Trajetoria
{
    public class PontoDeTrajetória
    {

        private Func<double> _getAngleFunc;

        internal PontoDeTrajetória(double pm, double pv, double inclinação, double azimute, double ew, double ns, double dls, double polCoordDispl, double polCoordDirec, Func<double> getAngleFunc)
        {
            Pm = new Profundidade(pm);
            Pv = new Profundidade(pv);
            Inclinação = Math.Abs(azimute - 360) < 0.009 ? 0 : (double)Math.Truncate((decimal)inclinação * 100) / 100;
            Azimute = Math.Abs(azimute - 360) < 0.009 ? 0 : (double)Math.Truncate((decimal)azimute * 100) / 100;
            //Inclinação = Math.Abs(azimute - 360) < 0.009 ? 0 : (double)Math.Truncate((decimal)inclinação * 10000000) / 10000000;
            //Azimute = Math.Abs(azimute - 360) < 0.009 ? 0 : (double)Math.Truncate((decimal)azimute * 10000000) / 10000000;
            EW = ew;
            NS = ns;
            DLS = dls;
            PolCoordDispl = polCoordDispl;
            PolCoordDirec = polCoordDirec;
            _getAngleFunc = getAngleFunc;
        }

        [Obsolete] // somente utilizado em testes
        public PontoDeTrajetória(double pm, double inclinação, double azimute) : this(pm, 0d, inclinação, azimute, 0d, 0d, 0d, 0d, 0d, null) { }

        #region Propriedades

        public Profundidade Pm { get; private set; }

        public Profundidade Pv { get; internal set; }

        public double Inclinação { get; private set; }

        public double Azimute { get; private set; }

        public double EW { get; internal set; }

        public double NS { get; internal set; }

        public double DLS { get; internal set; }

        public double PolCoordDispl { get; internal set; }

        public double PolCoordDirec { get; internal set; }

        public double PmProj => 0d;

        public double PvProj => PolCoordDispl != null ? PolCoordDispl * Math.Cos((PolCoordDirec - _getAngleFunc()) * Math.PI / 180) : 0d;

        #endregion

        #region Override Members

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PontoDeTrajetória)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Pm.GetHashCode();
                hashCode = (hashCode * 397) ^ Pv.GetHashCode();
                hashCode = (hashCode * 397) ^ Inclinação.GetHashCode();
                hashCode = (hashCode * 397) ^ Azimute.GetHashCode();
                hashCode = (hashCode * 397) ^ EW.GetHashCode();
                hashCode = (hashCode * 397) ^ NS.GetHashCode();
                hashCode = (hashCode * 397) ^ DLS.GetHashCode();
                hashCode = (hashCode * 397) ^ PolCoordDispl.GetHashCode();
                hashCode = (hashCode * 397) ^ PolCoordDirec.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        protected bool Equals(PontoDeTrajetória other)
        {
            return Equals(Pm, other.Pm) && Pv.Equals(other.Pv) && Inclinação.Equals(other.Inclinação) && Azimute.Equals(other.Azimute) && EW.Equals(other.EW) && NS.Equals(other.NS) && DLS.Equals(other.DLS) && PolCoordDispl.Equals(other.PolCoordDispl) && PolCoordDirec.Equals(other.PolCoordDirec);
        }

        public void AlterarInclinação(double inclinação)
        {
            Inclinação = inclinação;
        }

        public void AlterarAzimute(double azimute)
        {
            Azimute = azimute;
        }

        public void RegisterGetAngleFunc(Func<double> getAngleFunc)
        {
            _getAngleFunc = getAngleFunc;
        }

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PontoDeTrajetória)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Profundidade)))
            {
                BsonClassMap.RegisterClassMap<Profundidade>(profundidade =>
                {
                    profundidade.MapMember(prof => prof.Valor);
                });
            }

            BsonClassMap.RegisterClassMap<PontoDeTrajetória>(pontoDeTrajetória =>
            {
                pontoDeTrajetória.AutoMap();
                pontoDeTrajetória.MapMember(ponto => ponto.Pm);
                pontoDeTrajetória.MapMember(ponto => ponto.Pv);
                pontoDeTrajetória.MapMember(ponto => ponto.Inclinação);
                pontoDeTrajetória.MapMember(ponto => ponto.Azimute);
                pontoDeTrajetória.MapMember(ponto => ponto.NS);
                pontoDeTrajetória.MapMember(ponto => ponto.DLS);
                pontoDeTrajetória.MapMember(ponto => ponto.PolCoordDispl);
                pontoDeTrajetória.MapMember(ponto => ponto.PolCoordDirec);

                pontoDeTrajetória.MapMember(ponto => ponto.PmProj);
                pontoDeTrajetória.MapMember(ponto => ponto.PvProj);

                pontoDeTrajetória.SetIgnoreExtraElements(true);
                pontoDeTrajetória.SetDiscriminator(nameof(pontoDeTrajetória));
            });
        }
    }
}
