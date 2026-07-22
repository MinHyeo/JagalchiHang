using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] Transform _spawnTransform;

    public Vector3 GetSpawnPosition()
    {
        return _spawnTransform.position;
    }
}