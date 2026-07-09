using UnityEngine;

[System.Serializable]
public class CropData : GameDataBase
{
    public string Name;
    public string Description;
    public int GrowthTimeSeconds;
    public int GrowthStageCount;
    public string SeedItemDataId;
    public string HarvestItemDataId;
    public int HarvestMinCount;
    public int HarvestMaxCount;
    public string IconPath;
    public string PrefabPath;
}
