using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name : "Move To Alert Position",
    story : "[Agent]가 경보 받은 위치로 이동한다",
    category : "Action",
    id : "0718293a4b5c6d7e8f90a1b2c3d4e5f6")]

public partial class MoveToAlertPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private IMonsterMoveable _moveable;
    private IMonsterAlertable _alertable;

    protected override Status OnStart()
    {
        _moveable = Agent.Value.GetComponent<IMonsterMoveable>();
        _alertable = Agent.Value.GetComponent<IMonsterAlertable>();

        if (!_alertable.IsAlerted)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_alertable.IsAlerted)
        {
            _moveable.Stop();
            return Status.Failure;
        }

        Vector3 targetPosition = _alertable.AlertPosition.Value;
        _moveable.MoveTo(targetPosition);

        if (_moveable.HasReachedDestination)
        {
            _alertable.ClearAlert();
            return Status.Success;
        }

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
