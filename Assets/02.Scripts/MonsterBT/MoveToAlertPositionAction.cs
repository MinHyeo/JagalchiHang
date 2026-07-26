using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Move To Alert Position",
    story: "[Agent]가 경보 받은 위치로 이동한다",
    category: "Action",
    id: "0718293a4b5c6d7e8f90a1b2c3d4e5f6")]

public partial class MoveToAlertPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private IMonsterMoveable _moveable;
    private IMonsterGroupBehavior _group;

    protected override Status OnStart()
    {
        Monster monster = Agent.Value.GetComponent<Monster>();

        _moveable = monster.Moveable;
        _group = monster.Group;

        if (!_group.IsAlerted)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_group.IsAlerted)
        {
            _moveable.Stop();
            return Status.Failure;
        }

        Vector3 targetPosition = _group.AlertPosition;
        _moveable.MoveTo(targetPosition);

        if (_moveable.HasReachedDestination)
        {
            _group.ClearAlert();
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        _moveable.Stop();
    }
}
