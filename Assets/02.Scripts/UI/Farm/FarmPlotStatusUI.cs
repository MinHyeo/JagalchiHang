using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;


public class FarmPlotStatusUI : UIBase
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Gauge;
    [SerializeField] private UIButton Button_Harvest;

    private int _plotUniqueId;
    private FarmManager _farmManager;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Button_Harvest.BindOnClickButtonEvent(OnClick_Harvest);
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += RefreshUI;
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged -= RefreshUI;
        }
    }

    private void OnClick_Harvest()
    {
        var plot = _farmManager.GetFarmPlotCanBeNull(_plotUniqueId);
        if (plot == null)
        {
            return;
        }

        bool result = _farmManager.RequestHarvestCrop(plot);
        if (result)
        {
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.FarmPlotStatusUI);
        }
    }

    public int GetPlotUniqueId()
    {
        return _plotUniqueId;
    }

    public void Init(int plotUniqueId)
    {
        _plotUniqueId = plotUniqueId;
        _farmManager = NetworkManager.Instance.FarmService.GetFarmViewModel().GetFarmManager();
        RefreshUI();
    }

    private void RefreshUI()
    {
        var plot = _farmManager.GetFarmPlotCanBeNull(_plotUniqueId);
        if (plot == null || plot.IsPlanted == false)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        var cropData = GameDataManager.Instance.GetData<CropData>(plot.CropDataId);
        if (cropData == null) return;

        var growthStageMinutes = cropData.GetGrowthStageMinutes();
        int totalMinutes = 0;
        for (int i = 0; i < growthStageMinutes.Count; i++)
        {
            totalMinutes += growthStageMinutes[i];
        }

        float progress;
        if (totalMinutes > 0)
        {
            progress = (float)plot.GrowthMinutes / totalMinutes;
        }
        else
        {
            progress = 0f;
        }
        Image_Gauge.fillAmount = progress;

        bool isHarvestable = plot.CurrentGrowthStage >= growthStageMinutes.Count;
        Button_Harvest.gameObject.SetActive(isHarvestable);

        var itemData = GameDataManager.Instance.GetData<ItemData>(plot.CropDataId);
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
}




