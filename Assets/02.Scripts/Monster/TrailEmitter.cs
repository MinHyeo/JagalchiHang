using UnityEngine;

public class TrailEmitter : MonoBehaviour
{
    [SerializeField] private string _trailMarkerPrefabPath;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _groundCheckDistance = 5f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _trailSpawnPoint;

    private IMonsterMoveable _moveable;
    private float _timeSinceLastSpawn;

    private void Awake()
    {
        _moveable = GetComponent<IMonsterMoveable>();
    }

    private void Update()
    {
        //if (!_moveable.IsMoving)
        //{
        //    return;
        //}

        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn < _spawnInterval)
        {
            return;
        }

        SpawnTrailMarker();

        _timeSinceLastSpawn = 0f;
    }

    private void SpawnTrailMarker()
    {
        if (string.IsNullOrEmpty(_trailMarkerPrefabPath))
        {
            return;
        }

        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"{name} : GameObjectManager.Instance가 null입니다. 씬에 GameObjectManager가 있는지 확인요망.");
            return;
        }

        Vector3 trailspawnPos = _trailSpawnPoint.position;

        if (Physics.Raycast(trailspawnPos, Vector3.down, out RaycastHit hit, _groundCheckDistance, _groundLayer))
        {
            trailspawnPos = hit.point;
        }

        // 추후 _tralilMarkerPrefabPath에 Addressables 주소 넣기추가
        GameObjectManager.Instance.CreateObject("sss", _trailMarkerPrefabPath, trailspawnPos);
    }
}
