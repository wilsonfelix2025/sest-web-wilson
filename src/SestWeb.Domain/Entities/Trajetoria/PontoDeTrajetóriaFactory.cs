using System;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Trajetoria
{
    public abstract class PontoDeTrajetóriaFactory
    {
        public PontoDeTrajetória CriarPontoInicial()
        {
            return CriarPonto(0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d, 0d);
        }

        public bool CriarPontoPorPm(double pm, double inclinação, double azimute, MétodoDeCálculoDaTrajetória métodoDeCálculo, out PontoDeTrajetória pontoDeTrajetória)
        {
            try
            {
                pontoDeTrajetória = CriarPonto(pm, 0d, inclinação, azimute, 0d, 0d, 0d, 0d, 0d);

                if (TryGetSessãoTrajetória(new Profundidade(pm), out SessãoTrajetória sessãoTrajetória))
                {
                    GeradorPontosTrajetória.GerarDadosPontoTrajetória(sessãoTrajetória.PontoInicial, pontoDeTrajetória, métodoDeCálculo);
                }
                else
                    GeradorPontosTrajetória.GerarDadosPontoTrajetória(GetÚltimoPonto(), pontoDeTrajetória, métodoDeCálculo);
                
                return true;
            }
            catch (Exception e)
            {
                pontoDeTrajetória = default;
                return false;
            }
        }

        public bool CriarPontoPorPm(double pm, MétodoDeCálculoDaTrajetória métodoDeCálculo, out PontoDeTrajetória pontoDeTrajetória)
        {
            try
            {
                if (!TryGetSessãoTrajetória(new Profundidade(pm), out SessãoTrajetória sessãoTrajetória))
                {
                    pontoDeTrajetória = default;
                    return false;
                }

                GeradorPontosTrajetória.ObterDadosDoPontoPorPm(new Profundidade(pm), sessãoTrajetória,
                    métodoDeCálculo, out double pv, out double inclinação, out double azimute, out double ew,
                    out double ns, out double dls, out double polCoordDispl, out double polCoordDirec);

                pontoDeTrajetória = CriarPonto(pm, pv, inclinação, azimute, ew, ns, dls, polCoordDispl, polCoordDirec);

                return true;
            }
            catch (Exception e)
            {
                pontoDeTrajetória = default;
                return false;
            }
        }

        public bool CriarPontoPorPv(double pv, MétodoDeCálculoDaTrajetória métodoDeCálculo, out PontoDeTrajetória pontoDeTrajetória)
        {
            try
            {
                if (!TryGetSessãoTrajetóriaEmPv(new Profundidade(pv), out SessãoTrajetória sessãoTrajetória))
                {
                    pontoDeTrajetória = default;
                    return false;
                }

                var gerouPonto = GeradorPontosTrajetória.ObterDadosDoPontoPorPv(new Profundidade(pv), sessãoTrajetória,
                    métodoDeCálculo, out double pm, out double inclinação, out double azimute, out double ew,
                    out double ns, out double dls, out double polCoordDispl, out double polCoordDirec);

                if (gerouPonto == false)
                {
                    pontoDeTrajetória = null;
                    return false;
                }

                pontoDeTrajetória = CriarPonto(pm, pv, inclinação, azimute, ew, ns, dls, polCoordDispl, polCoordDirec);
                return true;
            }
            catch (Exception e)
            {
                pontoDeTrajetória = default;
                return false;
            }
        }

        public PontoDeTrajetória CriarPonto(double pm, double pv, double inclinação, double azimute, double ew, double ns, double dls, double polCoordDispl, double polCoordDirec)
        {
            return new PontoDeTrajetória(pm, pv, inclinação, azimute, ew, ns, dls, polCoordDispl, polCoordDirec, GetDefaultAngle);
        }

        public abstract bool TryGetSessãoTrajetória(Profundidade pm, out SessãoTrajetória sessãoTrajetória);

        public abstract bool TryGetSessãoTrajetóriaEmPv(Profundidade pv, out SessãoTrajetória sessãoTrajetória);

        public abstract PontoDeTrajetória GetÚltimoPonto();

        public abstract PontoDeTrajetória GetPrimeiroPonto();

        public abstract double GetDefaultAngle();
    }
}
