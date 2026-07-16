using UnityEngine;

public class PlayerOilController : MonoBehaviour
{
    [SerializeField] private Transform _oilSpawnPoint;

    private void OnEnable()
    {
        TimeManager.Instance.OnMinuteChanged += OnMinuteChanged;
    }

    private void OnMinuteChanged()
    {
        if(TimeManager.Instance.Minute % 5 == 0)
        {
            CreateOil();
        }
    }

    private void CreateOil()
    {
        Vector3 oilPos = _oilSpawnPoint.position;
        oilPos.y = 0f;

        GameObjectManager.Instance.CreateObject("zzz", "Prefab/Oil", oilPos);
    }
}
