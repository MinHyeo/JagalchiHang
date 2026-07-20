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
        SubscribeNextFrame().Forget();

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
            Debug.Log($"RegisterCropObject 호출: {_plotUniqueId}");
            _farmManager.RegisterCropObject(_plotUniqueId, cropObject);
        }

    }


}
