using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NetworkInventoryService
{
    private InventoryViewModel _localInventoryViewModel;

    public InventoryViewModel GetLocalInventoryViewModel()
    {
        if (_localInventoryViewModel == null)
        {
            CreateLocalInventoryViewModel();
        }

        return _localInventoryViewModel;
    }

    private InventoryViewModel CreateLocalInventoryViewModel()
    {
        GameDataManager.Instance.LoadData<ItemData>();
        var inventoryVm = new InventoryViewModel();
        inventoryVm.AddInventorySlotViewModel();
        _localInventoryViewModel = inventoryVm;
        return inventoryVm;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        var invenVm = GetLocalInventoryViewModel();
        invenVm.AcquireItem(itemDataId, addItemCount);

        // TODO : 저장 기능 구현 후 연동
        // NetworkManager.Instance.SaveLoadService.RequestSaveData();
    }

    public bool RequestUseItem(long requestUseTargetItemUniqeuId)
    {
        var invenVm = GetLocalInventoryViewModel();
        invenVm.RequestUseItem(requestUseTargetItemUniqeuId);

        return true;
    }

    public void RequestRemoveItem(long removeTargetUniqueId)
    {
        var invenVm = GetLocalInventoryViewModel();
        invenVm.RemoveItemSlotViewModel(removeTargetUniqueId);

        // TODO : 세이브 필요
    }

    public void BindInventoryInputEvent()
    {
        InputManager.Instance.OnClickInventory += OnOpenInventoryUI;
    }

    public void UnBindInventoryInputEvent()
    {
        InputManager.Instance.OnClickInventory -= OnOpenInventoryUI;
    }

    private void OnOpenInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (UIManager.Instance.IsOpenUI(UIType.InventoryUI))
            {
                UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.InventoryUI);
            }
            else
            {
                UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.InventoryUI);
            }
        }
    }

}
