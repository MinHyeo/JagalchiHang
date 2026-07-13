using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField] private string _monsterPrefabPath;
    [SerializeField] private string _monsterId;
    [SerializeField] private bool _spawnOnSceneStart = true;

    public string MonsterPrefabPath
    {
        get { return _monsterPrefabPath; }
    }

    public string MonsterId
    {
        get { return _monsterId; }
    }

    public bool SpawnOnSceneStart
    {
        get { return _spawnOnSceneStart; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
    }

    private void OnEnable()
    {
        SpawnPointRegistry.Register(this);
    }

    private void OnDisable()
    {
        SpawnPointRegistry.Unregister(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
}
