using System.Collections.Generic;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.PontosEntity.Factory
{
    public interface IPontoFactory<T> where T : IPonto

    {
        bool Criar(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade profPv, double valor, TipoProfundidade tipoProfundidade,
        OrigemPonto interpolado, ILitologia litologia, out T ponto);

        bool Criar(IConversorProfundidade conversorProfundidade, double pm, double pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto);

        bool Criar(IConversorProfundidade conversorProfundidade, Profundidade pmProf, Profundidade profPv, string tipoRocha, TipoProfundidade tipoProfundidade,
            OrigemPonto interpolado, out T ponto);

        bool Criar(IConversorProfundidade conversorProfundidade, double pm, double pv, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto);


        bool CriarEmPm(IConversorProfundidade conversorProfundidade, Profundidade pmProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem,
            ILitologia litologia, out T ponto);

        bool CriarEmPm(IConversorProfundidade conversorProfundidade, double pm, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto);

        bool CriarEmPm(IConversorProfundidade conversorProfundidade, Profundidade pmProf, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem,
            out T ponto);

        bool CriarEmPm(IConversorProfundidade conversorProfundidade, double pm, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto);


        bool CriarEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvProf, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto);

        bool CriarEmPv(IConversorProfundidade conversorProfundidade, double pv, double valor, TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out T ponto);

        bool CriarEmPv(IConversorProfundidade conversorProfundidade, Profundidade pvProf, string TipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto);

        bool CriarEmPv(IConversorProfundidade conversorProfundidade, double pv, string tipoRocha, TipoProfundidade tipoProfundidade, OrigemPonto origem, out T ponto);


        bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<double> pvs, IList<double> valores, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, ILitologia litologia, out IList<T> pontos);

        bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<Profundidade> pvs, IList<double> valores,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, ILitologia litologia, out IList<T> pontos);

        bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<double> pvs, IList<string> tiposRocha, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, out IList<T> pontos);

        bool CriarPontos(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<Profundidade> pvs, IList<string> tiposRocha,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos);

        bool CriarPontos(IEnumerable<double> pms, IEnumerable<double> pvs, IList<double> valores,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos);

        bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IList<double> pms, IList<double> valores, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, ILitologia litologia, out IList<T> pontos);

        bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IEnumerable<double> pms, IEnumerable<double> valores, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, out IList<T> pontos);

        bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<double> valores, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, ILitologia litologia, out IList<T> pontos);

        bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IList<Profundidade> pms, IList<string> tipoRocha, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, out IList<T> pontos);

        bool CriarPontosEmPm(IConversorProfundidade conversorProfundidade, IEnumerable<Profundidade> pms, IEnumerable<double> valores,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos);

        bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<double> pvs, IList<double> valores, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, ILitologia litologia, out IList<T> pontos);

        bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<double> pvs, IList<string> tiposRocha, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, out IList<T> pontos);

        bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<Profundidade> pvs, IList<double> valores, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, ILitologia litologia, out IList<T> pontos);

        bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IList<Profundidade> pvs, IList<string> tiposRocha, TipoProfundidade tipoProfundidade,
            OrigemPonto origem, out IList<T> pontos);

        bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IEnumerable<double> pvs, IEnumerable<double> valores,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos);

        bool CriarPontosEmPv(IConversorProfundidade conversorProfundidade, IEnumerable<Profundidade> pvs, IEnumerable<double> valores,
            TipoProfundidade tipoProfundidade, OrigemPonto origem, out IList<T> pontos);

       
    }
}
