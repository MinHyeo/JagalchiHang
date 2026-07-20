using UnityEngine;

public class MonsterManager
{
    public int ActiveMonsterCount
    {
        get { return MonsterRegistry.ActiveCount; }
    }

    public void Init(ITargetable target)
    {
        if (SpawnManager.Instance == null)
        {
            Debug.LogWarning("MonsterManager : SpawnManager.Instance가 null이라 target을 전달하지 못했습니다.");
            return;
        }

        SpawnManager.Instance.SetPlayerTarget(target);
    }

    public void SetSpawningEnabled(bool isEnabled)
    {
        if (SpawnManager.Instance == null)
        {
            Debug.LogWarning("MonsterManager : SpawnManager.Instance가 null입니다.");
            return;
        }

        SpawnManager.Instance.SetSpawningEnabled(isEnabled);
    }
}
