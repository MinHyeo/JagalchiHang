using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestFarm : MonoBehaviour
{
    [SerializeField] private FarmPlot Plot_Test;

    private FarmPlotModel _testPlotModel;
    private FarmManager _farmManager;

    private void OnEnable()
    {

        SubscribeNextFrame().Forget();

        //Debug.Log($"TestFarm OnEnable, NetworkManager_re.Inst: {NetworkManager_re.Inst}");
        //if (NetworkManager_re.Inst != null)
        //{
        //    NetworkManager_re.Inst.OnFarmSpawnDataReceived += OnFarmViewModelReceived;

        //}
    }

    private async UniTaskVoid SubscribeNextFrame()
    {
        await UniTask.NextFrame();
        if (NetworkManager_re.Inst != null)
        {
            NetworkManager_re.Inst.OnFarmSpawnDataReceived += OnFarmViewModelReceived;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager_re.Inst != null)
        {
            NetworkManager_re.Inst.OnFarmSpawnDataReceived -= OnFarmViewModelReceived;
        }
    }

    private void OnFarmViewModelReceived(FarmViewModel viewModel)
    {
        _farmManager = viewModel.GetFarmManager();
        Debug.Log($"_farmManager: {_farmManager}");

        _testPlotModel = new FarmPlotModel();
        _testPlotModel.PlotUniqueId = 1;
        _testPlotModel.IsUnlocked = false;
        _testPlotModel.IsPlanted = false;

        _farmManager.AddFarmPlot(_testPlotModel);
        Debug.Log("밭 해금: 2/ 심기: 3/ 수확: 4");
    }

    private void Update()
    {

        if (_farmManager == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bool result = _farmManager.RequestUnlockPlot(_testPlotModel);
            if (result)
            {
                Plot_Test.ActivatePlot();
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
