using UnityEngine;
using System;

public class MonsterHealth : MonoBehaviour, IMonsterDamageable
{
    private IMonsterStatProvider _statProvider;

    private int _maxHealth;
    private int _currenHealth;
    private bool _isDead;

    public int Health
    {
        get { return _currenHealth; }
    }

    public int MaxHealth
    {
        get { return _maxHealth; }
    }

    public bool IsDead
    {
        get { return _isDead; }
    }

    public event Action OnDamaged;
    public event Action OnDied;

    private void Awake()
    {
        _statProvider = GetComponent<IMonsterStatProvider>();
        _statProvider.OnStatsLoaded += HandleStatsLoaded;
    }

    private void OnEnable()
    {
        MonsterRegistry.Register(this);
    }

    private void OnDisable()
    {
        MonsterRegistry.Unregister(this);
    }

    private void OnDestroy()
    {
        try
        {
            _statProvider.OnStatsLoaded -= HandleStatsLoaded;
        }
        catch (Exception)
        {

        }
    }

    private void HandleStatsLoaded()
    {
        _maxHealth = _statProvider.MaxHealth;
        _currenHealth = _maxHealth;
        _isDead = false;
    }

    private void Start()
    {
        _maxHealth = _statProvider.MaxHealth;
        _currenHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) { return; }

        if (amount <= 0) { return; }

        _currenHealth -= amount;

        if (_currenHealth < 0)
        {
            _currenHealth = 0;
        }

        Debug.Log($"{name} : 데미지 {amount}, 남은 체력 {_currenHealth} / {_maxHealth}");
        OnDamaged?.Invoke();

        if (_currenHealth <= 0)
        {
            _isDead = true;
            Debug.Log($"{name} : 사망");
            MonsterRegistry.Unregister(this);
            OnDied?.Invoke();
        }
    }

}
