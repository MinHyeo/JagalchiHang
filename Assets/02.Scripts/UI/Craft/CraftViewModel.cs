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

        var npcManager = GameUtil.GetNpcManager();

        for (int i = 0; i < recipeData.Count; i++)
        {
            var recipe = recipeData[i];
            _recipeDataList[recipe.Id] = recipe;

            var slotVm = new CraftCategorySlotViewModel();
            slotVm.SetSlotInfo(recipe);

            if (!string.IsNullOrEmpty(recipe.ResultId) && recipe.ResultId.StartsWith("Npc"))
            {
                if (npcManager != null)
                {
                    if (recipe.ResultId.Contains("Bag") && npcManager.HasBagNpc)
                    {
                        slotVm.IsLocked = true;
                    }
                    else if (recipe.ResultId.Contains("Battle") && npcManager.HasBattleNpc)
                    {
                        slotVm.IsLocked = true;
                    }
                }
            }

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

                int currentCount = 0;
                if (itemId == "Item_Electricity")
                {
                    var generatorVm = NetworkManager.Instance.GeneratorService.GetGeneratorViewModel();
                    if (generatorVm != null)
                    {
                        currentCount = generatorVm.CurrentPower;
                    }
                }
                else
                {
                    if (invenVm != null)
                    {
                        currentCount = invenVm.GetItemCount(itemId);
                    }
                }

                var ingVm = new CraftIngredientSlotViewModel();
                ingVm.SetIngredientInfo(itemId, requiredCount, currentCount);

                _ingredientSlots.Add(ingVm);
            }
        }
    }

    public bool CanCraft()
    {
        if (_selectedRecipe == null || _ingredientSlots.Count == 0) return false;

        string resultId = _selectedRecipe.ResultId;

        if (!string.IsNullOrEmpty(resultId) && resultId.StartsWith("Npc"))
        {
            var npcManager = GameUtil.GetNpcManager();
            if (npcManager != null)
            {
                if (resultId.Contains("Bag") && npcManager.HasBagNpc)
                {
                    return false;
                }
                if (resultId.Contains("Battle") && npcManager.HasBattleNpc)
                {
                    return false;
                }
            }
        }

        if (!string.IsNullOrEmpty(resultId) && !resultId.StartsWith("Npc"))
        {
            var invenVm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();

            if (invenVm.IsEnoughSpace(resultId, _selectedRecipe.ResultCount) == false)
            {
                return false;
            }
        }


        bool isEnoughIngredients = false;

        if (_selectedRecipe.CraftType == "Any")
        {
            for (int i = 0; i < _ingredientSlots.Count; i++)
            {
                if (_ingredientSlots[i].HasEnough)
                {
                    isEnoughIngredients = true;
                    break;
                }
            }
        }
        else
        {
            isEnoughIngredients = true;
            for (int i = 0; i < _ingredientSlots.Count; i++)
            {
                if (_ingredientSlots[i].HasEnough == false)
                {
                    isEnoughIngredients = false;
                    return false;
                }
            }
        }

        return isEnoughIngredients;
    }

    public bool RequestCraft()
    {
        if (CanCraft() == false) return false;

        var invenVm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();

        if (_selectedRecipe.CraftType == "Any")
        {
            CraftIngredientSlotViewModel targetIngredient = null;
            for (int i = 0; i < _ingredientSlots.Count; i++)
            {
                if (_ingredientSlots[i].HasEnough)
                {
                    targetIngredient = _ingredientSlots[i];
                    break;
                }
            }

            if (targetIngredient == null) return false;

            invenVm.RemoveItem(targetIngredient.ItemId, targetIngredient.RequireCount);
        }
        else
        {
            for (int j = 0; j < _ingredientSlots.Count; j++)
            {
                var ingredient = _ingredientSlots[j];

                if (ingredient.ItemId == "Item_Electricity")
                {
                    bool canUsePower = NetworkManager.Instance.GeneratorService.CanUsePower(ingredient.RequireCount);
                    if (canUsePower)
                    {
                        NetworkManager.Instance.GeneratorService.UsePower(ingredient.RequireCount);
                    }
                }
                else
                {
                    invenVm.RemoveItem(_ingredientSlots[j].ItemId, _ingredientSlots[j].RequireCount);
                }
            }
        }

        string resultId = _selectedRecipe.ResultId;
        if (resultId.StartsWith("Npc"))
        {
            var npcManager = GameUtil.GetNpcManager();
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

            for (int i = 0; i < _categorySlots.Count; i++)
            {
                if (_categorySlots[i].RecipeId == _selectedRecipe.Id)
                {
                    _categorySlots[i].IsLocked = true;
                    break;
                }
            }
        }
        else
        {
            invenVm.AcquireItem(_selectedRecipe.ResultId, _selectedRecipe.ResultCount);
        }

        SelectRecipe(_selectedRecipe.Id);

        return true;
    }
}
