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
        // TODO : 정보 저장용
        // long uniqueId = GameUtil.GenerateUniqueId();

        // TODO : 데이트 드리븐용
        //var newItem = new InventorySlotViewModel();
        //if (newItem == null) return;

        //var itemData = GameDataManager.Instance.GetData<ItemData>(itemDataId);
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
