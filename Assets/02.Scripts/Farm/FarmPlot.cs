using Cysharp.Threading.Tasks;
using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private GameObject Object_PlotSet;
    [SerializeField] private Transform Transform_CropSpawnPoint;
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

    public Vector3 GetSpawnPosition()
    {
        return Transform_CropSpawnPoint.position;
    }

    public async UniTask ChangeCropModel(string prefabPath, string cropDataId)
    {
        var gObj = await GameObjectManager.Instance.CreateObjectAsync(cropDataId, prefabPath, Transform_CropSpawnPoint.position);
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
