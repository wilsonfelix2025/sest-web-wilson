namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.EditingRelacionamentoPropMec
{
    public class RelacionamentoPropMecEmEdição
    {
        public RelacionamentoPropMecEmEdição(string origem)
        {
            Origem = origem;
        }

        public string Origem { get; }
    }
}
