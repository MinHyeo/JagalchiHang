using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private GameObject Object_PlotSet;
    [SerializeField] private Transform Transform_CropSpawnPoint;

    private int _plotUniqueId;

    private void Awake()
    {
        Object_PlotSet.SetActive(false);
    }

    public void InitPlot(int plotUniqueId)
    {
        _plotUniqueId = plotUniqueId;
        FarmManager.Instance.RegisterFarmPlot(_plotUniqueId, this);
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
            FarmManager.Instance.RegisterCropObject(_plotUniqueId, cropObject);
        }

    }


}
