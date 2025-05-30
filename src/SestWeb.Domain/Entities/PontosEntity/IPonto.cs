using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.PontosEntity
{
    public interface IPonto
    {
        Profundidade Pm { get; }

        Profundidade Pv { get; }

        Profundidade Profundidade { get; }

        double Valor { get; }

        TipoProfundidade TipoProfundidade { get; }
        
        OrigemPonto Origem { get; }

        TipoRocha TipoRocha { get; }

        bool EstáEmTrechoHorizontal { get; }

        void Edit(double newValue);
        void Edit(string newValue);

        void TrocarProfundidadeReferenciaParaPV();

        void TrocarProfundidadeReferenciaParaPM();

        void AtualizarPV();

        void ConverterParaPv();

        void ConverterParaPM();

        void Shift(double delta, bool gradiente = false);
    }
}
