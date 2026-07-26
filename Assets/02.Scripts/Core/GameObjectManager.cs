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

    public async UniTask<GameObject> CreateObjectAsync(string dataId, string path, Vector3 spawnSpot)
    {
        Debug.Log($"CreateObjectAsync 호출됨, path: {path}");
        GameObject prefab = await ResourceManager.Instance.LoadAsset<GameObject>(path);
        Debug.Log($"프리팹 로드 결과: {prefab}");



        if (prefab == null)
            return null;

        GameObject gameObject = _poolManager.Pop(prefab);
        if (gameObject == null)
            return null;

        gameObject.transform.SetParent(_rootTransform);
        gameObject.transform.position = spawnSpot;
        AddObjectOnCreate(gameObject, dataId);

        return gameObject;
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
