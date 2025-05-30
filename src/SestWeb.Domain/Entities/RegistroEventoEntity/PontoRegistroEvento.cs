using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using MongoDB.Bson.Serialization;

namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public class PontoRegistroEvento : IEquatable<PontoRegistroEvento>, IPontoRegistroEvento
    {
        private IConversorProfundidade _conversorProfundidade;

        #region Properties
        public Profundidade Pm { get; set; }
        public Profundidade Pv { get; private set; }
        #endregion

        public PontoRegistroEvento(double pm, double pv, IConversorProfundidade conversorProfundidade)
        {
            _conversorProfundidade = conversorProfundidade;
            Pv = new Profundidade(pv);
            Pm = new Profundidade(pm);

            if (pm == null || Math.Abs(pm) < 0.0099)
            {
                TryGetPm();
            }
            else if (pv == null || Math.Abs(pv) < 0.0099)
            {
                TryGetPv();
            }
        }

        #region Methods

        public override string ToString() => $"{Pm}m";

        public override bool Equals(object obj)
        {
            return Equals(obj as PontoRegistroEvento);
        }

        public bool Equals(PontoRegistroEvento other)
        {
            return other != null &&
                   Pm == other.Pm;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pm);
        }

        public static bool operator ==(PontoRegistroEvento ponto1, PontoRegistroEvento ponto2)
        {
            return EqualityComparer<PontoRegistroEvento>.Default.Equals(ponto1, ponto2);
        }

        public static bool operator !=(PontoRegistroEvento ponto1, PontoRegistroEvento ponto2)
        {
            return !(ponto1 == ponto2);
        }

        public void AtualizarPV(IConversorProfundidade conversor)
        {
            _conversorProfundidade = conversor;
            TryGetPv();
        }

        public void ConverterParaPv()
        {
            if (Pv == null || Math.Abs(Pv.Valor) < 0.0099)
                TryGetPv();
        }

        public void ConverterParaPM()
        {
            if (Pm == null || Math.Abs(Pm.Valor) < 0.0099)
                TryGetPm();
        }

        public void Shift(double delta)
        {
            Pm = new Profundidade(Pm.Valor + delta);
            Pv = new Profundidade(Pv.Valor + delta);
        }

        private void TryGetPv()
        {
            if (_conversorProfundidade != null && _conversorProfundidade.TryGetTVDFromMD(Pm.Valor, out double pv))
            {
                pv = (double)Math.Truncate((decimal)pv * 100) / 100;
                Pv = new Profundidade(pv);
            }
        }

        private void TryGetPm()
        {
            if (_conversorProfundidade != null && _conversorProfundidade.TryGetMDFromTVD(Pv.Valor, out double pm))
            {
                pm = (double)Math.Truncate((decimal)pm * 100) / 100;
                Pm = new Profundidade(pm);
            }
        }

        #region Map()

        public static void Map()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Profundidade)))
            {
                Profundidade.Map();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PontoRegistroEvento)))
            {
                BsonClassMap.RegisterClassMap<PontoRegistroEvento>(ponto =>
                {
                    ponto.AutoMap();
                    ponto.MapMember(p => p.Pm);
                    ponto.MapMember(p => p.Pv);
                    ponto.UnmapMember(p => p._conversorProfundidade);
                });
            }
        }

        #endregion

        #endregion
    }
}