using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private GameObject Object_PlotSet;
    [SerializeField] private Transform Transform_CropSpawnPoint;

    private int _plotUniqueId;
    private GameObject _currentCropObject;

    private void Awake()
    {
        Object_PlotSet.SetActive(false);
    }

    public void InitPlot(int plotUniqueId)
    {
        _plotUniqueId = plotUniqueId;
    }

    public void ActivatePlot()
    {
        Object_PlotSet.SetActive(true);
    }

    public void SpawnCropObject(GameObject cropPrefab, string cropDataId, int stage)
    {
        var gObj = Instantiate(cropPrefab, Transform_CropSpawnPoint);
        _currentCropObject = gObj;

        var cropObject = gObj.GetComponent<CropObject>();

        if (cropObject != null)
        {
            cropObject.InitCrop(cropDataId, stage);
        }
    }

    public void OnCropRequestDestroy(GameObject cropObject)
    {
        //GameObjectManager.Instance.RequestDestroyObject(cropObject);
        //ISpawnable  머지 후 GameObject6Manager로 교체


        Destroy(cropObject);
    }

    public void RemoveCropObject()
    {
        Debug.Log($"RemoveCropObject: _currentCropObject = {_currentCropObject}");
        
        if (_currentCropObject != null)
        {
            Destroy(_currentCropObject);
            _currentCropObject = null;
        }
    }

    public void ChangeCropObject(GameObject newCropPrefab)
    {
        RemoveCropObject();
        _currentCropObject = Instantiate(newCropPrefab, Transform_CropSpawnPoint);
    }
    
}
