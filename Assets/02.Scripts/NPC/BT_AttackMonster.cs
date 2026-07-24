using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BT_AttackMonster", story: "[Self] Attack:[EnemyTarget]", category: "Action", id: "e7ed2070b2ed284b797d91deea88c14f")]
public partial class BT_AttackMonster : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;//감지된 몬스터 블랙보드

    [SerializeReference] public BlackboardVariable<GameObject> Self;

    [SerializeReference] public BlackboardVariable<NpcState> CurrentState;

    [SerializeReference] public BlackboardVariable<GameObject> PlayerTarget;

    private NpcAttack _attacker;
    private NavMeshAgent _agent;
    private NpcManager _npcManager;
    protected override Status OnStart()
    {
        if (_attacker == null)
        {
            _attacker = Self.Value.GetComponent<NpcAttack>(); // Npc배틀 오브젝트에서 NpcAttacker 컴포넌트 빼오기 
            _agent = Self.Value.GetComponent<NavMeshAgent>();
        }

        if(_npcManager == null)
        {
            _npcManager = GameUtil.GetNpcManager();
        }

        if (EnemyTarget.Value == null)
        {
            return Status.Failure;
        }

       _attacker.StartAttack(EnemyTarget.Value.transform); // 타겟이 된 몬스터 위치 정보 주고 공격 시작 명령 전달

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (CheckPlayerDistance() == true) //플레이어와 너무 멀어졌을 경우
        {
            return Status.Success;
        }

        if(CheckEnemyDead() == true) //타겟이 죽었을 경우
        {
            return Status.Success;
        }
        return Status.Running;

    }

    protected override void OnEnd()
    {

        if (_attacker != null)
        {
            _attacker.StopAttack();
        }
    }

    private bool CheckPlayerDistance()
    {
        if (PlayerTarget.Value == null)
        {
            return false;
        }

        float distanceToPlayer = Vector3.Distance(Self.Value.transform.position, PlayerTarget.Value.transform.position);

        if (distanceToPlayer >= 15.0f)
        {
            Debug.Log("[BT_AttackMonster] 플레이어랑 너무 멀어졌다. 공격중단하고 쫓아가기");

            _attacker.StopAttack();

            EnemyTarget.Value = null;
            CurrentState.Value = NpcState.Chase;

            if(_agent != null && _agent.isOnNavMesh) // 공격 중 네비게이션을 풀고 플레이어 목적지 넣기
            {
                _agent.isStopped = false;
                _agent.SetDestination(PlayerTarget.Value.transform.position);
            }

            return true;
        }
        return false;
    }

    private bool CheckEnemyDead() // 사망 체크
    {
        if (EnemyTarget.Value == null)  
        {
            _attacker.StopAttack();

            _npcManager.ClearTargetMonster();


            CurrentState.Value = NpcState.Chase;

            return true;
        }

        MonsterHealth monsterHealth = EnemyTarget.Value.GetComponentInParent<MonsterHealth>();

        if (monsterHealth != null && monsterHealth.IsDead) //타겟 오브젝트가 파괴전이라도 죽었으면 즉시 죽음 처리
        {
            _attacker.StopAttack();
            _npcManager.ClearTargetMonster();
            CurrentState.Value = NpcState.Chase;
            EnemyTarget.Value = null;
            return true;
        }
        return false;
    }
}


