using Cysharp.Threading.Tasks;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class NpcManager 
{
    private GameObject _battleNpc;
    private GameObject _bagNpc;

    [SerializeField] private BattleNpc battleNpc; 
    [SerializeField] private BagNpc bagNpc;


    private ITargetable _chasePlayer;

    private Monster _targetMonster;

    private Vector3 _BattleNPCSpawnPos = new Vector3(19f, 0.5f, -3f);
    private Vector3 _BagNPCSpawnPos = new Vector3(20f, 0.5f, -3f);

    public void Init(ITargetable target)
    {
        _chasePlayer = target;
        Debug.Log($"{_chasePlayer}");

    }

    public void RegisterBattleNpc(GameObject obj, BattleNpc component)
    {
        _battleNpc = obj;
        battleNpc = component;
    }


    public void RegisterBagNpc(GameObject obj, BagNpc component)
    {
        _bagNpc = obj;
        bagNpc = component;
    }


    public void SpawnBattleNpc(string npcdataId)
    {
        if (_battleNpc != null)
        {
            Debug.Log("[NpcManager] BattleNpc가 이미 존재합니다.");

            return;
        }

        Vector3 spawnPos = _BattleNPCSpawnPos;

        if (_chasePlayer != null){
            spawnPos = _chasePlayer.GetPosition() + new Vector3(1.0f, 0f, 0f);
        }

        if(TryGetNavMeshPosition(spawnPos, out Vector3 navMeshPosition, 3f))
        {
            spawnPos = navMeshPosition;
        }
        GameObjectManager.Instance.CreateObject(npcdataId, "Prefab/Npc_Battle", spawnPos);

        Debug.Log($"[NpcManager] BattleNpc 생성 요청 완료");
    }

    public void SpawnBagNpc(string npcdataId)
    {
        if(_bagNpc != null)
        {
            Debug.Log("[NpcManager] BagNpc가 이미 존재합니다.");
            return;
        }

        Vector3 spawnPos = _BagNPCSpawnPos;

        if (_chasePlayer != null) 
        {
            spawnPos = _chasePlayer.GetPosition() + new Vector3(-1.0f, 0f, 0f);
        }

        if(TryGetNavMeshPosition(spawnPos, out Vector3 navMeshPosition, 3f))
        {
            spawnPos = navMeshPosition;
        }

        GameObjectManager.Instance.CreateObject(npcdataId, "Prefab/Npc_Bag", spawnPos);

        Debug.Log($"[NpcManager] BagNpc 생성 요청 완료");
     
    }

    public void NpcUpdate()
    {
        if (_chasePlayer == null)
        {
            Debug.Log("예외 발생");
            return;
        }
            

        if(battleNpc != null)
        {
            battleNpc.UpdatePlayerPosition(_chasePlayer.GetPosition());
        }
        if(bagNpc != null)
        {
            bagNpc.UpdatePlayerPosition(_chasePlayer.GetPosition());
        }

    }

    public void ChangeBattleMode(BattleMode battleMode) //BattleNpc로 전달
    {
        if(battleNpc != null)
        {
            battleNpc.SetBattleMode(battleMode);
            Debug.Log($"[NpcManager] 배틀 Npc에게 새로운 전투 모드 {battleMode} 전달 ");
        }
    }
    public void OnBunkerData(bool isInBunker) // 게임매니저한테 추후 전달 받을 곳 
    {
        if (_chasePlayer == null)
        {
            return;
        }

        Vector3 playerPos = _chasePlayer.GetPosition();

        Vector3 battleNpcPos = playerPos + new Vector3(1.0f, 0f, 1.0f);
        Vector3 bagNpcPos = playerPos + new Vector3(-1.0f, 0f, 1.0f);
        
        /*NPC를 순간 이동 시키기 전에 순간 이동할 곳이 NavMesh 바닥 위가 맞는지 검사하고 
        안전한 위치로 보정해서 옮겨주는 함수(가고싶은 위치, 보정되어 나올 위치, 검색반경)*/
        
        TryGetNavMeshPosition(battleNpcPos, out battleNpcPos, (3.0f));
        TryGetNavMeshPosition(bagNpcPos, out bagNpcPos, (3.0f));

        if(battleNpc != null)
        {
            battleNpc.InOutBunkerData(isInBunker, battleNpcPos);
        }

        if(bagNpc != null)
        {
            bagNpc.InOutBunkerData(isInBunker, bagNpcPos);
        }
  

        Debug.Log($"[NPC 매니저] 벙커 진입");
    }



    // NavMesh 위치 찾았는지 여부 함수
    private bool TryGetNavMeshPosition(Vector3 desiredPosition, out Vector3 navMeshPosition, float maxDistance)
    {
        // NavMesh 위치 찾기
        if(NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            navMeshPosition = hit.position;
            Debug.DrawLine(desiredPosition, navMeshPosition, Color.red);

            return true;
        }

        navMeshPosition = desiredPosition;
        return false;
    }

    public void ClearTargetMonster()
    {
        _targetMonster = null;
    }

    // 플레이어가 공격한 몬스터 객체 세팅
    public void SetTargetMonster(Monster targetMonster)
    {
        if (targetMonster == null) return;

        _targetMonster = targetMonster;
    }

    // 타겟 몬스터 객체 반환
    public Monster GetTargetMonster()
    {
        return _targetMonster;
    }


}
