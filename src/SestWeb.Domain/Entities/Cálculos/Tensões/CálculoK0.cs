using MathNet.Numerics;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public class CálculoK0 : CálculoTensãoHorizontalBase
    {
        private readonly IList<ParâmetrosLotDTO> _parametrosLot;
        private readonly IList<ParâmetrosLotDTO> _parametrosRetroanálise;

        public CálculoK0(List<PerfilBase> entradas, IList<ParâmetrosLotDTO> parametros, IConversorProfundidade trajetória
            , Geometria geometria, ILitologia litologia, bool K0Acomp) : base(entradas, trajetória, geometria, litologia)
        {
            if (parametros.Count <= 0)
                throw new ArgumentException("LOT não definidos.");

            if (K0Acomp)
            {
                _parametrosLot = parametros.OrderBy(p => p.ProfundidadeVertical).ToList();
                var ultimoLot = _parametrosLot.Last();

                foreach (var perfil in entradas)
                {
                    trajetória.TryGetMDFromTVD(ultimoLot.ProfundidadeVertical.Value, out double pm);

                    if (!perfil.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var ponto, GrupoCálculo.Tensões))
                    {
                        throw new ArgumentException($"LOT PV: {ultimoLot.ProfundidadeVertical}, definido fora do limite do perfil {perfil.Nome}.");
                    }
                }
                _parametrosRetroanálise = new List<ParâmetrosLotDTO>();
            } else
            {
                _parametrosLot = new List<ParâmetrosLotDTO>();
                _parametrosRetroanálise = parametros;
            }
        }

        public RetornoLotDTO GerarDadosGráfico()
        {
            var retornoLot = new RetornoLotDTO();

            var yArray = _parametrosRetroanálise.Select(p => OperaçõesDeConversão.ObterPressão(p.ProfundidadeVertical.Value, p.Lot.Value) - p.GradPressãoPoros).ToArray();
            var xArray = _parametrosRetroanálise.Select(p => p.Tvert - p.GradPressãoPoros).ToArray();

            double x = 0, y = 0;
            for (int i = 0; i < xArray.Length; i++)
            {
                y += xArray[i].Value * yArray[i].Value;
                x += xArray[i].Value * xArray[i].Value;
                        
            var pontoRetorno = new RetornoPontoLotDTO
                {
                    valorX = x,
                    valorY = y
                };

                retornoLot.PontosDTO.Add(pontoRetorno);
            }

            var k0 = y / x;
            retornoLot.Coeficiente = x == 0 ? 0 : k0;

            return retornoLot;
        }


        public Tuple<double, double>[] Calcular()
        {
            double profundidadeInicial;
            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
                    break;
                case CategoriaPoço.OnShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OnShore.AlturaDeAntePoço + Geometria.OnShore.LençolFreático;
                    break;
                default: throw new InvalidOperationException("Categoria de poço não reconhecida!");
            }

            if (_parametrosLot.Any())
                return CalcularK0(profundidadeInicial);

            return CalcularK0Retroanálise();
        }

        private Tuple<double, double>[] CalcularK0Retroanálise()
        {
            var yArray = _parametrosRetroanálise.Select(p => OperaçõesDeConversão.ObterPressão(p.ProfundidadeVertical.Value, p.Lot.Value) - p.GradPressãoPoros).ToArray();
            var xArray = _parametrosRetroanálise.Select(p => p.Tvert - p.GradPressãoPoros).ToArray();

            double x = 0, y = 0;
            for (int i = 0; i < xArray.Length; i++)
            {
                y += xArray[i].Value * yArray[i].Value;
                x += xArray[i].Value * xArray[i].Value;
            }

            var k0 = y / x;
            var profundidades = ObterProfundidades();
            return profundidades.Select(p => new Tuple<double, double>(p, k0)).ToArray();
        }

        private Tuple<double, double>[] CalcularK0(double profundidadeInicial)
        {
            if (_parametrosLot.Count == 1)
                return CalcularK0Unico(ObterProfundidades(), profundidadeInicial);

            var profundidades = ObterProfundidades();

            var resultado = CalcularPontosIniciais(profundidades, profundidadeInicial).ToList();
            resultado.AddRange(CalcularPontosIntermediários(profundidades));
            resultado.AddRange(CalcularPontosFinais(profundidades));

            return resultado.ToArray();
        }

        private Tuple<double, double>[] CalcularK0Unico(double[] profundidades, double profundidadeInicial)
        {
            Tuple<double, double>[] resultado;

            var gporo = Entradas.Single(x => x.Mnemonico == "GPORO" || x.Mnemonico == "GPPI");
            var tvert = Entradas.Single(x => x.Mnemonico == "TVERT");
            var parametroLot = _parametrosLot.First();

            Trajetória.TryGetMDFromTVD(parametroLot.ProfundidadeVertical.Value, out double pm);

            if (parametroLot.ProfundidadeVertical >= profundidadeInicial &&
                gporo.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoGporo, GrupoCálculo.Tensões) &&
                tvert.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoTvert, GrupoCálculo.Tensões))
            {
                var k0 = ObterK0(parametroLot, pontoGporo.Valor, pontoTvert.Valor);
                resultado = profundidades.Select(p => new Tuple<double, double>(p, k0)).ToArray();
            }
            else
            {
                throw new InvalidOperationException("PV inválido.");
            }

            return resultado;
        }

        private Tuple<double, double>[] CalcularPontosIniciais(double[] profundidades, double profundidadeInicial)
        {
            Tuple<double, double>[] resultado;

            var gporo = Entradas.Single(x => x.Mnemonico == "GPORO" || x.Mnemonico == "GPPI");
            var tvert = Entradas.Single(x => x.Mnemonico == "TVERT");

            var lot = _parametrosLot.First();

            double pm = -1;
            Trajetória.TryGetMDFromTVD(lot.ProfundidadeVertical.Value, out pm);

            if (lot.ProfundidadeVertical >= profundidadeInicial &&
                gporo.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoGporo, GrupoCálculo.Tensões) &&
                tvert.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoTvert, GrupoCálculo.Tensões))
            {
                var k0 = ObterK0(lot, pontoGporo.Valor, pontoTvert.Valor);
                var profundidadesIniciais = profundidades.Where(p => p <= pm);
                resultado = profundidadesIniciais.Select(p => new Tuple<double, double>(p, k0)).ToArray();
            }
            else
            {
                throw new InvalidOperationException("PV inicial inválido.");
            }
            return resultado;
        }

        private Tuple<double, double>[] CalcularPontosIntermediários(double[] profundidades)
        {
            var resultado = new ConcurrentBag<Tuple<double, double>>();
            var gporo = Entradas.Single(p => p.Mnemonico == "GPORO");
            var tvert = Entradas.Single(p => p.Mnemonico == "TVERT");

            var primeiroLot = _parametrosLot.First();
            var ultimoLot = _parametrosLot.Last();

            double pmInicial = -1, pmFinal = -1;

            if (!Trajetória.TryGetMDFromTVD(primeiroLot.ProfundidadeVertical.Value, out pmInicial) ||
                !Trajetória.TryGetMDFromTVD(ultimoLot.ProfundidadeVertical.Value, out pmFinal))
                throw new InvalidOperationException("PVs inválidos.");

            var profundidadesIntermediárias = profundidades.Where(p => p > pmInicial && p <= pmFinal).ToArray();

            List<double> kzeros = new List<double>();
            foreach (var lot in _parametrosLot)
            {
                Trajetória.TryGetMDFromTVD(lot.ProfundidadeVertical.Value, out double pm);
                if (gporo.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoGporo, GrupoCálculo.Tensões) &&
                    tvert.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoTvert, GrupoCálculo.Tensões))
                {
                    var k0 = ObterK0(lot, pontoGporo.Valor, pontoTvert.Valor);
                    kzeros.Add(k0);
                }
            }

            var y = _parametrosLot.Select(p => p.ProfundidadeVertical.Value).ToArray();
            var x = kzeros.ToArray();

            var interpolador = Interpolate.Linear(y, x);

            //TODO Parallel para performance
            for (int i = 0; i < profundidadesIntermediárias.Length; i++)
            {
                double profundidadePm = profundidadesIntermediárias[i];

                if (!Trajetória.TryGetTVDFromMD(profundidadePm, out double profundidadePv))
                    return null;

                var k0 = interpolador.Interpolate(profundidadePv);
                resultado.Add(new Tuple<double, double>(profundidadePm, k0));
            }

            return resultado.ToArray();
        }

        private Tuple<double, double>[] CalcularPontosFinais(double[] profundidades)
        {
            Tuple<double, double>[] resultado;

            var gporo = Entradas.Single(x => x.Mnemonico == "GPORO" || x.Mnemonico == "GPPI");
            var tvert = Entradas.Single(x => x.Mnemonico == "TVERT");
            var lot = _parametrosLot.Last();

            double pm = -1;
            Trajetória.TryGetMDFromTVD(lot.ProfundidadeVertical.Value, out pm);

            if (gporo.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoGporo, GrupoCálculo.Tensões) &&
                tvert.TryGetPontoEmPm(Trajetória, new Profundidade(pm), out var pontoTvert, GrupoCálculo.Tensões))
            {
                double valorGPoro = pontoGporo.Valor;
                double valorTVert = pontoTvert.Valor;

                var k0 = ObterK0(lot, valorGPoro, valorTVert);
                var profundidadesFinais = profundidades.Where(p => p > pm);
                resultado = profundidadesFinais.Select(p => new Tuple<double, double>(p, k0)).ToArray();
            }
            else
            {
                throw new InvalidOperationException("PV final inválido.");
            }
            return resultado;
        }

        private double ObterK0(ParâmetrosLotDTO lot, double gporo, double tvert)
        {
            var pp = OperaçõesDeConversão.ObterPressão(lot.ProfundidadeVertical.Value, gporo);
            var lotPP = OperaçõesDeConversão.ObterPressão(lot.ProfundidadeVertical.Value, lot.Lot.Value);
            var K0 = (lotPP - pp) / (tvert - pp);
            return K0;
        }
    }
}
