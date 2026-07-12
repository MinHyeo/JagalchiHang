using UnityEngine;

public class TestFarm : MonoBehaviour
{
    [SerializeField] private FarmPlot Plot_Test;
    [SerializeField] private GameObject Prefab_CropStage1;
    [SerializeField] private GameObject Prefab_CropStage2;
    [SerializeField] private GameObject Prefab_CropStage3;


    private FarmPlotModel _testPlotModel;

    private void Start()
    {
        _testPlotModel = new FarmPlotModel();
        _testPlotModel.PlotUniqueId = 1;
        _testPlotModel.IsUnlocked = false;
        _testPlotModel.IsPlanted = false;

        Plot_Test.InitPlot(_testPlotModel.PlotUniqueId);

        Debug.Log("테스트 준비 - 2: 해금/ 3: 심기/ 4: 수확");
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bool result = FarmManager.Instance.RequestUnlockPlot(_testPlotModel);
            if (result)
            {
                Plot_Test.ActivatePlot();
                Debug.Log("밭 해금");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bool result = FarmManager.Instance.RequestPlantCrop(_testPlotModel, "Crop_item1");
            if (result)
            {
                Plot_Test.SpawnCropObject(Prefab_CropStage1, "Crop_Item1", 0);
                Debug.Log("심기 완료");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bool result = FarmManager.Instance.RequestHarvestCrop(_testPlotModel);
            if (result)
            {
                Plot_Test.RemoveCropObject();
                Debug.Log("수확 완료");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _testPlotModel.CurrentGrowthStage++;
            _testPlotModel.GrowthMiniutes += 999;
            Debug.Log($"현재 성장 단계: {_testPlotModel.CurrentGrowthStage}");

            switch (_testPlotModel.CurrentGrowthStage)
            {
                case 1:
                    Plot_Test.ChangeCropObject(Prefab_CropStage1);
                    break;
                case 2:
                    Plot_Test.ChangeCropObject(Prefab_CropStage2);
                    break;
                case 3:
                    Plot_Test.ChangeCropObject(Prefab_CropStage3);
                    break;
            }
        }



    }
}
