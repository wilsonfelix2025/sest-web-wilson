using System;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.PontosEntity
{
    public class Ponto : IPonto
    {
        private readonly IConversorProfundidade _conversorProfundidade;
        private readonly ILitologia _litologia;

        public Ponto(Profundidade pm, Profundidade pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, IConversorProfundidade conversorProfundidade, ILitologia litologia)
        {
            _conversorProfundidade = conversorProfundidade;
            _litologia = litologia;
            Pm = pm;
            Pv = pv;
            Valor = valor;
            TipoProfundidade = tipoProfundidade;
            Origem = origem;
            
            ComplementarPonto();
        }

        public Ponto(Profundidade pm, Profundidade pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, string tipoRocha)
        {
            Pm = pm;
            Pv = pv;
            Valor = valor;
            TipoProfundidade = tipoProfundidade;
            Origem = origem;
            if (!string.IsNullOrWhiteSpace(tipoRocha))
                TipoRocha = TipoRocha.FromMnemonico(tipoRocha);
        }

        #region Properties

        public Profundidade Pm { get; private set; }

        public Profundidade Pv { get; private set; }

        public double Valor { get; private set; }

        public TipoProfundidade TipoProfundidade { get; private set; }

        public OrigemPonto Origem { get; private set; }

        public TipoRocha TipoRocha { get; private set; }

        public Profundidade Profundidade => TipoProfundidade == TipoProfundidade.PM ? Pm : Pv;

        public bool EstáEmTrechoHorizontal => _conversorProfundidade?.EstáEmTrechoHorizontal(Pm, Pv) ?? false;

        #endregion

        #region Methods

        public void Edit(double newValue)
        {
            Valor = newValue;
        }

        public void Edit(string newValue)
        {
            Valor = newValue.ToDouble();
        }

        public void TrocarProfundidadeReferenciaParaPV()
        {
            TipoProfundidade = TipoProfundidade.PV;
        }

        public void TrocarProfundidadeReferenciaParaPM()
        {
            TipoProfundidade = TipoProfundidade.PM;
        }

        public void AtualizarPV()
        {
            TryGetPv();
        }

        public void ConverterParaPv()
        {
            if (TipoProfundidade == TipoProfundidade.PV)
                return;

            if (Pv == null || Math.Abs(Pv.Valor) < 0.0099)
                TryGetPv();

            TrocarProfundidadeReferenciaParaPV();
        }

        public void ConverterParaPM()
        {
            if (Pm == null || Math.Abs(Pm.Valor) < 0.0099)
                TryGetPm();

            TrocarProfundidadeReferenciaParaPM();
        }

        public void Shift(double delta, bool gradiente = false)
        {
            double pvAnterior = Pv.Valor;
            Pm = new Profundidade(Pm.Valor + delta);
            Pv = new Profundidade(Pv.Valor + delta);

            if (gradiente)
            {
                Valor *= pvAnterior / Pv.Valor;
            }
        }

        private void ComplementarPontoLitologia()
        {
            if (_litologia != null && _litologia.TryGetLitoPontoEmPm(_conversorProfundidade, Pm, out var pontoLito))
            {
                TipoRocha = pontoLito.TipoRocha;
            }
        }

        private void ComplementarPonto()
        {
            // Pm e Pv null
            if (Pm == null && Pv == null)
                throw new ArgumentNullException("Um ponto não pode ser instanciado com valores null para Pm e Pv.");

            // Pm zerado
            if (Pm == null || Math.Abs(Pm.Valor) < 0.009)
            {
                TryGetPm();
            }

            // Pv zerado
            if (Pv == null || Math.Abs(Pv.Valor) < 0.0099)
            {
                TryGetPv();
            }

            ComplementarPontoLitologia();
        }

        private void TryGetPv()
        {
            if (_conversorProfundidade != null && _conversorProfundidade.TryGetTVDFromMD(Pm.Valor, out double pv))
            {
                //pv = (double)Math.Truncate((decimal)pv * 100) / 100;
                pv = (double)Math.Truncate((decimal)pv * 10000000) / 10000000;
                Pv = new Profundidade(pv);
            }
        }

        private void TryGetPm()
        {
            if (_conversorProfundidade != null && _conversorProfundidade.TryGetMDFromTVD(Pv.Valor, out double pm))
            {
                //pm = (double)Math.Truncate((decimal)pm * 100) / 100;
                pm = (double)Math.Truncate((decimal)pm * 10000000) / 10000000;
                Pm = new Profundidade(pm);
            }
        }

        #endregion

        
    }
}
