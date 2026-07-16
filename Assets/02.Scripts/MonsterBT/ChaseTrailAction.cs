using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name : "Chase Trail",
    story : "[Agent]가 발견한 흔적 위치로 이동한다",
    category : "Action",
    id : "d4e5f60718293a4b5c6d7e8f90a1b2c3")]

public partial class ChaseTrailAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> ArrivalTolerance;

    private IMonsterMoveable _moveable;
    private IMonsterPerceivable _perceivable;

    protected override Status OnStart()
    {
        Monster monster = Agent.Value.GetComponent<Monster>();

        _moveable = monster.Moveable;
        _perceivable = monster.Perceivable;

        if (!_perceivable.HasDetectedTrail)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_perceivable.HasDetectedTrail)
        {
            _moveable.Stop();
            return Status.Failure;
        }

        Vector3 trailPosition = _perceivable.TrailPosition;
        float distanceToTrail = Vector3.Distance(Agent.Value.transform.position, trailPosition);

        if (distanceToTrail <= ArrivalTolerance.Value)
        {
            _moveable.Stop();
            _perceivable.ClearTrail();
            return Status.Success;
        }

        _moveable.MoveTo(trailPosition);
        return Status.Running;
    }

    protected override void OnEnd()
    {
        _moveable.Stop();
    }
}
