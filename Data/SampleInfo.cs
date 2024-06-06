namespace PenPro.Data;

public class SampleInfo
{
    public string Id { get; set; } = "";
    public string ProjectId { get; set; } = "";
    public string TestId { get; set; } = "";
    public double SampleRecovery { get; set; } = 0;
    public double BoreholeStartDepth { get; set; } = 0;
    public double BoreholeEndDepth { get; set; } = 0;
    public bool ContainsCarbonate { get; set; } = false;
    public string SampleImagePath { get; set; } = "";
    public string SampleDesc { get; set; } = "";
    public string SampleType { get; set; } = "";
    public DateTime Date { get; set; }
    public SampleCalculationParameters Calculus { get; set; } = new();
}

public class SampleCalculationParameters
{
    public string Id { get; set; } = "";
    public (string ContainerLabel,double weightOfRingInGram, double weightOfRingAndWetSampleInGram, double diameterOfRingInCm, double heightOfRingInCm, double volumeofRingInCm, double resultIngcm3, double resultInKNm3) BulkDensity { get; set; }
    public (string ContainerLabel, double weightOfRingInGram, double weightOfRingAndDrySampleInGram, double diameterOfRingInCm, double heightOfRingInCm, double volumeofRingInCm, double resultIngcm3, double resultInKNm3)  DryDensity { get; set; } 
    public double SubmergedDensity { get; set; } 
    public (double weightOfRing, double weightOfRingAndWetSample, double weightOfRingAndDrySample, double result) MoistureContent { get; set; } 
    public (string ppUnit, string ppShoe, string scaleReading, double resultInKpa, double resultInMpa) PP { get; set; } 
    public (string vaneBlade, string scaleReading, double resultInKpa, double resultInMpa) _Torvane { get; set; } 
}