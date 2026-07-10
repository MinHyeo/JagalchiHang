using System.Collections.Generic;
using UnityEngine;

public class InventorySlotViewModel : ViewModelBase
{

    private long _itemUniqueId;
    public long ItemUniqueId
    {
        get => _itemUniqueId;
        set
        {
            if (_itemUniqueId != value) 
            { 
                _itemUniqueId = value;
                OnPropertyChanged(nameof(ItemUniqueId));
            }
        }
    }

    private string _itemDataId;
    public string ItemDataId
    {
        get => _itemDataId;
        set
        {
            if (_itemDataId != value)
            {
                _itemDataId = value;
                OnPropertyChanged(nameof(ItemDataId));
            }
        }
    }

    private int _itemStackCount;
    public int ItemStackCount
    {
        get => _itemStackCount;
        set
        {
            if (_itemStackCount != value)
            {
                _itemStackCount = value;
                OnPropertyChanged(nameof(ItemStackCount));
            }
        }
    }

    private int _maxCount;
    public int MaxCount
    {
        get => _maxCount;
        set
        {
            if (_maxCount != value)
            {
                _maxCount = value;
                OnPropertyChanged(nameof(MaxCount));
            }
        }
    }

    private bool _isStackable;
    public bool IsStackable
    {
        get => _isStackable;
        set { 
            if (_isStackable != value) 
            { 
                _isStackable = value;
                OnPropertyChanged(nameof(IsStackable)); 
            } 
        }
    }

    // TODO : 데이터 드리븐으로 받아오기
    public void SetItem(string id, int count, bool stackable = true, int max = 99)
    {
        ItemDataId = id;
        ItemStackCount = count;
        IsStackable = stackable;
        MaxCount = max;
    }

    public void Clear()
    {
        SetItem(null, 0, false, 0);
    }
}
