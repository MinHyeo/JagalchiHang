using UnityEngine;
using System;
using Unity.Behavior;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(
    name : "Is Alerted",
    story : "[Agent]가 동료로부터 경보를 받았는가",
    category : "Conditions",
    id : "f60718293a4b5c6d7e8f90a1b2c3d4e5")]

public partial class IsAlertedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private IMonsterAlertable _alertable;

    public override bool IsTrue()
    {
        if (_alertable == null)
        {
            _alertable = Agent.Value.GetComponent<IMonsterAlertable>();
        }

        return _alertable.IsAlerted;
    }

    public override void OnEnd()
    {
        
    }
}
