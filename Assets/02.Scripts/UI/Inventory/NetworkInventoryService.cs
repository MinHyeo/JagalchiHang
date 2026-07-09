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
        // TODO : 정보 저장용
        // long uniqueId = GameUtil.GenerateUniqueId();

        // TODO : 데이트 드리븐용
        //var newItem = new InventorySlotViewModel();
        //if (newItem == null) return;

        //var itemData = GameDataManager.Instance.GetData<ItemData>();
        //if (ItemData == null) return;

        //newItem.ItemDataId = itemData.Id;

        // TODO : 아이템 저장 부에서 불러오기
        //newItem.ItemUniqueId = uniqueId;
        //newItem.ItemStackCount = addItemCount;

        //var invenVm = GetLocalPlayerInventoryViewModel();
        //invenVm.AddInventorySlotViewModel(newItem);

        // TODO : 저장 기능 구현 후 연동
        // NetworkManager_re.Inst.SaveLoadService.RequestSaveData();
    }

}
