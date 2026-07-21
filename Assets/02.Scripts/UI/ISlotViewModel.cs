using UnityEngine;

public interface ISlotViewModel
{
    long ItemUniqueId { get; set; }
    string ItemDataId { get; set; }
    int ItemStackCount { get; set; }
    int MaxCount { get; set; }
    bool IsStackable { get; set; }

    void SetItem(string itemDataId, int stackCount);
    void Clear();
}
