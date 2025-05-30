using SestWeb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Helpers
{
    public static class BuscaBinariaExtension
    {
        public static bool BuscaBinariaPorPmMenorIgual<T>(this List<T> pontos, double pm, out int index, out bool equal) where T : IIndexadoPorPM
        {
            index = -1;

            if (!pontos.Any() || pm > pontos[pontos.Count - 1].Pm || pm < pontos[0].Pm)
            {
                equal = false;
                return false;
            }

            //pm = Profundidade.Round(pm);

            var low = 0;
            var high = pontos.Count - 1;
            var mid = 0;

            while (low <= high)
            {
                mid = low + ((high - low) / 2);

                var pontoCorrente = pontos[mid];

                if (Math.Abs(pontos[mid].Pm - pm) < 0.001)
                {
                    index = mid; // key found
                    equal = true;
                    return true;
                }

                if (pontos[mid].Pm < pm)
                {
                    low = mid + 1;
                }
                else if (pontoCorrente.Pm > pm)
                {
                    high = mid - 1;
                }
            }

            if (pm > pontos[mid].Pm)
            {
                index = mid;
            }
            else // less
            {
                index = mid - 1;
            }

            equal = false;
            return index != -1;
        }
    }
}