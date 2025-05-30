using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD;
using SestWeb.Domain.Entities.Cálculos.Filtros.Corte;
using SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia;
using SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples;
using SestWeb.Domain.Entities.Cálculos.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Perfis;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga;
using SestWeb.Domain.Entities.Cálculos.Tensões;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Base
{
    public abstract class Cálculo : ICálculo, IDisposable, IEquatable<Cálculo>, IComparable<Cálculo>, IComparable
    {
        protected Cálculo(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, IConversorProfundidade conversorProfundidade, ILitologia litologia)
        {
            Nome = nome;
            GrupoCálculo = grupoCálculo;
            PerfisEntrada = perfisEntrada;
            PerfisSaída = perfisSaída;
            ConversorProfundidade = conversorProfundidade;
            Litologia = litologia;
            _disposed = false;
            if (Id == ObjectId.Empty)
            {
                Id = ObjectId.GenerateNewId();
            }
        }

        #region Fields

        private bool _disposed;
        private string _nomeCálculo;

        #endregion

        #region Properties

        public ObjectId Id { get; private set; }

        public string Nome
        {
            get => _nomeCálculo;
            private set
            {
                var nomeAntigo = _nomeCálculo;
                _nomeCálculo = value;

                if (nomeAntigo != null)
                    RenomearPerfisSaída(_nomeCálculo, nomeAntigo);
            }
        }
        
        public GrupoCálculo GrupoCálculo { get; private set; }
        
        public IPerfisEntrada PerfisEntrada { get; private set; }
        
        public IPerfisSaída PerfisSaída { get; private set; }
        
        public IConversorProfundidade ConversorProfundidade { get; private set; }

        public ILitologia Litologia { get; private set; }

        public bool PerfisEntradaPossuemPontos => PerfisEntrada.PossuemPontos();

        public bool TemSaídaZerada => PerfisSaída.TemSaidaZerada();

        public static IEqualityComparer<Cálculo> CálculoComparer { get; } = new CálculoEqualityComparer();

        public static IComparer<Cálculo> NomeCálculoComparer { get; } = new NomeCálculoRelationalComparer();

        #endregion

        #region Methods

        public abstract void Execute(bool chamadaPelaPipeline = false);

        public abstract List<string> GetTiposPerfisEntradaFaltantes();

        public abstract List<PerfilBase> GetPerfisEntradaSemPontos();

        public virtual bool ContémPerfilEntradaPorId(string idPerfil)
        {
            return PerfisEntrada.ContémPerfilPorId(idPerfil);
        }

        public virtual void AtualizarPvsSaídas(IConversorProfundidade conversor)
        {
            if (ConversorProfundidade.ContémDados())
            {
                PerfisSaída.AtualizarPvs(conversor);
            }
        }

        public virtual void ZerarPerfisSaída()
        {
            Parallel.ForEach(PerfisSaída.Perfis, perfilSaída => { perfilSaída.Clear(); });
        }

        public virtual void ZerarPerfisSaídaAfetados(PerfilBase perfil)
        {
            if (ContémPerfilEntradaPorId(perfil.Id.ToString()))
            {
                PerfisSaída.ApagarPontos();
            }
        }

        public virtual void RenomearPerfisSaída(string nomeNovo, string nomeAntigo)
        {
            PerfisSaída.Renomear(nomeNovo, nomeAntigo);
        }

        
        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    Litologia = null;
                    ConversorProfundidade = null;
                    PerfisSaída = null;
                    PerfisEntrada = null;
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.

                _disposed = true;
            }
        }

        ~Cálculo()
        {
            Dispose(false);
        }

        #endregion

        #region IEquatable<Cálculo> Members

        public bool Equals(Cálculo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _disposed == other._disposed && _nomeCálculo == other._nomeCálculo && GrupoCálculo == other.GrupoCálculo && Equals(PerfisEntrada, other.PerfisEntrada) && Equals(PerfisSaída, other.PerfisSaída) && Equals(ConversorProfundidade, other.ConversorProfundidade) && Equals(Litologia, other.Litologia);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Cálculo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _disposed.GetHashCode();
                hashCode = (hashCode * 397) ^ (_nomeCálculo != null ? _nomeCálculo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)GrupoCálculo;
                hashCode = (hashCode * 397) ^ (PerfisEntrada != null ? PerfisEntrada.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PerfisSaída != null ? PerfisSaída.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ConversorProfundidade != null ? ConversorProfundidade.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Litologia != null ? Litologia.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Cálculo left, Cálculo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Cálculo left, Cálculo right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IEqualityComparer<Cálculo> Members

        private sealed class CálculoEqualityComparer : IEqualityComparer<Cálculo>
        {
            public bool Equals(Cálculo x, Cálculo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x._disposed == y._disposed && x._nomeCálculo == y._nomeCálculo && x.GrupoCálculo == y.GrupoCálculo && Equals(x.PerfisEntrada, y.PerfisEntrada) && Equals(x.PerfisSaída, y.PerfisSaída) && Equals(x.ConversorProfundidade, y.ConversorProfundidade) && Equals(x.Litologia, y.Litologia);
            }

            public int GetHashCode(Cálculo obj)
            {
                unchecked
                {
                    var hashCode = obj._disposed.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj._nomeCálculo != null ? obj._nomeCálculo.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int)obj.GrupoCálculo;
                    hashCode = (hashCode * 397) ^ (obj.PerfisEntrada != null ? obj.PerfisEntrada.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.PerfisSaída != null ? obj.PerfisSaída.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.ConversorProfundidade != null ? obj.ConversorProfundidade.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Litologia != null ? obj.Litologia.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        #endregion

        #region  IComparable<Cálculo> Members

        public int CompareTo(Cálculo other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(_nomeCálculo, other._nomeCálculo, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is Cálculo other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Cálculo)}");
        }

        public static bool operator <(Cálculo left, Cálculo right)
        {
            return Comparer<Cálculo>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(Cálculo left, Cálculo right)
        {
            return Comparer<Cálculo>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(Cálculo left, Cálculo right)
        {
            return Comparer<Cálculo>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(Cálculo left, Cálculo right)
        {
            return Comparer<Cálculo>.Default.Compare(left, right) >= 0;
        }

        #endregion

        #region IComparer<Cálculo> Members

        private sealed class NomeCálculoRelationalComparer : IComparer<Cálculo>
        {
            public int Compare(Cálculo x, Cálculo y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return string.Compare(x._nomeCálculo, y._nomeCálculo, StringComparison.Ordinal);
            }
        }

        #endregion

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Cálculo)))
                return;

            BsonClassMap.RegisterClassMap<Cálculo>(cálculo => {
                cálculo.AutoMap();
                cálculo.MapMember(calc => calc.Id);
                cálculo.SetIdMember(cálculo.GetMemberMap(calc => calc.Id));
                cálculo.MapMember(calc => calc.Nome);
                cálculo.MapMember(calc => calc.GrupoCálculo).SetSerializer(new EnumSerializer<GrupoCálculo>(BsonType.String));
                cálculo.MapMember(calc => calc.PerfisEntrada).SetSerializer(new ImpliedImplementationInterfaceSerializer<IPerfisEntrada, PerfisEntrada>());
                cálculo.MapMember(calc => calc.PerfisSaída).SetSerializer(new ImpliedImplementationInterfaceSerializer<IPerfisSaída, PerfisSaída>());
                cálculo.UnmapMember(calc => calc.ConversorProfundidade);
                cálculo.UnmapMember(calc => calc.Litologia);
                cálculo.UnmapMember(calc => calc.PerfisEntradaPossuemPontos);
                cálculo.UnmapMember(calc => calc.TemSaídaZerada);                
                cálculo.SetIsRootClass(true);
                cálculo.SetIgnoreExtraElements(true);
                cálculo.SetDiscriminator(nameof(Cálculo));
            });


            CálculoPerfis.Map();
            CálculoPressãoPoros.Map();
            CálculoPropriedadesMecânicas.Map();
            CálculoTensões.Map();
            CálculoGradientes.Map();
            CálculoExpoenteD.Map();

            BsonClassMap.RegisterClassMap<CálculoSobrecarga>(calcSobr =>
            {
                calcSobr.SetDiscriminator(nameof(CálculoSobrecarga));
            });
            
            FiltroCorte.Map();
            FiltroSimples.Map();
            FiltroMédiaMóvel.Map();
            FiltroLitologia.Map();
            FiltroLinhaBaseFolhelho.Map();
        }

        #region ISupportInitialize members

        public void BeginInit() { }

        public void EndInit() { }

        #endregion

        #endregion
    }
}
