using System;
using UnityEditor.Build.Pipeline.Tasks;
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

        var cropData = GameDataManager.Instance.GetData<CropData>();
        if (cropData == null)
        {
            Debug.LogWarning($"작물을 찾을 수 없습니다.: {cropDataId}");
            return false;
        }

        plot.CropDataId = cropDataId;
        plot.IsPlanted = true;
        plot.PlantedTimeStampTicks = DateTime.UtcNow.Ticks;
        plot.CurrentGrowthStage = 0;

        return true;
    }

    public int CalculateGrowthStage(FarmPlotModel plot)
    {
        if (plot.IsPlanted == false)
        {
            return 0;
        }

        var cropData = GameDataManager.Instance.GetData<CropData> ();
        if (cropData == null)
        {
            return 0;
        }

        // TimeManager연동 후 시간 기반 성장 단계 구현 예정

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
        var cropData = GameDataManager.Instance.GetData<CropData>();
        if (cropData == null)
        {
            Debug.LogWarning("작물 데이터를 찾을 수 없습니다.");
            return false;
        }

        if (currentStage < cropData.GrowthStageCount)
        {
            Debug.LogWarning("아직 다 자라지 않았습니다.");
            return false;
        }

        // 작물 아이템 지급 (인벤 연동 후에)
        //int harvestCount = Random.Range(cropData.HarvestMinCount, cropData.HarvestMaxCount + 1);

        // 씨앗 아이템 지급 (인벤 연동 후)
        // int seedCount = Random.Range(cropData.SeedDropMinCount,cropData.SeedDropMaxCount + 1);


        plot.CropDataId = string.Empty;
        plot.IsPlanted = false;
        plot.CurrentGrowthStage = 0;

        return true;

    }

}
