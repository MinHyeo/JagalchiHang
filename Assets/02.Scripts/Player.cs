using System.Collections;
using System.Security;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private StateController _stateController;
    [SerializeField] private float _smoothness = 10f;
    [SerializeField] private bool _toggleCameraRotation;    // Alt를 눌렀을 때 둘러보기 가능하도록 하는 변수

    private Camera _camera;
    private Vector2 _moveInput;
    private Vector3 _moveDirection;   

    private bool _isAttacking;
    private bool _isRuning;

    private void Start()
    {
        _camera = Camera.main;    // 태그에 MainCamera가 활성화 돼있어야 함
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt))
        {
            _toggleCameraRotation = true;    // 둘러보기 활성화
        }
        else
        {
            _toggleCameraRotation = false;   // 둘러보기 비활성화
        }

        Vector3 camForward = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = _camera.transform.right;

        _moveDirection = camForward * _moveInput.y + camRight * _moveInput.x;

        if(_isAttacking == false)
        {
            float spped = _isRuning ? _runSpeed : _moveSpeed;

            // 계산된 방향으로 플레이어 이동
            transform.position += _moveDirection * spped * Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if(_toggleCameraRotation == false)
        {
            // 두 벡터 곱하기
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * _smoothness);
        }
    }

    private void ChangeState(EntityAnimState curState)
    {
        _stateController.SetState(curState);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            ChangeState(EntityAnimState.Walk);
        }
        else
        {
            ChangeState(EntityAnimState.Idle);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        // _isRuning = context.ReadValueAsButton(); 와 같음
        if (context.started)
        {
            _isRuning = true;
        }

        if(context.canceled)
        {
            _isRuning = false;
        }

        if(_isRuning == true)
        {
            ChangeState(EntityAnimState.Run);
        }
        else
        {
            ChangeState(EntityAnimState.Walk);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        if (_isAttacking == true) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;

        ChangeState(EntityAnimState.Attack);
        yield return new WaitForSeconds(0.7f);

        _isAttacking = false;

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            ChangeState(EntityAnimState.Walk);

            if(_isRuning == true)
            {
                ChangeState(EntityAnimState.Run);
            }
        }
        else
        {
            ChangeState(EntityAnimState.Idle);
        }
    }
    
}
