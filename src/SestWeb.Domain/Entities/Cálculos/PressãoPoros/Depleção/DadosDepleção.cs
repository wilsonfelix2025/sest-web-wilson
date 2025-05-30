using SestWeb.Domain.Entities.ProfundidadeEntity;
using System;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção
{
    public class DadosDepleção
    {
        public Reservatório Reservatório { get; set; }
        public string NomePoço { get; set; }
        public Profundidade PvCorrelação { get; set; }
        public double PressãoCorrelação { get; set; }

        public CasosDepleção ObterCaso(Profundidade pvi)
        {
            var pvOleoAgua = Reservatório.ContatoÓleoÁgua;
            var pvGasOleo = Reservatório.ContatoGásÓleo;

            if (PvCorrelação < pvi)
            {
                if (PvCorrelação < pvGasOleo &&
                    pvOleoAgua < pvi)
                {
                    return CasosDepleção.Caso1;
                }

                if (pvGasOleo < PvCorrelação && PvCorrelação < pvOleoAgua &&
                    pvOleoAgua < pvi)
                {
                    return CasosDepleção.Caso2;
                }

                if (pvGasOleo < pvOleoAgua && pvOleoAgua < PvCorrelação && PvCorrelação < pvi)
                {
                    return CasosDepleção.Caso3;
                }

                if (PvCorrelação < pvGasOleo && pvGasOleo < pvi && pvi < pvOleoAgua)
                {
                    return CasosDepleção.Caso4;
                }

                if (PvCorrelação < pvGasOleo && pvGasOleo < pvOleoAgua && pvi < pvGasOleo)
                {
                    return CasosDepleção.Caso5;
                }

                if (pvGasOleo < PvCorrelação && PvCorrelação < pvOleoAgua &&
                    pvGasOleo < pvi && pvi < pvOleoAgua)
                {
                    return CasosDepleção.Caso6;
                }
            }
            else if (PvCorrelação > pvi)
            {
                if (PvCorrelação > pvOleoAgua && pvOleoAgua > pvGasOleo && pvGasOleo > pvi)
                {
                    return CasosDepleção.Caso7;
                }

                if (pvOleoAgua > PvCorrelação && PvCorrelação > pvGasOleo &&
                    pvi < pvGasOleo)
                {
                    return CasosDepleção.Caso8;
                }

                if (pvOleoAgua > pvGasOleo && pvGasOleo > PvCorrelação)
                {
                    return CasosDepleção.Caso9;
                }

                if (PvCorrelação > pvOleoAgua && pvGasOleo < pvi && pvi < pvOleoAgua)
                {
                    return CasosDepleção.Caso10;
                }

                if (PvCorrelação > pvOleoAgua && pvOleoAgua > pvGasOleo && pvi > pvOleoAgua)
                {
                    return CasosDepleção.Caso11;
                }

                if (pvOleoAgua > PvCorrelação && PvCorrelação > pvGasOleo
                                              && pvGasOleo < pvi && pvi < pvOleoAgua)
                {
                    return CasosDepleção.Caso12;
                }
            }

            throw new ArgumentException("Nenhum caso de depleção foi enquadrado");
        }
    }
}
