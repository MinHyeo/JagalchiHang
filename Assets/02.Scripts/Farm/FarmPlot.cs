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

    public void SpawnCropObject(GameObject cropPrefab)
    {
        if (_currentCropObject != null)
        {
            Destroy(_currentCropObject);
        }

        _currentCropObject = Instantiate(cropPrefab, Transform_CropSpawnPoint);
    }

    public void RemoveCropObject()
    {
        Debug.Log($"RemoveCropObject 호출, _currentCropObject: {_currentCropObject}");
        if (_currentCropObject != null)
        {
            Destroy(_currentCropObject);
            _currentCropObject = null;
        }
    }

}
