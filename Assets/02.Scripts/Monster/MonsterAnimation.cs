using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    [SerializeField] private Animator _animator;
    private IMonsterDamageable _damageable;
    private IMonsterMoveable _moveable;
    private IMonsterCombatable _combatable;

    private void Awake()
    {
        _damageable = GetComponent<IMonsterDamageable>();
        _moveable = GetComponent<IMonsterMoveable>();
        _combatable = GetComponent<IMonsterCombatable>();
    }

    private void OnEnable()
    {
        _damageable.OnDamaged += HandleDamaged;
        _damageable.OnDied += HandleDied;
        _moveable.OnMovingStateChanged += HandleMovingStateChanged;
        _combatable.OnAttackStarted += HandleAttackStarted;
    }

    private void OnDisable()
    {
        _damageable.OnDamaged -= HandleDamaged;
        _damageable.OnDied -= HandleDied;
        _moveable.OnMovingStateChanged -= HandleMovingStateChanged;
        _combatable.OnAttackStarted -= HandleAttackStarted;
    }

    private void HandleDamaged()
    {
        _animator.SetTrigger(HitHash);
    }

    private void HandleDied()
    {
        _animator.SetTrigger(DieHash);
    }

    private void HandleMovingStateChanged(bool isMoving)
    {
        _animator.SetBool(IsMovingHash, isMoving);
    }

    private void HandleAttackStarted()
    {
        _animator.SetTrigger(AttackHash);
    }
}
