using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.LitologiaDoPoco
{
    public class PontoLitologia : IEquatable<PontoLitologia>, IPonto
    {
        private readonly IConversorProfundidade _conversorProfundidade;

        #region Properties
        public Profundidade Pm { get; set; }
        public Profundidade Pv { get; private set; }
        public double Valor { get; private set; }
        public TipoProfundidade TipoProfundidade { get; private set; }
        public OrigemPonto Origem { get; private set; }
        public TipoRocha TipoRocha { get; private set; }
        public Profundidade Profundidade => TipoProfundidade == TipoProfundidade.PM ? Pm : Pv;
        public bool EstáEmTrechoHorizontal => _conversorProfundidade?.EstáEmTrechoHorizontal(Pm, Pv) ?? false;
        #endregion

        public PontoLitologia(Profundidade pm, Profundidade pv, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, IConversorProfundidade conversorProfundidade)
        {
            _conversorProfundidade = conversorProfundidade;
            Pm = pm;
            Pv = pv;
            TipoProfundidade = tipoProfundidade;
            Origem = origem;
            if (!string.IsNullOrWhiteSpace(tipoRocha))
                TipoRocha = TipoRocha.FromMnemonico(tipoRocha);
        }

        #region Methods

        public override string ToString() => $"{Pm}m - {TipoRocha}";

        public override bool Equals(object obj)
        {
            return Equals(obj as PontoLitologia);
        }

        public bool Equals(PontoLitologia other)
        {
            return other != null &&
                   Pm == other.Pm &&
                   EqualityComparer<TipoRocha>.Default.Equals(TipoRocha, other.TipoRocha);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pm, TipoRocha);
        }

        public static bool operator ==(PontoLitologia litologia1, PontoLitologia litologia2)
        {
            return EqualityComparer<PontoLitologia>.Default.Equals(litologia1, litologia2);
        }

        public static bool operator !=(PontoLitologia litologia1, PontoLitologia litologia2)
        {
            return !(litologia1 == litologia2);
        }

        public void Edit(string newTipoRocha)
        {
            TipoRocha = TipoRocha.FromMnemonico(newTipoRocha);
        }

        public void Edit(double newValue)
        {
            
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