using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BT_AttackMonster", story: "[Self] Attack:[EnemyTarget]", category: "Action", id: "e7ed2070b2ed284b797d91deea88c14f")]
public partial class BT_AttackMonster : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;//감지된 몬스터 블랙보드

    [SerializeReference] public BlackboardVariable<GameObject> Self;

    [SerializeReference] public BlackboardVariable<NpcState> CurrentState;

    private NpcAttack _attacker;
    protected override Status OnStart()
    {
        if (_attacker == null)
        {
            _attacker = Self.Value.GetComponent<NpcAttack>(); // Npc배틀 오브젝트에서 NpcAttacker 컴포넌트 빼오기 
        }

        if (EnemyTarget.Value == null)
        {
            return Status.Failure;
        }

        if (_attacker.isAttack == false)
        {
            _attacker.StartAttack(EnemyTarget.Value.transform); // 타겟이 된 몬스터 위치 정보 주고 공격 시작 명령 전달
        }

            return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(EnemyTarget.Value == null) // 몬스터가 죽었을나 범위 벗어나는 경우
        { 
                _attacker.StopAttack();

                CurrentState.Value = NpcState.Chase;
            
            return Status.Success; // 성공하면 다시 추적 상태로 넘어감
        }

        return Status.Running;

    }
}

