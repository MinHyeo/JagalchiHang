using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ISpawnable
{
    [SerializeField] private float _rotationSmoothness = 10f;
    [SerializeField] private Animator _animator;

    [Header("Attack")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius = 1f;
    [SerializeField] private LayerMask _monsterLayer;
    
    private Vector2 _moveInput;
    private Vector3 _moveDirection;   

    private bool _isWalking;
    private bool _isRuning;
    private bool _isAttacking;
    private bool _isPressedMouseRight;
    private bool _isDie;
    private bool _isHit;
    private bool _isPickUp;
    
    private float _speed;

    private string _dataId;
    private int _instanceId;

    private PlayerData _playerData;

    public bool IsRunning => _isRuning;
    public bool IsWalking => _isWalking;
    public bool IsAttacking => _isAttacking;
    public bool IsDie => _isDie;

    public bool IsHit => _isHit;

    public bool IsPickUp => _isPickUp;

    public Animator Animator => _animator;

    private PlayerStatusController _statusController;

    private ItemController _currentItem;

    private StateMachine _stateMachine = new StateMachine();

    public event Action<Monster> OnMonsterAttacked;

    private void Awake()
    {
        _statusController = this.GetComponent<PlayerStatusController>();
    }

    private void Start()
    {
        _stateMachine.AddState(StateType.Idle, new IdleState());
        _stateMachine.AddState(StateType.Walk, new WalkState());
        _stateMachine.AddState(StateType.Run, new RunState());
        _stateMachine.AddState(StateType.Attack, new AttackState());
        _stateMachine.AddState(StateType.Die, new DieState());
        _stateMachine.AddState(StateType.Hit, new HitState());
        _stateMachine.AddState(StateType.PickUp, new PickUpState());

        _stateMachine.SetState(StateType.Idle, this);
    }

    private void Update()
    {
        MoveDirection();

        _speed = _isRuning ? _playerData.MoveSpeed * 3f : _playerData.MoveSpeed;

        _stateMachine.Update(this);
        _stateMachine.FixedUpdate(this);

        if(_isPressedMouseRight == true)
        {
            LookToMousePos();
        }
        else
        {
            RotateToMoveDirection();
        }
    }

    public void Init(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _dataId = dataId;

        _playerData = GameDataManager.Instance.GetData<PlayerData>(_dataId);
        if(_playerData == null)
        {
            Debug.LogError($"플레이어 데이터를 찾을 수 없습니다.");
            return;
        }

        if(_statusController == null)
        {
            _statusController = GetComponent<PlayerStatusController>();
        }

        _statusController.InitPlayerStatus(_playerData);
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

        if(_isPressedMouseRight == true)
        {
            _isAttacking = true;

            SetState(StateType.Attack);
        }
    }

    // 공격 애니메이션이 종료되면 공격 상태 해제
    public void OnAttackEnd()
    {
        _isAttacking = false;
    }

    // 몬스터 공격
    public void AttackMoster()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_attackPoint.position, _attackRadius, _monsterLayer);

        HashSet<Monster> damageMonsters = new HashSet<Monster>();

        foreach(Collider hitCollider in hitColliders)
        {
            Monster monster = hitCollider.GetComponent<Monster>();

            if(monster == null)
            {
                continue;
            }

            if(monster.Damageable == null)
            {
                continue;
            }

            if(damageMonsters.Add(monster) == false)
            {
                continue;
            }

            monster.Damageable.TakeDamage(_playerData.AttackPower);
            Debug.LogWarning($"플레이어가 {monster.InstanceId}번 몬스터를 공격했다!");

            OnMonsterAttacked?.Invoke(monster);
        }
    }

    // 사망
    public void Die()
    {
        _isDie = true;

        SetState(StateType.Die);
    }

    // 피격
    public void Hit()
    {
        _isHit = true;
        SetState(StateType.Hit);
    }

    // 피격 애니메이션이 종료되면 피격 상태 해제
    public void OnHitEnd()
    {
        _isHit = false;
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (_currentItem == null) return;
        if (context.performed == false) return;
        if (_isPickUp == true) return;

        _isPickUp = true;

        SetState(StateType.PickUp);
        Debug.Log($"{_currentItem}을 주웠다.");
        _currentItem.DestroyItem();
    }

    public void OnPickUpEnd()
    {
        _isPickUp = false;
    }

    public void SetCurrentItem(ItemController Item)
    {
        _currentItem = Item;
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
        if (context.started)
        {
            _isPressedMouseRight = true;
        }

        if(context.canceled)
        {
            _isPressedMouseRight = false;
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

    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
    }
}
