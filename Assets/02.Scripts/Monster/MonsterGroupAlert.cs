using UnityEngine;

public class MonsterGroupAlert : MonoBehaviour, IMonsterAlertable
{
    [SerializeField] private float _alertRadius = 15f;
    [SerializeField] private LayerMask _monsterLayer;
    [SerializeField] private float _alertDuration = 8f;

    private IMonsterPerceivable _perceivable;
    private bool _isAlerted;
    private Vector3? _alertPosition;
    private float _alertElapsedTime;

    public bool IsAlerted
    {
        get { return _isAlerted; }
    }

    public Vector3? AlertPosition
    {
        get { return _alertPosition; }
    }

    private void Awake()
    {
        _perceivable = GetComponent<IMonsterPerceivable>();
        _perceivable.OnPlayerSpotted += HandlePlayerSpotted;
    }

    private void OnDestroy()
    {
        try
        {
            _perceivable.OnPlayerSpotted -= HandlePlayerSpotted;
        }
        catch
        {

        }
    }

    private void Update()
    {
        if (!_isAlerted)
        {
            return;
        }

        _alertElapsedTime += Time.deltaTime;
        
        if (_alertElapsedTime >= _alertDuration)
        {
            _isAlerted = false;
            _alertPosition = null;
        }
    }

    private void HandlePlayerSpotted(Vector3 spottedPosition)
    {
        BroadcastAlert(spottedPosition);
    }

    private void BroadcastAlert(Vector3 alertPosition)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _alertRadius, _monsterLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }

            IMonsterAlertable alertable = hit.GetComponent<IMonsterAlertable>();

            if (alertable == null)
            {
                continue;
            }

            alertable.ReceiveAlert(alertPosition);
        }
    }

    public void ReceiveAlert(Vector3 alertPosition)
    {
        _isAlerted = true;

        _alertPosition = alertPosition;
        _alertElapsedTime = 0f;

        // Debug.Log($"{name} : 경보 수신, 목표 위치 {alertPosition}");
    }

    public void ClearAlert()
    {
        _isAlerted =false;
        _alertPosition = null;

        // Debug.Log($"{name} : 경보 해제");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _alertRadius);

        if (_alertPosition.HasValue)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, _alertPosition.Value);
            Gizmos.DrawWireSphere(_alertPosition.Value, 0.5f);
        }
    }
}
