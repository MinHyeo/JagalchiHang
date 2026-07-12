using System.Collections.Generic;
using UnityEngine;

public class InventorySlotViewModel : ViewModelBase
{
    public void InvokeOnceInit()
    {
        OnPropertyChanged(nameof(ItemUniqueId));
        OnPropertyChanged(nameof(ItemDataId));
        OnPropertyChanged(nameof(ItemStackCount));
        OnPropertyChanged(nameof(MaxCount));
        OnPropertyChanged(nameof(IsStackable));
    }

    // isEmpty 상태 추가하기

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
    public void SetItem(string itemDataId, int stackCount)
    {
        _itemDataId = itemDataId;
        _itemStackCount = stackCount;

        //var itemData = GameDataManager.Instance.GetData<ItemData>(itemDataId);
        //ItemDataId = itemData.Id;
        //ItemStackCount = stackCount;
        //IsStackable = itemData.isStackable;
        //MaxCount = itemData.maxCount;
    }

    public void Clear()
    {
        SetItem(null, 0);
    }
}
