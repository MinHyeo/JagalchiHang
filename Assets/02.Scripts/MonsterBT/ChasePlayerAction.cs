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
        Monster monster = Agent.Value.GetComponent<Monster>();

        _moveable = monster.Moveable;
        _perceivable = monster.Perceivable;
        _combatable = monster.Combatable;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_perceivable.CanSeePlayer)
        {
            _moveable.Stop();
            return Status.Failure;
        }

        Vector3 targetPosition = _perceivable.LastKnownPlayerPosition;
        float distanceToTarget = Vector3.Distance(Agent.Value.transform.position, targetPosition);

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
        _moveable.Stop();
    }
}
