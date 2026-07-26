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

    public void BindCraftInputEvent()
    {
        InputManager.Instance.OnClickCraftUI += OnOpenCraftUI;
    }

    public void UnBindCraftInputEvent()
    {
        InputManager.Instance.OnClickCraftUI -= OnOpenCraftUI;
    }

    private void OnOpenCraftUI()
    {
        if (UIManager.Instance.IsOpenUI(UIType.CraftUI))
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CraftUI);
        }
        else
        {
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.CraftUI);
        }
    }
}
