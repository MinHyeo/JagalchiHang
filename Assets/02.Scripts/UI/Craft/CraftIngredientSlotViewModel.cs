using UnityEngine;

public class CraftIngredientSlotViewModel : ViewModelBase
{
    public void InvokeOnceInit()
    {
        OnPropertyChanged(nameof(ItemId));
        OnPropertyChanged(nameof(IconPath));
        OnPropertyChanged(nameof(RequireCount));
        OnPropertyChanged(nameof(CurrentCount));
    }

    private string _itemId;
    public string ItemId
    {
        get => _itemId;
        set
        {
            if(_itemId != value)
            {
                _itemId = value;
                OnPropertyChanged(nameof(ItemId));
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

    private int _requireCount;
    public int RequireCount
    {
        get => _requireCount;
        set
        {
            if (_requireCount != value)
            {
                _requireCount = value;
                OnPropertyChanged(nameof(RequireCount));
            }
        }
    }

    private int _currentCount;
    public int CurrentCount
    {
        get => _currentCount;
        set
        {
            if (_currentCount != value)
            {
                _currentCount = value;
                OnPropertyChanged(nameof(CurrentCount));
            }
        }
    }

    public bool HasEnough
    {
        get
        {
            return _currentCount >= _requireCount;
        }
    }
    

    public void SetIngredientInfo(string itemId, int requiredCount, int currentCount)
    {
        ItemId = itemId;
        RequireCount = requiredCount;
        CurrentCount = currentCount;

        var itemData = GameDataManager.Instance.GetData<ItemData>(itemId);
        if (itemData != null)
        {
            IconPath = itemData.IconPath;
        }

        InvokeOnceInit();
    }
}
