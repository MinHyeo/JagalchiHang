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
    public int day = 1;
    public PlayerSaveModel PlayerSaveModel;
    public List<ItemSaveModel> ItemSaveModel;
}

[System.Serializable]
public class PlayerSaveModel
{
    public int CurrentHp;
    public int CurrentHunger;
    public int CurrentThirst;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public MapType CurrentMapType;

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