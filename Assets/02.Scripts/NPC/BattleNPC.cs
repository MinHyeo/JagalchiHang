using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class BattleNPC : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    private BlackboardVariable<bool> _isInBunker; //벙커 안밖 여부
    private BlackboardVariable<NpcState> _currentState; //BattleNPC 현재 상태
    private BlackboardVariable<Vector3> _bunkerSpawnPosition; // 벙커 스폰위치
    private BlackboardVariable<Vector3> _returnSpawnPosition; //돌아갈 위치

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        //블랙보드와 연결 해주기 
        behaviorAgent.BlackboardReference.GetVariable("IsInBunker", out _isInBunker);
        behaviorAgent.BlackboardReference.GetVariable("CurrentState", out _currentState);
        behaviorAgent.BlackboardReference.GetVariable("BunkerSpawnPosition", out _bunkerSpawnPosition);
        behaviorAgent.BlackboardReference.GetVariable("ReturnSpawnPosition", out _returnSpawnPosition);
    }

    private void OnEnable()
    {
        NpcManager.OnBunkerEnterEvent += EnterBunker;
        NpcManager.OnBunkerExitEvent += ExitBunker;
    }

    private void OnDisable()
    {
        NpcManager.OnBunkerEnterEvent -= EnterBunker;
        NpcManager.OnBunkerExitEvent -= ExitBunker;
    }

    private void EnterBunker(bool value, Vector3 bunkerPos)
    {
        _isInBunker.Value = value; //블랙보드로 값 넣어주기 

        _bunkerSpawnPosition.Value = bunkerPos;

        /*NavMeshAgent를 켜놓은 상태로 BattleNPC를 위치 이동시키는 건 충돌을 일으키기 때문에
         * NavMeshAgent를 끄고 이동시킨 후 다시 켜야한다.*/

        if(_agent != null)
        {
            _agent.ResetPath(); // 경로 초기화

            _agent.enabled = false;
            transform.position = _bunkerSpawnPosition.Value;
            _agent.enabled = true;
        }

        else
        {
            transform.position = _bunkerSpawnPosition.Value;
        }

        _currentState.Value = NpcState.Idle;

    }

    private void ExitBunker(bool value, Vector3 bunkerExitPos)
    {
        _isInBunker.Value = value;
        _returnSpawnPosition.Value = bunkerExitPos;

        if (_agent != null)
        {
            _agent.ResetPath(); // 경로 초기화

            _agent.enabled = false;
            transform.position = _returnSpawnPosition.Value;
            _agent.enabled = true;
        }

        else
        {
            transform.position = _returnSpawnPosition.Value;
        }

        _currentState.Value = NpcState.Chase;

    }
}
