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

    private IMonsterGroupBehavior _group;

    protected override Status OnStart()
    {
        _group = Agent.Value.GetComponent<Monster>().Group;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_group.Neighbors.Count == 0)
        {
            return Status.Failure;
        }

        _group.Tick();

        return Status.Running;
    }

    protected override void OnEnd()
    {

    }
}
