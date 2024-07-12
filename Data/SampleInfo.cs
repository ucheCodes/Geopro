namespace PenPro.Data;

public class SampleInfo
{
    public string Id { get; set; } = "";
    public string ProjectId { get; set; } = "";
    public string TestId { get; set; } = "";
    public double SampleRecovery { get; set; } = 0;
    public double BoreholeStartDepth { get; set; } = 0;
    public double BoreholeEndDepth { get; set; } = 0;
    public double SampleDrillOut { get; set; } = 0;
    public string SubSampleInCsv { get; set; } = "";
    public string SubSampleIdInCsv { get; set; } = "";
    public string SamplingTool { get; set; } = "";
    public bool ContainsCarbonate { get; set; } = false;
    public string SampleImagePath { get; set; } = "";
    public string SampleDesc { get; set; } = "";
    public string SampleType { get; set; } = "";
    public DateTime Date { get; set; }
    public List<(string id, string strata, double startDepth, double endDepth)> BoreholeLogList  = new();
    public SampleCalculationParameters Calculus { get; set; } = new();
    //public Dictionary<string,List<double>> SuParameters = new Dictionary<string,List<double>>();
}

public class SampleCalculationParameters
{
    public string Id { get; set; } = "";
    //public (string ContainerLabel,double weightOfRingInGram, double weightOfRingAndWetSampleInGram, double diameterOfRingInCm, double heightOfRingInCm, double volumeofRingInCm, double resultIngcm3, double resultInKNm3) BulkDensity { get; set; }
    //public (string ContainerLabel, double weightOfRingInGram, double weightOfRingAndDrySampleInGram, double diameterOfRingInCm, double heightOfRingInCm, double volumeofRingInCm, double resultIngcm3, double resultInKNm3)  DryDensity { get; set; } 
    public List<(string id, string containerLabel, double depth, double weightOfRingInGram, double weightOfRingAndWetSampleInGram,double weightOfRingAndDrySampleInGram, double diameterOfRingInMillimeter, double heightOfRingInMillimeter, double volumeofRingInMillimeter3,double volumeofRingInCentimeter3, double bulkUnitWeightInKNm3, double dryUnitWeightInKNm3,double submergedDensityInKNm3)> UnitWeightParamList = new();
    public List<(string id, double depth, double weightOfRing, double weightOfRingAndWetSample, double weightOfRingAndDrySample, double result)> MoistureContentParamList = new();
    public (string ppUnit, string ppShoe, string scaleReading, double resultInKpa, double resultInMpa) PP { get; set; } 
    public (string vaneBlade, string scaleReading, double resultInKpa, double resultInMpa) _Torvane { get; set; } 
}
