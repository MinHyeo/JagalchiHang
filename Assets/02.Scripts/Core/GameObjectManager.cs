using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameObjectManager : SingletonBase<GameObjectManager>
{
    private Transform _rootTransform;
    private PoolManager _poolManager;

    private void OnEnable()
    {
        _poolManager = new PoolManager();
        _rootTransform = this.transform;
    }

    public void CreateObject(string path, Vector3 spawnSpot)
    {
        CreateObjectAsync(path, spawnSpot).Forget();
    }

    public async UniTaskVoid CreateObjectAsync(string path, Vector3 spawnSpot)
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

    public void RequestDestroyObject(GameObject gameObject)
    {
        _poolManager.Push(gameObject);
    }
}
