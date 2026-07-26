using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BT_IsTargetInBunker", story: "[벙커 진입 여부 체크]: Is [IsInBunker] InBunker?", category: "Conditions", id: "d59bd205ec61fbb149ca679e79632fe8")]
public partial class BT_IsTargetInBunker : Condition
{
    [SerializeReference] public BlackboardVariable<bool> IsInBunker; //벙커 진입 상태 여부
    public override bool IsTrue()
    {
        if (IsInBunker == null)
        {
            return false;
        }

        return IsInBunker.Value;

    }
}
