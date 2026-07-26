using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class BattleNpc : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    private BlackboardVariable<bool> _isInBunker; //벙커 안밖 여부
    private BlackboardVariable<NpcState> _currentState; //BattleNPC 현재 상태
    private BlackboardVariable<Vector3> _bunkerSpawnPosition; // 벙커 스폰위치
    private BlackboardVariable<Vector3> _playerPosition; //플레이어 위치  
    private BlackboardVariable<BattleMode> _currentBattleMode;
    private BlackboardVariable<GameObject> _enemyTarget;


    private NavMeshAgent _agent;
    private Npc_AnimController _animController;
    private EnemySensor _sensor;

    private int _currentEnergy = 100;
    private int _maxEnergy = 100;

    public int CurrentEnergy
    {
        get
        {
            return _currentEnergy;
        }
    }

    public int MaxEnergy
    {
        get
        {
            return _maxEnergy;
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animController = GetComponent<Npc_AnimController>();
        _sensor = GetComponent<EnemySensor>();

        //블랙보드와 연결 해주기 
        behaviorAgent.BlackboardReference.GetVariable("IsInBunker", out _isInBunker);
        behaviorAgent.BlackboardReference.GetVariable("CurrentState", out _currentState);
        behaviorAgent.BlackboardReference.GetVariable("BunkerSpawnPosition", out _bunkerSpawnPosition);
        behaviorAgent.BlackboardReference.GetVariable("CurrentBattleMode", out _currentBattleMode);
        behaviorAgent.BlackboardReference.GetVariable("PlayerPosition", out _playerPosition);
        behaviorAgent.BlackboardReference.GetVariable("EnemyTarget", out _enemyTarget);
    }


    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (_animController == null)
        {
            return;
        }

        bool isMoving = false;


        if (_agent != null && _agent.isOnNavMesh)
        {
            if (_agent.velocity.sqrMagnitude > 0.1f)
            {
                isMoving = true;
            }
        }

        if (_currentState != null && _currentState.Value == NpcState.Attack)
        {
            if (isMoving)
            {
                _animController.SetNpcAnimState(Npc_AnimController.Npc_AnimState.Walk);
            }

            else
            {
                _animController.SetNpcAnimState(Npc_AnimController.Npc_AnimState.Attack);
            }

            return;
        }

        if (isMoving == true)
        {
            _animController.SetNpcAnimState(Npc_AnimController.Npc_AnimState.Walk);
        }
        else
        {
            _animController.SetNpcAnimState(Npc_AnimController.Npc_AnimState.Idle);
        }
    }

    public void UseEnergy(int energy) //에너지 소모부(파밍)
    {

        if (_currentEnergy <= 0)
        {
            _currentEnergy = 0;
            EnergyNpcStop();

            Debug.LogWarning("[BattleNpc] 에너지가 이미 0입니다.");

            return;
        }
        _currentEnergy = _currentEnergy - energy;

        if(_currentEnergy <= 0) //에너지 차감 후 0 밑으로 떨어졌을 경우
        {
            _currentEnergy = 0;
            EnergyNpcStop();
            Debug.LogWarning("[BattleNpc] 에너지가 0이 되었습니다.");

            
        }
        Debug.Log($"[BattleNpc] 에너지 차감 (-{energy}) / 현재 에너지: {_currentEnergy}/{_maxEnergy}");
    }

    private void EnergyNpcStop()
    {
        Debug.LogWarning("[BattleNpc] 에너지가 0이 되어 기능 정지");

        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
        }

        if (_sensor != null)
        {
            _sensor.ClearTarget();
        }

        if (_enemyTarget != null)
        {
            _enemyTarget.Value = null;
        }

        if(_currentState != null)
        {
            _currentState.Value = NpcState.Idle;
        }

        if(behaviorAgent != null) //행동트리도 정지
        {
            behaviorAgent.enabled = false;
        }
    }

    public void ChargeEnergy(int energy) // 에너지 충전부 (벙커)
    { 
        bool isStop = (_currentEnergy <= 0);

        if(_currentEnergy >= _maxEnergy)
        {
            _currentEnergy = _maxEnergy;
            Debug.Log("[BattleNpc] 충전이 완료 되어있습니다.");
            return;
        }

        _currentEnergy = _currentEnergy + energy;

        if(_currentEnergy > _maxEnergy) // 충전 후 최대치를 초과했을 때
        {
            _currentEnergy = _maxEnergy;

            Debug.Log("[BattleNpc] 충전이 다 되었습니다.");
        }

        if(isStop == true && _currentEnergy > 0)
        {
            if(_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
            }

            if(behaviorAgent != null)
            {
                behaviorAgent.enabled = true; //행동트리 다시 작동 
            }
        }
        Debug.Log($"[BattleNpc] 벙커 에너지 충전 (+{energy}) / 현재 에너지: {_currentEnergy}/{_maxEnergy}");
    }

    public void UpdatePlayerPosition(Vector3 currentPlayerPosition)
    {
        if (_playerPosition != null)
        {
            _playerPosition.Value = currentPlayerPosition;
        }
    }
    public void SetBattleMode(BattleMode battleMode)
    {
        if (_currentBattleMode != null)
        {
            _currentBattleMode.Value = battleMode;
            Debug.Log($"[BattleNpc] 블랙보드 CurrentBattleMode 값을 {battleMode}로 변경");
        }

        if (_enemyTarget != null)
        {
            _enemyTarget.Value = null;
        }

        if (_sensor != null)
        {
            _sensor.ClearTarget();
        }

        if (_currentState != null)
        {
            _currentState.Value = NpcState.Chase;
        }

        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();

            _agent.SetDestination(_playerPosition.Value);
        }
    }

    public void ChangeNpcPosition(Vector3 targetSpawnPos)
    {
        /*NavMeshAgent를 켜놓은 상태로 BattleNPC를 위치 이동시키는 건 충돌을 일으키기 때문에
        * NavMeshAgent를 끄고 이동시킨 후 다시 켜야한다.*/

        if (_agent != null)
        {
            _agent.ResetPath(); // 경로 초기화

            _agent.enabled = false;
            transform.position = targetSpawnPos;
            _agent.enabled = true;
        }

        else
        {
            transform.position = targetSpawnPos;
        }
    }
    public void InOutBunkerData(bool isInBunker, Vector3 targetSpawnPos)
    {

        if (behaviorAgent != null)
        {
            behaviorAgent.enabled = false;
        }


        _isInBunker.Value = isInBunker; //블랙보드로 값 넣어주기 

        _bunkerSpawnPosition.Value = targetSpawnPos;

        ChangeNpcPosition(targetSpawnPos);

        if (_currentState != null)
        {
            if (isInBunker == true)
            {
                _currentState.Value = NpcState.Idle;
            }
            else
            {
                _currentState.Value = NpcState.Chase;
            }
        }

        if (behaviorAgent != null)
        {
            behaviorAgent.enabled = true;
        }

    }


}
