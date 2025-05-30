using System;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    public class Triangulo
    {
        public Triangulo(PontoDaMalha p1, PontoDaMalha p2, PontoDaMalha p3)
        {
            Ponto1 = p1;
            Ponto2 = p2;
            Ponto3 = p3;
        }

        public PontoDaMalha Ponto1 { private set; get; }
        public PontoDaMalha Ponto2 { private set; get; }
        public PontoDaMalha Ponto3 { private set; get; }

        public double ObterAreaPlastificada()
        {
            var pontos = new List<PontoDaMalha>() { Ponto1, Ponto2, Ponto3 };
            var pontosOrdenados = pontos.OrderBy(p => p.FatorPlastificação).ToList();
            double[] p1 = { pontosOrdenados[0].X, pontosOrdenados[0].Y };
            double[] p2 = { pontosOrdenados[1].X, pontosOrdenados[1].Y };
            double[] p3 = { pontosOrdenados[2].X, pontosOrdenados[2].Y };

            double area = 0;

            if (pontosOrdenados[0].FatorPlastificação >= 1 && pontosOrdenados[1].FatorPlastificação >= 1 &&
                pontosOrdenados[2].FatorPlastificação >= 1)
            {
                area = ObterArea(p1[0], p1[1], p2[0], p2[1], p3[0], p3[1]);
            }
            else if (pontosOrdenados[1].FatorPlastificação >= 1 && pontosOrdenados[2].FatorPlastificação >= 1)
            {
                // Obter ponto entre ponto 1 e ponto 3
                p1[0] = (1 - pontosOrdenados[0].FatorPlastificação) * (pontosOrdenados[2].X - pontosOrdenados[0].X) /
                        (pontosOrdenados[2].FatorPlastificação - pontosOrdenados[0].FatorPlastificação) +
                        pontosOrdenados[0].X;

                p1[1] = (1 - pontosOrdenados[0].FatorPlastificação) * (pontosOrdenados[2].Y - pontosOrdenados[0].Y) /
                        (pontosOrdenados[2].FatorPlastificação - pontosOrdenados[0].FatorPlastificação) +
                        pontosOrdenados[0].Y;

                area = ObterArea(p1[0], p1[1], p2[0], p2[1], p3[0], p3[1]);


                // Obter ponto entre ponto 1 e ponto 2
                p3[0] = (1 - pontosOrdenados[0].FatorPlastificação) * (pontosOrdenados[1].X - pontosOrdenados[0].X) /
                        (pontosOrdenados[1].FatorPlastificação - pontosOrdenados[0].FatorPlastificação) +
                        pontosOrdenados[0].X;

                p3[1] = (1 - pontosOrdenados[0].FatorPlastificação) * (pontosOrdenados[1].Y - pontosOrdenados[0].Y) /
                        (pontosOrdenados[1].FatorPlastificação - pontosOrdenados[0].FatorPlastificação) +
                        pontosOrdenados[0].Y;

                area += ObterArea(p1[0], p1[1], p2[0], p2[1], p3[0], p3[1]);
            }
            else if (pontosOrdenados[2].FatorPlastificação >= 1)
            {
                p1[0] = (1 - pontosOrdenados[0].FatorPlastificação) * (pontosOrdenados[2].X - pontosOrdenados[0].X) /
                        (pontosOrdenados[2].FatorPlastificação - pontosOrdenados[0].FatorPlastificação) +
                        pontosOrdenados[0].X;

                p1[1] = (1 - pontosOrdenados[0].FatorPlastificação) * (pontosOrdenados[2].Y - pontosOrdenados[0].Y) /
                        (pontosOrdenados[2].FatorPlastificação - pontosOrdenados[0].FatorPlastificação) +
                        pontosOrdenados[0].Y;


                p2[0] = (1 - pontosOrdenados[1].FatorPlastificação) * (pontosOrdenados[2].X - pontosOrdenados[1].X) /
                        (pontosOrdenados[2].FatorPlastificação - pontosOrdenados[1].FatorPlastificação) +
                        pontosOrdenados[1].X;

                p2[1] = (1 - pontosOrdenados[1].FatorPlastificação) * (pontosOrdenados[2].Y - pontosOrdenados[1].Y) /
                        (pontosOrdenados[2].FatorPlastificação - pontosOrdenados[1].FatorPlastificação) +
                        pontosOrdenados[1].Y;

                area = ObterArea(p1[0], p1[1], p2[0], p2[1], p3[0], p3[1]);
            }

            return area;
        }

        private double ObterArea(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Math.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1)) / 2;
        }
    }
}
