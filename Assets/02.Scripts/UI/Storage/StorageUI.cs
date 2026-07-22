using System.Collections.Generic;
using UnityEngine;

public class StorageUI : UIBase
{
    [Header("등록 부분")]
    [SerializeField] private Transform _inventorySlot;
    [SerializeField] private GameObject _slotPrefab;

    private Dictionary<int, StorageSlotUI> _slotUIList = new Dictionary<int, StorageSlotUI>();

    private StorageViewModel _vm;

    private void OnEnable()
    {
        _vm = NetworkManager.Instance.StorageService.GetLocalStorageViewModel();
        InitStorage();
    }

    private void OnDisable()
    {
        ClearSlotUIList();
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

            slotUI.Setup(this, i);
            slotUI.BindViewModel(_vm.StorageSlots[i]);
            _slotUIList.Add(i, slotUI);
        }
    }

    private void ClearSlotUIList()
    {
        foreach (var slotUI in _slotUIList.Values)
        {
            if (slotUI != null)
            {
                Destroy(slotUI.gameObject);
            }
        }

        _slotUIList.Clear();
    }

    public void RequestSwap(int startIdx, int endIdx)
    {
        _vm.SwapSlots(startIdx, endIdx);
    }

    public void RequestMoveFromInventory(int invenIdx, int storageIdx)
    {
        NetworkManager.Instance.RequestMoveItem_InvenToStorage(invenIdx, storageIdx);
    }
}
