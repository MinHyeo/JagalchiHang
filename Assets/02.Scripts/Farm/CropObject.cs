using System.Collections.Generic;
using UnityEngine;

public class CropObject : MonoBehaviour
{
    private int _instanceId;
    private int _currentStage;
    private string _cropDataId;
    private int _growthMinutes;

    public void Init(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _cropDataId = dataId;
        _currentStage = 0;
        _growthMinutes = 0;
        //FarmManager.Instance.RegisterCropObject(_plotUniqueId, this);
    }
    private void OnEnable()
    {
        Debug.Log($"CropObject OnEnable, TimeManager: {TimeManager.Instance}");
        
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += UpdateGrowth;
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged -= UpdateGrowth;
        }

    }

    private void UpdateGrowth()
    {
        Debug.Log("UpdateGrowth 호출됨");
        Debug.Log($"cropDataid: {_cropDataId}");

        if (string.IsNullOrEmpty(_cropDataId))
        {
            return;
        }

        var cropData = GameDataManager.Instance.GetData<CropData>(_cropDataId);
        if (cropData == null)
        {
            return;
        }

        var growthStageMinutes = cropData.GetGrowthStageMinutes();

        if (_currentStage >= growthStageMinutes.Count)
        {
            return;
        }

        _growthMinutes++;

        int newStage = CalculateGrowthStage(growthStageMinutes);

        if (newStage != _currentStage)
        {
            _currentStage = newStage;
            FarmManager.Instance.OnCropGrowthChanged(_instanceId, _cropDataId, _currentStage, _growthMinutes);

            if (_currentStage >= growthStageMinutes.Count)
            {
                Debug.Log("작물이 다 자랐습니다. 수확 가능");
                return;
            }

            //string nextPrefabPath = cropData.PrefabPath + "_" + _currentStage;
            //GameObjectManager.Instance.CreateObject(_cropDataId, nextPrefabPath, this.transform.position);
            //GameObjectManager.Instance.RequestDestroyObject(this.gameObject);

            Debug.Log($"성장 단계: {_currentStage}");
        }


    }

    private int CalculateGrowthStage(List<int> growthStageMinutes)
    {
        int cropGrowthTotalMinutes = 0;
        
        for (int i = 0; i < growthStageMinutes.Count; i++)
        {
            cropGrowthTotalMinutes += growthStageMinutes[i];
            
            if (_growthMinutes < cropGrowthTotalMinutes)
            {
                return i;
            }
        }

        return growthStageMinutes.Count;
    }

    public void RequestDestroySelf()
    {
        GameObjectManager.Instance.RequestDestroyObject(this.gameObject);
    }



}
