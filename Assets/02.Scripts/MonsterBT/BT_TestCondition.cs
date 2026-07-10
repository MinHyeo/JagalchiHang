using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TestCondition", story: "Test", category: "Conditions", id: "e978141be58a99e6a78f5ea9da9beb75")]
public partial class BT_TestCondition : Condition
{

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
