using System;

public interface IMonsterDamageable
{
    int Health { get; }
    int MaxHealth {  get; }

    bool IsDead { get; }

    event Action OnDamaged;
    event Action OnDied;

    void TakeDamage(int amount);
}
