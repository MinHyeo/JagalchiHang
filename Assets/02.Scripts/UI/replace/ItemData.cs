using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string IconPath;
    public bool isStackable;
    public int maxCount;
    public string UseItemType;
    public List<string> UseItemParameterList;
    public string Description;
}
