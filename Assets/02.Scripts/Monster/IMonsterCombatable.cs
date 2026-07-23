using System;

public interface IMonsterCombatable
{
    bool IsAttacking { get; }
    float AttackRange { get; }

    int AttackPower { get; }

    event Action OnAttackStarted;
    event Action OnAttackEnded;

    void Attack();

    bool CanAttack();
}
