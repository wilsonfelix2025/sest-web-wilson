using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Enums;
using MathNet.Numerics.LinearAlgebra;
using SestWeb.Domain.DTOs.InserirTrecho;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Validadores;
using SestWeb.Domain.Validadores.DTO;

namespace SestWeb.Domain.InserirTrecho
{
    public class InserirTrechos
    {
        private readonly InserirTrechoDTO InserirTrechoDto;
        private readonly IConversorProfundidade ConversorProfundidade;
        private readonly ILitologia _litologia;

        public InserirTrechos(IConversorProfundidade conversorProfundidade, ILitologia litologia, InserirTrechoDTO inserirTrecho)
        {
            InserirTrechoDto = inserirTrecho;
            ConversorProfundidade = conversorProfundidade;
            _litologia = litologia;
        }

        public Result InserirComplementoDeCurva()
        {
            var inserirTrechoValidação = new InserirTrechoValidator().Validate(InserirTrechoDto);

            if (inserirTrechoValidação.IsValid == false)
                return new Result{result = inserirTrechoValidação };

            var pontosAtéLimite = GetPontosAtéLimite(ConversorProfundidade, _litologia);
            var pmsTrechoInicial = GetPmsTrechoInicial();

            var valuesTrechoInicial = new List<double>();
            switch (InserirTrechoDto.TipoTratamentoTrecho)
            {
                case TipoTratamentoTrechoEnum.Exponencial:
                    valuesTrechoInicial = CalculaAjusteExponencial(pontosAtéLimite, pmsTrechoInicial);
                    break;
                case TipoTratamentoTrechoEnum.Linear:
                    valuesTrechoInicial = CalculaAjusteLinear(pontosAtéLimite, pmsTrechoInicial);
                    break;
                case TipoTratamentoTrechoEnum.Potência:
                    valuesTrechoInicial = CalculaAjustePotência(pontosAtéLimite, pmsTrechoInicial);
                    break;
                case TipoTratamentoTrechoEnum.Quadrático:
                    valuesTrechoInicial = CalculaAjusteQuadrático(pontosAtéLimite, pmsTrechoInicial);
                    break;
            }

           var novoPerfil = PerfisFactory.Create(InserirTrechoDto.PerfilSelecionado.Mnemonico,
               InserirTrechoDto.NovoNome, InserirTrechoDto.TrajetóriaPoço,
               InserirTrechoDto.LitologiaPoço);

           novoPerfil.AddPontosEmPm(ConversorProfundidade, pmsTrechoInicial.ToList(), valuesTrechoInicial,TipoProfundidade.PM, OrigemPonto.Completado, null);
           foreach (var ponto in InserirTrechoDto.PerfilSelecionado.Pontos)          {
               novoPerfil.AddPonto(ConversorProfundidade, ponto.Pm, ponto.Pv, ponto.Valor,ponto.TipoProfundidade, ponto.Origem);
           }

            var perfilValidationResult = new PerfilValidator().Validate(novoPerfil);
            var result = new Result {result = perfilValidationResult};

            if (perfilValidationResult.IsValid == false)
            {
                return result;
            }

            result.Entity = novoPerfil;
            return result;
        }

        private IEnumerable<Profundidade> GetPmsTrechoInicial()
        {
            var pontosACompletar = InserirTrechoDto.TipoDeTrecho == TipoDeTrechoEnum.Inicial ? 
                ObterTrechoInicialASerCompletado(InserirTrechoDto.BaseDeSedimentos, InserirTrechoDto.PerfilSelecionado) :
                ObterTrechoFinalASerCompletado(InserirTrechoDto.PerfilSelecionado.PmMáximo.Valor, InserirTrechoDto.ÚltimoPontoTrajetória);
            return pontosACompletar;
        }

        private IEnumerable<Ponto> GetPontosAtéLimite(IConversorProfundidade conversorProfundidade, ILitologia litologia)
        {
            IEnumerable<Ponto> pontosAtéLimite;

            InserirTrechoDto.PerfilSelecionado
                .TryGetPontosEmPm(InserirTrechoDto.PerfilSelecionado.PmMínimo,
                    new Profundidade(InserirTrechoDto.PmLimite), out IReadOnlyList<Ponto> lowerPoints);


            var lowerPointsComPeso1000 = lowerPoints.ToList();
            for (int i = 0; i <= 1000; i++)
            { 
                var pt = new Ponto(new Profundidade(InserirTrechoDto.BaseDeSedimentos), new Profundidade(InserirTrechoDto.BaseDeSedimentos), InserirTrechoDto.ValorTopo, TipoProfundidade.PM, OrigemPonto.Completado, conversorProfundidade, litologia);
                lowerPointsComPeso1000.Add(pt);
            }

            lowerPointsComPeso1000 = lowerPointsComPeso1000.OrderBy(s => s.Pm).ToList();

            InserirTrechoDto.PerfilSelecionado
                .TryGetPontosEmPm(new Profundidade(InserirTrechoDto.PmLimite),
                    InserirTrechoDto.PerfilSelecionado.PmMáximo, out IReadOnlyList<Ponto> upperPoints);

            
            pontosAtéLimite = InserirTrechoDto.TipoDeTrecho == TipoDeTrechoEnum.Inicial ? lowerPointsComPeso1000 : upperPoints;

            if (InserirTrechoDto.LitologiasSelecionadas != null && InserirTrechoDto.LitologiasSelecionadas.Any())
            {
                pontosAtéLimite = pontosAtéLimite.Where(p =>
                    p.TipoRocha != null && InserirTrechoDto.LitologiasSelecionadas.Contains(p.TipoRocha.Mnemonico));
            }

            return pontosAtéLimite;
        }

        private List<double> CalculaAjusteQuadrático(IEnumerable<Ponto> pontosDoPerfilAtéOLimite, IEnumerable<Profundidade> pontosACompletar)
        {
            var x = 0.0;
            var y = 0.0;
            var y2 = 0.0;
            var y3 = 0.0;
            var y4 = 0.0;
            var xy = 0.0;
            var xy2 = 0.0;
            var n = pontosDoPerfilAtéOLimite.Count();

            var valoresTrechoInicial = new List<double>();

            foreach (var ponto in pontosDoPerfilAtéOLimite)
            {
                x += ponto.Valor;
                y += ponto.Pm.Valor;
                y2 += ponto.Pm.Valor * ponto.Pm.Valor;
                y3 += ponto.Pm.Valor * ponto.Pm.Valor * ponto.Pm.Valor;
                y4 += ponto.Pm.Valor * ponto.Pm.Valor * ponto.Pm.Valor * ponto.Pm.Valor;
                xy += ponto.Pm.Valor * ponto.Valor;
                xy2 += ponto.Pm.Valor * ponto.Pm.Valor * ponto.Valor;
            }

            double[,] a =
            {
                {n, y, y2},
                {y, y2, y3},
                {y2, y3, y4}
            };
            double[] b = { x, xy, xy2 };

            var matrizA = Matrix<double>.Build.DenseOfArray(a);
            var inversaA = matrizA.Inverse();
            var vetorB = Vector<double>.Build.Dense(b);

            var vetorX = inversaA * vetorB;

            var a0 = vetorX[0];
            var a1 = vetorX[1];
            var a2 = vetorX[2];

            // cálculo dos pontos
            foreach (var profundidadeMedida in pontosACompletar)
            {
                var valorCalculado = a0 + a1 * profundidadeMedida.Valor + a2 * profundidadeMedida.Valor * profundidadeMedida.Valor;
                var valor = ObterValor(InserirTrechoDto.ValorTopo, valorCalculado, InserirTrechoDto.TipoDeTrecho);
                valoresTrechoInicial.Add(valor);
            }

            return valoresTrechoInicial;
        }

        private List<double> CalculaAjustePotência(IEnumerable<Ponto> pontosDoPerfilAtéOLimite, IEnumerable<Profundidade> pontosACompletar)
        {
            var x = 0.0;
            var y = 0.0;
            var y2 = 0.0;
            var xy = 0.0;
            var n = pontosDoPerfilAtéOLimite.Count();

            var valoresTrechoInicial = new List<double>();

            foreach (var ponto in pontosDoPerfilAtéOLimite)
            {
                x += Math.Log(ponto.Valor);
                y += Math.Log(ponto.Pm.Valor);
                y2 += Math.Log(ponto.Pm.Valor) * Math.Log(ponto.Pm.Valor);
                xy += Math.Log(ponto.Pm.Valor) * Math.Log(ponto.Valor);
            }
            var a0 = (y2 * x - y * xy) / (n * y2 - y * y);
            var a1 = (n * xy - y * x) / (n * y2 - y * y);

            // cálculo dos pontos
            foreach (var profundidadeMedida in pontosACompletar)
            {
                var valorCalculado = Math.Exp(a0) * Math.Pow(profundidadeMedida.Valor, a1);
                var valor = ObterValor(InserirTrechoDto.ValorTopo, valorCalculado, InserirTrechoDto.TipoDeTrecho);
                valoresTrechoInicial.Add(valor);
            }

            return valoresTrechoInicial;
        }

        private List<double> CalculaAjusteLinear(IEnumerable<Ponto> pontosDoPerfilAtéOLimite, IEnumerable<Profundidade> pontosACompletar)
        {
            var x = 0.0;
            var y = 0.0;
            var y2 = 0.0;
            var xy = 0.0;
            var n = pontosDoPerfilAtéOLimite.Count();

            var valoresTrechoInicial = new List<double>();
            foreach (var ponto in pontosDoPerfilAtéOLimite)
            {
                x += ponto.Valor;
                y += ponto.Pm.Valor;
                y2 += ponto.Pm.Valor * ponto.Pm.Valor;
                xy += ponto.Pm.Valor * ponto.Valor;
            }
            var a0 = (y2 * x - y * xy) / (n * y2 - y * y);
            var a1 = (n * xy - y * x) / (n * y2 - y * y);

            // cálculo dos pontos
            foreach (var profundidadeMedida in pontosACompletar)
            {
                var valorCalculado = a0 + a1 * profundidadeMedida.Valor;
                var valor = ObterValor(InserirTrechoDto.ValorTopo, valorCalculado, InserirTrechoDto.TipoDeTrecho);
                valoresTrechoInicial.Add(valor);
            }

            return valoresTrechoInicial;
        }

        private List<double> CalculaAjusteExponencial(IEnumerable<Ponto> pontosDoPerfilAtéOLimite, IEnumerable<Profundidade> pontosACompletar)
        {
            var x = 0.0;
            var y = 0.0;
            var y2 = 0.0;
            var xy = 0.0;
            var n = pontosDoPerfilAtéOLimite.Count();
            var valoresTrechoInicial = new List<double>();

            foreach (var ponto in pontosDoPerfilAtéOLimite)
            {
                x += Math.Log(ponto.Valor);
                y += ponto.Pm.Valor;
                y2 += ponto.Pm.Valor * ponto.Pm.Valor;
                xy += ponto.Pm.Valor * Math.Log(ponto.Valor);
            }
            var a0 = (y2 * x - y * xy) / (n * y2 - y * y);
            var a1 = (n * xy - y * x) / (n * y2 - y * y);

            // cálculo dos pontos
            foreach (var profundidadeMedida in pontosACompletar)
            {
                var valorCalculado = Math.Exp(a0) * Math.Exp(a1 * profundidadeMedida.Valor);
                var valor = ObterValor(InserirTrechoDto.ValorTopo, valorCalculado, InserirTrechoDto.TipoDeTrecho);
                valoresTrechoInicial.Add(valor);
            }

            return valoresTrechoInicial;
        }

        private double ObterValor(double valorTopo, double valorCalculado, TipoDeTrechoEnum tipoTrecho)
        {
    
            switch (InserirTrechoDto.PerfilSelecionado.Mnemonico)
            {
                case "DTC":
                    return valorCalculado > valorTopo ? valorTopo : valorCalculado;
                case "RHOB":
                    return valorCalculado < valorTopo ? valorTopo : valorCalculado;
                case "DTS":
                    return valorCalculado > valorTopo ? valorTopo : valorCalculado;
                default:
                    throw new InvalidOperationException("Tipo de perfil deve ser DTC, RHOB ou DTS.");
            }
        }

        private IEnumerable<Profundidade> ObterTrechoInicialASerCompletado(double baseDeSedimentos, PerfilBase perfil)
        {
            List<Profundidade> profundidades = new List<Profundidade>();
            for (int index = (int)baseDeSedimentos; index < perfil.PmMínimo.Valor; index++)
            {
                profundidades.Add(new Profundidade(index));
            }
            return profundidades;
        }

        private IEnumerable<Profundidade> ObterTrechoFinalASerCompletado(double últimoPontoPerfil, double últimoPontoTrajetória)
        {
            List<Profundidade> profundidades = new List<Profundidade>();
            for (int i = (int)últimoPontoPerfil; i < últimoPontoTrajetória; i++)
            {
                profundidades.Add(new Profundidade(i));
            }
            profundidades.Add(new Profundidade(últimoPontoTrajetória));
            return profundidades;
        }
    }
}
