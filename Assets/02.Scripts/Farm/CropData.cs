using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropData : GameDataBase
{
    public string Name;
    public string Description;
    public string SeedItemDataId;
    public string HarvestItemDataId;
    public int HarvestMinCount;
    public int HarvestMaxCount;
    public string IconPath;
    public string PrefabPath;
    public List<int> GrowthStageMinutesList;
}
