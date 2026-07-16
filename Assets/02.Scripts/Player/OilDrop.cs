using UnityEngine;

public class OilDrop : MonoBehaviour
{
    // [나라]TODO 
    [SerializeField] private float _maxLifeTime = 10f;   // 최대 유지 시간

    private float _elapsedTime;     // 현재까지 지난 시간

    private void OnEnable()
    {
        _elapsedTime = 0f;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if(_elapsedTime >= _maxLifeTime)
        {
            GameObjectManager.Instance.RequestDestroyObject(gameObject);
        }
    }
}
