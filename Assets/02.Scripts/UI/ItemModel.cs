using System;
using UnityEngine;

[Serializable]
public class ItemModel
{
    public long ItemUniqueId;
    public string ItemDataId;
    public int ItemStackCount;
    public ItemLocationType Location;
    public int SlotIndex;
}

public enum ItemLocationType
{
    None,
    Inventory = 1,
    Storage = 2,
    Farming = 3
}