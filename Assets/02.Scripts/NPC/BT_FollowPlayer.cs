using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BT_FollowPlayer", story: "[Self] Follow Player [Target]", category: "Action", id: "107259c8e0a53a3ea69149a2704dc9cb")]
public partial class BT_FollowPlayer : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<NPCState> CurrentState;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {
        _agent  = Self.Value.GetComponent<NavMeshAgent>(); //자기 자신에서 NavMesh 컴포넌트 가져오기 

        if(_agent == null || Target.Value == null)
        {
            return Status.Failure; //실패 처리 
        }

        CurrentState.Value = NPCState.Chase; //추적 상태로 변경

        _agent.speed = 5.0f; //NPC 이동속도 
        _agent.SetDestination(Target.Value.transform.position); // 설정해둔 타겟(플레이어)의 위치를 목적지로 설정

        return Status.Running; //실행중

    }

    protected override Status OnUpdate()
    {
        if(Target.Value == null || _agent == null)
        {
            return Status.Failure;
        }

        _agent.SetDestination(Target.Value.transform.position); // 매프레임마다 타겟위치로 좌표 설정

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if(_agent != null && _agent.isOnNavMesh) // 컴포넌트가 존재하고 NavMesh 바닥위에 정상적으로 서있다면 
        {
            _agent.ResetPath(); // 경로 초기화 
        }
    }
}

