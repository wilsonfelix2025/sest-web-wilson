using System.Collections.Generic;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public class TiposPadrão
    {
        public static List<RegistroEvento> GetLista(string idPoço)
        {
            List<RegistroEvento> lista = new List<RegistroEvento>();

            #region Registros
            lista.Add(criarRegistroEvento(idPoço,
                    PPORO._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#0000ff", "#0000ff"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    ANGAT._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    PERM._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    KS._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    UCS._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    RESTR._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    COESA._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    YOUNG._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    BIOT._descrição, TipoRegistroEvento.Registro,
                    "Circulo", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "FIT", TipoRegistroEvento.Registro,
                    "Dois-circulos", null, "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "LOT", TipoRegistroEvento.Registro,
                    "Triangulo-circulo", null, "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "XLOT", TipoRegistroEvento.Registro,
                    "Circulo", null, "#0000ff"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Minifrac", TipoRegistroEvento.Registro,
                    "Circulo", null, "#00ff00"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Microfrac", TipoRegistroEvento.Registro,
                    "Circulo", null, "#ff8800"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Micro TI", TipoRegistroEvento.Registro,
                    "Quadrado", null, "#ff8800"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Step rate test", TipoRegistroEvento.Registro,
                    "Quadrado", null, "#ff0000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Pump in/ flowback", TipoRegistroEvento.Registro,
                    "Quadrado", null, "#00ff00"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Perfilagem", TipoRegistroEvento.Registro,
                    "Circulo", null, "#ff0000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Cascalho angular", TipoRegistroEvento.Registro,
                    "Triangulo-invertido", "#ffff00", "#ffff00"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Cascalho lascado", TipoRegistroEvento.Registro,
                    "Triangulo-invertido", "#ff0000", "#ff0000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Cascalho tabular", TipoRegistroEvento.Registro,
                    "Triangulo-invertido", "#000000", "#000000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Cascalho arredondado", TipoRegistroEvento.Registro,
                    "Triangulo-invertido", "#0000ff", "#0000ff"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Cascalho outros", TipoRegistroEvento.Registro,
                    "Triangulo-invertido", "#ff00ff", "#ff00ff"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Testemunhagem", TipoRegistroEvento.Registro,
                    "Cruz", null, "#ff8800"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Falha operacional", TipoRegistroEvento.Registro,
                    "Circulo", "#ff0000", "#ff0000"
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Pescaria", TipoRegistroEvento.Registro,
                    "Pescaria", null, "#000000"
                ));

            #endregion

            #region Eventos
            lista.Add(criarRegistroEvento(idPoço,
                    "Prisão", TipoRegistroEvento.Evento,
                    "Circulo", "#000000", "#000000", 7
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Ameaça de prisão", TipoRegistroEvento.Evento,
                    "Circulo", "#ffff00", "#ffff00", 6
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Topada", TipoRegistroEvento.Evento,
                    "Circulo", "#ff00ff", "#ff00ff", 8
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Perda", TipoRegistroEvento.Evento,
                    "Circulo", "#0000ff", "#0000ff", 1
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Ganho", TipoRegistroEvento.Evento,
                    "Sol-rosa", null, "#ff00ff", 1.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Repasse", TipoRegistroEvento.Evento,
                    "Circulo", "#ff8800", "#ff8800", 4
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Obstrução do anular", TipoRegistroEvento.Evento,
                    "Quadrado", "#888888", "#888888", 6.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Pontos trabalhados", TipoRegistroEvento.Evento,
                    "Círculo-dividido", null, "#00ff00", 3.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Breakout", TipoRegistroEvento.Evento,
                    "Circulo", "#884400", "#884400", 4.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Fraturas induzidas", TipoRegistroEvento.Evento,
                    "Diamante", "#ff8800", "#ff8800", 5.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Arraste", TipoRegistroEvento.Evento,
                    "Circulo", "#00ff00", "#00ff00", 5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Gás de formação", TipoRegistroEvento.Evento,
                    "Sol-laranja", null, "#ff8800", 2.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Torque", TipoRegistroEvento.Evento,
                    "Ponto-circulo", null, "#000000", 7.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Fraturas Naturais", TipoRegistroEvento.Evento,
                    "Diamante", "#008800", "#008800", 8.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Vibração", TipoRegistroEvento.Evento,
                    "Zig-Zag", null, "#ff0000", 10.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Enceramento de broca", TipoRegistroEvento.Evento,
                    "Circulo", "#000000", "#000000", 11
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Kick", TipoRegistroEvento.Evento,
                    "Circulo", "#ff0000", "#ff0000", 9
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Falso kick", TipoRegistroEvento.Evento,
                    "Circulo", "#8800ff", "#8800ff", 9.5
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Estolamento", TipoRegistroEvento.Evento,
                    "Circulo", "#0088ff", "#0088ff", 3
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Fundo falso", TipoRegistroEvento.Evento,
                    "Circulo", "#008800", "#008800", 10
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Gás de conexão", TipoRegistroEvento.Evento,
                    "Quadrado", "#ffff00", "#ffff00", 2
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Gás de manobra", TipoRegistroEvento.Evento,
                    "Quadrado", "#ff0000", "#ff0000", 2
                ));
            lista.Add(criarRegistroEvento(idPoço,
                    "Empacotamento", TipoRegistroEvento.Evento,
                    "Circulo", "#2F4F2F", "#2F4F2F", 2
                ));
            #endregion

            return lista;
        }
        private static RegistroEvento criarRegistroEvento(string idPoço, string nome, TipoRegistroEvento tipo, string marcador, string corDoMarcador, string contornoDoMarcador, double valorPadrão)
        {
            RegistroEvento r = new RegistroEvento(nome, tipo, valorPadrão);
            r.IdPoço = idPoço;
            r.SetEstiloVisual(marcador, corDoMarcador, contornoDoMarcador);

            return r;
        }
        private static RegistroEvento criarRegistroEvento(string idPoço, string nome, TipoRegistroEvento tipo, string marcador, string corDoMarcador, string contornoDoMarcador)
        {
            RegistroEvento r = new RegistroEvento(nome, tipo);
            r.IdPoço = idPoço;
            r.SetEstiloVisual(marcador, corDoMarcador, contornoDoMarcador);

            return r;
        }

    }
}