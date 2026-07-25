using UnityEngine;

public class CraftCategorySlotViewModel : ViewModelBase
{
    public void InvokeOnceInit()
    {
        OnPropertyChanged(nameof(RecipeId));
        OnPropertyChanged(nameof(IconPath));
        OnPropertyChanged(nameof(IsSelected));
        OnPropertyChanged(nameof(ItemName));
        OnPropertyChanged(nameof(IsLocked));
    }

    private string _recipeId;
    public string RecipeId
    {
        get => _recipeId;
        set
        {
            if (_recipeId != value)
            {
                _recipeId = value;
                OnPropertyChanged(nameof(RecipeId));
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

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }

    private string _itemName;
    public string ItemName
    {
        get => _itemName;
        set
        {
            if (_itemName != value)
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }
    }

    private bool _isLocked;
    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            if (_isLocked != value)
            {
                _isLocked = value;
                OnPropertyChanged(nameof(IsLocked));
            }
        }
    }

    public void SetSlotInfo(RecipeData recipeData)
    {
        if (recipeData == null)
        {
            RecipeId = null;
            IconPath = null;
            IsSelected = false;
            ItemName = null;
            return;
        }

        RecipeId = recipeData.Id;

        var resultItemData = GameDataManager.Instance.GetData<ItemData>(recipeData.ResultId);
        if (resultItemData != null)
        {
            IconPath = resultItemData.IconPath;
            ItemName = resultItemData.ItemName;
        }
        else
        {
            IconPath = null;
        }

        IsSelected = false;
        InvokeOnceInit();
    }
}
