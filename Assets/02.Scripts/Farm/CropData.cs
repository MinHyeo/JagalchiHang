using System.Collections.Generic;

[System.Serializable]
public class CropData : GameDataBase
{
    public string Name;
    public string Description;
    public string SeedItemDataId;
    public string HarvestItemDataId;
    public int HarvestMinCount;
    public int HarvestMaxCount;
    public int SeedDropMinCount;
    public int SeedDropMaxCount;
    public string IconPath;
    public string PrefabPath;
    public string GrowthStageMinutesList;

    public List<int> GetGrowthStageMinutes()
    {
        var list = new List<int>();
        string[] split = GrowthStageMinutesList.Split(',');

        for (int i = 0; i < split.Length; i++)
        {
            if (int.TryParse(split[i], out int value))
            {
                list.Add(value);
            }
        }

        return list;
    }
}
