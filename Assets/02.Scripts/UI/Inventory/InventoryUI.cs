using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : UIBase
{
    [Header("슬롯 개수")]
    [SerializeField] private int _slotCount = 36;

    [Header("등록 부분")]
    [SerializeField] private Transform _inventorySlot; // TODO : 생성 위치인데 수정이 필요??
    [SerializeField] private GameObject _slotPrefab; // TODO

    private Dictionary<int, InventorySlotViewModel> _slots = new Dictionary<int, InventorySlotViewModel>();
    public Dictionary<int, InventorySlotViewModel> slots => _slots;

    private List<InventorySlotUI> _slotUIList = new List<InventorySlotUI>();

    private void Start()
    {
        InitInventory();
    }

    void InitInventory()
    {
        _slots.Clear();
        _slotUIList.Clear();

        for (int i = 0; i < _slotCount; i++)
        {
            InventorySlotViewModel slotVM = new InventorySlotViewModel();
            _slots.Add(i, slotVM);

            GameObject gObj = Instantiate(_slotPrefab, _inventorySlot);
            InventorySlotUI slotUI = gObj.GetComponent<InventorySlotUI>();

            slotUI.Setup(i, this);
            _slotUIList.Add(slotUI);

            slotUI.UpdateSlot(null);
        }
    }

    public void SwapSlots(int startIdx, int endIdx)
    {
        if (startIdx == endIdx) return;
        if (!_slots.ContainsKey(startIdx) || !_slots.ContainsKey(endIdx)) return;

        ItemData tempItem = _slots[startIdx].ItemData;
        int tempCount = _slots[startIdx].Count;

        _slots[startIdx].SetItem(_slots[endIdx].ItemData, _slots[endIdx].Count);
        _slots[endIdx].SetItem(tempItem, tempCount);
    }

    public bool AcquireItem(ItemData item, int count = 1)
    {
        if (item.isStackable)
        {
            for (int i = 0; i < _slotCount; i++)
            {
                if (_slots[i].ItemData == item && _slots[i].Count < _slots[i].MaxCount)
                {
                    _slots[i].Count += count;
                    return true;
                }
            }
        }

        for (int i = 0; i < _slotCount; i++)
        {
            if (_slots[i].ItemData == null)
            {
                _slots[i].SetItem(item, count);
                return true;
            }
        }

        return false;
    }
}
