using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MonsterGroup : MonoBehaviour, IMonsterGroupBehavior
{
    [Header("경보")] 
    [SerializeField] private float _alertRadius = 15f;
    [SerializeField] private float _alertDuration = 8f;
    [SerializeField] private LayerMask _monsterLayer;

    [SerializeField] private FlockRangeSensor _flockRangeSensor;

    private IMonsterPerceivable _perceivable;
    private IMonsterMoveable _moveable;
    private bool _isAlerted;
    private Vector3 _alertPosition;
    private float _alertElapsedTime;

    public bool IsAlerted
    {
        get { return _isAlerted; }
    }

    public Vector3 AlertPosition
    {
        get { return _alertPosition; }
    }

    public IReadOnlyList<Transform> Neighbors
    {
        get { return _flockRangeSensor != null ? _flockRangeSensor.Neighbors : Array.Empty<Transform>(); }
    }

    private void Awake()
    {
        _perceivable = GetComponent<IMonsterPerceivable>();
        _moveable = GetComponent<IMonsterMoveable>();

        if (_flockRangeSensor == null)
        {
            Debug.LogWarning($"{name} : FlockRangeSensor가 연결되어 있지 않습니다.");
        }

        _perceivable.OnPlayerSpotted += HandlePlayerSpotted;
    }

    private void OnDestroy()
    {
        if (_perceivable is UnityEngine.Object perceivableObject && perceivableObject != null)
        {
            _perceivable.OnPlayerSpotted -= HandlePlayerSpotted;
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

            IMonsterGroupBehavior groupBehavior = hit.GetComponent<IMonsterGroupBehavior>();

            if (groupBehavior == null)
            {
                continue;
            }

            groupBehavior.ReceiveAlert(alertPosition);
        }
    }

    public void ReceiveAlert(Vector3 alertPosition)
    {
        _isAlerted = true;
        _alertPosition = alertPosition;
        _alertElapsedTime = 0f;

        Debug.Log($"{name} : 경보 수신, 목표 위치 {alertPosition}");
    }

    public void ClearAlert()
    {
        _isAlerted = false;

        Debug.Log($"{name} : 경보 해제");
    }

    public void Tick()
    {
        if (_flockRangeSensor == null)
        {
            return;
        }

        Vector3 moveDirection = _flockRangeSensor.CalculateMoveDirection();
        _moveable.Move(moveDirection);
    }

}
