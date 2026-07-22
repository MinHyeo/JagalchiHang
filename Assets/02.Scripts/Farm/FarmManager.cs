using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmManager
{
    private FarmViewModel _viewModel;
    //private List<FarmPlotModel> _farmPlotList = new List<FarmPlotModel>();
    private Dictionary<int, List<CropObject>> _cropObjectDictionary = new Dictionary<int, List<CropObject>>();

    public static event Action OnFarmPlotsSpawned;
    public FarmManager()
    {
        Debug.Log("FarmManager 생성자 호출됨");

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.LoadData<CropData>();
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += UpdateAllPlotGrowth;
        }

        _viewModel = NetworkManager.Instance.FarmService.GetFarmViewModel();
        _viewModel.SetFarmManager(this);

        OnFarmPlotsSpawned?.Invoke();
    }

    


    public void Dispose()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged -= UpdateAllPlotGrowth;
        }
    }


    private void UpdateAllPlotGrowth()
    {
        if (_viewModel == null)
        {
            return;
        }


        for (int i = 0; i < _viewModel.FarmPlotList.Count; i++)
        {
            var plot = _viewModel.FarmPlotList[i];

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
                OnPlotGrowthChanged(plot).Forget();
            }
        }

    }

    private async UniTask OnPlotGrowthChanged(FarmPlotModel plot)
    {
        Debug.Log($"OnPlotGrowthChanged 호출됨, plotId: {plot.PlotUniqueId}, stage: {plot.CurrentGrowthStage}");


        var cropData = GameDataManager.Instance.GetData<CropData>(plot.CropDataId);
        if (cropData == null)
        {
            return;
        }

        if (_viewModel.FarmPlotDictionary.ContainsKey(plot.PlotUniqueId) == false)
        {
            return;
        }

        var farmPlot = _viewModel.FarmPlotDictionary[plot.PlotUniqueId];
        var growthStageMinutes = cropData.GetGrowthStageMinutes();

        if (plot.CurrentGrowthStage >= growthStageMinutes.Count)
        {
            Debug.Log($"밭 {plot.PlotUniqueId} 수확가능");
            return;
        }

        if (_cropObjectDictionary.ContainsKey(plot.PlotUniqueId))
        {
            var cropObjects = _cropObjectDictionary[plot.PlotUniqueId];
            for (int i = 0; i < cropObjects.Count; i++)
            {
                cropObjects[i].RequestDestroySelf();
            }
            UnregisterCropObject(plot.PlotUniqueId);
        }

        string prefabPath = cropData.PrefabPath + "_" + plot.CurrentGrowthStage;
        await farmPlot.ChangeCropModel(prefabPath, plot.CropDataId);
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
        return _viewModel.FarmPlotList;
    }

    public FarmPlotModel GetFarmPlotCanBeNull(int plotUniqueId)
    {
        for (int i = 0; i < _viewModel.FarmPlotList.Count; i++)
        {
            if (_viewModel.FarmPlotList[i].PlotUniqueId == plotUniqueId)
            {
                return _viewModel.FarmPlotList[i];
            }
        }
        return null;
    }

    public void AddFarmPlot(FarmPlotModel newPlot)
    {
        if (_viewModel == null)
        {
            _viewModel = new FarmViewModel();
        }
        _viewModel.FarmPlotList.Add(newPlot);
    }

    public void RegisterCropObject(int plotUniqueId, CropObject cropObject)
    {
        if (_cropObjectDictionary.ContainsKey(plotUniqueId) == false)
        {
            _cropObjectDictionary.Add(plotUniqueId, new List<CropObject>());
        }
        _cropObjectDictionary[plotUniqueId].Add(cropObject);

    }

    public void UnregisterCropObject(int plotUniqueId)
    {
        if (_cropObjectDictionary.ContainsKey(plotUniqueId))
        {
            _cropObjectDictionary.Remove(plotUniqueId);
        }
    }

    public void RegisterFarmPlot(int plotUniqueId, FarmPlot farmPlot)
    {
        Debug.Log($"RegisterFarmPlot 호출됨, id: {plotUniqueId}");
        if (_viewModel.FarmPlotDictionary.ContainsKey(plotUniqueId) == false)
        {
            _viewModel.FarmPlotDictionary.Add(plotUniqueId, farmPlot);
        }
    }

    public void UnregisterFarmPlot(int plotUniqueId)
    {
        if (_viewModel.FarmPlotDictionary.ContainsKey(plotUniqueId))
        {
            _viewModel.FarmPlotDictionary.Remove(plotUniqueId);
        }
    }

    public bool  RequestPlantCrop(FarmPlotModel plot, string cropDataId)
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

        if (_viewModel.FarmPlotDictionary.ContainsKey(plot.PlotUniqueId) == false)
        {
            Debug.LogWarning($"밭 오브젝트를 찾을 수 없습니다.: {plot.PlotUniqueId}");
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

        var invenVm = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();
        int harvestCount = UnityEngine.Random.Range(cropData.HarvestMinCount, cropData.HarvestMaxCount +1);
        invenVm.AcquireItem(cropData.HarvestItemDataId, harvestCount);
        int seedCount = UnityEngine.Random.Range(cropData.SeedDropMinCount, cropData.SeedDropMaxCount + 1);
        invenVm.AcquireItem(cropData.SeedItemDataId, seedCount);


        plot.CropDataId = string.Empty;
        plot.IsPlanted = false;
        plot.CurrentGrowthStage = 0;
        plot.GrowthMinutes = 0;

        if (_cropObjectDictionary.ContainsKey(plot.PlotUniqueId))
        {
            var cropObjects = _cropObjectDictionary[plot.PlotUniqueId];
            for (int i = 0; i < cropObjects.Count; i++)
            {
                cropObjects[i].RequestDestroySelf();
            }
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
        if (_viewModel.FarmPlotDictionary.ContainsKey(plot.PlotUniqueId))
        {
            _viewModel.FarmPlotDictionary[plot.PlotUniqueId].ActivatePlot();
        }

        return true;
    }

    public bool RequestUnlockNextPlot()
    {
        for (int i = 0; i < _viewModel.FarmPlotList.Count; i++)
        {
            if (_viewModel.FarmPlotList[i].IsUnlocked == false)
            {
                return RequestUnlockPlot(_viewModel.FarmPlotList[i]);
            }
        }
        Debug.LogWarning("해금할 밭이 없습니다.");
        return false;
    }

    public void OnMapChanged()
    {
        OnFarmPlotsSpawned?.Invoke();
    }



}
