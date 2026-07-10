using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

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


    }


    public bool RequestPlantCrop(FarmPlotModel plot, string cropDataId)
    {
        if (plot == null)
        {
            Debug.LogWarning("존재하지 않는 밭입니다.");
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

        //var cropData = GameDataManager.Instance.GetData<CropData>(cropDataId);
        //if (cropData == null)
        //{
        //    Debug.LogWarning($"작물을 찾을 수 없습니다.: {cropDataId}");
        //    return false;
        //}

        plot.CropDataId = cropDataId;
        plot.IsPlanted = true;
        plot.GrowthMinutes = 0;
        plot.CurrentGrowthStage = 0;

        return true;
    }

    public int CalculateGrowthStage(FarmPlotModel plot)
    {
        if (plot.IsPlanted == false)
        {
            return 0;
        }

        //var cropData = GameDataManager.Instance.GetData<CropData> (plot.CropDataId);
        //if (cropData == null)
        //{
        //    return 0;
        //}

        //int cropGrowthTotalMinutes = 0;

        //for (int i = 0; i < cropData.GrowthStageMinutesList.Count; i++)
        //{
        //    cropGrowthTotalMinutes += cropData.GrowthStageMinutesList[i];

        //    if (plot.GrowthMinutes < cropGrowthTotalMinutes)
        //    {
        //        return i;
        //    }
        //}

        //return cropData.GrowthStageMinutesList.Count;

        return plot.CurrentGrowthStage;
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

        int currentStage = CalculateGrowthStage(plot);
        //var cropData = GameDataManager.Instance.GetData<CropData>(plot.CropDataId);
        //if (cropData == null)
        //{
        //    Debug.LogWarning("작물 데이터를 찾을 수 없습니다.");
        //    return false;
        //}

        //if (currentStage < cropData.GrowthStageMinutesList.Count)
        //{
        //    Debug.LogWarning("아직 다 자라지 않았습니다.");
        //    return false;
        //}

        // 작물 아이템 지급 (인벤 연동 후에)
        //int harvestCount = Random.Range(cropData.HarvestMinCount, cropData.HarvestMaxCount + 1);

        // 씨앗 아이템 지급 (인벤 연동 후)
        // int seedCount = Random.Range(cropData.SeedDropMinCount,cropData.SeedDropMaxCount + 1);


        plot.CropDataId = string.Empty;
        plot.IsPlanted = false;
        plot.CurrentGrowthStage = 0;

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


}
