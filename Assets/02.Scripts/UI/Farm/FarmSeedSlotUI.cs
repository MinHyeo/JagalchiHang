using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FarmSeedSlotUI : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TextMeshProUGUI Text_SeedCount;
    [SerializeField] private UIButton Button_Slot;

    private string _cropDataId;
    private int _plotUniqueId;


    public void Init(CropData cropData, int plotUniqueId, InventoryViewModel invenVm)
    {
        //Button_Slot.BindOnClickButtonEvent(OnClick_Slot);

        _cropDataId = cropData.Id;
        _plotUniqueId = plotUniqueId;

        int seedCount = GetSeedCount(cropData.SeedItemDataId, invenVm);
        Debug.Log($"씨앗 개수: {seedCount}, SeedItemDataId: {cropData.SeedItemDataId}");

        if (seedCount < cropData.RequiredSeedCount)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        Button_Slot.BindOnClickButtonEvent(OnClick_Slot);
        Text_SeedCount.text = $"{seedCount} / {cropData.RequiredSeedCount}";

        var itemData = GameDataManager.Instance.GetData<ItemData>(cropData.SeedItemDataId);
        if (itemData != null && string.IsNullOrEmpty(itemData.IconPath) == false)
        {
            LoadIcon(itemData.IconPath).Forget();
        }
    
    }

    private async UniTaskVoid LoadIcon(string iconPath)
    {
        Sprite sprite = await ResourceManager.Instance.LoadAsset<Sprite>(iconPath);
        if (sprite != null)
        {
            Image_Icon.sprite = sprite;
        }
    }

    private int GetSeedCount(string seedItemDataId, InventoryViewModel invenVm)
    {
        int totalCount = 0;
        Debug.Log($"인벤토리 슬롯 개수: {invenVm.InventorySlots.Count}");
        Debug.Log($"GetSeedCount 호출됨, seedItemDataId: {seedItemDataId}");
        for (int i = 0; i < invenVm.InventorySlots.Count; i++)
        {
            Debug.Log($"슬롯 {i} 아이디: {invenVm.InventorySlots[i].ItemDataId}");

            if (invenVm.InventorySlots[i].ItemDataId == seedItemDataId)
            {
                totalCount += invenVm.InventorySlots[i].ItemStackCount;
            }
        }

            
        return totalCount;
    }

    private void OnClick_Slot()
    {
        Debug.Log("OnClick_Slot 호출됨");
        var farmManager = NetworkManager.Instance.FarmService.GetFarmViewModel().GetFarmManager();
        var plot = farmManager.GetFarmPlotCanBeNull(_plotUniqueId);
        if (plot == null) return;

        bool result = farmManager.RequestPlantCrop(plot, _cropDataId);
        if (result)
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmSeedSelectUI);
        }
    }
}
