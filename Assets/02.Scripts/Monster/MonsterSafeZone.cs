using UnityEngine;

public class MonsterSafeZone : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log($"{name} : MonsterSafeZone OnEnable 호출됨 (Time: {Time.time})");

        if (SpawnManager.Instance == null)
        {
            Debug.LogWarning($"{name} : SpawnManager.Instance가 null입니다.");
            return;
        }

        SpawnManager.Instance.SetSpawningEnabled(false);
        SpawnManager.Instance.DespawnAllDynamicMonsters();
    }

    private void OnDisable()
    {
        Debug.Log($"{name} : MonsterSafeZone OnDisable 호출됨 (Time: {Time.time})");

        if (SpawnManager.Instance == null)
        {
            return;
        }

        SpawnManager.Instance.SetSpawningEnabled(true);
    }
}
