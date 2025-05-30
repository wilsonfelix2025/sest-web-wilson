using System;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.RegistrosDePerfuração
{
    public class PontoRegistroPerfuração 
    {
        public PontoRegistroPerfuração(double prof, double valor, string descrição)
        {
            if (prof < 0)
                throw new ArgumentException("Profundidade fora de valores permitidos.");

            Profundidade = new Profundidade(prof);
            Valor = valor;
            Descrição = descrição;
        }

        public Profundidade Profundidade { get; private set; }

        public double Valor { get; private set; }

        public string Descrição { get; set; }

        public TipoProfundidade TipoProfundidade { get; private set; }

        public void ConverterParaPv(Trajetória trajetória)
        {
            if (TipoProfundidade == TipoProfundidade.PV)
                return;

            if (!trajetória.TryGetTVDFromMD(Profundidade.Valor, out double pv))
                return;

            //Profundidade = new Profundidade((double)Math.Truncate((decimal)pv * 100) / 100);
            Profundidade = new Profundidade((double)Math.Truncate((decimal)pv * 10000000) / 10000000);

            TipoProfundidade = TipoProfundidade.PV;
        }

        public void ConverterParaPm(Trajetória trajetória)
        {
            if (TipoProfundidade == TipoProfundidade.PM)
                return;

            if (!trajetória.TryGetMDFromTVD(Profundidade.Valor, out double pm))
                return;

            //Profundidade = new Profundidade((double)Math.Truncate((decimal)pm * 100) / 100);
            Profundidade = new Profundidade((double)Math.Truncate((decimal)pm * 10000000) / 10000000);

            TipoProfundidade = TipoProfundidade.PM;
        }

        public void Shift(double delta)
        {
            Profundidade = new Profundidade(Profundidade.Valor + delta);
        }

        public void AtualizarPv(Trajetória trajetória)
        {
            if (!trajetória.TryGetTVDFromMD(Profundidade.Valor, out double pv))
                return;
        }
    }
}
