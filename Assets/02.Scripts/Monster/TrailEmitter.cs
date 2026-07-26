using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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

    private List<GameObject> _trailMarkerList = new();

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

        var mapManager =  GameUtil.GetMapManager();
        if (mapManager == null) return;

        if (mapManager.CurrentMapType == MapType.ParmingMap)
        {
            _timeSinceLastSpawn += Time.deltaTime;

            if (_timeSinceLastSpawn < _spawnInterval)
            {
                return;
            }

            _timeSinceLastSpawn = 0f;
            SpawnTrailMarker().Forget();
        }
        else if (mapManager.CurrentMapType == MapType.ParkingGarage)
        {
            ClearTrailMarkerList();
            return;
        }
    }

    private async UniTaskVoid SpawnTrailMarker()
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
        GameObject trailMarker = await GameObjectManager.Instance.CreateObjectAsync("sss", _trailMarkerPrefabPath, trailspawnPos);
        if (trailMarker == null) return;

        _trailMarkerList.Add(trailMarker);
    }

    private void ClearTrailMarkerList()
    {
        foreach(var trailMarker in _trailMarkerList)
        {
            if(trailMarker != null)
            {
                DestroyImmediate(trailMarker);
            }
        }

        _trailMarkerList.Clear();
    }
}
