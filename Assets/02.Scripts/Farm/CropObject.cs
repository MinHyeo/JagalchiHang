using System.Collections.Generic;
using UnityEngine;

public class CropObject : MonoBehaviour, ISpawnable
{
    private int _instanceId;
    private string _cropDataId;

    public void Init(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _cropDataId = dataId;
        //FarmManager.Instance.RegisterCropObject(_plotUniqueId, this);
    }

    public void RequestDestroySelf()
    {
        GameObjectManager.Instance.RequestDestroyObject(this.gameObject);
    }



}
