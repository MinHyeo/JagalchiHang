using UnityEngine;

public class TestFarm : MonoBehaviour
{
    [SerializeField] private FarmPlot Plot_Test;
    [SerializeField] private GameObject Prefab_TestCrop;

    private FarmPlotModel _testPlotModel;

    private void Start()
    {
        _testPlotModel = new FarmPlotModel();
        _testPlotModel.PlotUniqueId = 1;
        _testPlotModel.IsUnlocked = false;
        _testPlotModel.IsPlanted = false;

        Plot_Test.InitPlot(_testPlotModel.PlotUniqueId);

        Debug.Log("테스트 준비 - 1: 해금/ 2: 심기/ 3: 수확");
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
            bool result = FarmManager.Instance.RequestPlantCrop(_testPlotModel, "test_crop");
            if (result)
            {
                Plot_Test.SpawnCropObject(Prefab_TestCrop);
                Debug.Log("심기 완료");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bool result = FarmManager.Instance.RequestHarvestCrop(_testPlotModel);
            if (result)
            {
                Plot_Test.ActivatePlot();
                Debug.Log("수확 완료");
            }
        }



    }
}
