public class ModelBase
{
    public int modelId;
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

}