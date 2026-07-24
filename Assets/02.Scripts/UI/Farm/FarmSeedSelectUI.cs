using System.Collections.Generic;
using UnityEngine;

public class FarmSeedSelectUI : UIBase
{
    [SerializeField] private Transform Transform_SlotRoot;
    [SerializeField] private GameObject Prefab_SeedSlot;
    [SerializeField] private UIButton Button_Close;


    private int _plotUniqueId;
    private List<FarmSeedSlotUI> _slotList = new List<FarmSeedSlotUI>();
    private bool _isInitialized = false;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        if (_isInitialized == false)
        {
            CreateSlots();
            _isInitialized = true;  
        }
    }

    public void Init(int plotUniqueId)
    {
        _plotUniqueId = plotUniqueId;
        RefreshSlots();
    }

    private void CreateSlots()
    {
        var cropDataList = GameDataManager.Instance.GetAllDataId<CropData>();
        if (cropDataList == null) return;

        for (int i = 0; i < cropDataList.Count; i++)
        {
            var gObj = Instantiate(Prefab_SeedSlot, Transform_SlotRoot);
            var slotUI = gObj.GetComponent<FarmSeedSlotUI>();
            if (slotUI = null) continue;
            _slotList.Add(slotUI);
        }
    }

    private void RefreshSlots()
    {
        var cropDataList = GameDataManager.Instance.GetAllDataId<CropData>();
        if (cropDataList == null) return;

        var invenVm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();

        for (int i = 0;  i < _slotList.Count; i++)
        {
            var cropData = GameDataManager.Instance.GetData<CropData>(cropDataList[i]);
            _slotList[i].Init(cropData,_plotUniqueId, invenVm);
        }
    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmSeedSelectUI);
    }

}
