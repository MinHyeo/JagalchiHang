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
    public PlayerSaveModel PlayerSaveModel;
    public ItemSaveModel ItemSaveModel;
}

public class PlayerSaveModel
{
    public int currentHp;
}

public class ItemSaveModel 
{
    public long ItemUniqueId;
    public string ItemDataId;
    public int ItemStackCount;
    public ItemLocationType Location;
    public int SlotIndex;
}