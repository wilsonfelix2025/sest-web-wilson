using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorPerfisPoçoWeb
    {
        public List<PerfilDTO> ListaPerfil { get; private set; }
        private readonly PoçoWebDto _poçoWeb;
        private bool _buscarDadosShallow;
        private Poço _poçoAtual;
        private List<PerfilParaImportarDTO> _perfisSelecionados;

        public LeitorPerfisPoçoWeb(PoçoWebDto poçoWeb, bool buscarDadosShallow, Poço PoçoAtual, List<PerfilParaImportarDTO> perfisSelecionados)
        {
            _poçoWeb = poçoWeb;
            _buscarDadosShallow = buscarDadosShallow;
            _poçoAtual = PoçoAtual;
            _perfisSelecionados = perfisSelecionados;
            ListaPerfil = new List<PerfilDTO>();
            GetPerfilGeoPressão();
        }

        private void GetPerfilGeoPressão()
        {
            if (_poçoWeb.Revision.Content.Input.GeoTables == null)
                return;

            var éCota = false;

            if (_poçoWeb.Revision.Content.Input.GeoTables.FracturePressure != null)
            {
                
                foreach (var perfilPoço in _poçoWeb.Revision.Content.Input.GeoTables.FracturePressure)
                {
                    bool importaPerfil = false;
                    var perfil = new PerfilDTO();
                    perfil.Nome = perfilPoço.Value.Name;
                    perfil.Mnemonico = "GQUEBRA";

                    if (_perfisSelecionados != null && _perfisSelecionados.Any())
                    {
                        foreach (var pi in _perfisSelecionados)
                        {
                            if (pi.Nome == perfil.Nome)
                            {
                                importaPerfil = true;
                                break;
                            }
                        }
                    }

                    if (!importaPerfil && _perfisSelecionados != null && _perfisSelecionados.Any())
                        break;

                    foreach (var pontos in perfilPoço.Value.Values)
                    {
                        var ponto = new PontoDTO();
                        var profundidade = pontos.MeasuredDepth;

                        if(éCota == true || profundidade == null)
                        {
                            éCota = true;
                            profundidade = (pontos.Elevation.Value * -1).ToString();
                            perfil.TipoProfundidade = "Cota";
                        }


                        ponto.Origem = OrigemPonto.Importado;

                        ponto.Valor = pontos.Pressure;
                        ponto.Pm = profundidade;

                        perfil.PontosDTO.Add(ponto);

                        if (_buscarDadosShallow)
                            break;
                    }

                    ListaPerfil.Add(perfil);
                    éCota = false;

                }
            }

            if (_poçoWeb.Revision.Content.Input.GeoTables.OverbalancePressure != null)
            {
                foreach (var perfilPoço in _poçoWeb.Revision.Content.Input.GeoTables.OverbalancePressure)
                {
                    var importaPerfil = false;
                    var perfil = new PerfilDTO();
                    perfil.Nome = perfilPoço.Value.Name;
                    perfil.Mnemonico = "GSOBR";

                    if (_perfisSelecionados != null && _perfisSelecionados.Any())
                    {
                        foreach (var pi in _perfisSelecionados)
                        {
                            if (pi.Nome == perfil.Nome)
                            {
                                importaPerfil = true;
                                break;
                            }
                        }
                    }

                    if (!importaPerfil && _perfisSelecionados != null && _perfisSelecionados.Any())
                        break;

                    foreach (var pontos in perfilPoço.Value.Values)
                    {
                        var ponto = new PontoDTO();
                        ponto.Origem = OrigemPonto.Importado;
                        ponto.Valor = pontos.Pressure;

                        var profundidade = pontos.MeasuredDepth;

                        if (éCota == true || profundidade == null)
                        {
                            éCota = true;
                            profundidade = (pontos.Elevation.Value * -1).ToString();
                            perfil.TipoProfundidade = "Cota";
                        }

                        ponto.Pm = profundidade;

                        perfil.PontosDTO.Add(ponto);

                        if (_buscarDadosShallow)
                            break;
                    }

                    ListaPerfil.Add(perfil);
                    éCota = false;
                }
            }

            if (_poçoWeb.Revision.Content.Input.GeoTables.SuperiorCollapse != null)
            {
                foreach (var perfilPoço in _poçoWeb.Revision.Content.Input.GeoTables.SuperiorCollapse)
                {
                    var perfil = new PerfilDTO();
                    var importaPerfil = false;
                    perfil.Nome = perfilPoço.Value.Name;
                    perfil.Mnemonico = "GCOLS";

                    if (_perfisSelecionados != null && _perfisSelecionados.Any())
                    {
                        foreach (var pi in _perfisSelecionados)
                        {
                            if (pi.Nome == perfil.Nome)
                            {
                                importaPerfil = true;
                                break;
                            }
                        }
                    }

                    if (!importaPerfil && _perfisSelecionados != null && _perfisSelecionados.Any())
                        break;

                    foreach (var pontos in perfilPoço.Value.Values)
                    {
                        var ponto = new PontoDTO();
                        ponto.Origem = OrigemPonto.Importado;
                        ponto.Valor = pontos.Pressure;
                        var profundidade = pontos.MeasuredDepth;

                        if (éCota == true || profundidade == null)
                        {
                            éCota = true;
                            profundidade = (pontos.Elevation.Value * -1).ToString();
                            perfil.TipoProfundidade = "Cota";
                        }

                        ponto.Pm = profundidade;

                        perfil.PontosDTO.Add(ponto);

                        if (_buscarDadosShallow)
                            break;
                    }

                    ListaPerfil.Add(perfil);
                    éCota = false;
                }
            }

            if (_poçoWeb.Revision.Content.Input.GeoTables.InferiorCollapse != null)
            {
                foreach (var perfilPoço in _poçoWeb.Revision.Content.Input.GeoTables.InferiorCollapse)
                {
                    var perfil = new PerfilDTO();
                    var importaPerfil = false;
                    perfil.Nome = perfilPoço.Value.Name;
                    perfil.Mnemonico = "GCOLI";

                    if (_perfisSelecionados != null && _perfisSelecionados.Any())
                    {
                        foreach (var pi in _perfisSelecionados)
                        {
                            if (pi.Nome == perfil.Nome)
                            {
                                importaPerfil = true;
                                break;
                            }
                        }
                    }

                    if (!importaPerfil && _perfisSelecionados != null && _perfisSelecionados.Any())
                        break;

                    foreach (var pontos in perfilPoço.Value.Values)
                    {
                        var ponto = new PontoDTO();
                        ponto.Origem = OrigemPonto.Importado;
                        ponto.Valor = pontos.Pressure;
                        var profundidade = pontos.MeasuredDepth;

                        if (éCota == true || profundidade == null)
                        {
                            éCota = true;
                            profundidade = (pontos.Elevation.Value * -1).ToString();
                            perfil.TipoProfundidade = "Cota";
                        }

                        ponto.Pm = profundidade;

                        perfil.PontosDTO.Add(ponto);

                        if (_buscarDadosShallow)
                            break;
                    }

                    ListaPerfil.Add(perfil);
                    éCota = false;
                }
            }

            if (_poçoWeb.Revision.Content.Input.GeoTables.UCS != null)
            {
                foreach (var perfilPoço in _poçoWeb.Revision.Content.Input.GeoTables.UCS)
                {
                    var perfil = new PerfilDTO();
                    var importaPerfil = false;
                    perfil.Nome = perfilPoço.Value.Name;
                    perfil.Mnemonico = "UCS";

                    if (_perfisSelecionados != null && _perfisSelecionados.Any())
                    {
                        foreach (var pi in _perfisSelecionados)
                        {
                            if (pi.Nome == perfil.Nome)
                            {
                                importaPerfil = true;
                                break;
                            }
                        }
                    }

                    if (!importaPerfil && _perfisSelecionados != null && _perfisSelecionados.Any())
                        break;

                    foreach (var pontos in perfilPoço.Value.Values)
                    {
                        var ponto = new PontoDTO();
                        ponto.Origem = OrigemPonto.Importado;
                        ponto.Valor = pontos.Pressure;
                        var profundidade = pontos.MeasuredDepth;

                        if (éCota == true || profundidade == null)
                        {
                            éCota = true;
                            profundidade = (pontos.Elevation.Value * -1).ToString();
                            perfil.TipoProfundidade = "Cota";
                        }

                        ponto.Pm = profundidade;

                        perfil.PontosDTO.Add(ponto);

                        if (_buscarDadosShallow)
                            break;
                    }

                    ListaPerfil.Add(perfil);
                    éCota = false;
                }
            }

            if (_poçoWeb.Revision.Content.Input.GeoTables.PorePressure != null)
            {
                foreach (var perfilPoço in _poçoWeb.Revision.Content.Input.GeoTables.PorePressure)
                {
                    var perfil = new PerfilDTO();
                    var importaPerfil = false;
                    perfil.Nome = perfilPoço.Value.Name;
                    perfil.Mnemonico = "GPORO";

                    if (_perfisSelecionados != null && _perfisSelecionados.Any())
                    {
                        foreach (var pi in _perfisSelecionados)
                        {
                            if (pi.Nome == perfil.Nome)
                            {
                                importaPerfil = true;
                                break;
                            }
                        }
                    }

                    if (!importaPerfil && _perfisSelecionados != null && _perfisSelecionados.Any())
                        break;

                    foreach (var pontos in perfilPoço.Value.Values)
                    {
                        var ponto = new PontoDTO();
                        ponto.Origem = OrigemPonto.Importado;
                        ponto.Valor = pontos.Pressure;
                        var profundidade = pontos.MeasuredDepth;

                        if (éCota == true || profundidade == null)
                        {
                            éCota = true;
                            profundidade = (pontos.Elevation.Value * -1).ToString();
                            perfil.TipoProfundidade = "Cota";
                        }

                        ponto.Pm = profundidade;

                        perfil.PontosDTO.Add(ponto);

                        if (_buscarDadosShallow)
                            break;
                    }

                    ListaPerfil.Add(perfil);
                    éCota = false;
                }
            }
        }

        private string ObterPM(string ponto)
        {
            if (_buscarDadosShallow)
                return ponto;

            var valorPm = StringUtils.ToDoubleInvariantCulture(ponto, 2);

            if (valorPm < 0)
                valorPm = valorPm * -1;

            var pontoTrajetória = _poçoAtual.Trajetória.GetPointsByPv(new Profundidade(valorPm))[0];
            valorPm = pontoTrajetória.Pm.Valor;

            return valorPm.ToString();
        }

    }
}
