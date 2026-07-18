using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BT_BagFollowPlayer", story: "[Self] Follow [PlayerPosition]", category: "Action", id: "79f9bfbf7e8bc89da66f0e397893d069")]
public partial class BT_BagFollowPlayer : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> PlayerPosition;
    [SerializeReference] public BlackboardVariable<NpcState> CurrentState;
    [SerializeReference] public BlackboardVariable<bool> IsInBunker;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {

        _agent = Self.Value.GetComponent<NavMeshAgent>(); //자기 자신에서 NavMesh 컴포넌트 가져오기 

        CurrentState.Value = NpcState.Chase; //추적 상태로 변경

        _agent.speed = 5.0f; //NPC 이동속도 
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if (IsInBunker.Value == true) //쫓아가는 상태이던중 벙커 안으로 들어올때 
        {
            if (_agent.isOnNavMesh)
            {
                _agent.ResetPath(); //벙커 밖 플레어를 향하던 경로를 초기화
                return Status.Failure;

            }
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
}

