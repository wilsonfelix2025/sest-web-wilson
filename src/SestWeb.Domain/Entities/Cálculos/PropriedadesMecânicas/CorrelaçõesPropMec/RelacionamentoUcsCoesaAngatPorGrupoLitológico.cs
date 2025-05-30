using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public class RelacionamentoUcsCoesaAngatPorGrupoLitológico : RelacionamentoUcsCoesaAngat, ISupportInitialize
    {
        #region Constructor

        private RelacionamentoUcsCoesaAngatPorGrupoLitológico(GrupoLitologico grupoLitológico, Origem origem, IAutor autor, Correlação ucsCorr, Correlação coesaCorr, Correlação angatCorr, Correlação restrCorr)
            : base(ucsCorr, coesaCorr, angatCorr, restrCorr)
        {
            GrupoLitológico = grupoLitológico;
            Origem = origem;
            Autor = autor;
            Nome = grupoLitológico.Nome + "_" + ucsCorr.Nome + "_" + coesaCorr.Nome + "_" + angatCorr.Nome + "_" +
                   restrCorr.Nome;
        }

        public static void RegisterRelacionamentoPropMecCtor()
        {
            RelacionamentoPropMecFactory
                .RegisterCorrelaçãoCtor((grupoLitológico, origem, autor, ucsCorr, coesaCorr, angatCorr, restrCorr) => new RelacionamentoUcsCoesaAngatPorGrupoLitológico(grupoLitológico, origem, autor, ucsCorr, coesaCorr, angatCorr, restrCorr));
        }

        #endregion

        #region Properties

        public GrupoLitologico GrupoLitológico { get; private set; }

        public string Nome { get; private set; }

        public Origem Origem { get; private set; }

        public IAutor Autor { get; private set; }

        #endregion

        #region Methods

        public static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(RelacionamentoUcsCoesaAngatPorGrupoLitológico)))
                return;

            if (!BsonClassMap.IsClassMapRegistered(typeof(Autor)))
                Correlações.AutorCorrelação.Autor.Map();

            BsonClassMap.RegisterClassMap<RelacionamentoUcsCoesaAngatPorGrupoLitológico>(rel =>
            {
                rel.AutoMap();
                rel.MapCreator(c =>
                    new RelacionamentoUcsCoesaAngatPorGrupoLitológico(c.GrupoLitológico, c.Origem, c.Autor, c.Ucs, c.Coesa, c.Angat, c.Restr));
                rel.MapMember(r => r.Nome);
                rel.SetIdMember(rel.GetMemberMap(r => r.Nome));
                rel.MapMember(r => r.GrupoLitológico);
                rel.MapMember(r => r.Autor).SetSerializer(new ImpliedImplementationInterfaceSerializer<IAutor, Autor>());
                rel.MapMember(r => r.Origem).SetSerializer(new EnumSerializer<Origem>(BsonType.String));
                rel.SetIgnoreExtraElements(true);
                rel.SetDiscriminator("RelacionamentoUcsCoesaAngatPorGrupoLitológico");
            });
        }


        #region ISupportInitialize members

        public void BeginInit() { }

        public void EndInit()
        {
            int a = 2;
        }

        #endregion

        #endregion
    }
}
