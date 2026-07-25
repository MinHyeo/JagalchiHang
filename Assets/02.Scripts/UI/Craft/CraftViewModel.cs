using System.Collections.Generic;
using UnityEngine;

public class CraftViewModel : ViewModelBase
{
    private Dictionary<string, RecipeData> _recipeDataList = new Dictionary<string, RecipeData>();

    private List<CraftCategorySlotViewModel> _categorySlots = new List<CraftCategorySlotViewModel>();
    public List<CraftCategorySlotViewModel> CategorySlots
    {
        get => _categorySlots;
        set
        {
            if (_categorySlots != value)
            {
                _categorySlots = value;
                OnPropertyChanged(nameof(CategorySlots));
            }
        }
    }

    private RecipeData _selectedRecipe;
    public RecipeData SelectedRecipe
    {
        get => _selectedRecipe;
        set 
        { 
            if (_selectedRecipe != value)
            {
                _selectedRecipe = value;
                OnPropertyChanged(nameof(SelectedRecipe));
            }
        }
    }

    private string _resultIconPath;
    public string ResultIconPath
    {
        get => _resultIconPath;
        set
        {
            if (_resultIconPath != value)
            {
                _resultIconPath = value;
                OnPropertyChanged(nameof(ResultIconPath));
            }
        }
    }

    private List<CraftIngredientSlotViewModel> _ingredientSlots = new List<CraftIngredientSlotViewModel>();
    public List<CraftIngredientSlotViewModel> IngredientSlots
    {
        get => _ingredientSlots;    
        set
        {
            if (_ingredientSlots != value)
            {
                _ingredientSlots = value;
                OnPropertyChanged(nameof(IngredientSlots));
            }
        }
    }

    public void InitCraftRecipes()
    {
        _categorySlots.Clear();
        _recipeDataList.Clear();

        var recipeData = GameDataManager.Instance.GetAllData<RecipeData>();
        if (recipeData == null) return;

        for (int i = 0; i < recipeData.Count; i++)
        {
            var recipe = recipeData[i];

            _recipeDataList[recipe.Id] = recipe;

            var slotVm = new CraftCategorySlotViewModel();
            slotVm.SetSlotInfo(recipe);
            _categorySlots.Add(slotVm);
        }

        OnPropertyChanged(nameof(CategorySlots));

        if (_categorySlots.Count > 0)
        {
            SelectRecipe(_categorySlots[0].RecipeId);
        }
    }

    public void SelectRecipe(string recipeId)
    {
        if (!_recipeDataList.ContainsKey(recipeId)) return;

        RecipeData recipe = _recipeDataList[recipeId];

        if (recipe == null) return;

        SelectedRecipe = recipe;

        for (int j = 0; j < _categorySlots.Count; j++)
        {
            _categorySlots[j].IsSelected = (_categorySlots[j].RecipeId == recipeId);
        }

        var resultItemData = GameDataManager.Instance.GetData<ItemData>(recipe.ResultId);
        ResultIconPath = resultItemData.IconPath;
        
        UpdateIngredientSlots(recipe.Ingredients);

        OnPropertyChanged(nameof(SelectedRecipe));
        OnPropertyChanged(nameof(ResultIconPath));
        OnPropertyChanged(nameof(IngredientSlots));
    }

    private void UpdateIngredientSlots(string ingredients)
    {
        _ingredientSlots.Clear();

        if (string.IsNullOrEmpty(ingredients)) return;

        var invenVm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();

        string[] pairs = ingredients.Split(',');
        for (int i = 0; i < pairs.Length; i++)
        {
            string[] data = pairs[i].Split(':');
            if (data.Length == 2)
            {
                string itemId = data[0].Trim();

                int requiredCount = System.Convert.ToInt32(data[1].Trim());

                int currentCount = GetInventoryItemCount(invenVm, itemId);

                var ingVm = new CraftIngredientSlotViewModel();
                ingVm.SetIngredientInfo(itemId, requiredCount, currentCount);

                _ingredientSlots.Add(ingVm);
            }
        }
    }

    private int GetInventoryItemCount(InventoryViewModel invenVm, string itemId)
    {
        int count = 0;
        if (invenVm?.InventorySlots == null) return count;

        foreach (var slot in invenVm.InventorySlots.Values)
        {
            if (slot.ItemDataId == itemId)
            {
                count += slot.ItemStackCount;
            }
        }
        return count;
    }

    public bool CanCraft()
    {
        if (_selectedRecipe == null || _ingredientSlots.Count == 0) return false;

        if (_selectedRecipe.CraftType == "Any")
        {
            for (int i = 0; i < _ingredientSlots.Count; i++)
            {
                if (_ingredientSlots[i].HasEnough) return true;
            }

            return false;
        }
        else
        {
            for (int i = 0; i < _ingredientSlots.Count; i++)
            {
                if (_ingredientSlots[i].HasEnough == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public bool RequestCraft()
    {
        if (CanCraft() == false) return false;

        var invenVm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();

        if (_selectedRecipe.CraftType == "Any")
        {
            CraftIngredientSlotViewModel targetIngredient = null;
            for(int i = 0; i < _ingredientSlots.Count; i++)
            {
                if (_ingredientSlots [i].HasEnough)
                {
                    targetIngredient = _ingredientSlots [i];
                    break;
                }
            }

            if (targetIngredient == null) return false;

            ReduceIngredient(invenVm, targetIngredient);
        }
        else
        {
            for (int j = 0; j < _ingredientSlots.Count; j++)
            {
                ReduceIngredient(invenVm, _ingredientSlots[j]);
            }
        }

        string resultId = _selectedRecipe.ResultId;
        var npcManager = GameUtil.GetNpcManager();
        if (resultId.StartsWith("Npc"))
        {
            if (resultId.Contains("Battle"))
            {
                npcManager.SpawnBattleNpc(resultId).Forget();
            }
            else if (resultId.Contains("Bag"))
            {
                npcManager.SpawnBagNpc(resultId).Forget();
            }
            else
            {
                Debug.LogWarning($"잘못된 NPC{resultId}");
            }
        }
        else
        {
            invenVm.AcquireItem(_selectedRecipe.ResultId, _selectedRecipe.ResultCount);
        }

        SelectRecipe(_selectedRecipe.Id);

        return true;
    }

    private void ReduceIngredient(InventoryViewModel invenVm, CraftIngredientSlotViewModel ingVm)
    {
        int remainToRemove = ingVm.RequireCount;
        foreach (var slot in invenVm.InventorySlots.Values)
        {
            if (slot.ItemDataId == ingVm.ItemId && slot.ItemStackCount > 0)
            {
                int removeAmount = Mathf.Min(slot.ItemStackCount, remainToRemove);
                slot.ItemStackCount -= removeAmount;
                remainToRemove -= removeAmount;

                if (slot.ItemStackCount <= 0) slot.Clear();
                if (remainToRemove <= 0) break;
            }
        }
    }
}
