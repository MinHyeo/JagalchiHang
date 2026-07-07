using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Instance;

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
    }

    public async UniTaskVoid CreateObject(string path, Vector3 spawnSpot)
    {
        GameObject prefab = await ResourceManager.Instance.LoadAsset<GameObject>(path);
        if (prefab == null)
            return;

        GameObject gameObject = _poolManager.Pop(prefab);
        if (gameObject == null)
            return;

        gameObject.transform.position = spawnSpot;
    }

    public void RequestDestroyObject(GameObject gameObject)
    {
        _poolManager.Push(gameObject);
    }
}
