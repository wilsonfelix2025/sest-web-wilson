using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Domain.DTOs.Cálculo.Gradientes
{
    public class InformaçãoDTO
    {
        public double Pm { get; set; }
        public double Pv { get; set; }
        public double GCOLI { get; set; }
        public double GCOLS { get; set; }
        public double GQUEBRA { get; set; }
        public string Obs { get; set; }        
    }
}
