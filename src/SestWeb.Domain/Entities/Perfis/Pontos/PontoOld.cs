using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Interfaces;

namespace SestWeb.Domain.Entities.Perfis.Pontos
{
    [Obsolete]
    public class PontoOld : IEquatable<PontoOld>, IIndexadoPorPM
    {
        [BsonElement]
        private double _pm;

        private double _pv = 0;

        public double Pm { get => Round(_pm); private set => _pm = value; }
        public double Valor { get; private set; }

        [BsonIgnore]
        public double Pv { get => Round(_pv); set => _pv = value; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public OrigemPonto Origem { get; private set; }

        [BsonIgnore]
        public TipoRocha TipoRocha { get; set; }

        [BsonConstructor]
        public PontoOld(double pm, double valor, OrigemPonto origem)
        {
            Pm = pm;
            Valor = valor;
            Origem = origem;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PontoOld);
        }

        public bool Equals(PontoOld other)
        {
            return other != null &&
                   Pm == other.Pm &&
                   Valor == other.Valor;
        }

        public override int GetHashCode()
        {
            return 1856238518 + Pm.GetHashCode();
        }

        public static bool operator ==(PontoOld ponto1, PontoOld ponto2)
        {
            return EqualityComparer<PontoOld>.Default.Equals(ponto1, ponto2);
        }

        public static bool operator !=(PontoOld ponto1, PontoOld ponto2)
        {
            return !(ponto1 == ponto2);
        }

        public static double Round(double profundidade)
        {
            return FastMath.Round(profundidade, 1);
        }
    }

    public static class FastMath
    {
        private static readonly double[] RoundLookup = CreateRoundLookup();

        private static double[] CreateRoundLookup()
        {
            double[] result = new double[15];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Pow(10, i);
            }

            return result;
        }

        public static double Round(double value)
        {
            return Math.Floor(value + 0.5);
        }

        public static double Round(double value, int decimalPlaces)
        {
            double adjustment = RoundLookup[decimalPlaces];
            return Math.Floor(value * adjustment + 0.5) / adjustment;
        }
    }
}