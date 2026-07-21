using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BT_FollowPlayer", story: "[Self] Follow[PlayerPosition]", category: "Action", id: "107259c8e0a53a3ea69149a2704dc9cb")]
public partial class BT_FollowPlayer : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> PlayerPosition;
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    [SerializeReference] public BlackboardVariable<NpcState> CurrentState;

    [SerializeReference] public BlackboardVariable<bool> IsInBunker;

    [SerializeReference] public BlackboardVariable<BattleMode> CurrentBattleMode;

    private NavMeshAgent _agent;
    private EnemySensor _sensor;


    protected override Status OnStart()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>(); //자기 자신에서 NavMesh 컴포넌트 가져오기 
        _sensor = Self.Value.GetComponentInChildren<EnemySensor>();

        CurrentState.Value = NpcState.Chase; //추적 상태로 변경

        EnemyTarget.Value = null;

        if (_sensor != null)
        {
            _sensor.ClearTarget();
        }
        _agent.speed = 5.0f; //NPC 이동속도 
        return Status.Running; //실행중

    }

    protected override Status OnUpdate()
    {

        if (CheckBunkerStatus() == true)
        {
            return Status.Failure;
        }

        if (CheckPlayerDistance() == true)
        {
            return Status.Running;
        }

        UpdateSensorSetting();

        if (BattleModeSetting() == true)
        {
            return Status.Success;
        }

        _agent.SetDestination(PlayerPosition.Value);

        return Status.Running;

    }

    protected override void OnEnd()
    {
        if (_agent != null && _agent.isOnNavMesh) // 컴포넌트가 존재하고 NavMesh 바닥위에 정상적으로 서있다면 
        {
            _agent.ResetPath(); // 경로 초기화 
        }
    }

    private bool CheckBunkerStatus() // 벙커 진입 상태 체크
    {
        if (IsInBunker.Value == true && _agent.isOnNavMesh) //쫓아가는 상태이던중 벙커 안으로 들어올때 
        {
            _agent.ResetPath(); //벙커 밖 플레어를 향하던 경로를 초기화
            return true;
        }
        return false;
    }

    private bool CheckPlayerDistance() //플레이어와의 거리가 벌어졌을 때 강제 복귀
    {
        float distanceToPlayer = Vector3.Distance(Self.Value.transform.position, PlayerPosition.Value);

        if (distanceToPlayer >= 15.0f)
        {
            EnemyTarget.Value = null;

            if (_sensor != null)
            {
                _sensor.isAutoDetect = false;
            }
            _agent.SetDestination(PlayerPosition.Value);
            return true;
        }
        return false;
    }

    private void UpdateSensorSetting() // 자동탐색 제어 
    {
        if (_sensor == null)
        {
            return;
        }
        //자동 공격 모드일 때만 센서 키기 위함
        _sensor.isAutoDetect = (CurrentBattleMode.Value == BattleMode.AutoAttack);
    }

    private bool BattleModeSetting() // 배틀모드 설정
    {
       switch (CurrentBattleMode.Value)
        {
            case BattleMode.AutoAttack:
                return AutoAttackMode();

            case BattleMode.AssistAttack:
                return AssistAttackMode();

            case BattleMode.FollowOnly:
                return FollowOnlyMode();

            default:
                return false;
        }

    }

    private bool FollowOnlyMode()
    {
        EnemyTarget.Value = null;
        Debug.Log("[BT_FollowPlayer] 동행 전용모드 시작");

        return false;
    }

    private bool AutoAttackMode()
    {

        if (_sensor != null && _sensor.CurrentTarget != null)
        {
            EnemyTarget.Value = _sensor.CurrentTarget;

            CurrentState.Value = NpcState.Attack;

            Debug.Log("[BT_FollowPlayer] 몬스터 발견 -> Attack");
            Debug.Log("[BT_FollowPlayer] 자동 전투모드 시작");

            return true;
        }
        return false;
    }

    private bool AssistAttackMode()
    {

        //TestPlayer testPlayer = PlayerPosition.Value.GetComponent<TestPlayer>();

        //if (testPlayer != null && _sensor != null)
        //{
        //    GameObject playerTargetMonster = testPlayer.GetPlayerTarget();


        //    if (playerTargetMonster != null)
        //    {
        //        EnemyTarget.Value = playerTargetMonster;

        //        CurrentState.Value = NpcState.Attack;

        //        Debug.Log("[BT_FollowPlayer] 협동 공격모드 시작");

        //        return true;
        //    }
        //}
        return false;
    }
}

