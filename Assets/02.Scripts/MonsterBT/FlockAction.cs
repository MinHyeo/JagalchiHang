using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Flock",
    story: "[Agent]가 무리와 함께 군집 이동한다",
    category: "Action",
    id: "18293a4b5c6d7e8f90a1b2c3d4e5f607")]

public partial class FlockAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private IMonsterFlockMovable _flockMovable;
    private IMonsterFlockperceivable _flockPerceivable;

    protected override Status OnStart()
    {
        _flockMovable = Agent.Value.GetComponent<IMonsterFlockMovable>();
        _flockPerceivable = Agent.Value.GetComponent<IMonsterFlockperceivable>();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_flockPerceivable.Neighbors.Count == 0)
        {
            return Status.Failure;
        }

        _flockMovable.Tick();

        return Status.Running;
    }

    protected override void OnEnd()
    {
        
    }
}
