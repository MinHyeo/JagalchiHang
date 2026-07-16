using UnityEngine;

public class StorageSlotViewModel : ViewModelBase
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
        set
        {
            if (_isStackable != value)
            {
                _isStackable = value;
                OnPropertyChanged(nameof(IsStackable));
            }
        }
    }

    private bool _isUsable;
    public bool IsUsable
    {
        get => _isUsable;
        set
        {
            if (_isUsable != value)
            {
                _isUsable = value;
                OnPropertyChanged(nameof(IsUsable));
            }
        }

    }

    private string _iconPath;
    public string IconPath
    {
        get => _iconPath;
        set
        {
            if (_iconPath != value)
            {
                _iconPath = value;
                OnPropertyChanged(nameof(IconPath));
            }
        }
    }

    public void SetItem(string itemDataId, int stackCount)
    {
        if (string.IsNullOrEmpty(itemDataId) || stackCount <= 0)
        {
            ItemDataId = null;
            ItemStackCount = 0;
            IsStackable = false;
            MaxCount = 0;
            IsUsable = false;
            return;
        }

        var itemData = GameDataManager.Instance.GetData<ItemData>(itemDataId);
        if (itemData == null) return;

        ItemDataId = itemData.Id;
        ItemStackCount = stackCount;
        IsStackable = itemData.IsStackable;
        MaxCount = itemData.MaxCount;
        IsUsable = itemData.IsUsable;
        IconPath = itemData.IconPath;
    }

    public void Clear()
    {
        SetItem(null, 0);
    }
}
