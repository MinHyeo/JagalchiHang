using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MonsterCombat : MonoBehaviour, IMonsterCombatable
{
    [SerializeField] private float _attackCooldown = 2.5f;
    [SerializeField] private float _attackDuration = 0.6f;
    [SerializeField] private float _corpseLingerDuration = 5f;

    private IMonsterDamageable _damageable;
    private IMonsterStatProvider _statProvider;
    private IMonsterMoveable _moveable;
    private Unity.Behavior.BehaviorGraphAgent _behaviorGraphAgent;
    private Collider _collider;
    private bool _isAttacking;
    private float _lastAttackTime;

    public bool IsAttacking
    {
        get { return _isAttacking; }
    }

    public float AttackRange
    {
        get { return _statProvider.AttackRange; }
    }

    public float AttackPower
    {
        get { return _statProvider.AttackPower; }
    }

    public event Action OnAttackStarted;
    public event Action OnAttackEnded;

    private void Awake()
    {
        _damageable = GetComponent<IMonsterDamageable>();
        _moveable = GetComponent<IMonsterMoveable>();
        _behaviorGraphAgent = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
        _collider = GetComponent<Collider>();
        _damageable.OnDied += HandleDied;
    }

    private void Start() 
    {
        _statProvider = GetComponent<Monster>().StatProvider;
    }

    private void OnDestroy()
    {
        if (_damageable is UnityEngine.Object damageableObject && damageableObject != null)
        {
            _damageable.OnDied -= HandleDied;
        }
    }

    private void OnEnable()
    {
        _isAttacking = false;
        _lastAttackTime = 0f;

        CancelInvoke(nameof(ReturnToPool));

        if (_collider != null)
        {
            _collider.enabled = true;
        }

        if (_behaviorGraphAgent != null)
        {
            _behaviorGraphAgent.enabled = true;
        }
    }

    public bool CanAttack()
    { 
        if (_damageable.IsDead)
        {
            return false;
        }

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
        if (!CanAttack())
        {
            return ;
        }

        _isAttacking = true;
        _lastAttackTime = Time.time;

        Debug.Log($"{name} : 공격 시작 (사거리 {AttackRange})");

        OnAttackStarted?.Invoke();

        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), _attackDuration);
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
        _moveable.Stop();

        if (_behaviorGraphAgent != null)
        {
            _behaviorGraphAgent.enabled = false;
        }

        Invoke(nameof(ReturnToPool), _corpseLingerDuration);
    }

    private void ReturnToPool()
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"{name} : GameObjectManager.Instance가 null이라 풀 반납을 건너뜁니다.");
            return;
        }

        GameObjectManager.Instance.RequestDestroyObject(gameObject);
    }
}
