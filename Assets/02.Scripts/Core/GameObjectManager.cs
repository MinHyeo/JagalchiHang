using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameObjectManager : SingletonBase<GameObjectManager>
{
    private static int _objectInstanceKeyGenerator = 0;

    private Transform _rootTransform;
    private PoolManager _poolManager;

    private void OnEnable()
    {
        _poolManager = new PoolManager();
        _rootTransform = this.transform;
    }

    public void CreateObject(string dataId, string path, Vector3 spawnSpot)
    {
        CreateObjectAsync(dataId, path, spawnSpot).Forget();
    }

    public async UniTaskVoid CreateObjectAsync(string dataId, string path, Vector3 spawnSpot)
    {
        GameObject prefab = await ResourceManager.Instance.LoadAsset<GameObject>(path);
        if (prefab == null)
            return;

        GameObject gameObject = _poolManager.Pop(prefab);
        if (gameObject == null)
            return;

        gameObject.transform.SetParent(_rootTransform);
        gameObject.transform.position = spawnSpot;
        AddObjectOnCreate(gameObject, dataId);
    }

    private void AddObjectOnCreate(GameObject createdObject, string dataId)
    {
        int generatedInstanceId = _objectInstanceKeyGenerator++;
        ISpawnable createdScript = createdObject.GetComponent<ISpawnable>();

        if (createdScript == null)
            return;

        createdScript.Init(generatedInstanceId, dataId);
    }

    public void RequestDestroyObject(GameObject gameObject)
    {
        _poolManager.Push(gameObject);
    }
}
