using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.RegistrosDePerfuração
{
    public class RegistroDePerfuração
    {
        private List<PontoRegistroPerfuração> _pontos;

        public RegistroDePerfuração(string nome, IEnumerable<PontoRegistroPerfuração> pontos = null)
        {
            Nome = nome;

            _pontos = new List<PontoRegistroPerfuração>();
            if (pontos != null)
                _pontos = pontos.ToList();
        }


        public string Nome { get; set; }

        public TipoProfundidade TipoProfundidade { get; private set; }

        public void ConverterParaPv(Trajetória trajetória, bool finalizarInterpolação = true)
        {
            if (TipoProfundidade == TipoProfundidade.PV)
                return;

            Parallel.ForEach(_pontos, ponto => ponto.ConverterParaPv(trajetória));

            TipoProfundidade = TipoProfundidade.PV;
        }

        public void ConverterParaPm(Trajetória trajetória)
        {
            if (TipoProfundidade == TipoProfundidade.PM)
                return;

            Parallel.ForEach(_pontos, ponto => ponto.ConverterParaPm(trajetória));
            
            TipoProfundidade = TipoProfundidade.PM;
        }

        public void Shift(double delta)
        {
            Parallel.ForEach(_pontos, ponto => ponto.Shift(delta));
        }

        public void AtualizarPvs(Trajetória trajetória)
        {
            Parallel.ForEach(_pontos, ponto => ponto.AtualizarPv(trajetória));
        }

    }
}
