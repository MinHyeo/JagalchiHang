using UnityEngine;
using System;

public class TestPlayerMovement : MonoBehaviour, IMonsterMoveable
{
    [SerializeField] private float _moveSpeed = 5f;

    private CharacterController _characterController;
    private bool _isMoving;
    private Vector3 _velcoity;

    public bool HasReachedDestination
    {
        get { return true; }
    }

    public bool IsMoving
    {
        get { return _isMoving; }
    }

    public Vector3 Velocity
    {
        get { return _velcoity; }
    }

    public event Action<bool> OnMovingStateChanged;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        if ( _characterController == null )
        {
            _characterController = gameObject.AddComponent<CharacterController>();
        }
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput);

        bool wasMoving = _isMoving;

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            _characterController.SimpleMove(inputDirection.normalized * _moveSpeed);
            _velcoity = inputDirection.normalized * _moveSpeed;
            _isMoving = true;
        }
        else
        {
            _characterController.SimpleMove(Vector3.zero);
             _velcoity = Vector3.zero;
            _isMoving = false;
        }

        if ( wasMoving != _isMoving )
        {
            OnMovingStateChanged?.Invoke(_isMoving);
        }
    }

    public void MoveTo(Vector3 destination)
    {

    }

    public void Move(Vector3 direction)
    {
        
    }

    public void Stop()
    {
        _isMoving = false;
        _velcoity = Vector3.zero;
    }
}
