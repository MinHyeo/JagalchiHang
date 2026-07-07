using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("슬롯 개수")]
    [SerializeField] private int _slotCount = 36;

    private Dictionary<int, InventorySlot> _slots = new Dictionary<int, InventorySlot>();
    public Dictionary<int, InventorySlot> slots => _slots;

    [Header("등록 부분")]
    [SerializeField] private Transform _inventorySlot;
    [SerializeField] private GameObject _slotPrefab;

    private List<SlotUI> slotUIList = new List<SlotUI>();

    void Start()
    {
        InitInventory();
    }

    void InitInventory()
    {
        _slots.Clear();
        slotUIList.Clear();

        for (int i = 0; i < _slotCount; i++)
        {
            GameObject gObj = Instantiate(_slotPrefab, _inventorySlot);
            SlotUI slotUI = gObj.GetComponent<SlotUI>();

            slotUI.Setup(i, this);
            slotUIList.Add(slotUI);

            slotUI.UpdateSlot(null);
        }
    }

    public void SwapSlots(int startIdx, int endIdx)
    {
        if (startIdx == endIdx) return;

        bool hasStart = _slots.ContainsKey(startIdx);
        bool hasEnd = _slots.ContainsKey(endIdx);

        InventorySlot startSlot = hasStart ? _slots[startIdx] : null;
        InventorySlot endSlot = hasEnd ? _slots[endIdx] : null;

        if (hasEnd)
        {
            _slots[startIdx] = endSlot;
        }
        else
        {
            _slots.Remove(startIdx);
        }

        if (hasStart)
        {
            _slots[endIdx] = startSlot;
        }
        else
        {
            _slots.Remove(endIdx);
        }

        UpdateAllSlotsUI();
    }

    public void UpdateAllSlotsUI()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (slots.ContainsKey(i))
            {
                slotUIList[i].UpdateSlot(_slots[i]);
            }
            else
            {
                slotUIList[i].UpdateSlot(null);
            }
        }
    }

    public bool AcquireItem(ItemData _item, int _count = 1)
    {
        if (_item.isStackable)
        {
            foreach (var kvp in _slots)
            {
                if (kvp.Value.item != null && kvp.Value.item == _item)
                {
                    kvp.Value.count += _count;
                    slotUIList[kvp.Key].UpdateSlot(kvp.Value);
                    return true;
                }
            }
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            if (!_slots.ContainsKey(i))
            {
                InventorySlot newSlot = new InventorySlot();
                newSlot.AddItem(_item, _count);

                _slots.Add(i, newSlot);
                slotUIList[i].UpdateSlot(newSlot);
                return true;
            }
        }

        return false;
    }
}
