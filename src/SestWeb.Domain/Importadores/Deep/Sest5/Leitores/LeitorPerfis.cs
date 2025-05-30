using System.Collections.Generic;
using System.Xml.Linq;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public class LeitorPerfis
    {
        private readonly XDocument xDoc;
        private readonly IConversorProfundidade _conversorProfundidade;
        private readonly ILitologia _litologia;

        public LeitorPerfis(XDocument xDoc, IConversorProfundidade conversorProfundidade, ILitologia litologia)
        {
            this.xDoc = xDoc;
            _conversorProfundidade = conversorProfundidade;
            _litologia = litologia;
        }

        public List<PerfilBase> ObterTodosPerfis()
        {
            var perfis = ObterPerfis();
            perfis.AddRange(ObterPropMec());
            perfis.AddRange(ObterTensões());
            perfis.AddRange(ObterGradientes());

            return perfis;
        }

        private List<PerfilBase> ObterPerfis()
        {
            List<PerfilBase> perfisObtidos = new List<PerfilBase>();
            DTC dtc = PerfisFactory<DTC>.Create(nameof(DTC), _conversorProfundidade, _litologia);
            DTS dts = PerfisFactory<DTS>.Create(nameof(DTS), _conversorProfundidade, _litologia);
            DTMC dtmc = PerfisFactory<DTMC>.Create(nameof(DTMC), _conversorProfundidade, _litologia);
            DTMS dtms = PerfisFactory<DTMS>.Create(nameof(DTMS), _conversorProfundidade, _litologia);
            RHOB rhob = PerfisFactory<RHOB>.Create(nameof(RHOB), _conversorProfundidade, _litologia);
            RHOG rhog = PerfisFactory<RHOG>.Create(nameof(RHOG), _conversorProfundidade, _litologia);
            VCL vcl = PerfisFactory<VCL>.Create(nameof(VCL), _conversorProfundidade, _litologia);
            GRAY gray = PerfisFactory<GRAY>.Create(nameof(GRAY), _conversorProfundidade, _litologia);
            RESIST resist = PerfisFactory<RESIST>.Create(nameof(RESIST), _conversorProfundidade, _litologia);
            CALIP calip = PerfisFactory<CALIP>.Create(nameof(CALIP), _conversorProfundidade, _litologia);
            PORO poro = PerfisFactory<PORO>.Create(nameof(PORO), _conversorProfundidade, _litologia);

            var xElements = xDoc.Root.Elements("Perfis").Elements("Perfil");

            // O(n)
            foreach (var n in xElements)
            {
                var PM = (double) n.Attribute("PM");
                var valueDTC = (double) n.Attribute("DTC");
                var valueDTS = (double) n.Attribute("DTS");
                var valueDTMC = (double)n.Attribute("DTMC");
                var valueDTMS = (double)n.Attribute("DTMS");
                var valueDENF = (double)n.Attribute("DENF");
                var valueDENG = (double)n.Attribute("DENG");
                var valueVCL = (double) n.Attribute("VCL");
                var valueGRAY = (double) n.Attribute("GRAY");
                var valueREST = (double) n.Attribute("REST");
                var valueCALIP = (double) n.Attribute("CALIP");
                var valuePORO = (double) n.Attribute("PORO");

                if(valueDTC >= 0)
                    dtc.AddPontoEmPm(_conversorProfundidade, PM, valueDTC, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueDTS >= 0)
                    dts.AddPontoEmPm(_conversorProfundidade, PM,  valueDTS, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueDTMC >= 0)
                    dtmc.AddPontoEmPm(_conversorProfundidade, PM,  valueDTMC, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueDTMS >= 0)
                    dtms.AddPontoEmPm(_conversorProfundidade, PM,  valueDTMS, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueDENF >= 0)
                    rhob.AddPontoEmPm(_conversorProfundidade, PM,  valueDENF, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueDENG >= 0)
                    rhog.AddPontoEmPm(_conversorProfundidade, PM,  valueDENG, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueVCL >= 0)
                    vcl.AddPontoEmPm(_conversorProfundidade, PM,  valueVCL, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueGRAY >= 0)
                    gray.AddPontoEmPm(_conversorProfundidade, PM,  valueGRAY, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueREST >= 0)
                    resist.AddPontoEmPm(_conversorProfundidade, PM,  valueREST, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueCALIP >= 0)
                    calip.AddPontoEmPm(_conversorProfundidade, PM,  valueCALIP, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valuePORO >= 0)
                    poro.AddPontoEmPm(_conversorProfundidade, PM,  valuePORO, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            if(dtc.ContémPontos())
                perfisObtidos.Add(dtc);

            if (dts.ContémPontos())
                perfisObtidos.Add(dts);

            if (dtmc.ContémPontos())
                perfisObtidos.Add(dtmc);

            if (dtms.ContémPontos())
                perfisObtidos.Add(dtms);

            if (rhob.ContémPontos())
                perfisObtidos.Add(rhob);

            if (rhog.ContémPontos())
                perfisObtidos.Add(rhog);

            if (vcl.ContémPontos())
                perfisObtidos.Add(vcl);

            if (gray.ContémPontos())
                perfisObtidos.Add(gray);

            if (resist.ContémPontos())
                perfisObtidos.Add(resist);

            if (calip.ContémPontos())
                perfisObtidos.Add(calip);

            if (poro.ContémPontos())
                perfisObtidos.Add(poro);


            return perfisObtidos;
        }

        
        public List<PerfilBase> ObterPropMec()
        {
            List<PerfilBase> perfisObtidos = new List<PerfilBase>();
            YOUNG young = PerfisFactory<YOUNG>.Create(nameof(YOUNG), _conversorProfundidade, _litologia);
            POISS poiss = PerfisFactory<POISS>.Create(nameof(POISS), _conversorProfundidade, _litologia);
            BIOT biot = PerfisFactory<BIOT>.Create(nameof(BIOT), _conversorProfundidade, _litologia);
            RESTR restr = PerfisFactory<RESTR>.Create(nameof(RESTR), _conversorProfundidade, _litologia);
            ANGAT angat = PerfisFactory<ANGAT>.Create(nameof(ANGAT), _conversorProfundidade, _litologia);
            COESA coesa = PerfisFactory<COESA>.Create(nameof(COESA), _conversorProfundidade, _litologia);
            PERM perm = PerfisFactory<PERM>.Create(nameof(PERM), _conversorProfundidade, _litologia);
            KS ks = PerfisFactory<KS>.Create(nameof(KS), _conversorProfundidade, _litologia);
            UCS ucs = PerfisFactory<UCS>.Create(nameof(UCS), _conversorProfundidade, _litologia);

            var xElements = xDoc.Root.Elements("PropriedadeMecanica").Elements("PropMec");

            // O(n)
            foreach (var n in xElements)
            {
                var PM = n.Attribute("PM").Value.ToDouble();
                var valueYOUNG = n.Attribute("YOUNG").Value.ToDouble();
                var valuePOISS = n.Attribute("POISS").Value.ToDouble();
                var valueBIOT = n.Attribute("BIOT").Value.ToDouble();
                var valueRESTR = n.Attribute("RESTR").Value.ToDouble();
                var valueANGAT = n.Attribute("ANGAT").Value.ToDouble();
                var valueCOESA = n.Attribute("COESA").Value.ToDouble();
                var valuePERM = n.Attribute("PERM").Value.ToDouble();
                var valueKS = n.Attribute("KS").Value.ToDouble();
                var valueRESCMP = n.Attribute("RESCMP").Value.ToDouble();

                if (valueYOUNG >= 0)
                    young.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valueYOUNG, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valuePOISS >= 0)
                    poiss.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valuePOISS, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueBIOT >= 0)
                    biot.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valueBIOT, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueRESTR >= 0)
                    restr.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valueRESTR, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueANGAT >= 0)
                    angat.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valueANGAT, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueCOESA >= 0)
                    coesa.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valueCOESA, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valuePERM >= 0)
                    perm.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valuePERM, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueKS >= 0)
                    ks.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valueKS, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueRESCMP >= 0)
                    ucs.AddPontoEmPm(_conversorProfundidade, new Profundidade(PM),  valueRESCMP, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            if (young.ContémPontos())
                perfisObtidos.Add(young);

            if (poiss.ContémPontos())
                perfisObtidos.Add(poiss);

            if (biot.ContémPontos())
                perfisObtidos.Add(biot);

            if (restr.ContémPontos())
                perfisObtidos.Add(restr);

            if (angat.ContémPontos())
                perfisObtidos.Add(angat);

            if (ks.ContémPontos())
                perfisObtidos.Add(ks);

            if (coesa.ContémPontos())
                perfisObtidos.Add(coesa);

            if (perm.ContémPontos())
                perfisObtidos.Add(perm);

            if (ucs.ContémPontos())
                perfisObtidos.Add(ucs);

            return perfisObtidos;
        }

        public List<PerfilBase> ObterTensões()
        {
            List<PerfilBase> perfisObtidos = new List<PerfilBase>();
            THORmin thormin = PerfisFactory<THORmin>.Create(nameof(THORmin), _conversorProfundidade, _litologia);
            THORmax thormax = PerfisFactory<THORmax>.Create(nameof(THORmax), _conversorProfundidade, _litologia);
            AZTHmin azthmin = PerfisFactory<AZTHmin>.Create(nameof(AZTHmin), _conversorProfundidade, _litologia);

            var xElements = xDoc.Root.Elements("TensoesInSitu").Elements("TensInSitu");

            // O(n)
            foreach (var n in xElements)
            {
                var PM = n.Attribute("PM").Value.ToDouble();
                var valueThormin = n.Attribute("THORm").Value.ToDouble();
                var valueThormax = n.Attribute("THORM").Value.ToDouble();
                var valueAzthmin = n.Attribute("AZTHM").Value.ToDouble();

                if (valueThormin >= 0)
                    thormin.AddPontoEmPm(_conversorProfundidade, PM, valueThormin, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueThormax >= 0)
                    thormax.AddPontoEmPm(_conversorProfundidade, PM, valueThormax, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueAzthmin >= 0)
                    azthmin.AddPontoEmPm(_conversorProfundidade, PM, valueAzthmin, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            if (thormin.ContémPontos())
                perfisObtidos.Add(thormin);

            if (thormax.ContémPontos())
                perfisObtidos.Add(thormax);

            if (azthmin.ContémPontos())
                perfisObtidos.Add(azthmin);

            return perfisObtidos;
        }
        
        
        public List<PerfilBase> ObterGradientes()
        {
            List<PerfilBase> perfisObtidos = new List<PerfilBase>();
            GSOBR gsobr = PerfisFactory<GSOBR>.Create(nameof(GSOBR), _conversorProfundidade, _litologia);
            GPORO gporo = PerfisFactory<GPORO>.Create(nameof(GPORO), _conversorProfundidade, _litologia);
            GFRAT gfrat = PerfisFactory<GFRAT>.Create(nameof(GFRAT), _conversorProfundidade, _litologia);
            GCOLS gcols = PerfisFactory<GCOLS>.Create(nameof(GCOLS), _conversorProfundidade, _litologia);
            GCOLI gcoli = PerfisFactory<GCOLI>.Create(nameof(GCOLI), _conversorProfundidade, _litologia);
            GLAMA glama = PerfisFactory<GLAMA>.Create(nameof(GLAMA), _conversorProfundidade, _litologia);
            GECD gecd = PerfisFactory<GECD>.Create(nameof(GECD), _conversorProfundidade, _litologia);

            var xElements = xDoc.Root.Elements("Gradientes").Elements("Grad");

            // O(n)
            foreach (var n in xElements)
            {
                var PM = n.Attribute("PM").Value.ToDouble();
                var valueGSOBR = n.Attribute("GSOBR").Value.ToDouble();
                var valueGPORO = n.Attribute("GPORO").Value.ToDouble();
                var valueGFRAT = n.Attribute("GFRAT").Value.ToDouble();
                var valueGCOLS = n.Attribute("GCOLS").Value.ToDouble();
                var valueGCOLI = n.Attribute("GCOLI").Value.ToDouble();
                var valueGLAMA = n.Attribute("GLAMA").Value.ToDouble();
                var valueGECD = n.Attribute("GECD").Value.ToDouble();

                if (valueGSOBR >= 0)
                    gsobr.AddPontoEmPm(_conversorProfundidade, PM, valueGSOBR, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueGPORO >= 0)
                    gporo.AddPontoEmPm(_conversorProfundidade, PM, valueGPORO, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueGFRAT >= 0)
                    gfrat.AddPontoEmPm(_conversorProfundidade, PM, valueGFRAT, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueGCOLS >= 0)
                    gcols.AddPontoEmPm(_conversorProfundidade, PM, valueGCOLS, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueGCOLI >= 0)
                    gcoli.AddPontoEmPm(_conversorProfundidade, PM, valueGCOLI, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueGLAMA >= 0)
                    glama.AddPontoEmPm(_conversorProfundidade, PM, valueGLAMA, TipoProfundidade.PM, OrigemPonto.Importado);

                if (valueGECD >= 0)
                    gecd.AddPontoEmPm(_conversorProfundidade, PM, valueGECD, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            if (gsobr.ContémPontos())
                perfisObtidos.Add(gsobr);

            if (gporo.ContémPontos())
                perfisObtidos.Add(gporo);

            if (gfrat.ContémPontos())
                perfisObtidos.Add(gfrat);

            if (gcols.ContémPontos())
                perfisObtidos.Add(gcols);

            if (gcoli.ContémPontos())
                perfisObtidos.Add(gcoli);

            if (glama.ContémPontos())
                perfisObtidos.Add(glama);

            if (gecd.ContémPontos())
                perfisObtidos.Add(gecd);

            return perfisObtidos;
        }
    }
}