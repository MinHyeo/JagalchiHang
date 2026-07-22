using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FarmingUI : UIBase
{
    [Header("등록 부분")]
    [SerializeField] private Transform _farmingSlot; // TODO : 생성 위치인데 수정이 필요??
    [SerializeField] private GameObject _slotPrefab; // TODO

    private Dictionary<int, FarmingSlotUI> _slotUIList = new Dictionary<int, FarmingSlotUI>();
    private FarmingViewModel _vm;
    private string _boxtUniqueId;

    private void OnDisable()
    {
        ClearAllFarmingSlot();
    }

    public void Init(string boxId)
    {
        _boxtUniqueId = boxId;

        ClearAllFarmingSlot();

        _vm = NetworkManager.Instance.FarmingService.LoadFarmingBox(boxId);

        InitFarmingSlot();
    }

    private void InitFarmingSlot()
    {
        for (int i = 0; i < _vm.FarmingSlots.Count; i++)
        {
            GameObject gObj = Instantiate(_slotPrefab, _farmingSlot);
            if (gObj == null) return;

            FarmingSlotUI slotUI = gObj.GetComponent<FarmingSlotUI>();
            if (slotUI == null) return;

            slotUI.Setup(this, i);
            slotUI.BindViewModel(_vm.FarmingSlots[i]);
            _slotUIList.Add(i, slotUI);
        }
    }

    private void ClearAllFarmingSlot()
    {
        foreach (var slotUI in _slotUIList.Values)
        {
            if (slotUI != null)
            {
                slotUI.UnbindViewModel();
                Destroy(slotUI.gameObject);
            }
        }

        _slotUIList.Clear();
    }

    public void RequestSwap(int startIdx, int endIdx)
    {
        _vm.SwapSlots(startIdx, endIdx);
    }

    public void RequestMoveFromInventory(int invenIdx, int farmingIdx)
    {
        NetworkManager.Instance.RequestMoveItem_InvenToFarming(invenIdx, farmingIdx, _boxtUniqueId);
    }
}
