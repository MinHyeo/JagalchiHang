using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : GameDataBase
{
    public string ItemName;
    public string IconPath;
    public bool IsStackable;
    public int MaxCount;
    public bool IsUsable;
    public string UseItemType;
    public int UseItemParameterList;
    public string UseItemDescription;
    public int DropWeight;
    public int MinDropCount;
    public int MaxDropCount;
}
