using System.Collections.Generic;

public class ModelBase
{
    public int modelId;
}

public enum ItemLocationType
{
    None,
    Inventory = 1,
    Storage = 2,
}

[System.Serializable]
public class SaveModel : ModelBase
{
    public int Day = 0;
    public float Time = 0.4f;
    public MapType MapType = MapType.ParmingMap;
    public PlayerSaveModel PlayerSaveModel;
    public List<ItemSaveModel> ItemSaveModel;
    public FarmSaveModel FarmSaveModel;
    public NpcSaveModel NpcSaveModel;
}

[System.Serializable]
public class PlayerSaveModel
{
    public int CurrentHp;
    public int CurrentHunger;
    public int CurrentThirst;
    public float PositionX = 20f;
    public float PositionY= 1f;
    public float PositionZ= -3f;
    public int MaxInventorySlotCount;
}

[System.Serializable]
public class ItemSaveModel 
{
    public long ItemUniqueId;
    public string ItemDataId;
    public int ItemStackCount;
    public ItemLocationType Location;
    public int SlotIndex;
}

[System.Serializable]
public class GeneratorSaveModel
{
    public bool IsStop = false;
    public int Power = 100;
    public int TroublePower = 200;
}

[System.Serializable]
public class FarmSaveModel
{
    public List<FarmPlotModel> FarmPlotList;
}

[System.Serializable]
public class NpcSaveModel
{
    public List<string> UnlockedNpcIds = new List<string>();
}