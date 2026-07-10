using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BT_IsInBunker", story: "Is [Self] Inside Bunker", category: "Conditions", id: "d59bd205ec61fbb149ca679e79632fe8")]
public partial class BT_IsInBunker : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<BTState> CurrentState; 

    public override bool IsTrue() //조건 구분해주는 함수 
    {
        if (Self.Value == null)  
        {
            return false;
        }

        if(CurrentState.Value == BTState.Idle) // Idle 상태 확인 
        {
            return true;
        }
        return false;
    }


}
