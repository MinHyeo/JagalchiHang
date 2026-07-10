using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name : "Chase Player",
    story : "[Agent]가 플레이의 마지막 목격 위치로 추격한다",
    category : "Action",
    id : "c3d4e5f60718293a4b5c6d7e8f90a1b2")]

public partial class ChasePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private IMonsterMoveable _moveable;
    private IMonsterPerceivable _perceivable;
    private IMonsterCombatable _combatable;

    protected override Status OnStart()
    {
        _moveable = Agent.Value.GetComponent<IMonsterMoveable>();
        _perceivable = Agent.Value.GetComponent<IMonsterPerceivable>();
        _combatable = Agent.Value.GetComponent<IMonsterCombatable>();

       // Debug.Log($"{Agent.Value.name} : Chase Player 노드 시작됨 (moveable null? {_moveable == null}, perceivable null? {_perceivable == null}, combatable null? {_combatable == null})");

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_perceivable.CanSeePlayer)
        {
           // Debug.Log($"{Agent.Value.name} : Chase 중 시야 잃음, 실패 처리");

            _moveable.Stop();
            return Status.Failure;
        }

        Vector3 targetPosition = _perceivable.LastKnownPlayerPosition.Value;
        float distanceToTarget = Vector3.Distance(Agent.Value.transform.position, targetPosition);

       // Debug.Log($"{Agent.Value.name} : 추격 중, 목표까지 거리 {distanceToTarget:F1} (공격사거리 {_combatable.AttackRange})");

        if (distanceToTarget <= _combatable.AttackRange)
        {
            _moveable.Stop();
            return Status.Success;
        }

        _moveable.MoveTo(targetPosition);
        return Status.Running;
    }

    protected override void OnEnd()
    {
        try
        {
            _moveable.Stop();
        }
        catch
        {

        }
    }
}
