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

    public bool HasBattleNpc => _battleNpc != null || battleNpc != null;
    public bool HasBagNpc => _bagNpc != null || bagNpc != null;

    private ITargetable _chasePlayer;

    private Monster _targetMonster;

    private Vector3 _BattleNPCSpawnPos = new Vector3(19f, 0.5f, -3f);
    private Vector3 _BagNPCSpawnPos = new Vector3(20f, 0.5f, -3f);

    private bool _isInBunker = false;
    private int _energyUse= 1; //파밍 맵에서 10분당 NPC 소모 에너지 
    private int _energyCharge = 2; // 벙커 안에서 10분당 NPC 충전 에너지 

    private int _minuteCount = 0; //10분 측정하기 위한 

    private NpcViewModel _viewModel;

    public void Init(ITargetable target)
    {
        _chasePlayer = target;
        Debug.Log($"{_chasePlayer}");

        if(TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += HandleMinuteChange;
        }
        _viewModel = NetworkManager.Instance.NpcService.GetNpcViewModel();
    }

    private void HandleMinuteChange()
    {
        _minuteCount = _minuteCount + 1;

        if(_minuteCount < 10)
        {
            return;
        }

        _minuteCount = 0;

        if (_isInBunker == true)
        {
            if (battleNpc != null)
            {
                battleNpc.ChargeEnergy(_energyCharge);
            }

            if (bagNpc != null)
            {
                bagNpc.ChargeEnergy(_energyCharge);
            }
        }

        else
        {
            if (battleNpc != null)
            {
                battleNpc.UseEnergy(_energyUse);

            }

            if (bagNpc != null)
            {
                bagNpc.UseEnergy(_energyUse);
            }
        }
    }

    public async UniTaskVoid SpawnBattleNpc(string npcdataId) {

        if(_battleNpc !=  null)
        {
            Debug.LogError("Battle NPC가 이미 존재합니다.");
            return;
        }

        Vector3 spawnPos = _BattleNPCSpawnPos;

        if (_chasePlayer != null)
        {
            spawnPos = _chasePlayer.GetPosition() + new Vector3(1.0f, 0f, 0f);

        }
            _battleNpc = await GameObjectManager.Instance.CreateObjectAsync(npcdataId, "Prefab/Npc_Battle", spawnPos);
       

        if(_battleNpc == null)
        {
            Debug.LogError("Battle Npc 생성 실패");
        }

        NavMeshAgent agent = _battleNpc.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        BehaviorGraphAgent behaviorGraphAgent = _battleNpc.GetComponent<BehaviorGraphAgent>();
        if (behaviorGraphAgent == null) return;

        agent.enabled = false;
        behaviorGraphAgent.enabled = false;

        // spawnPos 근처 2m 내에 NavMesh 없으면
        if (TryGetNavMeshPosition(spawnPos, out Vector3 navMeshPosition, 2f) == false)
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
        _viewModel.UnlockedNpcIds.Add(npcdataId);

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

        BunkerStateBattleNpc(); //스폰 시점에 벙커 상태 반영

        _viewModel.UnlockedNpcIds.Add(npcdataId);
    }

    public void TransNpcPosition(Vector3 transPosition)
    {

        Vector3 battleNpcPos = transPosition + new Vector3(1.0f, 0f, 0f);
        Vector3 bagNpcPos = transPosition + new Vector3(-1.0f, 0f, 0f);

        TryGetNavMeshPosition(battleNpcPos, out battleNpcPos, 3.0f);
        TryGetNavMeshPosition(bagNpcPos, out bagNpcPos, 3.0f);

        battleNpc.ChangeNpcPosition(transPosition);
        bagNpc.ChangeNpcPosition(transPosition);

    }

    public async UniTaskVoid SpawnBagNpc(string npcdataId)
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
            _bagNpc = await GameObjectManager.Instance.CreateObjectAsync(npcdataId, "Prefab/Npc_Bag", spawnPos);
        

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

        // spawnPos 근처 2m 내에 NavMesh 없으면
        if (TryGetNavMeshPosition(spawnPos, out Vector3 navMeshPosition, 2f) == false)
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
        _viewModel.UnlockedNpcIds.Add(npcdataId);

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
        _isInBunker = isInBunker;

        if (_chasePlayer == null)
        {
            return;
        }

        BunkerStateBattleNpc();
        BunkerStateBagNpc();

        Debug.Log($"[NPC 매니저] 벙커 진입");
    }

    private void BunkerStateBattleNpc()
    {
        if(battleNpc == null || _chasePlayer == null)
        {
            return;
        }

        Vector3 playerPos = _chasePlayer.GetPosition();
        Vector3 battleNpcPos = playerPos + new Vector3(1.0f, 0f, 1.0f);

        TryGetNavMeshPosition(battleNpcPos, out battleNpcPos, (3.0f));

        battleNpc.InOutBunkerData(_isInBunker, battleNpcPos);

    }

    private void BunkerStateBagNpc()
    {
        if(bagNpc == null || _chasePlayer == null)
        {
            return;
        }

        Vector3 playerPos = _chasePlayer.GetPosition();
        Vector3 bagNpcPos = playerPos + new Vector3(-1.0f, 0f, 1.0f);

        TryGetNavMeshPosition(bagNpcPos, out bagNpcPos, (3.0f));

        bagNpc.InOutBunkerData(_isInBunker, bagNpcPos);

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
