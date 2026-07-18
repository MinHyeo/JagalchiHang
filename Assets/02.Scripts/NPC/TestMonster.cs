using System;
using UnityEngine;


public class TestMonster : MonoBehaviour // 테스트용 몬스터 코드
{
    private int maxHealth = 100;
    private int _currentHealth;
    private bool _isDead = false;

    public event Action<TestMonster> OnDead;
    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        _currentHealth -= amount;

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        Debug.Log($"{name}이  {amount}의 피해를 입었습니다.");
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;

        Debug.Log($"[TestMonster] {name}의 체력이 0이 되어 소멸");

        OnDead?.Invoke(this);

        Destroy(gameObject);
    }
}
    

