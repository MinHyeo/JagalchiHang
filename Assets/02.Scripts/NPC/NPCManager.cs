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

        SpawnNpc().Forget();
    }

    public async UniTask SpawnNpc()
    {
        await SpawnBattleNpc();
        await SpawnBagNpc();
    }

    public async UniTask SpawnBattleNpc() {

        _battleNpc = await GameObjectManager.Instance.CreateObjectAsync("1", "Prefab/Npc_Battle", _BattleNPCSpawnPos);
        if(_battleNpc == null)
        {
            Debug.LogError("Battle NPC 생성 실패");
            return;
        }

        NavMeshAgent agent = _battleNpc.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        BehaviorGraphAgent behaviorGraphAgent = _battleNpc.GetComponent<BehaviorGraphAgent>();
        if (behaviorGraphAgent == null) return;

        agent.enabled = false;
        behaviorGraphAgent.enabled = false;

        // _BattleNPCSpawnPos 근처 2m 내에 NavMesh 없으면
        if (TryGetNavMeshPosition(_BattleNPCSpawnPos, out Vector3 navMeshPosition, 2f) == false)
        {
            Debug.LogError($"Battle 생성 위치 주변에 NavMesh가 없습니다. " + $"요청 위치: {_BattleNPCSpawnPos}");
            return;
        }

        _battleNpc.transform.position = navMeshPosition;

        // Transform 위치 적용 - 새 위치 바로 인식
        Physics.SyncTransforms();

        agent.enabled = true;

        if (agent.isOnNavMesh == false)
        {
            Debug.LogError($"BattleNpc가 NavMesh 위에 배치되지 않았습니다. " + $"현재 위치: {_battleNpc.transform.position}");

            return;
        }

        // Agent 위치 강제로 맞춤
        agent.Warp(navMeshPosition);
        // 이전 경로 삭제
        agent.ResetPath();
        // 현재 속도 제거
        agent.velocity = Vector3.zero;
        // 이동 정지
        agent.isStopped = true;

        battleNpc = _battleNpc.GetComponent<BattleNpc>();

        if (battleNpc == null)
        {
            Debug.LogError("BattleNpc 컴포넌트가 없습니다.");
            return;
        }

        Debug.Log(
            $"[NpcManager] BattleNpc 생성 완료\n" +
            $"요청 위치: {_BattleNPCSpawnPos}\n" +
            $"NavMesh 위치: {navMeshPosition}\n" +
            $"실제 위치: {_battleNpc.transform.position}"
        );

        // 이동 허용
        agent.isStopped = false;
        behaviorGraphAgent.enabled = true;
    }

    private async UniTask SpawnBagNpc()
    {
        _bagNpc = await GameObjectManager.Instance.CreateObjectAsync("2", "Prefab/Npc_Bag", _BagNPCSpawnPos);
        if (_bagNpc == null)
        {
            Debug.LogError("Bag NPC 생성 실패");
            return;
        }

        NavMeshAgent agent = _bagNpc.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        BehaviorGraphAgent behaviorGraphAgent = _bagNpc.GetComponent<BehaviorGraphAgent>();
        if (behaviorGraphAgent == null) return;

        agent.enabled = false;
        behaviorGraphAgent.enabled = false;

        // _BattleNPCSpawnPos 근처 2m 내에 NavMesh 없으면
        if (TryGetNavMeshPosition(_BagNPCSpawnPos, out Vector3 navMeshPosition, 2f) == false)
        {
            Debug.LogError($"BagNpc 생성 위치 주변에 NavMesh가 없습니다. " + $"요청 위치: {_BagNPCSpawnPos}");
            return;
        }

        _bagNpc.transform.position = navMeshPosition;

        // Transform 위치 적용 - 새 위치 바로 인식
        Physics.SyncTransforms();

        agent.enabled = true;

        if (agent.isOnNavMesh == false)
        {
            Debug.LogError($"BattleNpc가 NavMesh 위에 배치되지 않았습니다. " + $"현재 위치: {_bagNpc.transform.position}");

            return;
        }

        // Agent 위치 강제로 맞춤
        agent.Warp(navMeshPosition);
        // 이전 경로 삭제
        agent.ResetPath();
        // 현재 속도 제거
        agent.velocity = Vector3.zero;
        // 이동 정지
        agent.isStopped = true;

        bagNpc = _bagNpc.GetComponent<BagNpc>();

        if (bagNpc == null)
        {
            Debug.LogError("BattleNpc 컴포넌트가 없습니다.");
            return;
        }

        Debug.Log(
            $"[NpcManager] BattleNpc 생성 완료\n" +
            $"요청 위치: {_BagNPCSpawnPos}\n" +
            $"NavMesh 위치: {navMeshPosition}\n" +
            $"실제 위치: {_bagNpc.transform.position}"
        );

        // 이동 허용
        agent.isStopped = false;
        behaviorGraphAgent.enabled = true;
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
