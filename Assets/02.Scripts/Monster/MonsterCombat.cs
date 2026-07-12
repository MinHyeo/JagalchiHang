using System;
using UnityEngine;

public class MonsterCombat : MonoBehaviour, IMonsterCombatable
{
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private float _attackDuration = 0.6f;

    private IMonsterDamageable _damageable;
    private IMonsterStatProvider _statProvider;
    private Collider _collider;

    private float _attackPower;
    private float _attackRange;
    private bool _isAttacking;
    private float _lastAttackTime;

    public bool IsAttacking
    {
        get { return _isAttacking; }
    }

    public float AttackRange
    {
        get { return _attackRange; }
    }

    public float AttackPower
    {
        get { return _attackPower; }
    }

    public event Action OnAttackStarted;
    public event Action OnAttackEnded;

    private void Awake()
    {
        _damageable = GetComponent<IMonsterDamageable>();
        _statProvider = GetComponent<IMonsterStatProvider>();
        _collider = GetComponent<Collider>();

        _damageable.OnDied += HandleDied;
    }

    private void Start()
    {
        _attackPower = _statProvider.AttackPower;
    }

    private void OnDestroy()
    {
        try
        {
            _damageable.OnDied -= HandleDied;
        }
        catch
        {

        }
    }

    public bool CanAttack()
    { 
        if (_isAttacking)
        {
            return false;
        }

        if (Time.time - _lastAttackTime < _attackCooldown)
        {
            return false;
        }

        return true;
    }
    
    public void Attack()
    {
        if (!CanAttack()) { return ; }

        _isAttacking = true;

        _lastAttackTime = Time.time;

        Debug.Log($"{name} : 공격 시작 (사거리 {_attackRange})");

        OnAttackStarted?.Invoke();

        CancelInvoke(nameof(EndAttack));

        Invoke(nameof(EndAttack), _attackDuration);

        // 추후 애니메이션 생기면 수정예정
    }

    public void EndAttack()
    {
        _isAttacking = false;

        Debug.Log($"{name} : 공격 종료");

        OnAttackEnded?.Invoke();
    }

   private void HandleDied()
    {
        Debug.Log($"{name} : 사망");

        _collider.enabled = false;
        enabled = false;
    }
}
