using SestWeb.Domain.DTOs;
using SestWeb.Domain.Importadores.Shallow.SestTR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorRegistrosTR2
    {
        public List<RegistroDTO> Registros { get; private set; } = new List<RegistroDTO>();
        public bool LendoPonto = false;
        public bool LendoTrecho = false;
        public bool LendoProfundidade = false;
        public RegistroDTO Registro = null;
        PontoRegistroDTO Ponto = null;

        public LeitorRegistrosTR2()
        {
        }

        public void ProcessaLinha(string linha)
        {
            linha = linha.Trim();

            if (linha.Contains("<d5p1:RegistroDePerfuração"))
            {

                Registro = new RegistroDTO();
            }
            else if (linha.Contains("</d5p1:RegistroDePerfuração>"))
            {                
                    Registros.Add(Registro);
            }
            else if (linha.Contains("<d5p1:TipoDeRegistro"))
            {
                Registro.Tipo = LeitorHelperTR2.ObterValorTag(linha);

                //Tratamento de registros duplicados no arquivo.  Pego o Registro já preenchido para acrescento os pontos novos
                var registroJáExistente = Registros.Find(r => r.Tipo == Registro.Tipo);
                if (registroJáExistente != null)
                {
                    Registro = registroJáExistente;
                    Registros.Remove(Registro);
                }
            }
            else if (linha.Contains("<d2p1:KeyValueOfProfundidadePontoDeRegistroDePerfuração"))
            {
                LendoPonto = true;
                Ponto = new PontoRegistroDTO();
            }
            else if (linha.Contains("</d2p1:KeyValueOfProfundidadePontoDeRegistroDePerfuração"))
            {
                LendoPonto = false;
                Registro.Pontos.Add(Ponto);
            }
            else if (linha.Contains("<d5p1:TrechoDeRegistroDePerfuração "))
            {
                LendoTrecho = true;
                Ponto = new PontoRegistroDTO();
            }
            else if (linha.Contains("</d5p1:TrechoDeRegistroDePerfuração>"))
            {
                LendoTrecho = false;
                Registro.Pontos.Add(Ponto);
            }

            if (LendoTrecho)
            {
                if (linha.Contains("<Base "))
                    Ponto.PmBase = LeitorHelperTR2.ObterValorTag(linha);
                else if (linha.Contains("<Topo "))
                    Ponto.PmTopo = LeitorHelperTR2.ObterValorTag(linha);
                else if (linha.Contains("<Valor "))
                    Ponto.Valor = LeitorHelperTR2.ObterValorTag(linha);
                else if (linha.Contains("<d5p1:Descrição "))
                    Ponto.Comentário = LeitorHelperTR2.ObterValorTag(linha);
            }

            if (LendoPonto)
            {
                if (LendoTrecho)
                {
                    if (linha.Contains("<Base "))
                    {
                        Ponto.PmBase = LeitorHelperTR2.ObterValorTag(linha);
                    }
                    else if (linha.Contains("<Topo "))
                    {
                        Ponto.PmTopo = LeitorHelperTR2.ObterValorTag(linha);
                    }
                    else if (linha.Contains("<Valor "))
                    {
                        Ponto.Valor = LeitorHelperTR2.ObterValorTag(linha);
                    }
                }

                if (LendoProfundidade == false)
                {
                    if (linha.Contains("<d8p1:_valor>"))
                    {
                        Ponto.Pm = LeitorHelperTR2.ObterValorTag(linha);
                        LendoProfundidade = true;
                    }
                }
                else
                {
                    if (linha.Contains("<d5p1:Descrição"))
                    {
                        Ponto.Comentário = LeitorHelperTR2.ObterValorTag(linha);
                    }
                    if (linha.Contains("<d5p1:Valor>"))
                    {
                        Ponto.Valor = LeitorHelperTR2.ObterValorTag(linha);
                        LendoProfundidade = false;
                    }
                }
            }

        }
    }
}
