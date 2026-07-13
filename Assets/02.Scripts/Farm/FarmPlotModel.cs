using System;

[Serializable]
public class FarmPlotModel
{
    public int PlotUniqueId;
    public string CropDataId;
    public bool IsPlanted;
    public int GrowthMinutes;
    public int CurrentGrowthStage;
    public bool IsWatered;
    public bool IsUnlocked;

}
