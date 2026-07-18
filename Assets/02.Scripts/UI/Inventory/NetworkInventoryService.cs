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
        var inventoryVm = new InventoryViewModel();
        _localInventoryViewModel = inventoryVm;
        inventoryVm.AddInventorySlotViewModel();
        return inventoryVm;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        var invenVm = GetLocalInventoryViewModel();
        invenVm.AcquireItem(itemDataId, addItemCount);

        // TODO : 저장 기능 구현 후 연동
        // NetworkManager_re.Inst.SaveLoadService.RequestSaveData();
    }
}
