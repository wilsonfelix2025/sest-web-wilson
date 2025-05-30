using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.PontosEntity
{
    public class PontosLitologiaSerializer : IBsonSerializer<Pontos<PontoLitologia>>
    {
        public Type ValueType => typeof(Pontos<PontoLitologia>);

        public Pontos<PontoLitologia> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var lista = new Pontos<PontoLitologia>(null, null, null);

            context.Reader.ReadStartArray();
            context.Reader.ReadBsonType();

            while (context.Reader.State != BsonReaderState.EndOfArray)
            {
                context.Reader.ReadStartDocument();

                var pm = context.Reader.ReadDouble();
                var pv = context.Reader.ReadDouble();
                var valor = context.Reader.ReadDouble();
                var origem = (OrigemPonto)Enum.Parse(typeof(OrigemPonto), context.Reader.ReadString());
                var tipoRocha = TipoRocha.FromMnemonico(context.Reader.ReadString());
                var tipoProfundiadde = (TipoProfundidade)Enum.Parse(typeof(TipoProfundidade), context.Reader.ReadString());

                var ponto = new PontoLitologia(new Profundidade(pm), new Profundidade(pv), tipoRocha?.Mnemonico, tipoProfundiadde, origem, null);

                context.Reader.ReadEndDocument();
                context.Reader.ReadBsonType();

                lista.AddPonto(ponto);
            }

            context.Reader.ReadEndArray();

            return lista;
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Pontos<PontoLitologia> value)
        {
            context.Writer.WriteStartArray();
            foreach (var pto in value._pmPointsCache.OrderBy(o => o.Key))
            {
                var ponto = new PontoMongoDb()
                {
                    Valor = pto.Value.Valor,
                    Pm = pto.Value.Pm.Valor,
                    Pv = pto.Value.Pv.Valor,
                    Origem = pto.Value.Origem.ToString(),
                    TipoProfundidade = pto.Value.TipoProfundidade.ToString(),
                    TipoRocha = pto.Value.TipoRocha != null ? pto.Value.TipoRocha.Mnemonico : string.Empty
                };

                var rawDoc = new RawBsonDocument(ponto.ToBson());
                context.Writer.WriteRawBsonDocument(rawDoc.Slice);
            }
            context.Writer.WriteEndArray();
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, value as Pontos<PontoLitologia>);
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }
    }

   
}

