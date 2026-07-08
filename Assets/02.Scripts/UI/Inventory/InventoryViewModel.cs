using System.Collections.Generic;
using UnityEngine;

public class InventoryViewModel : ViewModelBase
{
    private Dictionary<long, InventorySlotViewModel> _itemList = new Dictionary<long, InventorySlotViewModel>();
    public Dictionary<long, InventorySlotViewModel> ItemList
    {
        get => _itemList;
        set
        {
            if (_itemList != value)
            { 
                _itemList = value;
                OnPropertyChanged(nameof(ItemList));
            }
        }
    }

    public void AddInventorySlotViewModel(InventorySlotViewModel slotVm)
    {
        _itemList.Add(slotVm.ItemUniqueId, slotVm);
        OnPropertyChanged("itemListAdded");
    }

    public void RemoveInventorySlotViewModel(long uniqueId)
    {
        if (_itemList.ContainsKey(uniqueId) == true)
        {
            _itemList.Remove(uniqueId);
        }

        OnPropertyChanged("itemListRemoveed");
    }
}
