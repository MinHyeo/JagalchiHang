using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BT_IsTargetInBunker", story: "Is [Target] InBunker", category: "Conditions", id: "d59bd205ec61fbb149ca679e79632fe8")]
public partial class BT_IsTargetInBunker : Condition
{
    [SerializeReference] public BlackboardVariable<bool> IsInBunker; //벙커 진입 상태 여부
    [SerializeReference] public BlackboardVariable<GameObject> Self; // NPC
    [SerializeReference] public BlackboardVariable<BTState> CurrentState; //NPC의 상태 
    [SerializeReference] public BlackboardVariable<Vector3> BunkerSpawnPosition; // 벙커에 맨처음 스폰 되어야 할  위치 

    private NavMeshAgent _agent;
    private bool _isTeleported = false;


    public override void OnStart()
    {
        TestNPCManager.OnBunkerEvent += ReceiveBunkerSignal;

        CheckEnterBunker();
    }

    public override void OnEnd()
    {
        TestNPCManager.OnBunkerEvent -= ReceiveBunkerSignal;
    }

    private void ReceiveBunkerSignal(bool value, Vector3 spawnPos) //NPC 매니저에서 값을 받은 곳 
    {
        if (IsInBunker != null)
        {
            IsInBunker.Value = value;
        }

        if (BunkerSpawnPosition != null)
        {
            BunkerSpawnPosition.Value = spawnPos;
        }
    }
    private void EnterToBunker()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>();

        /*NavMeshAgent를 켜놓은 상태로 BattleNPC를 위치이동시키는 건 충돌을 일으키기 때문에
          NavMeshAgent를 끄고 이동시킨 후 다시 켜야한다.*/
        if (_agent != null)
        {
            _agent.ResetPath(); // 경로 초기화 
            _agent.enabled = false;
            Self.Value.transform.position = BunkerSpawnPosition.Value;
            _agent.enabled = true;
        }

        else
        {
            Self.Value.transform.position = BunkerSpawnPosition.Value;
        }

        CurrentState.Value = BTState.Idle;
    }

    private void CheckEnterBunker()
    {
        if (IsInBunker != null && IsInBunker.Value == true && _isTeleported == false)
        {
            _isTeleported = true;
            EnterToBunker();

        }
    }

    public override bool IsTrue()
    {
        if (IsInBunker == null)
        {
            return false;
        }

        return IsInBunker.Value;

    }
}
