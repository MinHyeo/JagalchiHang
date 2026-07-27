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
        Debug.Log($"FarmSeedSelectUI OnEnable, _isInitialized: {_isInitialized}");
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
        Debug.Log("CreateSlots 호출됨");
        var allItemData = GameDataManager.Instance.GetAllData<ItemData>();
        Debug.Log($"ItemData 개수: {allItemData?.Count}");
        if (allItemData == null) return;

        for (int i = 0; i < allItemData.Count; i++)
        {
            Debug.Log($"ItemData Id: {allItemData[i].Id}");
            if (allItemData[i].Id.StartsWith("Item_Seed_") == false) continue;
            Debug.Log($"필터 통과:{allItemData[i].Id}");

            Debug.Log($"Instantiate 시도: {Prefab_SeedSlot}");
            var gObj = Instantiate(Prefab_SeedSlot, Transform_SlotRoot);
            gObj.SetActive(false);
            Debug.Log($"슬롯 생성됨: {gObj}");
            var slotUI = gObj.GetComponent<FarmSeedSlotUI>();
            if (slotUI == null) continue;
            if (slotUI == null) continue;
            _slotList.Add(slotUI);
        }

    }

    private void RefreshSlots()
    {
        Debug.Log("RefreshSlots 호출됨");

        var allItemData = GameDataManager.Instance.GetAllData<ItemData>();
        if (allItemData == null) return;

        var invenVm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();
        if (invenVm == null) return;


        int slotIndex = 0;
        for (int i = 0;  i < allItemData.Count; i++)
        {
            if (allItemData[i].Id.StartsWith("Item_Seed_") == false) continue;
            if (slotIndex  >= _slotList.Count) break;

            string cropDataId = allItemData[i].Id.Replace("Item_Seed_", "Crop_");
            var cropData = GameDataManager.Instance.GetData<CropData>(cropDataId);
            if (cropData == null) continue;


            _slotList[slotIndex].gameObject.SetActive(false);
            _slotList[slotIndex].gameObject.SetActive(true);
            _slotList[slotIndex].Init(cropData, _plotUniqueId, invenVm);
            slotIndex++;
        }
    }

    public int GetPlotUniqueId()
    {
        return _plotUniqueId;
    }
    private void OnClick_Close()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmSeedSelectUI);
    }

}
