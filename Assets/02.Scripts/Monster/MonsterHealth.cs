using UnityEngine;
using System;

public class MonsterHealth : MonoBehaviour, IMonsterDamageable
{
    private int _maxHealth;
    private int _currentHealth;
    private bool _isDead;

    public int Health
    {
        get { return _currentHealth; }
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

    public void ResetForSpawn(int maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _isDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) { return; }

        if (amount <= 0) { return; }

        _currentHealth -= amount;

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        Debug.Log($"{name} : 데미지 {amount}, 남은 체력 {_currentHealth} / {_maxHealth}");
        OnDamaged?.Invoke();

        if (_currentHealth <= 0)
        {
            _isDead = true;
            Debug.Log($"{name} : 사망");
            MonsterRegistry.Unregister(this);
            OnDied?.Invoke();
        }
    }

    private void OnEnable()
    {
        MonsterRegistry.Register(this);
    }

    private void OnDisable()
    {
        MonsterRegistry.Unregister(this);
    }
}
