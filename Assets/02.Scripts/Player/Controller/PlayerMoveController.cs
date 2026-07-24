using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] private float _rotationSmoothness = 10f;

    private Player _player;

    private Vector2 _moveInput;
    private Vector3 _moveDirection;

    private Rigidbody _rigid;

    private bool _isRunning;
    private bool _isLookingToMouse;

    public bool IsWalking => _moveInput.sqrMagnitude > 0.01f;
    public bool IsRunning => _isRunning;

    private void Awake()
    {
        _rigid= GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        if (_player == null) return;
    }

    private void Update()
    {
        MoveDirection();

        if (_isLookingToMouse == true)
        {
            LookToMousePos();
        }
        else
        {
            RotateToMoveDirection();
        }
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }

    public void SetRunning(bool isRunning)
    {
        _isRunning = isRunning;
    }

    public void SetLookingToMouse(bool isLookingToMouse)
    {
        _isLookingToMouse = isLookingToMouse;
    }

    // 플레이어 이동
    public void PlayerMove()
    {
        float speed = _isRunning ? _player.PlayerData.MoveSpeed * 3f : _player.PlayerData.MoveSpeed;

        //transform.position += _moveDirection * speed * Time.deltaTime;
        Vector3 nextPosition = _rigid.position + _moveDirection * speed * Time.fixedDeltaTime;
        _rigid.MovePosition(nextPosition);
    }

    // 플레이어의 이동 방향 설정
    private void MoveDirection()
    {
        _moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
    }

    // 플레이어의 이동 회전 설정
    private void RotateToMoveDirection()
    {
        if (_moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        }
    }

    // 마우스 위치를 바라보도록 회전
    private void LookToMousePos()
    {
        // 현재 마우스 위치 가져옴
        Vector3 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // 방금 만든 Ray가 어떤 Collider에 부딪하면 hit에 들어감
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dir = hit.point - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
            }
        }
    }
}
