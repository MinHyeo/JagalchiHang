using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SingletonBase<InputManager>
{
    private PlayerInputAction _inputAction;

    // 플레이어 관련 이벤트
    public event Action<Vector2> OnMoveEvent;
    public event Action<bool> OnRunEvent;
    public event Action OnAttackEvent;
    public event Action<bool> OnLookAtEvent;
    public event Action OnPickUpEvent;

    public event Action OnClickInventory;
    public event Action OnClickNpcUI;

    private void Start()
    {
        _inputAction = new PlayerInputAction();
        BindInputAction();
    }

    private void OnEnable()
    {
        _inputAction?.Enable();
    }

    private void OnDisable()
    {
        _inputAction?.Disable();
    }

    public void EnableGamePlayInput(bool isEnabled)
    {
        if(isEnabled == true)
        {
            _inputAction.Player.Enable();
        }
        else
        {
            _inputAction.Player.Disable();
        }
    }

    public void EnableUIInput(bool isEnabled)
    {
        if (isEnabled == true)
        {
            _inputAction.UI.Enable();
        }
        else
        {
            _inputAction.UI.Disable();
        }
    }

    private void BindInputAction()
    {
        _inputAction.Player.Move.performed += OnMove;
        _inputAction.Player.Move.canceled += OnMove;

        _inputAction.Player.Attack.started += OnAttack;

        _inputAction.Player.Run.started += OnRun;
        _inputAction.Player.Run.canceled += OnRun;

        _inputAction.Player.LookToMousePos.started += OnLookAt;
        _inputAction.Player.LookToMousePos.canceled += OnLookAt;

        _inputAction.Player.Pickup.started += OnPickUp;

        _inputAction.Player.Inventory.started += OnInventory;
        _inputAction.Player.NpcUI.started += OnNpcUI;
    }

    private void OnMove(InputAction.CallbackContext callback)
    {
        OnMoveEvent?.Invoke(callback.ReadValue<Vector2>());
    }

    private void OnRun(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            OnRunEvent?.Invoke(true);
        }

        if (callback.canceled)
        {
            OnRunEvent?.Invoke(false);
        }
    }

    private void OnAttack(InputAction.CallbackContext callback)
    {
        OnAttackEvent?.Invoke();
    }

    private void OnLookAt(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            OnLookAtEvent?.Invoke(true);
        }

        if (callback.canceled)
        {
            OnLookAtEvent?.Invoke(false);
        }
    }

    private void OnPickUp(InputAction.CallbackContext callback)
    {
        OnPickUpEvent?.Invoke();
    }

    private void OnInventory(InputAction.CallbackContext callback)
    {
        OnClickInventory?.Invoke();
    }

    private void OnNpcUI(InputAction.CallbackContext callback)
    {
        OnClickNpcUI?.Invoke();
    }
}
