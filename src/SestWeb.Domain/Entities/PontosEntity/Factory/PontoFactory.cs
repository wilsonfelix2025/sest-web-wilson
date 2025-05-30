using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.PontosEntity.Factory
{
    public abstract class PontoFactory<T> : IPontoFactory<T> where T : IPonto
    {
        //private readonly IConversorProfundidade _conversorProfundidade;
        private readonly ILitologia _litologia;

        private protected PontoFactory(IConversorProfundidade conversorProfundidade, ILitologia litologia)
        {
            //_conversorProfundidade = conversorProfundidade;
            _litologia = litologia;
        }

        private protected PontoFactory(IConversorProfundidade conversorProfundidade)
        {

        }

        public bool Criar(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade pvProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto) 
        {
            ponto = (T) Criar(conversorProfundidade, pmProf, pvProf, valor, tipoProfundidade, origem, litologia);
            return ponto!= null;
        }

        public bool Criar(IConversorProfundidade conversorProfundidade, double pm, double pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto) 
        {
            var pmProf = new Profundidade(pm);
            var pvProf = new Profundidade(pv);

            ponto = (T) Criar(conversorProfundidade, pmProf, pvProf, valor, tipoProfundidade, origem, litologia);
            return ponto != null;
        }

        public bool CriarEmPm(IConversorProfundidade conversorProfundidade, Profundidade pmProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto) 
        {
            if (!conversorProfundidade.TryGetTVDFromMD(pmProf.Valor, out double pv))
            {
                ponto = default;
                return false;
            }

            var pvProf = new Profundidade(pv);
            ponto = (T) Criar(conversorProfundidade, pmProf, pvProf, valor, tipoProfundidade, origem, litologia);
            return true;
        }

        public bool CriarEmPm(IConversorProfundidade conversorProfundidade, double pm, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto) 
        {
            if (!conversorProfundidade.TryGetTVDFromMD(pm, out double pv))
            {
                ponto = default;
                return false;
            }
            var pmProf = new Profundidade(pm);
            var pvProf = new Profundidade(pv);

            ponto = (T) Criar(conversorProfundidade, pmProf, pvProf, valor, tipoProfundidade, origem, litologia);
            return true;
        }

        public bool CriarEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto)
        {
            if (!conversorProfundidade.TryGetMDFromTVD(pvProf.Valor, out double pm))
            {
                ponto = default;
                return false;
            }

            var pmProf = new Profundidade(pm);
            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, valor, tipoProfundidade, origem, litologia);
            return true;
        }

        public bool CriarEmPv(IConversorProfundidade conversorProfundidade, double pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto)
        {
            if (!conversorProfundidade.TryGetMDFromTVD(pv, out double pm))
            {
                ponto = default;
                return false;
            }

            var pmProf = new Profundidade(pm);
            var pvProf = new Profundidade(pv);

            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, valor, tipoProfundidade, origem, litologia);
            return true;
        }

        public bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<double> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out IList<T> pontos)
        {
            if (pms == null || pvs == null || valores == null || pms.Count != pvs.Count || pms.Count != valores.Count || pvs.Count != valores.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            for (int index = 0; index < pms.Count; index++)
            {
                if (Criar(conversorProfundidade, pms[index], pvs[index], valores[index], tipoProfundidade, origem, litologia, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<Profundidade> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia,  out IList<T> pontos)
        {
            if (pms == null || pvs == null || valores == null || pms.Count != pvs.Count || pms.Count != valores.Count || pvs.Count != valores.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            for (int index = 0; index < pms.Count; index++)
            {
                if (Criar(conversorProfundidade, pms[index], pvs[index], valores[index], tipoProfundidade, origem, litologia, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontos(IEnumerable<Profundidade> pms, IEnumerable<Profundidade> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || pvs == null || valores == null || pms.Count() != pvs.Count() || pms.Count() != valores.Count || pvs.Count() != valores.Count)
            {
                pontos = default;
                return false;
            }

            return CriarPontos(pms.ToList(), pvs.ToList(), valores, tipoProfundidade, origem, out pontos);
            
        }

        public bool CriarPontos(IEnumerable<double> pms, IEnumerable<double> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || pvs == null || valores == null || pms.Count() != pvs.Count() || pms.Count() != valores.Count() || pvs.Count() != valores.Count())
            {
                pontos = default;
                return false;
            }

            return CriarPontos(pms.ToList(), pvs.ToList(), valores, tipoProfundidade, origem, out pontos);
        }

        public bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out IList<T> pontos)
        {
            if (pms == null || valores == null || tipoProfundidade != TipoProfundidade.PM || pms.Count != valores.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            if (pms.Count != valores.Count)
            {
                return false;
            }

            for (int index = 0; index < pms.Count; index++)
            {
                if(CriarEmPm(conversorProfundidade, pms[index], valores[index], tipoProfundidade, origem, litologia, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out IList<T> pontos)
        {
            if (pms == null || valores == null || tipoProfundidade != TipoProfundidade.PM || pms.Count != valores.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            if (pms.Count != valores.Count)
            {
                return false;
            }

            for (int index = 0; index < pms.Count; index++)
            {
                if (CriarEmPm(conversorProfundidade, pms[index], valores[index], tipoProfundidade, origem, litologia, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IEnumerable<double> pms, IEnumerable<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || valores == null || tipoProfundidade != TipoProfundidade.PM || pms.Count() != valores.Count())
            {
                pontos = default;
                return false;
            }

            return CriarPontosEmPm(conversorProfundidade, pms.ToList(), valores.ToList(), tipoProfundidade, origem, out pontos);
        }

        public bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IEnumerable<Profundidade> pms, IEnumerable<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || valores == null || tipoProfundidade != TipoProfundidade.PM || pms.Count() != valores.Count())
            {
                pontos = default;
                return false;
            }

            return CriarPontosEmPm(conversorProfundidade, pms.ToList(), valores.ToList(), tipoProfundidade, origem, out pontos);
        }

        public bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<double> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out IList<T> pontos)
        {
            if (pvs == null || valores == null || tipoProfundidade != TipoProfundidade.PV || pvs.Count != valores.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pvs.Count);

            if (pvs.Count != valores.Count)
            {
                return false;
            }

            for (int index = 0; index < pvs.Count; index++)
            {
                if (CriarEmPv(conversorProfundidade, pvs[index], valores[index], tipoProfundidade, origem, litologia, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<Profundidade> pvs, IList<double> valores, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out IList<T> pontos)
        {
            if (pvs == null || valores == null || tipoProfundidade != TipoProfundidade.PV || pvs.Count != valores.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pvs.Count);

            if (pvs.Count != valores.Count)
            {
                return false;
            }

            for (int index = 0; index < pvs.Count; index++)
            {
                if (CriarEmPv(conversorProfundidade, pvs[index], valores[index], tipoProfundidade, origem, litologia, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IEnumerable<double> pvs, IEnumerable<double> valores,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pvs == null || valores == null || tipoProfundidade != TipoProfundidade.PV || pvs.Count() != valores.Count())
            {
                pontos = default;
                return false;
            }

            return CriarPontosEmPv(conversorProfundidade, pvs.ToList(), valores.ToList(), tipoProfundidade, origem, out pontos);
        }

        public bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IEnumerable<Profundidade> pvs, IEnumerable<double> valores,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pvs == null || valores == null || tipoProfundidade != TipoProfundidade.PV || pvs.Count() != valores.Count())
            {
                pontos = default;
                return false;
            }

            return CriarPontosEmPv(conversorProfundidade, pvs.ToList(), valores.ToList(), tipoProfundidade, origem, out pontos);
        }

       

        private IPonto Criar(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade pvProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia)
        {
            var lito = _litologia == null ? litologia : _litologia;
            return new Ponto(pmProf, pvProf, valor, tipoProfundidade, origem, conversorProfundidade, lito);
        }

        #region Litologia

        public bool Criar(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade pvProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto)
        {
            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, tipoRocha, tipoProfundidade, origem);
            return ponto != null;
        }

        public bool Criar(IConversorProfundidade conversorProfundidade, double pm, double pv, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto)
        {
            var pmProf = new Profundidade(pm);
            var pvProf = new Profundidade(pv);

            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, tipoRocha, tipoProfundidade, origem);
            return ponto != null;
        }

        public bool CriarEmPm(IConversorProfundidade conversorProfundidade, Profundidade pmProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto)
        {
            if (!conversorProfundidade.TryGetTVDFromMD(pmProf.Valor, out double pv))
            {
                ponto = default;
                return false;
            }

            var pvProf = new Profundidade(pv);
            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, tipoRocha, tipoProfundidade, origem);
            return true;
        }

        public bool CriarEmPm(IConversorProfundidade conversorProfundidade, double pm, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto)
        {
            if (!conversorProfundidade.TryGetTVDFromMD(pm, out double pv))
            {
                ponto = default;
                return false;
            }
            var pmProf = new Profundidade(pm);
            var pvProf = new Profundidade(pv);

            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, tipoRocha, tipoProfundidade, origem);
            return true;
        }

        public bool CriarEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto)
        {
            if (!conversorProfundidade.TryGetMDFromTVD(pvProf.Valor, out double pm))
            {
                ponto = default;
                return false;
            }

            var pmProf = new Profundidade(pm);
            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, tipoRocha, tipoProfundidade, origem);
            return true;
        }

        public bool CriarEmPv(IConversorProfundidade conversorProfundidade, double pv, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto)
        {
            if (!conversorProfundidade.TryGetMDFromTVD(pv, out double pm))
            {
                ponto = default;
                return false;
            }

            var pmProf = new Profundidade(pm);
            var pvProf = new Profundidade(pv);

            ponto = (T)Criar(conversorProfundidade, pmProf, pvProf, tipoRocha, tipoProfundidade, origem);
            return true;
        }

        public bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<double> pvs, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || pvs == null || tiposRocha == null || pms.Count != pvs.Count || pms.Count != tiposRocha.Count || pvs.Count != tiposRocha.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            for (int index = 0; index < pms.Count; index++)
            {
                if (Criar(conversorProfundidade, pms[index], pvs[index], tiposRocha[index], tipoProfundidade, origem, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<Profundidade> pvs, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || pvs == null || tiposRocha == null || pms.Count != pvs.Count || pms.Count != tiposRocha.Count || pvs.Count != tiposRocha.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            for (int index = 0; index < pms.Count; index++)
            {
                if (Criar(conversorProfundidade, pms[index], pvs[index], tiposRocha[index], tipoProfundidade, origem, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        
        public bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || tiposRocha == null || tipoProfundidade != TipoProfundidade.PM || pms.Count != tiposRocha.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            if (pms.Count != tiposRocha.Count)
            {
                return false;
            }

            for (int index = 0; index < pms.Count; index++)
            {
                if (CriarEmPm(conversorProfundidade, pms[index], tiposRocha[index], tipoProfundidade, origem, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pms == null || tiposRocha == null || tipoProfundidade != TipoProfundidade.PM || pms.Count != tiposRocha.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pms.Count);

            if (pms.Count != tiposRocha.Count)
            {
                return false;
            }

            for (int index = 0; index < pms.Count; index++)
            {
                if (CriarEmPm(conversorProfundidade, pms[index], tiposRocha[index], tipoProfundidade, origem, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        
        public bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<double> pvs, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pvs == null || tiposRocha == null || tipoProfundidade != TipoProfundidade.PV || pvs.Count != tiposRocha.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pvs.Count);

            if (pvs.Count != tiposRocha.Count)
            {
                return false;
            }

            for (int index = 0; index < pvs.Count; index++)
            {
                if (CriarEmPv(conversorProfundidade, pvs[index], tiposRocha[index], tipoProfundidade, origem, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        public bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<Profundidade> pvs, IList<string> tiposRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos)
        {
            if (pvs == null || tiposRocha == null || tipoProfundidade != TipoProfundidade.PV || pvs.Count != tiposRocha.Count)
            {
                pontos = default;
                return false;
            }

            pontos = new List<T>(pvs.Count);

            if (pvs.Count != tiposRocha.Count)
            {
                return false;
            }

            for (int index = 0; index < pvs.Count; index++)
            {
                if (CriarEmPv(conversorProfundidade, pvs[index], tiposRocha[index], tipoProfundidade, origem, out T ponto))
                    pontos.Add(ponto);
            }

            return true;
        }

        
        private IPonto Criar(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade pvProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem)
        {
            return new PontoLitologia(pmProf, pvProf, tipoRocha, tipoProfundidade, origem, conversorProfundidade);
        }

        #endregion


    }
}
