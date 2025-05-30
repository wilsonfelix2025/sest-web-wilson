using System;
using System.Collections.Generic;
using System.Globalization;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;

namespace SestWeb.Domain.Entities.Correlações.LoaderCorrelaçõesSistema
{
    public class LoaderCorrelações : ILoaderCorrelações
    {
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private const string NomeAutor = "Puc-Rio";
        private const string ChaveAutor = "GTEP";
        private static readonly string DataCriação = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        private static readonly string Origem = OrigemCorrelação.Origem.Fixa.ToString();

        public LoaderCorrelações(ICorrelaçãoFactory correlaçãoFactory)
        {
            _correlaçãoFactory = correlaçãoFactory;
        }

        public List<ICorrelação> Load()
        {
            return new List<ICorrelação>
            {
                Load_DTMS_MECPRO(),
                Load_VCL_MECPRO(),
                Load_DTS_MECPRO(),
                Load_PERM_MECPRO(),
                Load_RHOB_GARDNER(),
                Load_RHOB_AGIP(),
                Load_PORO_MECPRO(),
                Load_PORO_MECPRO_1(),
                Load_POISS_MECPRO(),
                Load_YOUNG_MECPRO(),
                Load_KS_MECPRO(),
                Load_BIOT_REGIME_ELÁSTICO(),
                Load_BIOT_REGIME_POROELÁSTICO(),
                Load_ANGAT_30(),
                Load_ANGAT_LAL(),
                Load_ANGAT_PLUMB(),
                Load_ANGAT_CALCULADO(),
                Load_ANGAT_22_1(),
                Load_ANGAT_50(),
                Load_ANGAT_SANTOS_ET_AL(),
                Load_COESA_MECPRO(),
                Load_COESA_LAL(),
                Load_COESA_CALCULADO(),
                Load_UCS_CALCULADO(),
                Load_UCS_MECPRO(),
                Load_UCS_BREHM(),
                Load_UCS_MILITZER_STOLL(),
                Load_UCS_CHANG(),
                Load_UCS_CHANG1(),
                Load_UCS_CHANG2(),
                Load_UCS_GOLUBEV_RABINOVICH(),
                Load_UCS_LAL(),
                Load_UCS_CPM(),
                Load_UCS_MILITZER_STOLL_ADAP(),
                Load_UCS_PRASAD_ET_AL_1(),
                Load_BIOT_PRASAD_ET_AL_1(),
                Load_UCS_PRASAD_ET_AL_2(),
                Load_BIOT_PRASAD_ET_AL_2(),
                Load_UCS_REIS(),
                Load_UCS_REIS1(),
                Load_UCS_REIS2(),
                Load_UCS_HORSUD(),
                Load_UCS_CHANG_ET_AL(),
                Load_UCS_TEIKOKU(),
                Load_UCS_TEIKOKU_1(),
                Load_RESTR_MECPRO(),
                Load_DTC_VP()
            };
        }

        private ICorrelação Load_DTMS_MECPRO()
        {
            var nome = "DTMS_MECPRO";

            var expressão = @"
 (RHOG < 2.7) ? 
     DTMS = ((DTMC - 104.93) / (1.667 * RHOG - 3.968)) + 194.1 : DTMS = ((DTMC - 104.93) / (0.889 - 0.125 * RHOG)) + 194.1";

            var descrição = "SCHLUMBERGER MECPRO, 1985: Correlação não editável MECPRO para o cálculo de DTMS.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_VCL_MECPRO()
        {
            var nome = "VCL_MECPRO";

            var expressão = @" const vclmax = 0.4, const GRAYmax = 100, const vclmin = 0.1,
 (GRUPO_LITOLOGICO == Argilosas) ? 
    VCL = vclmax * GRAY / GRAYmax : VCL = vclmin ,
 (VCL < vclmin) ?
     VCL = vclmin : 0 ,
 (VCL > vclmax) ?
     VCL = vclmax : 0,
 (GRUPO_LITOLOGICO == Evaporitos) ?
     VCL = 0.0 : 0";

            var descrição = "Correlação não editável MECPRO para o cálculo de VCL.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_DTS_MECPRO()
        {
            var nome = "DTS_MECPRO";

            var expressão = @" var x = 1, const RVCL = 1.72,
 (RHOG < 2.7) ?
     x = ((DTC - 104.93) / (1.667 * RHOG - 3.968)) + 194.1 : x = ((DTC - 104.93) / (0.87 - 0.125 * RHOG)) + 194.1 ,
 (DTS == 0) ?
     DTS = DTC * (x * (1 - VCL) / DTC + RVCL * VCL),
     (DTS > 500.0) ? DTS = 500.0 : 0,
     (DTS < 40.0) ? DTS = 40.0 : 0 : 0";

            var descrição = "SCHLUMBERGER MECPRO, 1985: Correlação não editável MECPRO para o cálculo de DTS.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_PERM_MECPRO()
        {
            var nome = "PERM_MECPRO";

            var expressão = @" (GRUPO_LITOLOGICO == Argilosas)  ?
 PERM = 8.57 * ((1 / 10) ^ 8) * exp(15.15 * PORO) :  PERM = 10.0 * 5.44 * PORO * PORO * ((1 - PORO) ^ 2)";

            var descrição =
                "Correlação não editável MECPRO para o cálculo da Permeabilidade. Utilizada para diversos grupos litológicos.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_RHOB_GARDNER()
        {
            var nome = "RHOB_GARDNER";

            var expressão = @" var a = 0.23, var b = 0.25,
 (RHOB == 0) ?
     RHOB = a * (((10 ^ 6) / DTC) ^ b),
 (RHOB < 1.8) ?
     RHOB = 1.8 : ((RHOB > 2.9) ? RHOB = 2.9 : 0),
 (GRUPO_LITOLOGICO == Evaporitos) ?
     RHOB = RHOG : 0 : 0";

            var descrição = "Correlação não editável para o cálculo da densidade da formação.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_RHOB_AGIP()
        {
            var nome = "RHOB_AGIP";

            var expressão = @" 
 (RHOB == 0) ?
     (DTC <= 100) ?
         RHOB = 3.28 - (DTC / 89.0) : RHOB = 2.75 - 2.11 * (DTC - DTMC)/(DTC + 200),
     (RHOB < 1.8) ?
        RHOB = 1.8 : ((RHOB > 2.9) ? RHOB = 2.9 : 0),
     (GRUPO_LITOLOGICO == Evaporitos) ?
         RHOB = RHOG : 0 : 0";

            var descrição = "Correlação não editável para o cálculo da densidade da formação.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_PORO_MECPRO()
        {
            var nome = "PORO_MECPRO";

            var expressão = @" var DTfl_MECPRO = 200,
 (RHOB > 0) ?
     PORO = (RHOG - RHOB) / (RHOG - DENSIDADE_AGUA_MAR) : PORO = (DTC - DTMC) / (DTfl_MECPRO - DTMC),
 (PORO < 0.05) ?
     PORO = 0.05 : ((PORO > 0.4) ? PORO = 0.4 : 0),
 (GRUPO_LITOLOGICO == Evaporitos) ?
     PORO = 0.0 : 0";

            var descrição = "Correlação não editável MECPRO para o Cálculo de Porosidade (gardner + willie et al.).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_PORO_MECPRO_1()
        {
            var nome = "PORO_MECPRO_1";

            var expressão = @" var DTfl_MECPRO_1 = 200,
 (RHOB > 0) ?
    PORO = (RHOG - RHOB) / (RHOG - DENSIDADE_AGUA_MAR) : PORO = 1.228 * (DTC - DTMC) / (DTC + DTfl_MECPRO_1),
 (PORO < 0.05) ?
     PORO = 0.05 : ((PORO > 0.4) ? PORO = 0.4 : 0),
 (GRUPO_LITOLOGICO == Evaporitos) ?
     PORO = 0.0 : 0";

            var descrição = "Correlação fixa MECPRO1 para o Cálculo de Porosidade (gardner + Bellotti & Giacca).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_POISS_MECPRO()
        {
            var nome = "POISS_MECPRO";

            var expressão = @" POISS = 0.5 * (((DTS/DTC)^2 - 2)/((DTS/DTC)^2 - 1))";

            var descrição = "Correlação não editável MECPRO para o cálculo de Poisson.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_YOUNG_MECPRO()
        {
            var nome = "YOUNG_MECPRO";

            var expressão = @" var G = 0,
   G = 1.34e10 * RHOB/DTS^2,
 YOUNG = 2 * G * (1 + POISS) ";

            var descrição = "Correlação não editável MECPRO para o cálculo do Módulo de Young.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_KS_MECPRO()
        {
            var nome = "KS_MECPRO";

            var expressão = @" KS = 1.34e10 * RHOB * (1/DTMC^2 - 4/(3 * DTMS^2)) ";

            var descrição = "Correlação não editável MECPRO para o cálculo do Módulo de compressibilidade dos grãos (Ks).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_BIOT_REGIME_ELÁSTICO()
        {
            var nome = "BIOT_REGIME_ELÁSTICO";

            var expressão = @" BIOT = 1 ";

            var descrição = "Correlação não editável para o cálculo do Coeficiente de Biot (Considerando regime elástico).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_BIOT_REGIME_POROELÁSTICO()
        {
            var nome = "BIOT_REGIME_POROELÁSTICO";

            var expressão = @"  var kb = 0,
   kb = 1.34e10 * RHOB * (1/(DTC^2) - 4/(3*DTS^2)),
 BIOT = 1 - kb/KS ";

            var descrição =
                "Correlação não editável para o cálculo do Coeficiente de Biot (Considerando regime Poroelástico).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_ANGAT_30()
        {
            var nome = "ANGAT_30";

            var expressão = @" ANGAT = 30 ";

            var descrição = "Correlação não editável para o Ângulo de atrito.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_ANGAT_LAL()
        {
            var nome = "ANGAT_LAL";

            var expressão = @" var Rdtc = 0, var Vp = 0,
   Rdtc = 0.00328 * DTC,
   Vp = 1/Rdtc,
 ANGAT = asin((Vp - 1)/(Vp + 1))";

            var descrição = "Lal, 1999: correlação desenvolvida para rochas da família folhelho.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_ANGAT_PLUMB()
        {
            var nome = "ANGAT_PLUMB";

            var expressão = @" var Vg = 0,
   Vg = 1 - (PORO + VCL),
 ANGAT = 35.727 * Vg^2 - 2.0294 * Vg + 15.422";

            var descrição =
                "Plumb, 1994: correlação válida para um grande universo de rochas sedimentares, variando de arenitos até folhelhos.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_ANGAT_CALCULADO()
        {
            var nome = "ANGAT_CALCULADO";

            var expressão = @" ANGAT = (atan(UCS/(2 * COESA))/_pi - 0.25) * 360";

            var descrição =
                "Correlação não editável para o cálculo do Ângulo de Atrito apartir da Coesão e da Resistência à Compressão.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_ANGAT_22_1()
        {
            var nome = "ANGAT_22_1";

            var expressão = @" ANGAT = 22.1";

            var descrição = "Correlação não editável para o Ângulo de Atrito.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_ANGAT_50()
        {
            var nome = "ANGAT_50";

            var expressão = @" ANGAT = 50";

            var descrição = "Correlação não editável para o Ângulo de Atrito.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_ANGAT_SANTOS_ET_AL()
        {
            var nome = "ANGAT_SANTOS_ET_AL";

            var expressão = @" ANGAT = 40.255 - 190.4 * PORO";

            var descrição = "Correlação não editável para o cálculo de Ângulo de Atrito. Carbonatos do Pré-Sal.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_COESA_MECPRO()
        {
            var nome = "COESA_MECPRO";

            var expressão = @" var cb = 0, var kb = 0,
   kb = 1.34e10 * RHOB * (1/(DTC^2) - 4/(3*DTS^2)),
   cb = 1/kb,
 COESA = 0.025 * ((0.0045 * YOUNG * (1 - VCL) + 0.008 * YOUNG * VCL) / (1.0e6 * cb))";

            var descrição = "Correlação não editável para o cálculo de Coesão.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_COESA_LAL()
        {
            var nome = "COESA_LAL";

            var expressão = @" var Rdtc = 0, var Vp = 0,
   Rdtc = 0.00328 * DTC,
   Vp = 1/Rdtc,
 COESA = 5 * (Vp - 1)/(sqrt(Vp)) * 145.0377";

            var descrição = "Lal (1999): Correlação desenvolvida para folhelhos da região do Mar do Norte.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_COESA_CALCULADO()
        {
            var nome = "COESA_CALCULADO";

            var expressão = @" COESA = UCS/(2 * tan(_pi * (0.25 + ANGAT/360))) ";

            var descrição =
                "Relação não editável para o cálculo da Coesão apartir do Ângulo de Atrito e da Resistência à Compressão.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_CALCULADO()
        {
            var nome = "UCS_CALCULADO";

            var expressão = @" UCS = 2 * COESA * tan(_pi/4 + (ANGAT/2 * _pi/180)) ";

            var descrição =
                "Relação não editável para o cálculo da Resistência à Compressão apartir do Ângulo de Atrito e da Coesão.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_MECPRO()
        {
            var nome = "UCS_MECPRO";

            var expressão =
                @" UCS = 145.037743897283 * (1.9 * 10^-20) * (RHOB*1000)^2 * (304800/DTC)^4 * ((1+POISS)/(1-POISS))^2 * (1-2*POISS) * (1+0.78*VCL) ";

            var descrição =
                "Correlação não editável para o cálculo da Resistência à Compressão. Arenitos e folhelhos com UCS maior do que 30 MPa.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_BREHM()
        {
            var nome = "UCS_BREHM";

            var expressão = @" UCS = (2.05*(10^9))/(DTC^3) ";

            var descrição =
                "BREHM, 2004: correlação calibrada para dados de resistência obtidos em ensaios experimentais em rochas fracas e não consolidadas (aplicável folhelhos e arenitos) do Golfo do México.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_MILITZER_STOLL()
        {
            var nome = "UCS_MILITZER_STOLL";

            var expressão = @" UCS = (7682/DTC)^1.82 ";

            var descrição = "MILITZER_&_STOLL, 1973: Correlação desenvolvida para carbonatos mais profundos em geral.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_CHANG()
        {
            var nome = "UCS_CHANG";

            var expressão = @" UCS = 0.5 * ((304.8/DTC)^3) * 145.0377 ";

            var descrição = "CHANG, 2004: correlação desenvolvida para folhelhos do Golfo do México (USA).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_CHANG1()
        {
            var nome = "UCS_CHANG1";

            var expressão = @" UCS = 1.35 * ((304.8/DTC)^2.6) * 145.0377 ";

            var descrição = "CHANG1, 2004: correlação desenvolvida para folhelhos de qualquer região do mundo.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_CHANG2()
        {
            var nome = "UCS_CHANG2";

            var expressão = @" UCS = (14138000/DTC^3) * 145.0377 ";

            var descrição =
                "CHANG2, 2004: correlação desenvolvida para arenitos fracos e não consolidados da Costa do Golfo(USA).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_GOLUBEV_RABINOVICH()
        {
            var nome = "UCS_GOLUBEV_RABINOVICH";

            var expressão = @" UCS = 10^(2.44 + (109.14/DTC)) ";

            var descrição = "GOLUBEV_&_RABINOVICH, 1976: correlação desenvolvida para calcários e dolomitos.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_LAL()
        {
            var nome = "UCS_LAL";

            var expressão = @" UCS = 10*((304.8/DTC) - 1) * 145.0377 ";

            var descrição =
                "LAL, 1999: correlação desenvolvida para folhelhos do Mar do Norte (Terciário) de alta porosidade.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_CPM()
        {
            var nome = "UCS_CPM";

            var expressão =
                @" UCS = (1.214*(10^-18)) * (RHOB^2/DTC^4) * ((1 + POISS)/(1 - POISS))^2 * (1 - 2*POISS) * (8.63097484 * 10^27) ";

            var descrição = "CPM (Santos e Ferreira, 2010): Carbonatos do pré-sal das bacias BS e ES.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_MILITZER_STOLL_ADAP()
        {
            var nome = "UCS_MILITZER_STOLL_ADAP";

            var expressão = @" UCS = (7682/DTC)^1.8  ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para carbonatos. Campos Tartaruga Verde e Tartaruaga Mestiça / Grupo Macaé.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_PRASAD_ET_AL_1()
        {
            var nome = "UCS_PRASAD_ET_AL_1";

            var expressão = @" UCS = 2871747/(DTC^1.5) ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para calcarenito mais frágil (Reservatório Carbonático de Jabuti no campo de Marlim Leste.)";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_BIOT_PRASAD_ET_AL_1()
        {
            var nome = "BIOT_PRASAD_ET_AL_1";

            var expressão = @" BIOT = 0.88";

            var descrição =
                "Coeficiente de Biot sugerido para ser utilizado em conjunto com a correlação de UCS_PRASAD";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_PRASAD_ET_AL_2()
        {
            var nome = "UCS_PRASAD_ET_AL_2";

            var expressão = @" UCS = 6091585/(DTC^1.5) ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para calcarenito mais resistente (Reservatório carbonático de Jabuti no campo de Marlim Leste.)";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_BIOT_PRASAD_ET_AL_2()
        {
            var nome = "BIOT_PRASAD_ET_AL_2";

            var expressão = @" BIOT = 0.84";

            var descrição =
                "Coeficiente de Biot sugerido para ser utilizado em conjunto com a correlação de UCS_PRASAD2";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_REIS()
        {
            var nome = "UCS_REIS";

            var expressão = @" UCS = 7978625/(DTC^(1.7876))";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para Arenitos Friáveis (Bacia Potiguar/Formação Açu).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_REIS1()
        {
            var nome = "UCS_REIS1";

            var expressão = @" UCS = 862.3 * (304.8/DTC)^0.8477  ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para Argilitos e folhelhos inconsolidados (Bacia Potiguar/Formação Açu).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_REIS2()
        {
            var nome = "UCS_REIS2";

            var expressão = @" UCS = 5771810 * exp(-0.1046*DTC)  ";

            var descrição =
                "Correlação desenvolvida para Arenitos e Conglomerados compactos (Bacia Potiguar/Formação Pendência).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_HORSUD()
        {
            var nome = "UCS_HORSUD";

            var expressão = @" UCS = 111.65 * (304.8/DTC)^2.93  ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para argilitos e folhelhos inconsolidados (Bacia Potiguar/Formação Alagamar).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_CHANG_ET_AL()
        {
            var nome = "UCS_CHANG_ET_AL";

            var expressão = @" UCS = 72.5 * (304.8/DTC)^3  ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para folhelhos consolidados (Bacia Potiguar/Formação Pendência).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_TEIKOKU()
        {
            var nome = "UCS_TEIKOKU";

            var expressão = @" UCS = 167185 * exp(-0.037*DTC)  ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para rochas Ígneas intactas em geral.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_UCS_TEIKOKU_1()
        {
            var nome = "UCS_TEIKOKU_1";

            var expressão = @" UCS = 28421.45 * exp(-0.037*DTC)  ";

            var descrição =
                "Correlação para estimativa da resistência à compressão simples para rochas Ígneas fragilizadas em geral).";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_RESTR_MECPRO()
        {
            var nome = "RESTR_MECPRO";

            var expressão = @" RESTR = UCS/12 ";

            var descrição = "Correlação não editável para o cálculo da resistência à tração.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }

        private ICorrelação Load_DTC_VP()
        {
            var nome = "DTC_VP";

            var expressão = @" DTC = 1/VP * 304800 ";

            var descrição = "Correlação não editável para o cálculo do DTC baseado na velocidade sísmica.";

            _correlaçãoFactory
                .CreateCorrelação(nome, NomeAutor, ChaveAutor, descrição, Origem, expressão, out ICorrelação correlação);
            return correlação;
        }
    }
}
