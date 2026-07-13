using UnityEngine;

public class TestFarm : MonoBehaviour
{
    [SerializeField] private FarmPlot Plot_Test;
    [SerializeField] private GameObject Prefab_CropStage0;
    [SerializeField] private GameObject Prefab_CropStage1;
    [SerializeField] private GameObject Prefab_CropStage2;


    private FarmPlotModel _testPlotModel;
    private CropObject _currentrCrop;

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
                var gObj = Instantiate(Prefab_CropStage0, Plot_Test.transform);
                _currentrCrop = gObj.GetComponent<CropObject>();
                if (_currentrCrop != null)
                {
                    _currentrCrop.Init(0, "Crop_item1");
                }
                Debug.Log("심기 완료");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bool result = FarmManager.Instance.RequestHarvestCrop(_testPlotModel);
            if (result)
            {
                if (_currentrCrop != null)
                {
                    Destroy(_currentrCrop.gameObject);
                    _currentrCrop = null;
                }
                Debug.Log("수확 완료");
            }
        }




    }
}
