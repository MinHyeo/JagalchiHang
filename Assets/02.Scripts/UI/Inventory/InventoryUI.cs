using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : UIBase
{
    [Header("등록 부분")]
    [SerializeField] private Transform _inventorySlot; // TODO : 생성 위치인데 수정이 필요??
    [SerializeField] private GameObject _slotPrefab; // TODO

    private Dictionary<int, InventorySlotUI> _slotUIList = new Dictionary<int, InventorySlotUI>();

    private InventoryViewModel _vm;

    private void OnEnable()
    {
        _vm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();
        _vm.PropertyChanged += OnPropertyChanged_View;
        _vm.AddInventorySlotViewModel();
        InitInventory();

        // 테스트용 
        _vm.TestAddItem();
    }

    private void OnDisable()
    {
        ClearSlotUIList();
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

            slotUI.Setup(this, i);
            slotUI.BindViewModel(_vm.InventorySlots[i]);

            slotUI.OnSlotDoubleClicked += OnSlotUseRequested;
            _slotUIList.Add(i, slotUI);
        }
    }

    private void ClearSlotUIList()
    {
        if(_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_View;
        }

        foreach (var slotUI in _slotUIList.Values)
        {
            if (slotUI != null)
            {
                slotUI.OnSlotDoubleClicked -= OnSlotUseRequested;
                Destroy(slotUI.gameObject);
            }
        }
        _slotUIList.Clear();
    }

    private void OnPropertyChanged_View(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ItemListAdded")
        {
            InitInventory();
        }
    }

    private void OnSlotUseRequested(int slotKey, InventorySlotViewModel slotVM)
    {
        if (slotVM == null) return;

        _vm.RequestUseItem(slotVM.ItemUniqueId);
    }

    // 드래그 앤 드랍 부분
    public void RequestSwap(int startIdx, int endIdx)
    {
        _vm.SwapSlots(startIdx, endIdx);
    }

    public void RequestMoveFromFarming(int farmingIdx, int invenIdx)
    {
        int currentBoxUniqueId = NetworkManager.Instance.FarmingService.CurrentActiveBoxUniqueId;
        NetworkManager.Instance.RequestMoveItem_FarmingToInven(farmingIdx, invenIdx, currentBoxUniqueId);
    }

    public void RequestMoveFromStorage(int storageIdx, int invenIdx)
    {
        NetworkManager.Instance.RequestMoveItem_StorageToInven(storageIdx, invenIdx);
    }
}
