using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class BagNpc : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    private BlackboardVariable<bool> _isInBunker; //벙커 안밖 여부
    private BlackboardVariable<NpcState> _currentState; //NPC 현재 상태
    private BlackboardVariable<Vector3> _bunkerSpawnPosition; // 벙커 스폰위치
    private BlackboardVariable<Vector3> _playerPosition; //플레이어 위치  

    private NavMeshAgent _agent;
    private Npc_AnimController _animController;

    private int _currentEnergy = 100;
    private int _maxEnergy = 100;


    private int bonusSlotCount = 12; //임시 설정 


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


        
        //블랙보드와 연결 해주기 
        behaviorAgent.BlackboardReference.GetVariable("IsInBunker", out _isInBunker);
        behaviorAgent.BlackboardReference.GetVariable("CurrentState", out _currentState);
        behaviorAgent.BlackboardReference.GetVariable("BunkerSpawnPosition", out _bunkerSpawnPosition);
        behaviorAgent.BlackboardReference.GetVariable("PlayerPosition", out _playerPosition);
    }

    private void Start()
    {
        AddInventorySlot(bonusSlotCount);
    }

    private void Update()
    {
        HandAnimation();
    }

    private void HandAnimation()
    {
        if (_animController == null)
        {
            return;
        }

        bool isMoving = false;

        if(_agent != null && _agent.isOnNavMesh)
        {
            if (_agent.velocity.sqrMagnitude > 1f)
            {
                isMoving = true;
            }
        }

        if (isMoving) 
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
            Debug.LogWarning("[BagNpc] 에너지가 이미 0입니다.");

            return;
        }
        _currentEnergy = _currentEnergy - energy;

        if (_currentEnergy < 0) //에너지 차감 후 0 밑으로 떨어졌을 경우
        {
            _currentEnergy = 0;
            EnergyNpcStop();
            Debug.LogWarning("[BagNpc] 에너지가 0이 되었습니다.");
        }

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

        if (_currentState != null)
        {
            _currentState.Value = NpcState.Idle;
        }

        if(behaviorAgent != null)
        {
            behaviorAgent.enabled = false;
        }
    }

    public void ChargeEnergy(int energy) // 에너지 충전부 (벙커)
    {
        if (_currentEnergy >= _maxEnergy)
        {
            _currentEnergy = _maxEnergy;
            Debug.Log("[BagNpc] 충전이 완료 되어있습니다.");
            return;
        }

        if (NetworkManager.Instance != null)
        {
            NetworkGeneratorService generatorService = NetworkManager.Instance.GeneratorService;

            if (generatorService != null)
            {
                generatorService.GetGeneratorViewModel();

                if (generatorService.CanUsePower(energy) == false)
                {
                    Debug.LogWarning("[BattleNpc] 발전기 전력이 부족하거나 고장이나서 충전 불가");

                    return;
                }

                generatorService.UsePower(energy);
            }
        }
        bool isStop = (_currentEnergy <= 0);

        _currentEnergy = _currentEnergy + energy;

        if (_currentEnergy > _maxEnergy) // 충전 후 최대치를 초과했을 때
        {
            _currentEnergy = _maxEnergy;

            Debug.Log("[BagNpc] 충전이 다 되었습니다.");
        }

        if (isStop == true && _currentEnergy > 0)
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
            }

            if (behaviorAgent != null)
            {
                behaviorAgent.enabled = true; //행동트리 다시 작동 
            }
        }
    }
    private void AddInventorySlot(int count)
    {
        //인벤토리 뷰 모델 주소 가져오기 
        InventoryViewModel inventoryVM = NetworkManager.Instance.InventoryService.GetLocalInventoryViewModel();

        if (inventoryVM != null && inventoryVM.InventorySlots != null)
        {
            /*InventoryViewModel에 있는 _slotCount 를 접근할 수 있게 바꿔주고 Const를 지워주시면 
             _slotCount에 값 더해주기 */
            int maxSlotLimit = 36;

            if (inventoryVM.SlotCount < maxSlotLimit)
            {
                inventoryVM.SlotCount += count;

            }


            // 인벤토리 슬롯 개수 값을 추가 슬롯으로 더해준 값으로 늘려주기 위해
            while (inventoryVM.InventorySlots.Count < inventoryVM.SlotCount)
            {

                //0번부터 값이 들어가니까 현재 카운트를 넣어서 개수 이어가기 
                int nextSlotIndex = inventoryVM.InventorySlots.Count;

                //새 key값을 넣고 키 값에 맞는 새 슬롯을 만들어준다
                inventoryVM.InventorySlots.Add(nextSlotIndex, new InventorySlotViewModel());
            }

            inventoryVM.NotifySlotCountChanged();

            Debug.Log($"[BagNpc] 추가 인벤토리 칸 연동 완료 총 인벤토리 칸: {inventoryVM.SlotCount} ");
        }
    }
    public void UpdatePlayerPosition(Vector3 currentPlayerPosition)
    {
        if(_playerPosition != null)
        {
            _playerPosition.Value = currentPlayerPosition;
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
