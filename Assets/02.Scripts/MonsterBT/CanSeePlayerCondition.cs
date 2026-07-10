using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(
    name : "Can See Player",
    story : "[Agent]가 플레이어를 볼 수 있는가",
    category : "Conditions",
    id : "a1b2c3d4e5f60718293a4b5c6d7e8f90")]

public partial class CanSeePlayerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private IMonsterPerceivable _perceivable;

    public override void OnStart()
    {
        
    }

    public override bool IsTrue()
    {
        if (Agent == null || Agent.Value == null)
        {
           //  Debug.LogWarning("CanSeePlayerCondition : Agent가 연결되어 있지 않습니다!");
            return false;
        }

        if (_perceivable == null)
        {
            _perceivable = Agent.Value.GetComponent<IMonsterPerceivable>();
        }

        bool result = _perceivable.CanSeePlayer;

       // Debug.Log($"{Agent.Value.name} : CanSeePlayerCondition 평가됨 -> {result}");

        return result;
    }

    public override void OnEnd()
    {
        
    }
}
