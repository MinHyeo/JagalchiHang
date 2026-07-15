using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

    private List<FarmPlotModel> _farmPlotList = new List<FarmPlotModel>();
    private Dictionary<int, CropObject> _cropObjectDictionary = new Dictionary<int, CropObject>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.LoadData<CropData>();
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += UpdateAllPlotGrowth;
        }

    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged -= UpdateAllPlotGrowth;

        }

    }

    private void UpdateAllPlotGrowth()
    {
        for (int i = 0; i < _farmPlotList.Count; i++)
        {
            var plot = _farmPlotList[i];

            if (plot.IsPlanted == false)
            {
                continue;
            }

            var cropData = GameDataManager.Instance.GetData<CropData>(plot.CropDataId);
            if (cropData == null)
            {
                continue;
            }

            var growthStageMinutes = cropData.GetGrowthStageMinutes();

            if (plot.CurrentGrowthStage >= growthStageMinutes.Count)
            {
                continue;
            }

            plot.GrowthMinutes++;

            int newStage = CalculateGrowthStage(plot, growthStageMinutes);

            if (newStage != plot.CurrentGrowthStage)
            {
                plot.CurrentGrowthStage = newStage;
                OnPlotGrowthChanged(plot);
            }
        }

    }

    private void OnPlotGrowthChanged(FarmPlotModel plot)
    {
        Debug.Log($"밭 {plot.PlotUniqueId}  성장 단계 변경: {plot.CurrentGrowthStage}");
    }

    private int CalculateGrowthStage(FarmPlotModel plot, List<int> growthStageMinutes)
    {
        int cropGrowthTotalMinutes = 0;

        for (int i = 0; i < growthStageMinutes.Count; i++)
        {
            cropGrowthTotalMinutes += growthStageMinutes[i];

            if (plot.GrowthMinutes < cropGrowthTotalMinutes)
            {
                return i;
            }
        }

        return growthStageMinutes.Count;
    }

    public List<FarmPlotModel> GetFarmPlotList()
    {
        return _farmPlotList;
    }

    public FarmPlotModel GetFarmPlotCanBeNull(int plotUniqueId)
    {
        for (int i = 0; i < _farmPlotList.Count; i++)
        {
            if (_farmPlotList[i].PlotUniqueId == plotUniqueId)
            {
                return _farmPlotList[i];
            }
        }
        return null;
    }

    public void AddFarmPlot(FarmPlotModel newPlot)
    {
        _farmPlotList.Add(newPlot);
    }

    public void RegisterCropObject(int plotUniqueId, CropObject cropObject)
    {
        if (_cropObjectDictionary.ContainsKey(plotUniqueId) == false)
        {
            _cropObjectDictionary.Add(plotUniqueId, cropObject);
        }
    }

    public void UnregisterCropObject(int plotUniqueId)
    {
        if (_cropObjectDictionary.ContainsKey(plotUniqueId))
        {
            _cropObjectDictionary.Remove(plotUniqueId);
        }
    }

    public bool RequestPlantCrop(FarmPlotModel plot, string cropDataId)
    {
        if (plot == null)
        {
            Debug.LogWarning("존재하지 않는 밭입니다.");
            return false;
        }

        if (plot.IsUnlocked == false)
        {
            Debug.LogWarning("아직 해금되지 않은 밭입니다.");
            return false;
        }

        if (plot.IsPlanted == true)
        {
            Debug.LogWarning("이미 작물이 심어져 있습니다.");
            return false;
        }

        var cropData = GameDataManager.Instance.GetData<CropData>(cropDataId);
        if (cropData == null)
        {
            Debug.LogWarning($"작물을 찾을 수 없습니다.: {cropDataId}");
            return false;
        }

        plot.CropDataId = cropDataId;
        plot.IsPlanted = true;
        plot.GrowthMinutes = 0;
        plot.CurrentGrowthStage = 0;

        return true;
    }


    public bool RequestHarvestCrop(FarmPlotModel plot)
    {
        if (plot == null)
        {
            Debug.LogWarning("존재하지 않는 밭입니다.");
            return false;
        }

        if (plot.IsPlanted == false)
        {
            Debug.LogWarning("심어진 작물이 없습니다.");
            return false;
        }

        var cropData = GameDataManager.Instance.GetData<CropData>(plot.CropDataId);
        if (cropData == null)
        {
            Debug.LogWarning("작물 데이터를 찾을 수 없습니다.");
            return false;
        }

        var growthStageMinutes = cropData.GetGrowthStageMinutes();
        if (plot.CurrentGrowthStage < growthStageMinutes.Count)
        {
            Debug.LogWarning("아직 다 자라지 않았습니다.");
            return false;
        }

        var invenVm = NetworkManager_re.Inst.InventoryService.GetLocalInventoryViewModel();
        int garvestCount = Random.Range(cropData.HarvestMinCount, cropData.HarvestMaxCount +1);


        plot.CropDataId = string.Empty;
        plot.IsPlanted = false;
        plot.CurrentGrowthStage = 0;
        plot.GrowthMinutes = 0;

        if (_cropObjectDictionary.ContainsKey(plot.PlotUniqueId))
        {
            _cropObjectDictionary[plot.PlotUniqueId].RequestDestroySelf();
            UnregisterCropObject(plot.PlotUniqueId);
        }

        return true;

    }

    public bool RequestUnlockPlot(FarmPlotModel plot)
    {
        if (plot == null)
        {
            Debug.LogWarning("존재하지 않는 밭입니다.");
            return false;
        }

        if (plot.IsUnlocked == true)
        {
            Debug.LogWarning("이미 해금된 밭입니다.");
            return false;
        }

        plot.IsUnlocked = true;

        return true;
    }

    public void OnCropGrowthChanged(int instanceId, string cropDataId, int currentStage, int growthMinutes)
    {

    }

}
