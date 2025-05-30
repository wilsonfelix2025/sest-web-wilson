using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorTrajetoriaPoçoWeb
    {
        public TrajetóriaDTO Trajetória { get; private set; }
        private readonly PoçoWebDto _poçoWeb;
        private bool _buscarDadosShallow;

        public LeitorTrajetoriaPoçoWeb(PoçoWebDto poçoWeb, bool buscarDadosShallow)
        {
            _poçoWeb = poçoWeb;
            _buscarDadosShallow = buscarDadosShallow;
            Trajetória = new TrajetóriaDTO();
            GetTrajetória();
        }

        private void GetTrajetória()
        {
            if (_poçoWeb.Revision.Content.Input.Trajectory == null)
                return;

            foreach (var ponto in _poçoWeb.Revision.Content.Input.Trajectory.Points)
            {

                var pm = ponto.Md;

                if (pm != null)
                {
                    Trajetória.Pontos.Add(new PontoTrajetóriaDTO
                    {
                        Azimute = ponto.Azimuth.ToString(),
                        Inclinação = ponto.Inclination.ToString(),
                        Pm = pm.ToString()
                    });
                }
                if (_buscarDadosShallow)
                    break;
            }
        }
    }
}
