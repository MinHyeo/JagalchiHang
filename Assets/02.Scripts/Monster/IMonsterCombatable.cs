using System;

public interface IMonsterCombatable
{
    bool IsAttacking { get; }
    float AttackRange { get; }

    float AttackPower { get; }

    event Action OnAttackStarted;
    event Action OnAttackEnded;

    void Attack();

    bool CanAttack();
}
