using UnityEngine;

public class NetworkCraftService
{
    private CraftViewModel _localCraftVeiwModel;

    public CraftViewModel GetCraftViewModel()
    {
        if (_localCraftVeiwModel == null)
        {
            CreateLocalCraftViewModel();
        }
        return _localCraftVeiwModel;
    }

    private CraftViewModel CreateLocalCraftViewModel()
    {
        GameDataManager.Instance.LoadData<ItemData>();
        GameDataManager.Instance.LoadData<RecipeData>();

        var craftVm = new CraftViewModel();
        _localCraftVeiwModel = craftVm;
        craftVm.InitCraftRecipes();
        return craftVm;
    }
}
