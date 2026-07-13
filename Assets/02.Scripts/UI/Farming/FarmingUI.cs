using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FarmingUI : UIBase
{
    [Header("등록 부분")]
    [SerializeField] private Transform _farmingSlot; // TODO : 생성 위치인데 수정이 필요??
    [SerializeField] private GameObject _slotPrefab; // TODO

    private List<FarmingSlotUI> _slotUIList = new List<FarmingSlotUI>();

    private FarmingViewModel _vm;

    private void OnEnable()
    {
        _vm = NetworkManager_re.Inst.FarmingService.GetFarmingViewModel();
        _vm.TestFarming();
        InitFarmingSlot();
    }

    private void OnDisable()
    {
        ClearSlotUIList();
    }

    private void InitFarmingSlot()
    {
        ClearSlotUIList();

        for (int i = 0; i < _vm.FarmingSlots.Count; i++)
        {
            GameObject gObj = Instantiate(_slotPrefab, _farmingSlot);
            if (gObj == null) return;

            FarmingSlotUI slotUI = gObj.GetComponent<FarmingSlotUI>();
            if (slotUI == null) return;

            slotUI.Setup(this);
            slotUI.BindViewModel(_vm.FarmingSlots[i]);
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

    public void RequestMoveFromInventory(int invenIdx, int farmingIdx)
    {
        NetworkManager_re.Inst.RequestMoveItem_InvenToFarming(invenIdx, farmingIdx);
    }
}
