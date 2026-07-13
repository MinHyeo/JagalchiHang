using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : UIBase
{
    [Header("등록 부분")]
    [SerializeField] private Transform _inventorySlot; // TODO : 생성 위치인데 수정이 필요??
    [SerializeField] private GameObject _slotPrefab; // TODO

    private List<InventorySlotUI> _slotUIList = new List<InventorySlotUI>();

    private InventoryViewModel _vm;

    private void Start()
    {
        _vm = NetworkManager_re.Inst.InventoryService.GetLocalInventoryViewModel();
        _vm.TestInventory();
        InitInventory();
    }

    private void InitInventory()
    {
        ClearSlotUIList();

        for (int i = 0; i < _vm.InventorySlots.Count; i++)
        {
            GameObject gObj = Instantiate(_slotPrefab, _inventorySlot);
            if (gObj == null) return;

            InventorySlotUI slotUI = gObj.GetComponent<InventorySlotUI>();
            if (slotUI == null) return;

            slotUI.Setup(this);
            slotUI.BindViewModel(_vm.InventorySlots[i]);
            _slotUIList.Add(slotUI);
        }
    }

    private void ClearSlotUIList()
    {
        foreach (var slotUI in _slotUIList)
        {
            Destroy(slotUI.gameObject);
        }
        _slotUIList.Clear();
    }

    public void RequestSwap(int startIdx, int endIdx)
    {
        _vm.SwapSlots(startIdx, endIdx);
    }

    public void RequestMoveFromFarming(int farmingIdx, int invenIdx)
    {
        NetworkManager_re.Inst.RequestMoveItem_InvenToFarming(invenIdx, farmingIdx);
    }

    public void RequestMoveFromStorage(int storageIdx, int invenIdx)
    {
        NetworkManager_re.Inst.RequestMoveItem_InvenToStorage(invenIdx, storageIdx);
    }

    // 테스트용, TODO : 게임매니저나 오브젝트 매니저로 이전
    public bool RequestAcquireItem(string id, int count, bool isStackable, int maxCount)
    {
        return _vm.AcquireItem(id, count, isStackable, maxCount);
    }

}
