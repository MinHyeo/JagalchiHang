using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class BagNpc : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    private BlackboardVariable<bool> _isInBunker; //벙커 안밖 여부
    private BlackboardVariable<NpcState> _currentState; //NPC 현재 상태
    private BlackboardVariable<Vector3> _bunkerSpawnPosition; // 벙커 스폰위치
    private BlackboardVariable<Vector3> _returnSpawnPosition; //돌아갈 위치

    private BlackboardVariable<Vector3> _playerPosition; //플레이어 위치  

    private NavMeshAgent _agent;
    private Npc_AnimController _animController;


    [Header("인벤토리 확장 설정")]
    [SerializeField] private int bonusSlotCount = 6; //임시 설정 


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animController = GetComponent<Npc_AnimController>();


        
        //블랙보드와 연결 해주기 
        behaviorAgent.BlackboardReference.GetVariable("IsInBunker", out _isInBunker);
        behaviorAgent.BlackboardReference.GetVariable("CurrentState", out _currentState);
        behaviorAgent.BlackboardReference.GetVariable("BunkerSpawnPosition", out _bunkerSpawnPosition);
        behaviorAgent.BlackboardReference.GetVariable("ReturnSpawnPosition", out _returnSpawnPosition);
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

    private void AddInventorySlot(int count)
    {
        ////인벤토리 뷰 모델 주소 가져오기 
        //InventoryViewModel inventoryVM = NetworkManager_re.Inst.InventoryService.GetLocalInventoryViewModel();

        //if(inventoryVM != null && inventoryVM.InventorySlots != null)
        //{
        //    /*InventoryViewModel에 있는 _slotCount 를 접근할 수 있게 바꿔주고 Const를 지워주시면 
        //     _slotCount에 값 더해주기 */

        //    inventoryVM._slotCount += count;


        //   // 인벤토리 슬롯 개수 값을 추가 슬롯으로 더해준 값으로 늘려주기 위해
        //    while (inventoryVM.InventorySlots.Count < inventoryVM._slotCount)
        //    {

        //        //0번부터 값이 들어가니까 현재 카운트를 넣어서 개수 이어가기 
        //        int nextSlotIndex = inventoryVM.InventorySlots.Count;

        //        //새 key값을 넣고 키 값에 맞는 새 슬롯을 만들어준다
        //        inventoryVM.InventorySlots.Add(nextSlotIndex, new InventorySlotViewModel());
        //    }

        //    Debug.Log($"[BagNpc] 추가 인벤토리 칸 연동 완료 총 인벤토리 칸: {inventoryVM._slotCount} ");
        //}
    }
    public void UpdatePlayerPosition(Vector3 currentPlayerPosition)
    {
        if(_playerPosition != null)
        {
            _playerPosition.Value = currentPlayerPosition;
        }
    }

    public void EnterBunker(bool value, Vector3 bunkerPos)
    {

        if (behaviorAgent != null)
        {
            behaviorAgent.enabled = false;
        }


        _isInBunker.Value = value; //블랙보드로 값 넣어주기 

        _bunkerSpawnPosition.Value = bunkerPos;

        /*NavMeshAgent를 켜놓은 상태로 BattleNPC를 위치 이동시키는 건 충돌을 일으키기 때문에
         * NavMeshAgent를 끄고 이동시킨 후 다시 켜야한다.*/

        if (_agent != null)
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

        if (behaviorAgent != null)
        {
            behaviorAgent.enabled = true;
        }

    }

    public void ExitBunker(bool value, Vector3 bunkerExitPos)
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
