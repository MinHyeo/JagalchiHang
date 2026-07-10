using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string _playerDataId;
    [SerializeField] private float _rotationSmoothness = 10f;
    [SerializeField] private Animator _animator;
    
    private Vector2 _moveInput;
    private Vector3 _moveDirection;   

    private bool _isWalking;
    private bool _isRuning;
    private bool _isAttacking;
    private bool _isPressedMouseRight;
    private bool _isDie;
    
    private float _speed;

    private PlayerData _playerData;

    public bool IsRunning => _isRuning;
    public bool IsWalking => _isWalking;
    public bool IsAttacking => _isAttacking;
    public bool IsDie => _isDie;

    public Animator Animator => _animator;

    private PlayerStatusController _statusController;

    private StateMachine _stateMachine = new StateMachine();

    private void Awake()
    {
        _statusController = this.GetComponent<PlayerStatusController>();
    }

    private void Start()
    {
        GameDataManager.Instance.LoadData<PlayerData>();
        _playerData = GameDataManager.Instance.GetData<PlayerData>(_playerDataId);

        _statusController.InitPlayerStatus(_playerData);

        _stateMachine.AddState(StateType.Idle, new IdleState());
        _stateMachine.AddState(StateType.Walk, new WalkState());
        _stateMachine.AddState(StateType.Run, new RunState());
        _stateMachine.AddState(StateType.Attack, new AttackState());
        _stateMachine.AddState(StateType.Die, new DieState());

        _stateMachine.SetState(StateType.Idle, this);
    }

    private void Update()
    {
        MoveDirection();

        _speed = _isRuning ? _playerData.MoveSpeed * 3f : _playerData.MoveSpeed;

        _stateMachine.Update(this);

        if(_isPressedMouseRight == true)
        {
            LookToMousePos();
        }
        else
        {
            RotateToMoveDirection();
        }
    }

    // 플레이어 상태 바꾸기
    public void SetState(StateType stateType)
    {
        _stateMachine.SetState(stateType, this);
    }

    // 이동 - WASD 키 입력 처리
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _isWalking = true;
        }
        else
        {
            _isWalking = false;
        }
    }

    // 달리기 - 쉬프트 키 입력 처리
    public void OnRun(InputAction.CallbackContext context)
    {
        // _isRuning = context.ReadValueAsButton(); 와 같음
        if (context.started)
        {
            _isRuning = true;
        }

        if(context.canceled)
        {
            _isRuning = false;
        }
    }

    // 공격 - 마우스 좌클릭 입력 처리
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        if (_isAttacking == true) return;

        _isAttacking = true;

        SetState(StateType.Attack);
    }

    // 공격 애니메이션이 종료되면 공격 상태 해제
    public void OnAttackEnd()
    {
        _isAttacking = false;
    }

    public void Die()
    {
        _isDie = true;

        SetState(StateType.Die);
    }

    // 플레이어 이동
    public void PlayerMove()
    {
        transform.position += _moveDirection * _speed * Time.deltaTime;
    }

    // 플레이어의 이동 방향 설정
    private void MoveDirection()
    {
        _moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
    }

    // 플레이어의 이동 회전 설정
    private void RotateToMoveDirection()
    {
        if (_moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        }
    }

    // 마우스 우클릭 입력 처리
    public void OnLookToMousePos(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 한번 누를 때마다 true/false 변경
            _isPressedMouseRight = !_isPressedMouseRight;
        }
    }

    // 마우스 위치를 바라보도록 회전
    private void LookToMousePos()
    {
        // 현재 마우스 위치 가져옴
        Vector3 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // 방금 만든 Ray가 어떤 Collider에 부딪하면 hit에 들어감
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dir = hit.point - transform.position;
            dir.y = 0f;

            if(dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
            }
        }
    }
}
