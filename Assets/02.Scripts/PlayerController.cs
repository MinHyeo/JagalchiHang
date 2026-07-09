using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _smoothness = 10f;
    [SerializeField] private bool _toggleCameraRotation;    // Alt를 눌렀을 때 둘러보기 가능하도록 하는 변수
    [SerializeField] private Animator _animator;

    private Camera _camera;
    private Vector2 _moveInput;
    private Vector3 _moveDirection;   // 플레이어가 현재 이동해야 하는 방향

    private bool _isWalking;
    private bool _isRuning;
    private bool _isAttacking;
    
    private float _speed;

    public bool IsRunning => _isRuning;
    public bool IsWalking => _isWalking;
    public bool IsAttacking => _isAttacking;

    public Animator Animator => _animator;

    private StateMachine _stateMachine = new StateMachine();

    private void Start()
    {
        _camera = Camera.main;    // 태그에 MainCamera가 활성화 돼있어야 함

        _stateMachine.AddState(StateType.Idle, new IdleState());
        _stateMachine.AddState(StateType.Walk, new WalkState());
        _stateMachine.AddState(StateType.Run, new RunState());
        _stateMachine.AddState(StateType.Attack, new AttackState());

        _stateMachine.SetState(StateType.Idle, this);
    }

    private void Update()
    {
        MoveDirection();
        CameraToggle();

        _stateMachine.Update(this);
    }

    private void LateUpdate()
    {
        RotatePlayer();
    }

    // 플레이어 상태 바꾸는 함수
    public void SetState(StateType stateType)
    {
        _stateMachine.SetState(stateType, this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();

        // 입력값의 크기가 거의 0이 아니면
        // Magnitude: 루트 씌워서 계산     / sqrMagnitude: 루트 벗겨서 계산 -> 비용 적게 듬
        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _isWalking = true;
        }
        else
        {
            _isWalking = false;
        }
    }

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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        if (_isAttacking == true) return;

        _isAttacking = true;

        SetState(StateType.Attack);
    }

    // 공격 애니메이션이 종료되면 공격 상태를 해제하는 함수
    public void OnAttackEnd()
    {
        _isAttacking = false;
    }

    public void PlayerMove()
    {
        // 계산된 방향으로 플레이어 이동
        transform.position += _moveDirection * _speed * Time.deltaTime;
    }

    private void MoveDirection()
    {
        // 카메라가 바라보는 방향에서 Y축을 제거, 평면 기준 전방 방향 생성
        Vector3 camForward = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
        // 카메라의 오른쪽 방향
        Vector3 camRight = _camera.transform.right;

        // 입력값(WASD)을 카메라 기준의 이동 방향으로 변환
        _moveDirection = camForward * _moveInput.y + camRight * _moveInput.x;

        _speed = _isRuning ? _runSpeed : _moveSpeed;
    }

    private void CameraToggle()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            _toggleCameraRotation = true;    // 둘러보기 활성화
        }
        else
        {
            _toggleCameraRotation = false;   // 둘러보기 비활성화
        }
    }

    private void RotatePlayer()
    {
        if (_toggleCameraRotation == false)
        {
            // 두 벡터 곱하기
            // X, Z축은 유지하고 Y축만 0으로 만들어지면 기준 방향만 사용
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            // Quaternion.LookRotation(playerRotate) : playerRotate 방향을 바라보는 목표 회전값
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * _smoothness);
        }
    }
}
