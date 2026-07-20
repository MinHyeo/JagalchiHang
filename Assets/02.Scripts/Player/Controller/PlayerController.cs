using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Attack")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius = 1f;
    [SerializeField] private LayerMask _monsterLayer;
    
    private bool _isAttacking;
    private bool _isPressedMouseRight;
    private bool _isDie;
    private bool _isHit;
    private bool _isPickUp;
    
    private Player _player;

    public PlayerData PlayerData => _player.PlayerData;
    public Animator Animator => _animator;
    public bool IsAttacking => _isAttacking;
    public bool IsDie => _isDie;
    public bool IsHit => _isHit;
    public bool IsPickUp => _isPickUp;
    public bool IsWalking => _moveController.IsWalking;
    public bool IsRunning => _moveController.IsRunning;

    private ItemController _currentItem;
    private PlayerMoveController _moveController;

    private StateMachine _stateMachine = new StateMachine();

    public event Action<Monster> OnMonsterAttacked;

    private void Awake()
    {
        _player = GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        _moveController = GetComponent<PlayerMoveController>();
        if (_moveController == null)
        {
            Debug.LogError("PlayerMoveController 컴포넌트를 찾을 수 없습니다.");
            return;
        }
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
        _stateMachine.Update(this);
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate(this);
    }

    // 플레이어 이동
    public void PlayerMove()
    {
        _moveController.PlayerMove();
    }

    // 플레이어 상태 바꾸기
    public void SetState(StateType stateType)
    {
        _stateMachine.SetState(stateType, this);
    }

    // 이동 - WASD 키 입력 처리
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        _moveController.SetMoveInput(moveInput);
    }

    // 달리기 - 쉬프트 키 입력 처리
    public void OnRun(InputAction.CallbackContext context)
    {
        // _isRuning = context.ReadValueAsButton(); 와 같음
        if (context.started)
        {
            _moveController.SetRunning(true);
        }

        if(context.canceled)
        {
            _moveController.SetRunning(false);
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

            monster.Damageable.TakeDamage(_player.PlayerData.AttackPower);
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

    // 마우스 우클릭 입력 처리
    public void OnLookToMousePos(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isPressedMouseRight = true;
            _moveController.SetLookingToMouse(true);
        }

        if(context.canceled)
        {
            _isPressedMouseRight = false;
            _moveController.SetLookingToMouse(false);
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
