using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SestWeb.Domain.DTOs.Importação
{
    public class PoçoWebDto
    {
        public Uri Url { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public bool? Archived { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty("last_updated_at")]
        public DateTimeOffset? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public Well Well { get; set; }
        public Revision Revision { get; set; }
        public Uri Revisions { get; set; }
    }

    public class Revision
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public string Schema { get; set; }
        public Well Author { get; set; }
        public string Description { get; set; }
        public Well Source { get; set; }
        public Content Content { get; set; }
    }

    public class Well
    {
        public Uri Url { get; set; }
        public string Name { get; set; }
    }

    public class Content
    {
        public Output Output { get; set; }
        public List<object> FileDependencies { get; set; }
        public string AppVersion { get; set; }
        [JsonProperty("source_url")]
        public Uri SourceUrl { get; set; }
        public Input Input { get; set; }
    }

    public class Input
    {
        public long? Lda { get; set; }
        public string Altitude { get; set; }
        public Abandonment Abandonment { get; set; }
        public ToleranceRadius ToleranceRadius { get; set; }
        public CorrelatedWells CorrelatedWells { get; set; }
        public Rig Rig { get; set; }
        public Shoeset Shoeset { get; set; }
        public Tortuosity Tortuosity { get; set; }
        public Completion Completion { get; set; }
        public List<object> PotentialFlowZones { get; set; }
        public List<Phase> Phases { get; set; }
        public Temperature Temperature { get; set; }
        [JsonProperty("completion_type")]
        public string CompletionType { get; set; }
        public Trajectory Trajectory { get; set; }
        public Objectives Objectives { get; set; }
        public long? Airgap { get; set; }
        public List<object> Faults { get; set; }
        public long? FinalMd { get; set; }
        public Location Location { get; set; }
        public InputWellhead Wellhead { get; set; }
        public List<CirculationLoss> CirculationLosses { get; set; }
        public List<object> Geohazards { get; set; }
        [JsonProperty("protection_level")]
        public long? ProtectionLevel { get; set; }
        public string WellRestart { get; set; }
        public Copcoi Copcoi { get; set; }
        public Lithology Lithology { get; set; }
        [JsonProperty("geo_tables")]
        public GeoTables GeoTables { get; set; }
        public Purpose Purpose { get; set; }
        [JsonProperty("well_classification")]
        public string WellClassification { get; set; }
        [JsonProperty("well_purpose")]
        public string WellPurpose { get; set; }
        [JsonProperty("well_complexity")]
        public long? WellComplexity { get; set; }
        public bool? HasFormationTest { get; set; }
        public long? Lifespan { get; set; }
        public Coring Coring { get; set; }
        [JsonProperty("well_criticalness")]
        public bool? WellCriticalness { get; set; }
        [JsonProperty("workover_intervention")]
        public bool? WorkoverIntervention { get; set; }

    }

    public class Abandonment
    {
        public long? ReservoirTemperature { get; set; }
        public List<object> Remainings { get; set; }
        public List<CementPlug> CementPlugs { get; set; }
        public bool? WearPlug { get; set; }
        public long? WellBottomMd { get; set; }
        public List<object> Perforations { get; set; }
        public List<Comment> Comments { get; set; }
        public bool? CorrosionCover { get; set; }
        public List<PipTag> PipTags { get; set; }
        public string Type { get; set; }
        public List<object> Packers { get; set; }
    }

    public class CementPlug
    {
        public bool? Bpp { get; set; }
        public long? BaseMd { get; set; }
        public long? TopMd { get; set; }
        public string PlugType { get; set; }
        public bool? ThroughTubing { get; set; }
    }

    public class Comment
    {
        public string CommentComment { get; set; }
    }

    public class PipTag
    {
        public long? Depth { get; set; }
        public string Reference { get; set; }
        public long? PhaseId { get; set; }
    }

    public class CirculationLoss
    {
        public string Estimate { get; set; }
        public long? Elevation { get; set; }
        public string Description { get; set; }
    }

    public class Completion
    {
        public DhsvCalibration Stimulation { get; set; }
        public DhsvCalibration DhsvCalibration { get; set; }
    }

    public class DhsvCalibration
    {
    }

    public class Copcoi
    {
        public List<object> Superior { get; set; }
        public List<object> Inferior { get; set; }
    }

    public class Coring
    {
        public List<object> Samples { get; set; }
    }

    public class CorrelatedWells
    {
        public The9Ll19Rjs The9Ll19Rjs { get; set; }
    }

    public class The9Ll19Rjs
    {
        public long? Lda { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? LastUpdatedAt { get; set; }
        public Uri Url { get; set; }
        public List<Test> Tests { get; set; }
        public string WellName { get; set; }
    }

    public class Test
    {
        public double? FluidDensity { get; set; }
        public DateTimeOffset? TestDate { get; set; }
        public double? NormalizedVolume { get; set; }
        public double? NormalizedTvd { get; set; }
        public double? VerticalDepth { get; set; }
        public string TestType { get; set; }
    }

    public class GeoTables
    {
        [JsonProperty("fracture_pressure")]
        public Dictionary<string, FracturePressure> FracturePressure { get; set; }
        [JsonProperty("overbalance_pressure")]
        public Dictionary<string, FracturePressure> OverbalancePressure { get; set; }
        [JsonProperty("superior_collapse")]
        public Dictionary<string, FracturePressure> SuperiorCollapse { get; set; }
        [JsonProperty("inferior_collapse")]
        public Dictionary<string, FracturePressure> InferiorCollapse { get; set; }
        public Dictionary<string, FracturePressure> UCS { get; set; }
        [JsonProperty("pore_pressure")]
        public Dictionary<string, FracturePressure> PorePressure { get; set; }
        public long? PorePressureId { get; set; }
        public long? FracturePressureId { get; set; }
        public long? OverbalancePressureId { get; set; }
        
    }

    public class FracturePressure
    {
        public List<FracturePressureValue> Values { get; set; }
        public string Name { get; set; }
    }

    public class FracturePressureValue
    {
        public string Pressure { get; set; }
        public double? Elevation { get; set; }
        [JsonProperty("measure_depth")]
        public string MeasuredDepth { get; set; }
    }

    public class EPressure
    {
        public FracturePressure The1 { get; set; }
    }

    public class Lithology
    {
        public List<object> HcPorousFormations { get; set; }
        public List<Age> Ages { get; set; }
        public List<object> Comments { get; set; }
        public List<Age> Rocks { get; set; }
        public List<object> InclinedFormations { get; set; }
        public List<Age> Formations { get; set; }
    }

    public class Age
    {
        [JsonProperty("base_elevation")]
        public double? BaseElevation { get; set; }
        [JsonProperty("base_md")]
        public double? BaseMd { get; set; }
        public string Name { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public LocationWellhead Wellhead { get; set; }
    }

    public class LocationWellhead
    {
        public long? CentralMeridian { get; set; }
        public string Datum { get; set; }
        public double? Longitude { get; set; }
        public long? Y { get; set; }
        public double? Latitude { get; set; }
        public long? X { get; set; }
    }

    public class Objectives
    {
        public long? H2SForecast { get; set; }
        public long? Co2Forecast { get; set; }
        public List<Target> Targets { get; set; }
    }

    public class Target
    {
        public string Name { get; set; }
        public long? TopElevation { get; set; }
        [JsonProperty("base_elevation")]
        public string BaseElevation { get; set; }
        [JsonProperty("base_md")]
        public string BaseMd { get; set; }
        [JsonProperty("top_md")]
        public string TopMd { get; set; }
        public string Hc { get; set; }
        public long? Y { get; set; }
        public long? X { get; set; }
        public string Type { get; set; }
    }

    public class Phase
    {
        public long? FinalTvd { get; set; }
        public long? CasingStringId { get; set; }
        public string WorkStringId { get; set; }
        public string Setup { get; set; }
        public long? InternalCementTopDepth { get; set; }
        public Comments Comments { get; set; }
        public Casingloads Casingloads { get; set; }
        public long? FinalMd { get; set; }
        public Columns Columns { get; set; }
        public double? DrillingFluidDensity { get; set; }
        public long? ShoeMd { get; set; }
        public long? DrillStringId { get; set; }
        public string DrillingFluidType { get; set; }
        public long? StingerId { get; set; }
        public long? ShoeTvd { get; set; }
        public Metalurgic Metalurgic { get; set; }
        public PhaseKicktol Kicktol { get; set; }
        public PhaseBopPressure BopPressure { get; set; }
    }

    public class PhaseBopPressure
    {
        public long? FracturePressureId { get; set; }
        public double? GasGradient { get; set; }
        public long? PorePressureId { get; set; }
    }

    public class Casingloads
    {
        public double? MaxFluidDensity { get; set; }
        public double? YieldStrengthFactor { get; set; }
        public long? OverpullMargin { get; set; }
        public List<string> ColumnKeys { get; set; }
        public Slip Slip { get; set; }
        [JsonProperty("casing_run")]
        public CasingRun CasingRun { get; set; }
    }

    public class CasingRun
    {
        public The2 Stinger { get; set; }
        public The1 Casingstring { get; set; }
        public The2 Workstring { get; set; }
    }

    public class The1
    {
        public string Name { get; set; }
        [JsonProperty("final_md")]
        public string FinalMd { get; set; }
        public double? TotalWeight { get; set; }
        public List<CasingstringValue> Values { get; set; }
        public long? InitialMd { get; set; }
        public string Type { get; set; }
        public long? Id { get; set; }
    }

    public class CasingstringValue
    {
        public long? InitialMd { get; set; }
        public ValueMaterial Material { get; set; }
        [JsonProperty("final_md")]
        public long? FinalMd { get; set; }
        public long? Length { get; set; }
        public double? TotalWeight { get; set; }
    }

    public class ValueMaterial
    {
        public double? DriftId { get; set; }
        public double? Collapse { get; set; }
        public double? Weight { get; set; }
        public double? Burst { get; set; }
        public string Grade { get; set; }
        public double? Od { get; set; }
        public long? MaterialId { get; set; }
        public double? NominalWeight { get; set; }
        public string Connection { get; set; }
        public long? BodyYieldStrength { get; set; }
        public double? AverageJointLength { get; set; }
        public string Type { get; set; }
        public double? Id { get; set; }
        public long? MakeupTorque { get; set; }
    }

    public class The2
    {
        public string Name { get; set; }
        //public Id? Id { get; set; }
        public long? FinalMd { get; set; }
        public double? TotalWeight { get; set; }
        public List<The2_Value> Values { get; set; }
        public long? InitialMd { get; set; }
        public string Type { get; set; }
        public long? StingerFinalMd { get; set; }
        public string Rig { get; set; }
        public double? BitSize { get; set; }
    }

    public class The2_Value
    {
        public long? InitialMd { get; set; }
        public Column Material { get; set; }
        public long? FinalMd { get; set; }
        public long? Length { get; set; }
        public double? TotalWeight { get; set; }
    }

    public class Column
    {
        public double? Weight { get; set; }
        public string Grade { get; set; }
        public double? Od { get; set; }
        public string Rig { get; set; }
        public string Type { get; set; }
    }

    public class Slip
    {
        public double? Angle { get; set; }
        public double? ContactEfficiency { get; set; }
        public long? Length { get; set; }
        public double? FrictionCoefficient { get; set; }
        public string Model { get; set; }
        public long? Id { get; set; }
    }

    public class Columns
    {
        [JsonProperty("1")]
        public The1 The1 { get; set; }
        public The2 The2 { get; set; }
        public The2 The3 { get; set; }
        public The2 Ns45_1 { get; set; }
        public The2 Ns45_2 { get; set; }
    }

    public class Comments
    {
        public string BhaSugerido { get; set; }
        public string PerfuraÃÃO { get; set; }
        public string RevestimentoECimentaÃÃO { get; set; }
        public string FluidoDePerfuraÃÃO { get; set; }
        public string JusitificativaSapata { get; set; }
        public string Lwd { get; set; }
        public string TesteDeBop { get; set; }
        public string PerfilagemACabo { get; set; }
    }

    public class PhaseKicktol
    {
        public long? KickFluidDensity { get; set; }
        public long? DeltaRhoktMin { get; set; }
        public long? KickFluidVolume { get; set; }
    }

    public class Metalurgic
    {
        public List<Casing> Cra { get; set; }
        public List<Casing> Casing { get; set; }
    }

    public class Casing
    {
        public long? TopMd { get; set; }
        public long? BaseMd { get; set; }
        public string MetalurgicType { get; set; }
    }

    public class Purpose
    {
        public Injection Injection { get; set; }
        public Production Production { get; set; }
        public Assessment Assessment { get; set; }
    }

    public class Assessment
    {
        public Fluid Fluid { get; set; }
    }

    public class Fluid
    {
        public Gas Oil { get; set; }
        public Gas Gas { get; set; }
        public Gas Gasoil { get; set; }
    }

    public class Gas
    {
        public long? ThirdFlowTime { get; set; }
        public long? SecondFlowTime { get; set; }
        public long? FirstStaticTime { get; set; }
        public long? SecondStaticTime { get; set; }
        public long? FlowRate { get; set; }
        public long? FirstFlowTime { get; set; }
        public long? Rgo { get; set; }
    }

    public class Injection
    {
        public List<DhsvCalibration> InjectionColumn { get; set; }
    }

    public class Production
    {
        public Development Development { get; set; }
        public List<ProductionColumn> ProductionColumn { get; set; }
        public bool? Applicable { get; set; }
        public Perforated Perforated { get; set; }
        public GasOilData GasOilData { get; set; }
        public InternGeometry InternGeometry { get; set; }
    }

    public class Development
    {
        public GasLift GasLift { get; set; }
        public NaturalFlow NaturalFlow { get; set; }
    }

    public class GasLift
    {
        public GasLiftPeakOil PeakWater { get; set; }
        public GasLiftPeakOil PeakOil { get; set; }
        public bool? Applicable { get; set; }
    }

    public class GasLiftPeakOil
    {
        public long? OilFlowRate { get; set; }
        public long? AnmAnnulusTemperature { get; set; }
        public long? PerforatedFlowPressure { get; set; }
        public long? AnmInjectionPressure { get; set; }
        public long? WaterFlowRate { get; set; }
        public long? GasLiftInjectionFlowRate { get; set; }
        public long? ValveMd { get; set; }
        public long? Rgo { get; set; }
    }

    public class NaturalFlow
    {
        public NaturalFlowPeakOil PeakWater { get; set; }
        public NaturalFlowPeakOil PeakOil { get; set; }
        public bool? Applicable { get; set; }
    }

    public class NaturalFlowPeakOil
    {
        public long? OilFlowRate { get; set; }
        public long? WaterFlowRate { get; set; }
        public long? PerforationsFlowPressure { get; set; }
        public long? Rgo { get; set; }
    }

    public class GasOilData
    {
        public Dictionary<string, double> GasComposition { get; set; }
        public long? OilApiGravity { get; set; }
    }

    public class InternGeometry
    {
        public long? MinIdMd { get; set; }
        public double? DhsvMinId { get; set; }
    }

    public class Perforated
    {
        public long? BaseMd { get; set; }
        public long? Temperature { get; set; }
    }

    public class ProductionColumn
    {
        public double? ElementOd { get; set; }
        public string Grade { get; set; }
        public ProductionColumnMaterial Material { get; set; }
        public long? NominalWeight { get; set; }
        public string Connection { get; set; }
        public double? PackerFluid { get; set; }
        public double? ElementId { get; set; }
        public long? PackerMd { get; set; }
    }

    public class ProductionColumnMaterial
    {
        public string Nome { get; set; }
        public string Grade { get; set; }
        public double? Od { get; set; }
        public double? Id { get; set; }
        public long? NominalWeight { get; set; }
        public string Connection { get; set; }
        public string Type { get; set; }
        public long? MaterialId { get; set; }
    }

    public class Rig
    {
        public long? TowerCapacity { get; set; }
        public double? BopStackHeight { get; set; }
        public long? BopFloatedLmrpWeight { get; set; }
        public long? BopMaxPressure { get; set; }
        public long? TopDriveWeight { get; set; }
        public long? BopAnnularMaxPressure { get; set; }
        public long? NumberPumps { get; set; }
        public DateTimeOffset? LastUpdatedAt { get; set; }
        public long? Airgap { get; set; }
        public string RigType { get; set; }
        public long? MaxFinalMd { get; set; }
        public List<Column> Columns { get; set; }
        public long? CompensatorCapacity { get; set; }
        public string Company { get; set; }
        public long? TopDriveCapacity { get; set; }
        public long? BopFloatedStackWeight { get; set; }
        public long? MaxLda { get; set; }
        public string Name { get; set; }
        public long? MaxPumpPressure { get; set; }
        public string Bop { get; set; }
        public Uri Url { get; set; }
        public double? BopLmrpHeight { get; set; }
        public string Fullname { get; set; }
    }

    public class Shoeset
    {
        public List<object> Phases { get; set; }
    }

    public class Temperature
    {
        public string Source { get; set; }
        public List<Gradient> Gradients { get; set; }
        public List<Point> Points { get; set; }
    }

    public class Gradient
    {
        public long? Elevation { get; set; }
        public double? GradientGradient { get; set; }
    }

    public class Point
    {
        public long? Elevation { get; set; }
        public double? Temperature { get; set; }
    }

    public class ToleranceRadius
    {
        public string UndesirableQuadrants { get; set; }
        public long? Length { get; set; }
        public long? VerticalLength { get; set; }
    }

    public class Tortuosity
    {
        public P90 P90 { get; set; }
        public List<object> OffsetWells { get; set; }
        public long? Pitch { get; set; }
    }

    public class P90
    {
        public DAzi DAzi { get; set; }
        public DAzi DInc { get; set; }
    }

    public class DAzi
    {
        public double? Turn { get; set; }
        public double? Build { get; set; }
        public double? Vertical { get; set; }
        public double? Slant { get; set; }
    }

    public class Trajectory
    {
        public List<TrajectoryPoint> Points { get; set; }
    }

    public class TrajectoryPoint
    {
        public double? Md { get; set; }
        public double? Displacement { get; set; }
        public double? Azimuth { get; set; }
        public double? Tvd { get; set; }
        public double? EW { get; set; }
        public double? NS { get; set; }
        public double? Inclination { get; set; }

    }

    public class InputWellhead
    {
        public long? MaxPressure { get; set; }
        public long? ShoulderLoadCapacity { get; set; }
        public long? AapFloatedWeight { get; set; }
        public long? BendingMoment { get; set; }
        public long? AbpFloatedWeight { get; set; }
        public string Model { get; set; }
        public string Housings { get; set; }
        public long? Id { get; set; }
    }

    public class Output
    {
        public Dictionary<string, KicktolValue> Kicktol { get; set; }
        public Dictionary<string, Dictionary<string, double>> Phases { get; set; }
        public Dictionary<string, BopPressureValue> BopPressure { get; set; }
        public Dictionary<string, Msr> Msr { get; set; }
        public Dictionary<string, Casingload> Casingloads { get; set; }
    }

    public class BopPressureValue
    {
        public long? FinalCota { get; set; }
        public List<MinTestPressure> MinTestPressures { get; set; }
        public bool? BopWorkPressureOk { get; set; }
        public double? BopWorkPressure { get; set; }
        public double? MaxTemperature { get; set; }
        public double? PCabPp { get; set; }
        public double? MaxPp { get; set; }
        public long? InitialCota { get; set; }
        public long? MaxPpTvd { get; set; }
        public double? BottomPp { get; set; }
        public double? PCabFr { get; set; }
        public double? Mapecab { get; set; }
        public double? MaxPressure { get; set; }
        public double? GasGradient { get; set; }
        public double? ShoePf { get; set; }
        public long? ShoeTvd { get; set; }
    }

    public class MinTestPressure
    {
        public string BeforeFormationTestHint { get; set; }
        public string BeforeDrillingHint { get; set; }
        public string Name { get; set; }
        public string BeforeDrilling { get; set; }
        public string BeforeFormationTest { get; set; }
        public string PeriodicHint { get; set; }
        public string Periodic { get; set; }
    }

    public class Casingload
    {
        public double? WeightOnHook { get; set; }
        public double? WeightOnColumn { get; set; }
        public double? FloatingFactor { get; set; }
        public bool? ColumnRequirementsOk { get; set; }
        public double? CompensatorMargin { get; set; }
        public double? CasingYieldStrength { get; set; }
        public string ColumnRequirements { get; set; }
        public bool? MaxOverpullOk { get; set; }
        public double? TotalFloatedWeight { get; set; }
        public string RigRequirements { get; set; }
        public bool? CompensatorMarginOk { get; set; }
        public double? TotalWeight { get; set; }
        public bool? SlipCrushingMarginOk { get; set; }
        public double? MaxOverpull { get; set; }
        public double? SlipCrushingMargin { get; set; }
        public bool? RigRequirementsOk { get; set; }
        public double? SlipCrushing { get; set; }
    }

    public class KicktolValue
    {
        public double? MaxPpTolerance { get; set; }
        public double? DrillFluidDensity { get; set; }
        public long? ShoeTvd { get; set; }
        public bool? DeltaOk { get; set; }
        public long? WellCota { get; set; }
        public double? FluidHeight { get; set; }
        public long? WellTvd { get; set; }
        public double? WellCapacity { get; set; }
        public double? ShoePf { get; set; }
        public double? CasingId { get; set; }
        public long? DeltaRhoktMin { get; set; }
        public long? IsKickOpenWell { get; set; }
        public long? KickFluidDensity { get; set; }
        public double? Delta { get; set; }
        public double? MaxPhasePp { get; set; }
        public long? BitTvd { get; set; }
        public double? HoleSize { get; set; }
        public double? MinPfTolerance { get; set; }
        public long? MinPfToleranceOk { get; set; }
        public long? VolumeBelowBit { get; set; }
        public List<Dictionary<string, double>> Volumes { get; set; }
        public long? MaxPpToleranceOk { get; set; }
    }

    public class Msr
    {
        public double? Margin { get; set; }
        public double? Delta { get; set; }
    }

}