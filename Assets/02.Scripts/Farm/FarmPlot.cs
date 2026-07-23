using Cysharp.Threading.Tasks;
using UnityEngine;

public class FarmPlot : MonoBehaviour, IInteractionable
{
    [SerializeField] private GameObject Object_PlotSet;
    [SerializeField] private Transform[] Transform_CropSpawnPoints;
    [SerializeField] private int _plotUniqueId;

    private FarmManager _farmManager;

    private void Awake()
    {
        Object_PlotSet.SetActive(false);
    }

    private void OnEnable()
    {
        FarmManager.OnFarmPlotsSpawned += OnFarmManagerReady;
    }

    private void OnDisable()
    {
        FarmManager.OnFarmPlotsSpawned -= OnFarmManagerReady;
    }

    private void OnFarmManagerReady()
    {
        Debug.Log("OnFarmManagerReady 호출됨");
        _farmManager = NetworkManager.Instance.FarmService.GetFarmViewModel().GetFarmManager();

        var newPlot = new FarmPlotModel();
        newPlot.PlotUniqueId = _plotUniqueId;
        newPlot.IsUnlocked = false;
        newPlot.IsPlanted = false;
        _farmManager.AddFarmPlot(newPlot);

        _farmManager.RegisterFarmPlot(_plotUniqueId, this);
    }

    public void ActivatePlot()
    {
        Object_PlotSet.SetActive(true);
    }


    public void Interaction(Transform transform)
    {
        Debug.Log($"FarmPlot Interaction 호출됨");
        var plot = _farmManager.GetFarmPlotCanBeNull(_plotUniqueId);
        if (plot == null)
        {
            return;
        }

        if (plot.IsUnlocked == false)
        {
            _farmManager.RequestUnlockNextPlot();
            return;
        }

        if (plot.IsPlanted == true)
        {
            _farmManager.RequestHarvestCrop(plot);
            return;
        }

        // UIManager.Instance.OpenFarmUI();
    }


    public Vector3 GetSpawnPosition()
    {
        return Transform_CropSpawnPoints[0].position;
    }

    public async UniTask ChangeCropModel(string prefabPath, string cropDataId)
    {
        Debug.Log($"ChangeCropModel 호출됨, path: {prefabPath}");

        for (int i = 0; i < Transform_CropSpawnPoints.Length; i++)
        {
            var gObj = await GameObjectManager.Instance.CreateObjectAsync(cropDataId, prefabPath, Transform_CropSpawnPoints[i].position);
            Debug.Log($"생성된 오브젝트: {gObj}");
            if (gObj == null)
            {
                return;
            }

            var cropObject = gObj.GetComponent<CropObject>();
            if (cropObject != null)
            {
                _farmManager.RegisterCropObject(_plotUniqueId, cropObject);
            }
        }

       

    }


}
