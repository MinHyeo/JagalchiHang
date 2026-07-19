using UnityEngine;
using System.Collections.Generic;

public static class MonsterRegistry
{
    private static readonly HashSet<IMonsterDamageable> _activeMonsters = new HashSet<IMonsterDamageable>();

    public static int ActiveCount
    {
        get { return _activeMonsters.Count; }
    }

    public static void Register(IMonsterDamageable monster)
    {
        _activeMonsters.Add(monster);
    }

    public static void Unregister(IMonsterDamageable monster)
    {
        _activeMonsters.Remove(monster);
    }
}
