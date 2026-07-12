using UnityEngine;
using System;

public class CropObject : MonoBehaviour
{
    private int _currentStage;
    private string _cropDataId;
    private FarmPlot _ownerPlot;

    private void Start()
    {
        _ownerPlot = GetComponentInParent<FarmPlot>();
    }

    public void InitCrop(string cropDataId, int currentStage)
    {
        _cropDataId = cropDataId;
        _currentStage = currentStage;
    }

    public void RequestDestroySelf()
    {
        if (_ownerPlot != null)
        {
            _ownerPlot.OnCropRequestDestroy(this.gameObject);
        }
    }


}
