using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace SestWeb.Domain.Entities.LitologiaDoPoco
{
    public class TipoRocha : IEquatable<TipoRocha>
    {
        // Precisa ser instanciado antes das demais propriedades estáticas!
        private static List<TipoRocha> All { get; } = new List<TipoRocha>();

        public static TipoRocha CAL { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(CAL), "Calcário", 1, 47.6, 2.71);
        public static TipoRocha CLC { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(CLC), "Calcário cristalino", 2, 47.6, 2.71);
        public static TipoRocha MRM { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(MRM), "Mármore calcítico", 3, 47.6, 2.71);
        public static TipoRocha COQ { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(COQ), "Coquina", 4, 47.6, 2.71);
        public static TipoRocha MRD { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(MRD), "Mármore dolomítico", 5, 47.6, 2.71);
        public static TipoRocha CLU { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(CLU), "Calcilutito", 6, 47.6, 2.71);
        public static TipoRocha CSI { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(CSI), "Calcissiltito", 7, 47.6, 2.71);
        public static TipoRocha CRE { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(CRE), "Calcarenito", 8, 47.6, 2.71);
        public static TipoRocha SID { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(SID), "Siderita", 9, 43.5, 2.87);
        public static TipoRocha CRU { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(CRU), "Calcirudito", 10, 47.6, 2.71);
        public static TipoRocha MDS { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(MDS), "Mudstone", 12, 47.6, 2.71);
        public static TipoRocha WKS { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(WKS), "Wackestone", 13, 47.6, 2.71);
        public static TipoRocha WAC { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(WAC), "Wackestone", 13, 47.6, 2.71);       
        public static TipoRocha PKS { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(PKS), "Packstone", 14, 47.6, 2.71);
        public static TipoRocha PAC { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(PAC), "Packstone", 14, 47.6, 2.71);
        public static TipoRocha GRS { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(GRS), "Grainstone", 15, 47.6, 2.71);
        public static TipoRocha BLT { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(BLT), "Biolitito", 16, 47.6, 2.71);
        public static TipoRocha BAF { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(BAF), "Bafflestone", 17, 47.6, 2.71);
        public static TipoRocha PHO { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(PHO), "Fosforita", 18, 47.6, 2.71);
        public static TipoRocha CHT { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(CHT), "Chert", 19, 45, 2.8);
        public static TipoRocha AGN { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(AGN), "Argilito arenoso", 20, 70.4, 2.71);
        public static TipoRocha AGL { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(AGL), "Argilito carbonático", 21, 70.4, 2.71);
        public static TipoRocha AGC { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(AGC), "Argilito carbonoso", 22, 70.4, 2.71);
        public static TipoRocha AGS { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(AGS), "Argilito síltico", 23, 70.4, 2.71);
        public static TipoRocha AGB { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(AGB), "Argilito tobáceo", 24, 70.4, 2.71);
        public static TipoRocha ARL { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(ARL), "Arenito argiloso", 25, 55.6, 2.65);
        public static TipoRocha ARC { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(ARC), "Arenito carbonático", 26, 55.6, 2.65);
        public static TipoRocha ARO { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(ARO), "Arenito conglomerático", 27, 55.6, 2.65);
        public static TipoRocha ARF { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(ARF), "Arenito fosfático", 28, 55.6, 2.65);
        public static TipoRocha ART { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(ART), "Arenito tobáceo", 29, 55.6, 2.65);
        public static TipoRocha DOL { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(DOL), "Dolomito", 30, 43.5, 2.87);
        public static TipoRocha RDS { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(RDS), "Rudstone", 31, 47.6, 2.71);
        public static TipoRocha BRR { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(BRR), "Brecha carbonática", 39, 47.6, 2.8);
        public static TipoRocha BRC { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(BRC), "Brecha", 40, 45, 2.8);
        public static TipoRocha BRV { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(BRV), "Brecha vulcânica", 41, 45, 2.8);
        public static TipoRocha CGL { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(CGL), "Conglomerado", 42, 55.6, 2.65);
        public static TipoRocha TON { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(TON), "Toba arenosa", 43, 55.6, 2.65);
        public static TipoRocha DMT { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(DMT), "Diamictito", 44, 45, 2.8);
        public static TipoRocha TOL { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(TOL), "Toba argilosa", 45, 70.4, 2.71);
        public static TipoRocha TIL { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(TIL), "Tilito", 46, 45, 2.8);
        public static TipoRocha TLT { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(TLT), "Tilito", 46, 45, 2.8);       
        public static TipoRocha TOS { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(TOS), "Toba síltica", 47, 45, 2.8);
        public static TipoRocha ARE { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(ARE), "Areia", 48, 55.6, 2.65);
        public static TipoRocha ARN { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(ARN), "Arenito", 49, 55.6, 2.65);
        public static TipoRocha BDL { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(BDL), "Boundstone laminado", 51, 47.6, 2.71);
        public static TipoRocha FLS { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(FLS), "Folhelho síltico", 52, 70.4, 2.75);
        public static TipoRocha LMA { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(LMA), "Limoarcilita", 53, 70.4, 2.75);
        public static TipoRocha SLT { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(SLT), "Siltito", 54, 55.6, 2.65);
        public static TipoRocha ARG { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(ARG), "Argila", 55, 84.7, 2.75);
        public static TipoRocha AGT { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(AGT), "Argilito", 56, 70.4, 2.75);
        public static TipoRocha FLH { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(FLH), "Folhelho", 57, 70.4, 2.75);
        public static TipoRocha MRG { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(MRG), "Marga", 58, 70.4, 2.71);
        public static TipoRocha TOB { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(TOB), "Toba", 59, 45, 2.4);
        public static TipoRocha VCL { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(VCL), "Rocha vulcanoclástica", 60, 47.6, 2.71);
        public static TipoRocha TFU { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(TFU), "Tufo vulcânico", 61, 45, 2.4);
        public static TipoRocha IGB { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(IGB), "Ignimbrito", 63, 45, 2.8);
        public static TipoRocha INI { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(INI), "Ígneas", 64, 61, 2.8);
        public static TipoRocha DIA { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(DIA), "Diabásio", 65, 61, 2.8);
        public static TipoRocha BAS { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(BAS), "Basalto", 66, 61, 2.8);
        public static TipoRocha GRN { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(GRN), "Granito", 67, 61, 2.8);
        public static TipoRocha RLT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(RLT), "Riolitito", 68, 61, 2.37);
        public static TipoRocha BDA { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(BDA), "Boundstone arborescente", 69, 47.6, 2.71);
        public static TipoRocha MNI { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(MNI), "Metamórficas", 70, 45, 2.8);
        public static TipoRocha GNA { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(GNA), "Gnaisse", 71, 45, 2.8);
        public static TipoRocha FLT { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(FLT), "Filito", 72, 45, 2.8);
        public static TipoRocha XST { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(XST), "Xisto", 73, 45, 2.8);
        public static TipoRocha QZT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(QZT), "Quartzito", 74, 45, 2.8);
        public static TipoRocha MAR { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(MAR), "Meta arenito", 75, 45, 2.8);
        public static TipoRocha MSI { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(MSI), "Meta siltito", 76, 45, 2.8);
        public static TipoRocha MST { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(MST), "Meta siltito", 76, 45, 2.8);        
        public static TipoRocha ARS { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(ARS), "Ardósia", 77, 45, 2.8);
        public static TipoRocha ULB { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(ULB), "Ultrabásica", 78, 45, 2.8);
        public static TipoRocha ULS { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(ULS), "Ultrabásica", 78, 45, 2.8);       
        public static TipoRocha MSD { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(MSD), "Metassedimento", 79, 45, 2.8);
        public static TipoRocha BOU { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(BOU), "Boundstone", 80, 47.6, 2.71);
        public static TipoRocha TQD { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(TQD), "Taquidrita", 81, 92, 1.66);
        public static TipoRocha AND { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(AND), "Anidrita", 82, 50, 2.98);
        public static TipoRocha GIP { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(GIP), "Gipsita", 83, 52.6, 2.35);
        public static TipoRocha SNI { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(SNI), "Sal não identificado", 84, 66.7, 2.03);
        public static TipoRocha HAL { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(HAL), "Halita", 85, 66.7, 2.16);
        public static TipoRocha SLV { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(SLV), "Silvinita", 86, 75, 1.86);
        public static TipoRocha CRN { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(CRN), "Carnalita", 87, 55.6, 2.65);
        public static TipoRocha FLO { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(FLO), "Floatstone", 88, 47.6, 2.71);
        public static TipoRocha FRA { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(FRA), "Framestone", 89, 47.6, 2.71);
        public static TipoRocha LNT { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(LNT), "Linhito", 90, 45, 2.8);
        public static TipoRocha TES { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(TES), "Talcoestivencita", 91, 70.4, 2.75);
        public static TipoRocha CRV { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(CRV), "Carvão", 92, 45, 2.8);
        public static TipoRocha BIN { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(BIN), "Bindstone", 93, 45, 2.8);
        public static TipoRocha SLX { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(SLX), "Silexito", 94, 45, 2.8);
        public static TipoRocha FNG { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(FNG), "Fangolita", 95, 45, 2.8);
        public static TipoRocha ETR { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(ETR), "Estromatolito", 109, 47.6, 2.71);
        public static TipoRocha ESF { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(ESF), "Esferulitito", 116, 47.6, 2.71);
        public static TipoRocha LMT { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(LMT), "Laminito", 118, 47.6, 2.71);
        public static TipoRocha OUT { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(OUT), "Outros", -1, 47.6, 2.71);
        public static TipoRocha CIM { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(CIM), "Cimento", 11, 47.6, 2.71);
        public static TipoRocha OCG { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(OCG), "ORTOCONGLOMERADO", 32, 55.6, 2.65);
        public static TipoRocha PNI { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(PNI), "PLUTONICA NÃO IDENTIFICADA", 33, 61, 2.8);
        public static TipoRocha VNI { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(VNI), "VULCANICA NÃO IDENTIFICADA", 34, 61, 2.8);
        public static TipoRocha GBR { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(GBR), "GABRO", 35, 61, 2.8);
        public static TipoRocha HIC { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(HIC), "HIALOCLASTICO", 36, 61, 2.8);
        public static TipoRocha MAF { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(MAF), "ROCHA MAFICA", 37, 61, 2.8);
        public static TipoRocha ULT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(ULT), "ROCHA ULTRA MAFICA", 38, 61, 2.8);
        public static TipoRocha ARB { get; } = new TipoRocha(GrupoLitologico.Arenitos, nameof(ARB), "ARENITO BIOTURBADO", 50, 55.6, 2.65);
        public static TipoRocha CZV { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(CZV), "CINZA VULCANICA", 62, 61, 2.8);
        public static TipoRocha LPL { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(LPL), "LAPILLI", 100, 61, 2.8);
        public static TipoRocha AGV { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(AGV), "AGLOMERADO VULCANICO", 101, 61, 2.8);
        public static TipoRocha GRD { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(GRD), "GRANODIORITO", 102, 61, 2.8);
        public static TipoRocha DIO { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(DIO), "DIORITO", 103, 61, 2.8);
        public static TipoRocha DCT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(DCT), "DACITO", 104, 61, 2.8);
        public static TipoRocha TQT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(TQT), "TRAQUITO", 105, 61, 2.8);
        public static TipoRocha ADT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(ADT), "ANDESITO", 106, 61, 2.8);
        public static TipoRocha FNT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(FNT), "FONOLITO", 107, 61, 2.8);
        public static TipoRocha SNT { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(SNT), "SIENITO", 108, 61, 2.8);
        public static TipoRocha EBT { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(EBT), "ESTROMATOLITO ARBUSTIFORME", 110, 47.6, 2.71);
        public static TipoRocha EAR { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(EAR), "ESTROMATOLITO ARBORESCENTE", 111, 47.6, 2.71);
        public static TipoRocha EDD { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(EDD), "ESTROMATOLITO DENDRIFORME", 112, 47.6, 2.71);
        public static TipoRocha TRO { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(TRO), "TROMBOLITO", 113, 47.6, 2.71);
        public static TipoRocha DEN { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(DEN), "DENDROLITO", 114, 47.6, 2.71);
        public static TipoRocha LEI { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(LEI), "LEIOLITO", 115, 47.6, 2.71);
        public static TipoRocha TRV { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(TRV), "TRAVERTINO", 117, 47.6, 2.71);
        public static TipoRocha LML { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(LML), "LAMINITO LISO", 119, 47.6, 2.71);
        public static TipoRocha LMC { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(LMC), "LAMINITO CRENULADO", 120, 47.6, 2.71);
        public static TipoRocha BAC { get; } = new TipoRocha(GrupoLitologico.Carbonatos, nameof(BAC), "BIOACUMULADO", 121, 47.6, 2.71);
        public static TipoRocha LAT { get; } = new TipoRocha(GrupoLitologico.Argilosas, nameof(LAT), "LAMITO", 122, 70.4, 2.75);

        public static TipoRocha N_IDENT { get; } = new TipoRocha(GrupoLitologico.Outros, nameof(N_IDENT), "Não Identificada", -1, 47.6, 2.71);
        public static TipoRocha AND_TQD_CRN { get; } = new TipoRocha(GrupoLitologico.Evaporitos, nameof(AND_TQD_CRN), "Anidrita Intercalada com Taquidrita e Carnalita", 84, 66.7, 2.03);
        public static TipoRocha HAL_IGN { get; } = new TipoRocha(GrupoLitologico.Ígneas, nameof(HAL_IGN), "Halita Intercalada com Ígnea", 84, 66.7, 2.03);

        public GrupoLitologico Grupo { get; private set; }
        public string Mnemonico { get; private set; }
        public string Nome { get; private set; }
        public int Numero { get; private set; }
        public double Dtmc { get; private set; }
        public double Rhog { get; private set; }

        [BsonConstructor]
        private TipoRocha(GrupoLitologico grupo, string mnemonico, string nome, int numero, double dtmc, double rhog)
        {
            Grupo = grupo;
            Mnemonico = mnemonico;
            Nome = nome;
            Numero = numero;
            Dtmc = dtmc;
            Rhog = rhog;

            if (All.All(x => x != this))
            {
                All.Add(this);
            }
        }

        public static TipoRocha FromMnemonico(string mnemonico)
        {
            if (string.IsNullOrWhiteSpace(mnemonico)) return null;

            foreach (var item in List())
            {
                if (String.Equals(item.Mnemonico, mnemonico, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        public static TipoRocha FromNome(string nome)
        {
            //Está com FirstOrDefault e não SingleOrDefault, pois tenho nomes repetidos e a lista está ordenada já para 
            //o mnemonico utilizado na conveção atual.
            return All.FirstOrDefault(r => string.Compare(r.Nome, nome, CultureInfo.InvariantCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0);
        }

        public static TipoRocha FromNumero(int numero)
        {
            foreach (var item in All)
            {
                if (item.Numero == numero)
                    return item;
            }

            return null;
        }

        public static IReadOnlyList<TipoRocha> List()
        {
            return All.AsReadOnly();
        }

        public override string ToString() => $"{Mnemonico}({Numero}) ({Nome}) - {Grupo}";

        public override bool Equals(object obj)
        {
            return Equals(obj as TipoRocha);
        }

        public bool Equals(TipoRocha other)
        {
            return other != null &&
                   EqualityComparer<GrupoLitologico>.Default.Equals(Grupo, other.Grupo) &&
                   Mnemonico == other.Mnemonico &&
                   Nome == other.Nome &&
                   Numero == other.Numero &&
                   Dtmc == other.Dtmc &&
                   Rhog == other.Rhog;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Grupo, Mnemonico, Nome, Numero, Dtmc, Rhog);
        }

        public static bool operator ==(TipoRocha rocha1, TipoRocha rocha2)
        {
            return EqualityComparer<TipoRocha>.Default.Equals(rocha1, rocha2);
        }

        public static bool operator !=(TipoRocha rocha1, TipoRocha rocha2)
        {
            return !(rocha1 == rocha2);
        }
    }
}