using UnityEngine;

public class OilTrailMarker : MonoBehaviour, ITrailmarker
{
    [SerializeField] private float _lifetime = 15f;

    private float _elapsedTime;
    
    public Vector3 Position
    {
        get { return transform.position; }
    }

    public float Strength
    {
        get { return Mathf.Clamp01(1f - (_elapsedTime / _lifetime)); }
    }

    private void OnEnable()
    {
        _elapsedTime = 0f;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _lifetime)
        {
            GameObjectManager.Instance.RequestDestroyObject(gameObject);
        }
    }
}
