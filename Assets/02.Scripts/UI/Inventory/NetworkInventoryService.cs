using UnityEditor;
using UnityEngine;

public class NetworkInventoryService
{
    private InventoryViewModel _localPlayerInventoryViewModel;

    public InventoryViewModel GetLocalPlayerInventoryViewModel()
    {
        if (_localPlayerInventoryViewModel == null)
        {
            CreateLocalPlayerInventoryViewModel();
        }

        return _localPlayerInventoryViewModel;
    }

    private InventoryViewModel CreateLocalPlayerInventoryViewModel()
    {
        var inventoryVm = new InventoryViewModel();
        _localPlayerInventoryViewModel = inventoryVm;
        inventoryVm.AddInventorySlotViewModel();
        return inventoryVm;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        // TODO : 데이터 드리븐용
        // long uniqueId = GameUtil.GenerateUniqueId();

        //var newItem = new InventorySlotViewModel();
        // newItem.ItemUniqueId = uniqueId;
        //newItem.ItemDataId = itemDataId;
        //newItem.ItemStackCount = addItemCount;

        //var invenVm = GetLocalPlayerInventoryViewModel();
        //invenVm.AddInventorySlotViewModel(newItem);

        // NetworkManager_re.Inst.SaveLoadService.RequestSaveData();
    }

}
