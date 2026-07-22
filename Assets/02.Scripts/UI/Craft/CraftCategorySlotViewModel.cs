using UnityEngine;

public class CraftCategorySlotViewModel : ViewModelBase
{
    public void InvokeOnceInit()
    {
        OnPropertyChanged(nameof(RecipeId));
        OnPropertyChanged(nameof(IconPath));
        OnPropertyChanged(nameof(IsSelected));
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

    public void SetSlotInfo(RecipeData recipeData)
    {
        if (recipeData == null)
        {
            RecipeId = null;
            IconPath = null;
            IsSelected = false;
            return;
        }

        RecipeId = recipeData.Id;

        var resultItemData = GameDataManager.Instance.GetData<ItemData>(recipeData.ResultId);
        if (resultItemData != null)
        {
            IconPath = resultItemData.IconPath;
        }
        else
        {
            IconPath = null;
        }

        IsSelected = false;
        InvokeOnceInit();
    }
}
