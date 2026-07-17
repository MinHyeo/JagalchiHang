using System;

public interface IMonsterStatProvider
{
    int MaxHealth { get; }
    int AttackPower { get; }  
    float AttackRange { get; }
    float MoveSpeed { get; }

    void LoadStats(string monsterId);
}
