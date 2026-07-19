using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name : "Attack",
    story : "[Agent]가 공격을 실행한다",
    category : "Action",
    id : "e5f60718293a4b5c6d7e8f90a1b2c3d4")]

public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private IMonsterCombatable _combatable;

    protected override Status OnStart()
    {
        _combatable = Agent.Value.GetComponent<Monster>().Combatable;

        if (!_combatable.CanAttack())
        {
            return Status.Failure;
        }

        _combatable.Attack();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_combatable.IsAttacking)
        {
            return Status.Running;
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
        
    }
}
