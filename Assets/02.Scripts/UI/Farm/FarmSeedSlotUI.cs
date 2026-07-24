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

    private void OnEnable()
    {
        Button_Slot.BindOnClickButtonEvent(OnClick_Slot);
    }

    public void Init(CropData cropData, int plotUniqueId, InventoryViewModel invenVm)
    {
        _cropDataId = cropData.Id;
        _plotUniqueId = plotUniqueId;

        int seedCount = GetSeedCount(cropData.SeedItemDataId, invenVm);

        if (seedCount <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        Text_SeedCount.text = $"{seedCount}";
        //Image_Icon.sprite
    }

    private int GetSeedCount(string seedItemDataId, InventoryViewModel invenVm)
    {
        int totalCount = 0;
        for (int i = 0; i < invenVm.InventorySlots.Count; i++)
        {
            if (invenVm.InventorySlots[i].ItemDataId == seedItemDataId)
            {
                totalCount += invenVm.InventorySlots[i].ItemStackCount;
            }
        }

            
        return totalCount;
    }

    private void OnClick_Slot()
    {
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
