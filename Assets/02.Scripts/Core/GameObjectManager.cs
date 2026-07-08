using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Instance;

    private Transform _rootTransform;
    private PoolManager _poolManager;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        _poolManager = new PoolManager();
        _rootTransform = this.transform;
    }

    // 예시 GameObjectManager.Instance.CreateObject("Prefab/Enemy/Enemy_01", Vector3.zero);
    public void CreateObject(string path, Vector3 spawnSpot)
    {
        CreateObjectAsync(path, spawnSpot).Forget();
    }

    private async UniTaskVoid CreateObjectAsync(string path, Vector3 spawnSpot)
    {
        GameObject prefab = await ResourceManager.Instance.LoadAsset<GameObject>(path);
        if (prefab == null)
            return;

        GameObject gameObject = _poolManager.Pop(prefab);
        if (gameObject == null)
            return;

        gameObject.transform.SetParent(_rootTransform);
        gameObject.transform.position = spawnSpot;
    }

    // 예시 GameObjectManager.Instance.RequestDestroyObject(this.gameObject);
    public void RequestDestroyObject(GameObject gameObject)
    {
        _poolManager.Push(gameObject);
    }
}
