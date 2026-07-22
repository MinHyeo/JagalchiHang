using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestFarm : MonoBehaviour
{

    private FarmPlotModel _testPlotModel;
    private FarmManager _farmManager;

    private void OnEnable()
    {

        FarmManager.OnFarmPlotsSpawned += OnFarmPlotsReady;
    }

    private void OnDisable()
    {

        FarmManager.OnFarmPlotsSpawned -= OnFarmPlotsReady;
    }

    private void OnFarmPlotsReady()
    {
        if (NetworkManager.Instance == null)
        {
            return;
        }

        _farmManager = NetworkManager.Instance.FarmService.GetFarmViewModel().GetFarmManager();
        if (_farmManager == null)
        {
            return;
        }

        _testPlotModel = new FarmPlotModel();
        _testPlotModel.PlotUniqueId = 1;
        _testPlotModel.IsUnlocked = false;
        _testPlotModel.IsPlanted = false;

        _farmManager.AddFarmPlot(_testPlotModel);
        Debug.Log("밭 해금:2/ 심기: 3/ 수확: 4");
    }


    private void Update()
    {

        if (_farmManager == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bool result = _farmManager.RequestUnlockNextPlot();
            if (result)
            {
                Debug.Log("밭 해금");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bool result = _farmManager.RequestPlantCrop(_testPlotModel, "Crop_Carrot");
            if (result)
            {
               
                Debug.Log("심기 완료");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bool result = _farmManager.RequestHarvestCrop(_testPlotModel);
            if (result)
            {
                Debug.Log("수확 완료");
            }
        }



    }
}
