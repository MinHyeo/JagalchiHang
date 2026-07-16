using UnityEngine;
using Unity.Behavior;
using Unity.Properties;
using System;

[Serializable, GeneratePropertyBag]
[Condition(
    name : "Has Detected Trail",
    story : "[Agent]가 흔적을 발견했는가",
    category : "Conditions",
    id : "b2c3d4e5f60718293a4b5c6d7e8f90a1")]

public partial class HasDetectedTrailCondition : Condition
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Agent;

    private IMonsterPerceivable _perceivable;

    public override void OnStart()
    {

    }

    public override bool IsTrue()
    {
        if (_perceivable == null)
        {
            _perceivable = Agent.Value.GetComponent<Monster>().Perceivable;
        }

        return _perceivable.HasDetectedTrail;
    }

    public override void OnEnd()
    {

    }
}
