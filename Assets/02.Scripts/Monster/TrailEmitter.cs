using UnityEngine;
using UnityEngine.AI;

public class TrailEmitter : MonoBehaviour
{
    [SerializeField] private string _trailMarkerPrefabPath;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _groundCheckDistance = 5f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] float _navMeshSnapRadius = 1f;

    private PlayerController _playerController;
    private float _timeSinceLastSpawn;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        bool isMoving = _playerController.IsWalking || _playerController.IsRunning;

        if (!isMoving)
        {
            return;
        }

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

        Vector3 spawnPosition = transform.position;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheckDistance, _groundLayer))
        {
            spawnPosition = hit.point;
        }

        NavMeshHit navMeshHit;

        if (!NavMesh.SamplePosition(spawnPosition, out navMeshHit, _navMeshSnapRadius, NavMesh.AllAreas))
        {
            Debug.Log($"{name} : 흔적 생성 위치가 NavMesh 밖이라 스킵함 (위치 {spawnPosition})");
            return;
        }

        // 추후 _tralilMarkerPrefabPath에 Addressables 주소 넣기추가
        GameObjectManager.Instance.CreateObject(_trailMarkerPrefabPath, navMeshHit.position);
    }
}
