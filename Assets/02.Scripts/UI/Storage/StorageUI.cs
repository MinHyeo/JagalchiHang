using System.Collections.Generic;
using UnityEngine;

public class StorageUI : UIBase
{
    [Header("등록 부분")]
    [SerializeField] private Transform _inventorySlot;
    [SerializeField] private GameObject _slotPrefab;

    private List<StorageSlotUI> _slotUIList = new List<StorageSlotUI>();

    private StorageViewModel _vm;

    private void Start()
    {
        _vm = NetworkManager_re.Inst.StorageService.GetLocalStorageViewModel();
        InitStorage();
    }

    private void InitStorage()
    {
        ClearSlotUIList();

        for (int i = 0; i < _vm.StorageSlots.Count; i++)
        {
            GameObject gObj = Instantiate(_slotPrefab, _inventorySlot);
            if (gObj == null) return;

            StorageSlotUI slotUI = gObj.GetComponent<StorageSlotUI>();
            if (slotUI == null) return;

            slotUI.Setup(this);
            slotUI.BindViewModel(_vm.StorageSlots[i]);
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

    public void RequestMoveFromInventory(int invenIdx, int storageIdx)
    {
        NetworkManager_re.Inst.RequestMoveItem_InvenToStorage(invenIdx, storageIdx);
    }
}
